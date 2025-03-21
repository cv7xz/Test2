using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    public enum CharaterType
    {
        Player = 0,
        Enemy = 1,
    };
    public enum EnemyType
    {
        Normal,
        Elite,
        Boss,
    }
    public EnemyType enemyType;
    public CharaterType type;
    public string characterName;
    public int CurrentIndex;
    public Sprite sprite;

    #region Special
    public bool cantAddStatus1067 = false;  //不可附加残梅绽buff
    #endregion

    public int currentIndex
    {
        get
        {
            return CurrentIndex;
        }
        set
        {
            CurrentIndex = value;
            if (type == CharaterType.Player)
            {
                GameManager.Instance.players[CurrentIndex] = this;
            }
            else if(type == CharaterType.Enemy)
            {
                GameManager.Instance.enemies[CurrentIndex] = this;
            }
        }
    }
    #region 角色UI
    public GameObject TargetFlag;
    public GameObject HealthBarPrefab, healthBar;
    public Transform HealthPos;
    #endregion

    public Equip equipObject;
    public CharacterData_SO characterData;
    public Skill_SO QSKILLObject, ESKILLObject, SKILLFINALObject;
    public Friend_SO FriendObject;

    public Equip equip;
    public CharacterData_SO data;
    public Skill_SO skillQ, skillE,skillFinal;
    public Friend_SO friend_SO;

    public List<Status> currentStatus;
    public virtual void Awake()
    {


    }
    public virtual void Start()
    {
        cantAddStatus1067 = false;
        TargetFlag.SetActive(false);
        healthBar = Instantiate(HealthBarPrefab, GameManager.Instance.canvas.transform);
        healthBar.transform.position = HealthPos.transform.position;
        healthText = Instantiate(HealthText, GameManager.Instance.canvas.transform);
        healthText.transform.position = HealthPos.transform.position;

        Messenger.Instance.AddListener(Messenger.EventType.SettleDeath, SettleDath);
        Messenger.Instance.AddListener<Character>(Messenger.EventType.TurnEnd, FreshStatusTurnEnd);
        Messenger.Instance.AddListener<Character>(Messenger.EventType.TurnStart, FreshStatusTurnStart);
    }

    public Text HealthText,healthText;
    public virtual void Update()
    {
        FreshHealthBar();
        if (Input.GetMouseButtonDown(1))
        {
            if(GameManager.Instance.InfomationPanel.gameObject.activeSelf == true)
            {
                GameManager.Instance.InfomationPanel.gameObject.SetActive(false);
                GameManager.Instance.InformingCharacterFlag.SetActive(false);

                InputManager.Instance.Dict[GameManager.Instance.mouseDownCharacter].GetComponent<Image>().color = new Color(1f,1f,1f,81/255f);
            }
        }
    }
    public void FreshHealthBar()
    {
        healthBar.transform.GetChild(0).GetComponent<Image>().fillAmount = characterData.currentHealth / characterData.maxHealth;
        healthText.text = $"{characterData.currentHealth}/{characterData.maxHealth}";
    }


    //在上一个角色行动完后会立即产生 下一个角色的TurnStart信号 此时就已经进行Status的结算  然后再经过 ActionTime 才有下一个角色的ActionExecute (如果是enemy则进行随机攻击)
    #region Status随回合刷新
    public void FreshStatusTurnStart(Character character)   //为什么要用信号做character的状态刷新？
    {
        if (character != this)
        {
            return;
        }
        bool FreshToughShield = true;
        
        InputManager.Instance.SkipAction = false;
        Status a = new Status();
        for (int i = currentStatus.Count - 1; i >= 0; i--)
        {
            if (currentStatus[i] != null)
            {
                #region 回合开始前 时点 结算持续伤害 控制效果等
                if (currentStatus[i].TurnStartTiming)
                {
                    if (currentStatus[i].statusType == Status.StatusType.ContinueDamageStatus)
                    {
                        DamageAction.TakeContinuousDamage(currentStatus[i].Caster, character, currentStatus[i]);
                    }
                    if (currentStatus[i].statusType == Status.StatusType.ControlStatus)
                    {
                        a = currentStatus[i];
                        InputManager.Instance.SkipAction = true;     //该角色有ControlStatus   默认跳过行动
                        foreach (var action in currentStatus[i].controlActions)
                        {
                            if (action.effect == Status.actionEffect.NotFreshBroken)
                            {
                                FreshToughShield = false;
                            }
                            else if (action.effect == Status.actionEffect.PushActionValue)
                            {
                                character.characterData.actionValue = action.value / characterData.currentSpeed;
                                InputManager.Instance.enemyActionCounterDown.ResetTimer();
                            }
                        }
                    }
                    if(currentStatus[i].statusType == Status.StatusType.EnergyRestore)
                    {
                        EnergyChangeAction.AddEnergyAction(this, currentStatus[i].StatusValue[0]);
                    }
                }

                #endregion

                #region  不同Status回合数的结算
                if (currentStatus[i].duration == -1 || currentStatus[i].turnStartDuration == false)  //持续时间无限 或非回合开始时减少时间
                {
                    continue;
                }
                if (currentStatus[i].statusType == Status.StatusType.ExistFieldStatus)  //ExistField光环Status  作为自身被动存在 永不消除
                {
                    if(currentStatus[i].duration >= 1)
                    {
                        currentStatus[i].duration -= 1;
                    }
                    if(currentStatus[i].duration == 0)
                    {
                        foreach (var status in currentStatus[i].fieldStatus)
                        {
                            StatusAction.RemoveStatusAllPlayer(status.StatusName);
                        }
                    }

                    continue;
                }
                if (currentStatus[i].duration == 0 && currentStatus[i].NotAutoDelete)  //光环类buff 0回合保持存在状态 不起效果但不Remove
                {
                    continue;
                }

                currentStatus[i].duration -= 1;
                if (currentStatus[i].duration <= 0)
                {
                    var temp = currentStatus[i];
                    currentStatus.Remove(currentStatus[i]);
                    FreshProperty(temp);
                }
                #endregion
            }
        }

        if (InputManager.Instance.SkipAction)
        {
            DamageAction.context += $"{character.characterName} 因 {a.name}Status 跳过行动\n  角色行动值:{character.characterData.actionValue}\n";
            //InputManager.Instance.enemyActionCounterDown.AddEndAction(() => InputManager.Instance.FreshAction());
            InputManager.Instance.FreshAction();
        }

        if(character.type == CharaterType.Enemy && character.characterData.currentToughShield < 1e-6 && FreshToughShield)
        {
            var s = currentStatus.Find(e => e.StatusName == "1067");
            if (s != null)
            {
                var RUANMEI = GameManager.Instance.players.Find(e => e.characterName == "RUANMEI");   //可以想到  残梅绽的Catser提供者不一定是阮梅 需要全局找
                if(RUANMEI == null)
                {
                    RUANMEI = s.Caster;
                }
                float damageValue = StaticNumber.brokenBaseDamage[RUANMEI.characterData.Level];
                damageValue = 2 * (1 + RUANMEI.characterData.BrokensFocus * 0.01f) * 0.5f;

                damageValue *= ((characterData.maxToughShield * 0.1f + 2f) / 4f);

                float damageDecrease = characterData.currentDefend / (characterData.Level * 10 + 200 + characterData.currentDefend);
                damageValue *= (1 - damageDecrease);
                characterData.currentHealth -= damageValue;

                DamageAction.context += $"受到残梅绽伤害 {damageValue}\n";
                cantAddStatus1067 = true;
                //InputManager.Instance.enemyActionCounterDown.ResetTimer();
                PushActionValueAction.SetActionValue(this, 1000f + 2000f * RUANMEI.characterData.BrokensFocus * 0.01f);
                InputManager.Instance.SkipAction = true;
                currentStatus.Remove(s);
                return;
            }

            cantAddStatus1067 = true;
            character.characterData.currentToughShield = character.characterData.maxToughShield;
        }
    }
    public void FreshStatusTurnEnd(Character character)
    {
        if(character != this)
        {
            return;
        }

        for(int i= currentStatus.Count -1;i>=0;i--)
        {
            if(currentStatus[i] != null)
            {
                if (currentStatus[i].duration == -1 || currentStatus[i].turnStartDuration)  //持续时间无限或回合开始时减少时间
                {
                    continue;
                }

                if (currentStatus[i].statusType == Status.StatusType.ExistFieldStatus && currentStatus[i].duration == 0)  //ExistField光环Status  作为自身被动存在 永不消除
                {
                    if (currentStatus[i].statusType == Status.StatusType.ExistFieldStatus)
                    {
                        foreach (var status in currentStatus[i].fieldStatus)
                        {
                            StatusAction.RemoveStatusAllPlayer(status.StatusName);
                        }
                    }
                    continue;
                }
                if (currentStatus[i].duration == 0 && currentStatus[i].NotAutoDelete)  //光环类buff 0回合保持存在状态 不起效果但不Remove
                {
                    continue;
                }
                currentStatus[i].duration -= 1;
                if (currentStatus[i].duration <= 0)
                {
                    var temp = currentStatus[i];
                    currentStatus.Remove(currentStatus[i]);
                    FreshProperty(temp);
                }
            }
        }
    }
    #endregion

    public void SettleDath()
    {
        if (characterData.currentHealth < 1e-6)
        {
            if (type == CharaterType.Enemy)
            {
                Messenger.Instance.BroadCast(Messenger.EventType.EnemyDie);
            }
            if (InputManager.Instance.currentSelectEnemy == null)
            {
                InputManager.Instance.FindFirstTarget();
            }
            Destroy(healthBar);
            Destroy(gameObject);

        }
    }
    public Character FindLeftTarget()
    {
        if(currentIndex > 0)
        {
            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (type == CharaterType.Player && GameManager.Instance.players[i] != null)
                {
                    return GameManager.Instance.players[i];
                }
                if(type == CharaterType.Enemy && GameManager.Instance.enemies[i] != null)
                {
                    return GameManager.Instance.enemies[i];
                }
            }
            return null;
        }
        return null;
    }
    public Character FindRightTarget()
    {
        if (type == CharaterType.Player)
        {
            if(currentIndex < 3)
            {
                for (int i = currentIndex + 1; i <= 3; i++)
                {
                    if (GameManager.Instance.players[i] != null)
                    {
                        return GameManager.Instance.players[i];
                    }
                }
            }
            else if(currentIndex == 3)
            {
                return null;
            }
        }
        else if (type == CharaterType.Enemy)
        {
            if (currentIndex < 4)
            {
                for (int i = currentIndex + 1; i <= 4; i++)
                {
                    if (GameManager.Instance.enemies[i] != null)
                    {
                        return GameManager.Instance.enemies[i];
                    }
                }
            }
            else if (currentIndex == 4)
            {
                return null;
            }
        }

        return null;
    }
    public void Targeted()
    {
        TargetFlag.SetActive(true);
    }
    public void CancelTargeted()
    {
        TargetFlag.SetActive(false);
    }
    public virtual void OnDestroy()
    {
        Messenger.Instance.RemoveListener(Messenger.EventType.SettleDeath, SettleDath);
    }
    public float GetDependValue(Status s)
    {
        float dependValue = 0;
        Character dependTarget = new Character();
        float tempValue = 0;
        foreach(var depend in s.dependValues)
        {

            #region  一般不会改 简化一堆if嵌套的情况 提出所有简单枚举 如目标 depend属性类型
            if (depend.dependTarget == Status.DependTarget.Owner)
            {
                dependTarget = s.Owner;
            }
            else if(depend.dependTarget == Status.DependTarget.Caster)
            {
                dependTarget = s.Caster;
            }

            if (depend.property == Status.InvolvedProperty.CriticalDamageValue)
            {
                tempValue = dependTarget.characterData.criticalDamage;  //加爆伤buff  依存施法者24% + 45%爆伤
            }
            else if (depend.property == Status.InvolvedProperty.HealthValue)
            {
                tempValue = dependTarget.characterData.maxHealth;  //加符玄最大生命值的6%
            }
            else if (depend.property == Status.InvolvedProperty.BrokenFocus)
            {
                tempValue = dependTarget.characterData.BrokensFocus;
            }

            if (depend.dependTarget == Status.DependTarget.Global)
            {
                if (depend.dependType == Status.DependTpye.Normal)
                {
                    if (depend.property == Status.InvolvedProperty.CertainElementPlayerNumber)
                    {
                        int number = Mathf.Max(StaticNumber.GetPlayerNumber(depend.certainElement) - 1, 0);   //减1对齐value数组下标
                        dependValue += depend.values[number] + depend.correctValues;   //加攻击buff  依存场上某属性角色数量  依存数值非线性 根据数量读取数组
                    }
                }
                continue;
            }
            #endregion


            if (depend.dependType == Status.DependTpye.Normal)
            {
                dependValue += depend.values[0] * tempValue + depend.correctValues;  //加爆伤buff  依存施法者24% + 45%爆伤
            }
            else if(depend.dependType == Status.DependTpye.MinLimit)
            {
                if (tempValue > depend.minLimit)
                {
                    int steps = (int)((tempValue - depend.minLimit) / depend.everyStep);
                    dependValue += Mathf.Min((steps * depend.stepValue), depend.maxValue);
                }
            }

        }
        return dependValue;
    }

    public float GetAttachDependValue(Status s)
    {
        float dependValue = 0;
        if(s.attachOtherStatus.hasDepend == false)
        {
            return s.attachOtherStatus.AddValue;
        }

        Character dependTarget = new Character();
        float tempValue = 0;
        foreach (var depend in s.attachOtherStatus.dependValues)
        {

            #region  一般不会改 简化一堆if嵌套的情况 提出所有简单枚举 如目标 depend属性类型
            if (depend.dependTarget == Status.DependTarget.Owner)
            {
                dependTarget = s.Owner;
            }
            else if (depend.dependTarget == Status.DependTarget.Caster)
            {
                dependTarget = s.Caster;
            }

            if (depend.property == Status.InvolvedProperty.CriticalDamageValue)
            {
                tempValue = dependTarget.characterData.criticalDamage;  //加爆伤buff  依存施法者24% + 45%爆伤
            }
            else if (depend.property == Status.InvolvedProperty.HealthValue)
            {
                tempValue = dependTarget.characterData.maxHealth;  //加符玄最大生命值的6%
            }
            else if (depend.property == Status.InvolvedProperty.BrokenFocus)
            {
                tempValue = dependTarget.characterData.BrokensFocus;
            }

            if (depend.dependTarget == Status.DependTarget.Global)
            {
                if (depend.dependType == Status.DependTpye.Normal)
                {
                    if (depend.property == Status.InvolvedProperty.CertainElementPlayerNumber)
                    {
                        int number = Mathf.Max(StaticNumber.GetPlayerNumber(depend.certainElement) - 1, 0);
                        dependValue += depend.values[number] + depend.correctValues;   //加攻击buff  依存场上某属性角色数量  依存数值非线性 根据数量读取数组
                    }
                }
                continue;
            }
            #endregion


            if (depend.dependType == Status.DependTpye.Normal)
            {
                dependValue += depend.values[0] * tempValue + depend.correctValues;  //加爆伤buff  依存施法者24% + 45%爆伤
            }
            else if (depend.dependType == Status.DependTpye.MinLimit)
            {
                if (tempValue > depend.minLimit)
                {
                    int steps = (int)((tempValue - depend.minLimit) / depend.everyStep);
                    dependValue += Mathf.Min((steps * depend.stepValue), depend.maxValue);
                }
            }

        }
        return dependValue;
    }

    public float StatusValueFunction(Status s)
    {
        float tempValue = 0f;
        if (s.IsAttached)
        {
            if (s.attachOtherStatus.attachTargte == Status.AttachTarget.Self && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
            {
                tempValue += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] + GetAttachDependValue(s) * s.StatusLayer);
            }
            else if (s.attachOtherStatus.attachTargte == Status.AttachTarget.Caster && s.Caster.currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
            {
                tempValue += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] + GetAttachDependValue(s) * s.StatusLayer);
            }
        }
        else
        {
            tempValue += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
        }

        return tempValue;
    }

    //当添加buff时 若buff本身类型对应数值变化 则Fresh对应属性
    //当角色有被动光环buff时(常驻击破转增伤) 即该buff涉及了其他属性  则在属性发生变化时调用对应Involved属性参数  遍历涉及的Status Fresh对应属性
    //当添加buff时 本身类型不对应Fresh属性   但是涉及相关属性变化   如 该buff使得另一个buff的数值增加
    public void FreshProperty(Status status)
    {
        foreach(var type in status.InvolvedName)
        {
            if (type == Status.InvolvedProperty.AttackValue)
            {
                float tempAttackBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.AttackPercentBonus)
                    {
                        tempAttackBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempAttackBonus = Mathf.Min(tempAttackBonus, status.ValueLimited);
                }

                characterData.AttackPercentBonus = tempAttackBonus;
                characterData.currentAttack = characterData.baseAttack * (1 + tempAttackBonus);
            }
            else if (type == Status.InvolvedProperty.HealthValue)
            {
                float tempHealthBonus = 0;

                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.HealthPercentBonus)
                    {
                        tempHealthBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempHealthBonus = Mathf.Min(tempHealthBonus, status.ValueLimited);
                }

                characterData.maxHealth = characterData.maxHealth * (1 + tempHealthBonus);
                characterData.healthPercentBonus = tempHealthBonus;
                characterData.currentHealth = characterData.currentHealth * (1 + tempHealthBonus);
            }
            else if (type == Status.InvolvedProperty.DamageIncreaseValue)
            {
                float tempDamageIncreaseBonus = 0;

                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DamageIncreaseBonus)
                    {
                        tempDamageIncreaseBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempDamageIncreaseBonus = Mathf.Min(tempDamageIncreaseBonus, status.ValueLimited);
                }
                characterData.damageIncrease = tempDamageIncreaseBonus;
            }
            else if (type == Status.InvolvedProperty.BrokenFocus)
            {
                float tempBrokenFocusBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.BrokenFocusBonus)
                    {
                        tempBrokenFocusBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempBrokenFocusBonus = Mathf.Min(tempBrokenFocusBonus, status.ValueLimited);
                }

                characterData.BrokensFocus = tempBrokenFocusBonus;
            }
            else if (type == Status.InvolvedProperty.CriticalDamageValue)
            {
                float tempCriticalDamageBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.CriticalDamageBonus)
                    {
                        tempCriticalDamageBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempCriticalDamageBonus = Mathf.Min(tempCriticalDamageBonus, status.ValueLimited);
                }

                characterData.criticalDamage = tempCriticalDamageBonus + 0.5f;
            }
            else if (type == Status.InvolvedProperty.EffectPercentValue)
            {
                float tempEffectPercentBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.EffectPercentBonus)
                    {
                        tempEffectPercentBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempEffectPercentBonus = Mathf.Min(tempEffectPercentBonus, status.ValueLimited);
                }

                characterData.effectPercent = tempEffectPercentBonus;
            }
            else if (type == Status.InvolvedProperty.EffectDefendValue)
            {
                float tempEffectDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.EffectDefendBonus)
                    {
                        tempEffectDefendBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempEffectDefendBonus = Mathf.Min(tempEffectDefendBonus, status.ValueLimited);
                }

                characterData.effectDefend = tempEffectDefendBonus;
            }
            else if (type == Status.InvolvedProperty.DefendValue)
            {
                #region 枚举属性抗性
                float BINGDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.BING || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            BINGDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentBINGDefend = characterData.BINGDefend + BINGDefendBonus;

                float HUODefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.HUO || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            HUODefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentHUODefend = characterData.HUODefend + HUODefendBonus;

                float FENGDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.FENG || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            FENGDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentFENGDefend = characterData.FENGDefend + FENGDefendBonus;

                float LEIDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.LEI || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            LEIDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentLEIDefend = characterData.LEIDefend + LEIDefendBonus;

                float WULIDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.WULI || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            WULIDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentWULIDefend = characterData.WULIDefend + WULIDefendBonus;

                float XUSHUDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.XUSHU || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            XUSHUDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentXUSHUDefend = characterData.XUSHUDefend + XUSHUDefendBonus;

                float LIANGZIDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if (s.involvedElement == CharacterData_SO.weaknessType.LIANGZI || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            LIANGZIDefendBonus += StatusValueFunction(s);
                        }
                    }
                }
                characterData.currentLIANGZIDefend = characterData.LIANGZIDefend + LIANGZIDefendBonus;
                #endregion
            }
            else if (type == Status.InvolvedProperty.DefendPenetrationValue)
            {
                
                float tempAllPentrationBonus = 0;
                float tempBINGPentrationBonus = 0;
                float tempHUOPentrationBonus = 0;
                float tempLEIPentrationBonus = 0;
                float tempFENGPentrationBonus = 0;
                float tempWULIPentrationBonus = 0;
                float tempXUSHUPentrationBonus = 0;
                float tempLIANGZIPentrationBonus = 0;

                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPenetration)
                    {
                        if(s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            tempAllPentrationBonus += StatusValueFunction(s);
                        }
                        else if(s.involvedElement == CharacterData_SO.weaknessType.BING)
                        {
                            tempBINGPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.HUO)
                        {
                            tempHUOPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.FENG)
                        {
                            tempFENGPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.LEI)
                        {
                            tempLEIPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.WULI)
                        {
                            tempWULIPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.XUSHU)
                        {
                            tempXUSHUPentrationBonus += StatusValueFunction(s);
                        }
                        else if (s.involvedElement == CharacterData_SO.weaknessType.LIANGZI)
                        {
                            tempLIANGZIPentrationBonus += StatusValueFunction(s);
                        }
                    }
                }

                characterData.currentBINGPentration = tempBINGPentrationBonus;
                characterData.currentHUOPentration = tempHUOPentrationBonus;
                characterData.currentFENGPentration = tempFENGPentrationBonus;
                characterData.currentLEIPentration = tempLEIPentrationBonus;
                characterData.currentWULIPentration = tempWULIPentrationBonus;
                characterData.currentXUSHUPentration = tempXUSHUPentrationBonus;
                characterData.currentLIANGZIPentration = tempLIANGZIPentrationBonus;
                characterData.currentAllPentration = tempAllPentrationBonus;
            }
            else if (type == Status.InvolvedProperty.PureDefendValue)
            {
                float tempDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendValueBonus)
                    {
                        tempDefendBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempDefendBonus = Mathf.Min(tempDefendBonus, status.ValueLimited);
                }

                characterData.DefendPercentBonus = tempDefendBonus;
                characterData.currentDefend = characterData.baseDefend * (1 + tempDefendBonus);
            }
            else if (type == Status.InvolvedProperty.CriticalPercentValue)
            {
                float tempCriticalPercentBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.CriticalPercentBonus)
                    {
                        tempCriticalPercentBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempCriticalPercentBonus = Mathf.Min(tempCriticalPercentBonus, status.ValueLimited);
                }

                characterData.criticalPercent = tempCriticalPercentBonus + 0.05f;
            }
            else if (type == Status.InvolvedProperty.DamageDecreseValue)
            {
                float tempDamageDecreaseBonus = 0;

                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DamageDecreseBonus)
                    {
                        tempDamageDecreaseBonus += StatusValueFunction(s);
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempDamageDecreaseBonus = Mathf.Min(tempDamageDecreaseBonus, status.ValueLimited);
                }
                characterData.damageDecrease = tempDamageDecreaseBonus;
            }
            else if (type == Status.InvolvedProperty.SpeedValue)
            {
                float tempSpeedFixBonus = 0;
                float tempSpeedPercentBonus = 0f;

                foreach(var s in currentStatus)
                {
                    if(s.statusType == Status.StatusType.SpeedFixBouns)
                    {
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempSpeedFixBonus += StatusValueFunction(s);
                        }
                    }
                    else if(s.statusType == Status.StatusType.SpeedPercentBonus)
                    {
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempSpeedPercentBonus += StatusValueFunction(s);
                        }
                    }
                }

                characterData.speedPercentBonus = tempSpeedPercentBonus;
                characterData.fixSpeedBonus = tempSpeedFixBonus;
                characterData.currentSpeed = (characterData.baseSpeed) * characterData.speedPercentBonus + characterData.fixSpeedBonus;
            }
            else if (type == Status.InvolvedProperty.BrokenEfficiencyValue)
            {
                float tempBrokenEfficiencyBonus = 0;

                foreach(var s in currentStatus)
                {
                    if(s.statusType == Status.StatusType.BrokenEfficiencyBouns)
                    {
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempBrokenEfficiencyBonus += StatusValueFunction(s);
                        }
                    }
                }

                characterData.BrokenEfficiencyBonus = tempBrokenEfficiencyBonus;
            }
        }
    }
    public void FreshProperty(Status.InvolvedProperty str)
    {
        foreach (var status in currentStatus)
        {
            if (status.InvolvedName.Contains(str))
            {
                FreshProperty(status);
            }
        }
    }
    private List<GameObject> weaknessImage = new List<GameObject>();
    public void ShowSelfWeakness()
    {

        foreach (var weak in weaknessImage)
        {
            Destroy(weak);
        }
        weaknessImage.Clear();  //Clear不会导致GameObject的Destroy

        foreach (var weak in characterData.weakness)
        {
            int Index = characterData.weakness.IndexOf(weak);
            if(weak != CharacterData_SO.weaknessType.NONE)
            {
                GameObject newWeak = Instantiate(GameManager.Instance.Elements[(int)weak]);   //要注意对应
                newWeak.transform.position = this.transform.position + new Vector3(-15 + 7 * Index, -22f, 0);
                weaknessImage.Add(newWeak);
            }
        }
    }

    public void FreshInformation()
    {
        string context = "";

        context += "基础攻击力:" + characterData.baseAttack.ToString() + "\n";
        context += "攻击力加成:" + (characterData.AttackPercentBonus * 100).ToString() + "%" + " + " + characterData.fixAttackBonus + "\n";
        context += "当前攻击力:" + characterData.currentAttack.ToString() + "\n";

        context += "基础生命值:" + characterData.baseHealth.ToString() + "\n";
        context += "生命值加成:" + (characterData.healthPercentBonus * 100).ToString() + "%" + " + " + characterData.fixHealthBonus + "\n";
        context += "当前生命值:" + characterData.currentHealth.ToString() + "\n";
        context += "最大生命值:" + characterData.maxHealth.ToString() + "\n";


        context += "基础防御值:" + characterData.baseDefend.ToString() + "\n";
        context += "防御值加成:" + (characterData.DefendPercentBonus * 100).ToString() + "%" + " + " + characterData.fixDefendBonus + "\n";
        context += "当前防御值:" + characterData.currentDefend.ToString() + "\n";

        context += "基础速度:" + characterData.baseSpeed.ToString() + "\n";
        context += "速度加成:" + (characterData.speedPercentBonus * 100).ToString() + "%" + " + " + characterData.fixSpeedBonus + "\n";
        context += "当前速度:" + characterData.currentSpeed.ToString() + "\n";

        context += "效果命中:" + (characterData.effectPercent * 100).ToString() + "%" + "\n";
        context += "效果抵抗:" + (characterData.effectDefend * 100).ToString() + "%" + " + " + characterData.fixSpeedBonus + "\n";

        context += "伤害加成:" + (characterData.damageIncrease * 100).ToString() + "%" + "\n";
        context += "击破特攻:" + (characterData.BrokensFocus).ToString() + "%" + "\n";
        context += "击破效率:" + (characterData.BrokenEfficiencyBonus * 100).ToString() + "%\n";

        context += "暴击率:" + (characterData.criticalPercent * 100).ToString() + "%" + "\n";
        context += "暴击伤害:" + (characterData.criticalDamage * 100).ToString() + "%" + "\n";
        context += "能量:" + characterData.currentEnergyValue.ToString() + "/" + characterData.maxEnergyValue.ToString() + "\n\n";

        foreach (var status in currentStatus)
        {
            if (status.isSpecialStatus)
            {
                context += "<color=red>";
            }
            //if (status.StatusName.Contains("BD"))
            //{
            //    continue;
            //}
            //context += status.description + "\n";
            context += status.name.Replace("(Clone)","C") + " 持续: " + status.duration.ToString() + " 层数: " + status.StatusLayer + " 数值:";
            if (status.isSpecialStatus)
            {
                context += "</color>";
            }
            if (status.dependValues.Count > 0)
            {
                context += GetDependValue(status).ToString() +"(依存) ";
            }
            else
            {
                foreach (var value in status.StatusValue)
                {
                    if (status.IsAttached)
                    {
                        context += StatusValueFunction(status).ToString() + "(依附于Status)"; 
                    }
                    else
                    {
                        context += value.ToString() + "  ";
                    }

                }
            }

            context +=  "\n";

        }
        GameManager.Instance.CharacterInfomText.text = context;
    }
    public void OnMouseDown()
    {
        if(GameManager.Instance.mouseDownCharacter != null)
        {
            InputManager.Instance.Dict[GameManager.Instance.mouseDownCharacter].GetComponent<Image>().color = new Color(1f, 1f, 1f, 81 / 255f);
        }

        var list = InputManager.Instance.characters;

        foreach(var character in list)
        {
            if (character != null && character == this)
            {
                InputManager.Instance.Dict[this].GetComponent<Image>().color = new Color(0, 1f, 0, 81 / 255f);
            }
        }
        GameManager.Instance.mouseDownCharacter = this;
        GameManager.Instance.InfomationPanel.transform.SetAsLastSibling();
        GameManager.Instance.InfomationPanel.gameObject.SetActive(true);
        GameManager.Instance.InformingCharacterFlag.transform.position = transform.position + new Vector3(5f,-5f,0) ;
        GameManager.Instance.InformingCharacterFlag.SetActive(true);
    }


}
