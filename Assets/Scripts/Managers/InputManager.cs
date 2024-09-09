using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public GameObject ActionPanel, actionBar;
    public GameObject SpecialActionPanel;
    public Character currentSelectEnemy,currentSelectPlayer;
    public GameObject IconQ, IconE, IconR;
    public enum CurrentInputStateEnum
    {
        Q,
        E,
        R,
        None,
    }
    public enum CurrentGameState
    {
        Outside,
        Inside,
        None,
    }
    public CurrentGameState currentGameState = CurrentGameState.Outside;
    private CurrentInputStateEnum currentInputState;
    public CurrentInputStateEnum CurrentInputState
    {
        get
        {
            return currentInputState;
        }
        set
        {
            currentInputState = value;
            IconQ.GetComponent<Image>().color = new Color(1, 1, 1, 81 / 255f);
            IconE.GetComponent<Image>().color = new Color(1, 1, 1, 81 / 255f);
            IconR.GetComponent<Image>().color = new Color(1, 1, 1, 81 / 255f);

            if(value == CurrentInputStateEnum.Q)
            {
                IconQ.GetComponent<Image>().color = new Color(1, 215/255f, 0, 150 / 255f);
            }
            else if(value == CurrentInputStateEnum.E)
            {
                IconE.GetComponent<Image>().color = new Color(1, 215 / 255f, 0, 150 / 255f);
            }
            else if(value == CurrentInputStateEnum.R)
            {
                IconR.GetComponent<Image>().color = new Color(1, 215 / 255f, 0, 150 / 255f);
            }
        }
    }

    public Character currentActionCharacter;
    public float distance = 10000f;

    public float randomAttackTimeLeft;   //TODO:弹射攻击考虑两次之间相隔一段时间

    public CounterDown<Skill_SO, Character> enemyActionCounterDown;
    public Text ActionCounterDownText;


    public Transform SelectPanel;
    public GameObject SpritePrefab;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        Init();
        CurrentInputState = CurrentInputStateEnum.None;
        enemyActionCounterDown = new CounterDown<Skill_SO, Character>(StaticNumber.ActionTime, StaticNumber.ActionTime, null, null);
    }

    //维护一个特殊行动条队列   终结技 追加攻击等，其优先于行动条
    public Queue<Character> SpecialActionQueue = new Queue<Character>();
    void Update()
    {
        //if(currentGameState == CurrentGameState.Outside)
        //{
        //    return;
        //}
        ActionCounterDownText.text = "当前人物" + currentActionCharacter.characterName +  "行动\n动画倒计时: " + enemyActionCounterDown.currentTime.ToString("f2");
        enemyActionCounterDown.Update();
        ActionExecute();
        ChangeTarget();

        #region 终结技前置条件通过  输入进入"是否释放终结技"状态
        if (TryingCastFinalSkill)
        {
            Character player = SpecialActionQueue.Peek();
            if (player.skillFinal.target == Skill_SO.Target.Enemy)
            {
                if (currentSelectEnemy == null)
                {
                    FindFirstTarget();
                }
            }
            else
            {
                if (currentSelectPlayer == null)
                {
                    FindFirstFriend();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SpecialActionQueue.Dequeue();
                FreshSpecialAction();


                if (SpecialActionQueue.Count == 0)
                {
                    CurrentInputState = CurrentInputStateEnum.None;
                    TryingCastFinalSkill = false;
                    enemyActionCounterDown.ResetTimer();
                    enemyActionCounterDown.pauseFlag = false;
                }
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {

                SpecialActionQueue.Dequeue();
                SkillExecute(player.skillFinal, player);
                FreshSpecialAction();


                if (SpecialActionQueue.Count == 0)
                {
                    CurrentInputState = CurrentInputStateEnum.None;
                    TryingCastFinalSkill = false;
                    enemyActionCounterDown.ResetTimer();
                    enemyActionCounterDown.pauseFlag = false;
                }

            }
        }
        #endregion
    }
    public void FreshSpecialAction()
    {
        for(int i = 0; i < SpecialActionPanel.transform.childCount; i++)
        {
            Destroy(SpecialActionPanel.transform.GetChild(i).gameObject);
        }
        foreach(var player in SpecialActionQueue)
        {
            GameObject newActionBar = Instantiate(actionBar, SpecialActionPanel.transform);
            newActionBar.transform.GetChild(0).GetComponent<Image>().sprite = player.sprite;
            newActionBar.transform.GetChild(1).GetComponent<Text>().text = "FinalSkill";
        }
    }
    /// <summary>
    /// A D切换目标
    /// </summary>
    public void ChangeTarget()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentSelectEnemy?.FindLeftTarget() != null)
            {
                Character cancelFlagCharacter = currentSelectEnemy;
                currentSelectEnemy = currentSelectEnemy.FindLeftTarget();
                Messenger.Instance.BroadCast(Messenger.EventType.ChangeTarget, cancelFlagCharacter, currentSelectEnemy);
            }
            if(currentSelectPlayer?.FindLeftTarget() != null)
            {
                Character cancelFlagCharacter = currentSelectPlayer;
                currentSelectPlayer = currentSelectPlayer.FindLeftTarget();
                Messenger.Instance.BroadCast(Messenger.EventType.ChangeTarget, cancelFlagCharacter, currentSelectPlayer);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentSelectEnemy?.FindRightTarget() != null)
            {
                Character cancelFlagCharacter = currentSelectEnemy;
                currentSelectEnemy = currentSelectEnemy.FindRightTarget();
                Messenger.Instance.BroadCast(Messenger.EventType.ChangeTarget, cancelFlagCharacter, currentSelectEnemy);
            }
            if (currentSelectPlayer?.FindRightTarget() != null)
            {
                Character cancelFlagCharacter = currentSelectPlayer;
                currentSelectPlayer = currentSelectPlayer.FindRightTarget();
                Messenger.Instance.BroadCast(Messenger.EventType.ChangeTarget, cancelFlagCharacter, currentSelectPlayer);
            }
        }
    }
    /// <summary>
    /// Q E行动
    /// </summary>

    public List<KeyCode> finalSkillKeyCodes = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};
    public void ActionExecute()
    {
        #region FinalSkill开启 不需要知道当前谁在行动
        for (int i = 0; i <= 3; i++)
        {
            if (Input.GetKeyDown(finalSkillKeyCodes[i]))
            {
                var player = GameManager.Instance.players[i];
                var skill = player.skillFinal;
                if (player != null)
                {
                    if(SpecialActionQueue.Contains(player))
                    {
                        GameManager.Instance.ShowPanelText("casting finalSkill now!");
                        return;
                    }
                    if (skill.energyConsumed > GameManager.Instance.players[i].characterData.currentEnergyValue)
                    {
                        GameManager.Instance.ShowPanelText("no enough energy!");
                        return;
                    }

                    CurrentInputState = CurrentInputStateEnum.R;
                    SpecialActionQueue.Enqueue(player);
                    FreshSpecialAction();
                    TryCastFinalSkill(skill, player);
                }
            }
        }
        #endregion

        #region 敌人回合行动
        if (currentActionCharacter.type == Character.CharaterType.Enemy && enemyActionCounterDown.currentTime <= 0)
        {
            List<Character> player = new List<Character>();
            foreach (var p in GameManager.Instance.players)
            {
                if(p != null)
                {
                    player.Add(p);
                }
            }
            GameManager.Instance.ShowPanelText($"Enemy {currentActionCharacter.CurrentIndex} Action");

            int randomTargetIndex = Random.Range(0, player.Count);
            DamageAction.DealDamageAction(currentActionCharacter, player[randomTargetIndex]);
            Messenger.Instance.BroadCast(Messenger.EventType.SettleDeath);

            FreshAction();

            enemyActionCounterDown.ResetTimer();
            return;
        }

        #endregion
        if (currentSelectEnemy == null && currentSelectPlayer == null)
        {
            FindFirstTarget();
        }

        if(currentActionCharacter.type == Character.CharaterType.Player && CurrentInputState != CurrentInputStateEnum.R)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Skill_SO skill = currentActionCharacter.skillQ;
                if (CurrentInputState == CurrentInputStateEnum.Q)
                {
                    SkillExecute(skill, currentActionCharacter);
                    CurrentInputState = CurrentInputStateEnum.None;
                }
                else
                {
                    CurrentInputState = CurrentInputStateEnum.Q;
                    if (skill.target == Skill_SO.Target.Enemy)
                    {
                        if (currentSelectEnemy == null)
                        {
                            FindFirstTarget();
                        }
                    }
                    else
                    {
                        if (currentSelectPlayer == null)
                        {
                            FindFirstFriend();
                        }
                    }
                }

            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Skill_SO skill = currentActionCharacter.skillE;
                if (CurrentInputState == CurrentInputStateEnum.E)
                {
                    SkillExecute(skill, currentActionCharacter);
                    CurrentInputState = CurrentInputStateEnum.None;
                }
                else
                {
                    CurrentInputState = CurrentInputStateEnum.E;
                    if (skill.target == Skill_SO.Target.Enemy)
                    {
                        if(currentSelectEnemy == null)
                        {
                            FindFirstTarget();
                        }
                    }
                    else
                    {
                        if(currentSelectPlayer == null)
                        {
                            FindFirstFriend();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 找到第一个可以选中的敌人
    /// </summary>
    public void FindFirstTarget()
    {
        foreach (var enemy in GameManager.Instance.enemies)
        {
            if (enemy != null)
            {
                currentSelectEnemy = enemy;
                if(currentSelectPlayer != null)
                {
                    currentSelectPlayer.TargetFlag.SetActive(false);
                    currentSelectPlayer = null;
                }
                enemy.TargetFlag.SetActive(true);
                return;
            }
        }
    }
    public void FindFirstFriend()
    {
        foreach (var player in GameManager.Instance.players)
        {
            if (player != null)
            {
                if(currentSelectEnemy != null)
                {
                    currentSelectEnemy.TargetFlag.SetActive(false);
                    currentSelectEnemy = null;
                }
                currentSelectPlayer = player;
                player.TargetFlag.SetActive(true);
                return;
            }
        }
    }

    public List<Character> characters = new List<Character>();
    public Dictionary<Character,GameObject> Dict = new Dictionary<Character,GameObject>();
    public void FreshAction()
    {
        if(currentActionCharacter != null)
        {
            Messenger.Instance.BroadCast(Messenger.EventType.TurnEnd, currentActionCharacter);
        }

        Dict.Clear();
        characters.Clear();
        for (int i = 0; i < ActionPanel.transform.childCount; i++)
        {
            Destroy(ActionPanel.transform.GetChild(i).gameObject);
        }


        foreach (var player in GameManager.Instance.players)
        {
            if (player != null)
            {
                characters.Add(player);
            }
        }
        foreach (var enemy in GameManager.Instance.enemies)
        {
            if (enemy != null)
            {
                characters.Add(enemy);
            }
        }
        characters.Sort((a, b) => { return (a.characterData.actionValue > b.characterData.actionValue) ? 1 : -1; });

        float passActionValue = characters[0].characterData.actionValue;
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].characterData.actionValue -= passActionValue;
            GameObject newActionBar = Instantiate(actionBar, ActionPanel.transform);
            newActionBar.transform.GetChild(0).GetComponent<Image>().sprite = characters[i].sprite;
            newActionBar.transform.GetChild(1).GetComponent<Text>().text = characters[i].characterData.actionValue.ToString("f2");
            Dict.Add(characters[i], newActionBar);
        }
        characters[0].characterData.actionValue = distance / characters[0].characterData.currentSpeed;
        currentActionCharacter = characters[0];

        if (currentActionCharacter != null)
        {
            Messenger.Instance.BroadCast(Messenger.EventType.TurnStart, currentActionCharacter);
        }
        if (GameManager.Instance.mouseDownCharacter != null)
        {
            Dict[GameManager.Instance.mouseDownCharacter].GetComponent<Image>().color = new Color(0f, 1f, 0f, 81 / 255f);
        }
    }
    public void Init()
    {
        foreach(var player in GameManager.Instance.AllPlayers)
        {
            GameObject newImage = Instantiate(SpritePrefab,SelectPanel.GetChild(0));
            newImage.GetComponent<Image>().sprite = player.GetComponent<Player>().sprite;
            newImage.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Elements[(int)player.GetComponent<Player>().data.elementType].GetComponent<SpriteRenderer>().sprite;

            var component = newImage.GetComponent<SelectPlayer>();

            component.PlayerName = player.GetComponent<Player>().characterName;
            component.playerIndex = GameManager.Instance.AllPlayers.IndexOf(player);
            
        }
        #region

        GameObject newEnmey = Instantiate(GameManager.Instance.AllEnemy[0]);
        newEnmey.GetComponent<Enemy>().currentIndex = 0;
        newEnmey.transform.position = GameManager.Instance.enemySlots[0].position;

        GameObject newEnmey2 = Instantiate(GameManager.Instance.AllEnemy[1]);
        newEnmey2.GetComponent<Enemy>().currentIndex = 1;
        newEnmey2.transform.position = GameManager.Instance.enemySlots[1].position;

        GameObject newEnmey3 = Instantiate(GameManager.Instance.AllEnemy[1]);
        newEnmey3.GetComponent<Enemy>().currentIndex = 2;
        newEnmey3.transform.position = GameManager.Instance.enemySlots[2].position;

        GameObject newEnmey4 = Instantiate(GameManager.Instance.AllEnemy[0]);
        newEnmey4.GetComponent<Enemy>().currentIndex = 3;
        newEnmey4.transform.position = GameManager.Instance.enemySlots[3].position;

        GameObject newEnmey5 = Instantiate(GameManager.Instance.AllEnemy[1]);
        newEnmey5.GetComponent<Enemy>().currentIndex = 4;
        newEnmey5.transform.position = GameManager.Instance.enemySlots[4].position;

        GameObject newEnmey6 = Instantiate(GameManager.Instance.AllPlayers[0]);
        newEnmey6.GetComponent<Player>().currentIndex = 0;
        newEnmey6.transform.position = GameManager.Instance.playerSlots[0].position;

        GameObject newEnmey7 = Instantiate(GameManager.Instance.AllPlayers[1]);
        newEnmey7.GetComponent<Player>().currentIndex = 1;
        newEnmey7.transform.position = GameManager.Instance.playerSlots[1].position;

        GameObject newEnmey8 = Instantiate(GameManager.Instance.AllPlayers[2]);
        newEnmey8.GetComponent<Player>().currentIndex = 2;
        newEnmey8.transform.position = GameManager.Instance.playerSlots[2].position;

        GameObject newEnmey9 = Instantiate(GameManager.Instance.AllPlayers[3]);
        newEnmey9.GetComponent<Player>().currentIndex = 3;
        newEnmey9.transform.position = GameManager.Instance.playerSlots[3].position;

        #endregion

        FreshAction();

        // Player Instantiate后 Awake紧跟着调用 然后这一帧代码调用 最后调用Start

        #region 角色在场光环效果
        for (int i = 0; i < GameManager.Instance.players.Count; i++)
        {
            var player = GameManager.Instance.players[i];
            if (player == null)
            {
                continue;
            }

            for (int j = 0; j < player.currentStatus.Count; j++)
            {
                if (player.currentStatus[j] == null)
                {
                    continue;
                }
                if (player.currentStatus[j].statusType != Status.StatusType.FieldStatus)
                {
                    continue;
                }

                for (int k = 0; k < player.currentStatus[j].fieldEffects.Count; k++)
                {
                    if (player.currentStatus[j].fieldEffects[k].field == Status.FieldTarget.AllFriends)
                    {
                        if(player.currentStatus[j].fieldEffects[k].limit == Status.PlayerLimit.Element)
                        {
                            StatusAction.AddStatusAllFriend(player, player.currentStatus[j].fieldEffects[k].status, player.currentStatus[j].fieldEffects[k].ElementType);
                        }
                        else if(player.currentStatus[j].fieldEffects[k].limit == Status.PlayerLimit.None)
                        {
                            StatusAction.AddStatusAllFriend(player, player.currentStatus[j].fieldEffects[k].status);
                        }
                    }
                    else if (player.currentStatus[j].fieldEffects[k].field == Status.FieldTarget.Global)
                    {
                        GameManager.Instance.maxSkillPoint += (int)player.currentStatus[j].fieldEffects[k].status.StatusValue[0];
                    }
                }
            }
        }
        #endregion

    }
    [HideInInspector]
    public bool TryingCastFinalSkill = false;
    public void TryCastFinalSkill(Skill_SO skill, Character executeCharacter)
    {
        GameManager.Instance.ShowPanelText("Try Cast Final Skill");
        if (skill.damageType == Skill_SO.DamageType.FinalAttack)
        {
            enemyActionCounterDown.currentTime = 20f;
            enemyActionCounterDown.pauseFlag = true;

            TryingCastFinalSkill = true;
        }
    }
    private CharacterData_SO.weaknessType tempType;
    public void SkillExecute(Skill_SO skill, Character executeCharacter)
    {
        #region 技能释放前时点


        if (skill.skillPointConsumed > 0)
        {
            if(skill.skillPointConsumed > GameManager.Instance.skillPoint)
            {
                GameManager.Instance.ShowPanelText("no enough skillPoint");
                return;
            }
            else
            {
                GameManager.Instance.skillPoint -= skill.skillPointConsumed;
                Messenger.Instance.BroadCast(Messenger.EventType.SkillPointChange, -skill.skillPointConsumed);
            }

        }
        if(skill.skillPointProvide > 0)
        {
            GameManager.Instance.skillPoint += skill.skillPointProvide;
            Messenger.Instance.BroadCast(Messenger.EventType.SkillPointChange, skill.skillPointProvide);
        }

        if(skill.damageType == Skill_SO.DamageType.FinalAttack && executeCharacter.characterData.currentEnergyValue >= skill.energyConsumed)
        {
            executeCharacter.characterData.currentEnergyValue = 0f;
            Messenger.Instance.BroadCast(Messenger.EventType.CastFinalSkill);
        }
        #endregion

        if (skill.skillID == "1032")   //超级特判！ 银狼E技能添加buff时获取即将添加的弱点属性 对应减抗性
        {
            List<CharacterData_SO.weaknessType> temp = new List<CharacterData_SO.weaknessType>();
            foreach (var player in GameManager.Instance.players)
            {
                if (player != null)
                {
                    if (temp.Contains(player.characterData.elementType) == false)
                    {
                        temp.Add(player.characterData.elementType);
                    }
                }
            }
            int randomIndex = Random.Range(0, temp.Count);
            tempType = temp[randomIndex];
        }
        foreach (var action in skill.addactions)
        {
            if (action.AfterDamage == false)
            {
                ExecuteAction(action);
            }
        }



        if (skill.target == Skill_SO.Target.Enemy)
        {
            foreach(var skillType in skill.skillType)   //一个技能涉及优先级时(先添加buff还是先造成伤害  可以通过填表顺序实现)
            {
                if (skillType == Skill_SO.SkillType.DealDamage)   //造成伤害可能涉及多个倍率  索引与读表人为规定
                {
                    #region 不同伤害类型
                    if (skill.attackType == Skill_SO.TargetType.SingleTarget)
                    {
                        DamageInfo info = DamageAction.DealDamageAction(executeCharacter, skill, currentSelectEnemy, 0);
                        Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                    }
                    else if (skill.attackType == Skill_SO.TargetType.NeighborTarget)
                    {
                        DamageInfo info = new DamageInfo(executeCharacter, null, skill);
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, currentSelectEnemy, 0).toughDamage;
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, currentSelectEnemy.FindLeftTarget(), 1).toughDamage;
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, currentSelectEnemy.FindRightTarget(), 1).toughDamage;

                        Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                    }
                    else if (skill.attackType == Skill_SO.TargetType.AllTarget)
                    {
                        DamageInfo info = new DamageInfo(executeCharacter, null, skill);
                        foreach (var enemy in GameManager.Instance.enemies)
                        {
                            info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, enemy, 0).toughDamage;
                        }
                        Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                    }
                    else if (skill.attackType == Skill_SO.TargetType.RandomTarget)
                    {

                        string attackTarget = "攻击目标为 :";
                        for (int i = 0; i < skill.RandomAttackNumber; i++)
                        {
                            List<Character> enemies = new List<Character>();
                            foreach (var enemy in GameManager.Instance.enemies)
                            {
                                if (enemy != null && enemy.characterData.currentHealth > 0)
                                {
                                    enemies.Add(enemy);
                                }
                            }

                            int randomIndex = Random.Range(0, enemies.Count);
                            attackTarget += randomIndex.ToString() + "  ,";
                            DamageAction.DealDamageAction(executeCharacter, skill, enemies[randomIndex], 0);
                        }


                        GameManager.Instance.ShowPanelText(attackTarget);
                    }
                    #endregion
                    Messenger.Instance.BroadCast(Messenger.EventType.SettleDeath);
                }
                else if(skillType == Skill_SO.SkillType.AddStatus)  //暂时只有对敌人添加buff时涉及 命中概率计算
                {
                    foreach(var s in skill.addStatus)
                    {
                        int StatusIndex = skill.addStatus.IndexOf(s);
                        if (s.statusTarget == Skill_SO.TargetType.SingleTarget)
                        {
                            if (skill.basePercent[StatusIndex] * (currentActionCharacter.characterData.effectPercent + 1) * (1 - currentSelectEnemy.characterData.effectDefend) > Random.Range(0f, 1f))
                            {
                                var cloneStatus = Instantiate(s.status);
                                if (s.status.StatusName == "1035")
                                {
                                    cloneStatus.involvedElement = tempType;
                                }
                                StatusAction.AddStatusAction(currentActionCharacter, currentSelectEnemy, cloneStatus);
                            }
                            else
                            {
                                GameManager.Instance.ShowPanelText("效果抵抗 !");
                            }
                        }

                    }
                }
            }
        }
        else if(skill.target == Skill_SO.Target.Friend)
        {
            foreach(var skillType in skill.skillType)
            {
                if (skillType == Skill_SO.SkillType.AddStatus)
                {
                    foreach(var statusStruct in skill.addStatus)
                    {
                        int StatusIndex = skill.addStatus.IndexOf(statusStruct);
                        if (statusStruct.statusTarget == Skill_SO.TargetType.SingleTarget)
                        {
                            var cloneStatus = Instantiate(statusStruct.status);
                            StatusAction.AddStatusAction(currentActionCharacter, currentSelectPlayer, cloneStatus);
                        }
                        else if (statusStruct.statusTarget == Skill_SO.TargetType.AllTarget)
                        {
                            StatusAction.AddStatusAllFriend(currentActionCharacter, statusStruct.status);
                        }
                        else if(statusStruct.statusTarget == Skill_SO.TargetType.AllOtherFriend)
                        {
                            StatusAction.AddStatusAllOtherFriend(currentActionCharacter, statusStruct.status);
                        }
                    }
                }
                else if(skillType == Skill_SO.SkillType.Healing)
                {
                    if(skill.attackType == Skill_SO.TargetType.Self)
                    {
                        HealingAction.DealHealingAction(executeCharacter, executeCharacter, skill);
                    }
                    else if(skill.attackType == Skill_SO.TargetType.AllTarget)
                    {
                        HealingAction.DealHealingAllAction(executeCharacter, skill);
                    }
                }
            }
        }
        foreach (var action in skill.addactions)
        {
            if (action.AfterDamage == true)
            {
                ExecuteAction(action);
            }
        }
        if (skill.damageType != Skill_SO.DamageType.FinalAttack)
        {
            FreshAction();
        }
        enemyActionCounterDown.ResetTimer();
    }
    
    public void ExecuteAction(Skill_SO.Actions action)
    {
        if (action.addaction == Skill_SO.AddAction.PushActon)
        {
            PushActionValueAction.PushActionValue(currentSelectPlayer, action.value);
        }
        if (action.addaction == Skill_SO.AddAction.GetSkillPoint)
        {
            GameManager.Instance.skillPoint = GameManager.Instance.skillPoint + 4;
            Messenger.Instance.BroadCast(Messenger.EventType.SkillPointChange, GameManager.Instance.skillPoint);
        }
        if (action.addaction == Skill_SO.AddAction.AddWeakness)
        {
            if (currentSelectEnemy != null)
            {
                currentSelectEnemy.characterData.weakness.Add(tempType);
                currentSelectEnemy.ShowSelfWeakness();
            }
        }
    }
}
