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
    public CharaterType type;
    public string characterName;
    public int CurrentIndex;
    public Sprite sprite;

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
    #region ��ɫUI
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
        TargetFlag.SetActive(false);
        healthBar = Instantiate(HealthBarPrefab, GameManager.Instance.canvas.transform);
        healthBar.transform.position = HealthPos.transform.position;


        Messenger.Instance.AddListener(Messenger.EventType.SettleDeath, SettleDath);
        Messenger.Instance.AddListener<Character>(Messenger.EventType.TurnEnd, FreshStatusTurnEnd);
        Messenger.Instance.AddListener<Character>(Messenger.EventType.TurnStart, FreshStatusTurnStart);
    }

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
    }

    #region Status��غ�ˢ��
    public void FreshStatusTurnStart(Character character)
    {
        if (character != this)
        {
            return;
        }

        for (int i = currentStatus.Count - 1; i >= 0; i--)
        {
            if (currentStatus[i] != null)
            {
                if (currentStatus[i].duration == 0 && currentStatus[i].NotAutoDelete)  //�⻷��buff 0�غϱ��ִ���״̬ ����Ч������Remove
                {
                    continue;
                }
                if (currentStatus[i].duration == -1 || currentStatus[i].turnStartDuration == false)  //����ʱ�����޻�غϿ�ʼʱ����ʱ��
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
                if(currentStatus[i].duration == 0 && currentStatus[i].NotAutoDelete)  //�⻷��buff 0�غϱ��ִ���״̬ ����Ч������Remove
                {
                    continue;
                }
                if(currentStatus[i].duration == -1 || currentStatus[i].turnStartDuration)  //����ʱ�����޻�غϿ�ʼʱ����ʱ��
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
        foreach(var depend in s.dependValues)
        {
            if(depend.dependTarget == Status.DependTarget.Owner)
            {
                if (depend.property == Status.InvolvedProperty.BrokenFocus)
                {
                    dependValue += depend.values[0] * characterData.BrokensFocus * 0.01f + depend.correctValues;   //������buff  ����100% �����ع�
                }
            }

            else if(depend.dependTarget == Status.DependTarget.Caster)
            {
                if(depend.property == Status.InvolvedProperty.CriticalDamageValue)
                {
                    dependValue += depend.values[0] * s.Caster.characterData.criticalDamage + depend.correctValues;  //�ӱ���buff  ����ʩ����24% + 45%����
                }
                else if(depend.property == Status.InvolvedProperty.HealthValue)
                {
                    dependValue += depend.values[0] * s.Caster.characterData.maxHealth + depend.correctValues;  //�ӷ����������ֵ��6%
                }
            }
            else if(depend.dependTarget == Status.DependTarget.Global)
            {
                if(depend.property == Status.InvolvedProperty.CertainElementPlayerNumber)
                {
                    int number = Mathf.Max(StaticNumber.GetPlayerNumber(depend.certainElement) - 1,0);
                    dependValue += depend.values[number] + depend.correctValues;   //�ӹ���buff  ���泡��ĳ���Խ�ɫ����  ������ֵ������ ����������ȡ����
                }
            }
        }
        return dependValue;

    }
    public void FreshCertainProperty(Status.InvolvedProperty type)
    {

    }


    //�����buffʱ ��buff�������Ͷ�Ӧ��ֵ�仯 ��Fresh��Ӧ����
    //����ɫ�б����⻷buffʱ(��פ����ת����) ����buff�漰����������  �������Է����仯ʱ���ö�ӦInvolved���Բ���  �����漰��Status Fresh��Ӧ����
    //�����buffʱ �������Ͳ���ӦFresh����   �����漰������Ա仯   �� ��buffʹ����һ��buff����ֵ����
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
                        tempAttackBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                        tempHealthBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempDamageIncreaseBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0] + s.attachOtherStatus.AddValue) * s.StatusLayer);
                        }
                        else
                        {
                            tempDamageIncreaseBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
                        }
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
                        tempBrokenFocusBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempCriticalDamageBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0] + s.attachOtherStatus.AddValue) * s.StatusLayer);
                        }
                        else
                        {
                            tempCriticalDamageBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0]) * s.StatusLayer);
                        }
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
                        tempEffectPercentBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                        tempEffectDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                #region ö�����Կ���
                float BINGDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendPercentBonus)
                    {
                        if(s.involvedElement == CharacterData_SO.weaknessType.BING || s.involvedElement == CharacterData_SO.weaknessType.NONE)
                        {
                            BINGDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            HUODefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            FENGDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            LEIDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            WULIDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            XUSHUDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                            LIANGZIDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
                        }
                    }
                }
                characterData.currentLIANGZIDefend = characterData.LIANGZIDefend + LIANGZIDefendBonus;
                #endregion
            }
            else if (type == Status.InvolvedProperty.PureDefendValue)
            {
                float tempDefendBonus = 0;
                foreach (var s in currentStatus)
                {
                    if (s.statusType == Status.StatusType.DefendValueBonus)
                    {
                        tempDefendBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
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
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempCriticalPercentBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0] + s.attachOtherStatus.AddValue) * s.StatusLayer);
                        }
                        else
                        {
                            tempCriticalPercentBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0]) * s.StatusLayer);
                        }
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
                        if (s.IsAttached && currentStatus.Find(e => e.StatusName == s.attachOtherStatus.StatusName))
                        {
                            tempDamageDecreaseBonus += s.IsDepend ? GetDependValue(s) : ((s.StatusValue[0] + s.attachOtherStatus.AddValue) * s.StatusLayer);
                        }
                        else
                        {
                            tempDamageDecreaseBonus += s.IsDepend ? GetDependValue(s) : (s.StatusValue[0] * s.StatusLayer);
                        }
                    }
                }

                if (status.ValueLimited > 1e-6)
                {
                    tempDamageDecreaseBonus = Mathf.Min(tempDamageDecreaseBonus, status.ValueLimited);
                }
                characterData.damageDecrease = tempDamageDecreaseBonus;
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
        weaknessImage.Clear();  //Clear���ᵼ��GameObject��Destroy

        foreach (var weak in characterData.weakness)
        {
            int Index = characterData.weakness.IndexOf(weak);
            if(weak != CharacterData_SO.weaknessType.NONE)
            {
                GameObject newWeak = Instantiate(GameManager.Instance.Elements[(int)weak]);   //Ҫע���Ӧ
                newWeak.transform.position = this.transform.position + new Vector3(-15 + 7 * Index, -22f, 0);
                weaknessImage.Add(newWeak);
            }
        }
    }

    public void FreshInformation()
    {
        string context = "";

        context += "����������:" + characterData.baseAttack.ToString() + "\n";
        context += "�������ӳ�:" + (characterData.AttackPercentBonus * 100).ToString() + "%" + " + " + characterData.fixAttackBonus + "\n";
        context += "��ǰ������:" + characterData.currentAttack.ToString() + "\n";

        context += "��������ֵ:" + characterData.baseHealth.ToString() + "\n";
        context += "����ֵ�ӳ�:" + (characterData.healthPercentBonus * 100).ToString() + "%" + " + " + characterData.fixHealthBonus + "\n";
        context += "��ǰ����ֵ:" + characterData.currentHealth.ToString() + "\n";
        context += "�������ֵ:" + characterData.maxHealth.ToString() + "\n";


        context += "��������ֵ:" + characterData.baseDefend.ToString() + "\n";
        context += "����ֵ�ӳ�:" + (characterData.DefendPercentBonus * 100).ToString() + "%" + " + " + characterData.fixDefendBonus + "\n";
        context += "��ǰ����ֵ:" + characterData.currentDefend.ToString() + "\n";

        context += "�����ٶ�:" + characterData.baseSpeed.ToString() + "\n";
        context += "�ٶȼӳ�:" + (characterData.speedPercentBonus * 100).ToString() + "%" + " + " + characterData.fixSpeedBonus + "\n";
        context += "��ǰ�ٶ�:" + characterData.currentSpeed.ToString() + "\n";

        context += "Ч������:" + (characterData.effectPercent * 100).ToString() + "%" + "\n";
        context += "Ч���ֿ�:" + (characterData.effectDefend * 100).ToString() + "%" + " + " + characterData.fixSpeedBonus + "\n";

        context += "�˺��ӳ�:" + (characterData.damageIncrease * 100).ToString() + "%" + "\n";
        context += "�����ع�:" + (characterData.BrokensFocus).ToString() + "%" + "\n";
        context += "����Ч��:" + (characterData.BrokenEfficiencyBonus * 100).ToString() + "\n";

        context += "������:" + (characterData.criticalPercent * 100).ToString() + "%" + "\n";
        context += "�����˺�:" + (characterData.criticalDamage * 100).ToString() + "%" + "\n";
        context += "����:" + characterData.currentEnergyValue.ToString() + "/" + characterData.maxEnergyValue.ToString() + "\n\n";

        foreach (var status in currentStatus)
        {
            //if (status.StatusName.Contains("BD"))
            //{
            //    continue;
            //}
            //context += status.description + "\n";
            context += status.name + " ����: " + status.duration.ToString() + " ����: " + status.StatusLayer + " ��ֵ:";

            if(status.dependValues.Count > 0)
            {
                context += GetDependValue(status).ToString() +"(����) ";
            }
            else
            {
                foreach (var value in status.StatusValue)
                {
                    context += value.ToString() + "  ";
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
