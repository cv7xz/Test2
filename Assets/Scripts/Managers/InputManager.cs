using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
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
    public CurrentGameState currentGameState = CurrentGameState.Inside;
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
    public const float distance = 10000f;

    public float randomAttackTimeLeft;   //TODO:弹射攻击考虑两次之间相隔一段时间

    public CounterDown<Skill_SO, Character> enemyActionCounterDown;
    public Text ActionCounterDownText;
    public GameObject currentCharacterTip;

    public Transform SelectPanel;
    public GameObject SpritePrefab,EnemySlotPrefab;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        FeiXiaoSpecialQ = Instantiate(FeiXiaoSpecialQObject);
        FeiXiaoSpecialE = Instantiate(FeiXiaoSpecialEObject);
        FeiXiaoFinalEnd = Instantiate(FeiXiaoFinalEndObject);
    }
    void Start()
    {
        Init();
        CurrentInputState = CurrentInputStateEnum.None;
        enemyActionCounterDown = new CounterDown<Skill_SO, Character>(StaticNumber.ActionTime, StaticNumber.ActionTime, null, null);
    }

    //维护一个特殊行动条队列   终结技 追加攻击等，其优先于行动条
    public struct SpecialAction
    {
        public Character executeCharacter;
        public Skill_SO skill;
        public Character Target;
        public int index;
        public SpecialAction(Character a,Skill_SO b,Character c = null, int d = 0)
        {
            executeCharacter = a;
            skill = b;
            Target = c;
            index = d;
        }
    }

    private bool FeiXiaoFinalSkillState;
    private int FeiXiaoFinalSkillCount = 0;
    public Skill_SO FeiXiaoSpecialQObject, FeiXiaoSpecialEObject,FeiXiaoFinalEndObject;
    public Skill_SO FeiXiaoSpecialQ, FeiXiaoSpecialE, FeiXiaoFinalEnd;
    public LinkedList<SpecialAction> SpecialActionList = new LinkedList<SpecialAction>();
    void Update()
    {
        if (currentGameState == CurrentGameState.Outside)
        {
            return;
        }
        ActionCounterDownText.text = "当前人物" + currentActionCharacter.characterName +  "行动\n动画倒计时: " + enemyActionCounterDown.currentTime.ToString("f2");
        currentCharacterTip.GetComponent<Image>().sprite = currentActionCharacter.GetComponent<Character>().sprite;
        enemyActionCounterDown.Update();

        FinalSkillCommand();     //Player的终结技指令  数字键 1 2 3 4
        ActionExecute();         //追加攻击 敌人的行动 
        PlayerActionCommand();   //Player的Q/E  指令

        if(FeiXiaoFinalSkillState == false)   //终结技状态
        {
            ChangeTarget();
        }

        ConfirmFinalSkill();

    }
    public void FreshSpecialAction()
    {
        for(int i = 0; i < SpecialActionPanel.transform.childCount; i++)
        {
            Destroy(SpecialActionPanel.transform.GetChild(i).gameObject);
        }

        List<SpecialAction> tempSpecialAction = new List<SpecialAction>(SpecialActionList);
        tempSpecialAction.Sort((a, b) =>
        {
            if (a.skill.damageType != b.skill.damageType)
            {
                return ((int)a.skill.damageType).CompareTo((int)b.skill.damageType);
            }
            else
            {
                return a.index.CompareTo(b.index);
            }
        });      //追加攻击优先，且按照时间顺序排列，之后是终结技的顺序

        SpecialActionList.Clear();

        foreach (var action in tempSpecialAction)
        {
            SpecialActionList.AddLast(action);
            GameObject newActionBar = Instantiate(actionBar, SpecialActionPanel.transform);
            newActionBar.transform.GetChild(0).GetComponent<Image>().sprite = action.executeCharacter.sprite;

            if(action.skill.damageType == Skill_SO.DamageType.FinalAttack)
            {
                newActionBar.transform.GetChild(1).GetComponent<Text>().text = "FinalSkill";
            }
            else if(action.skill.damageType == Skill_SO.DamageType.ExtraAttack)
            {
                newActionBar.transform.GetChild(1).GetComponent<Text>().text = "ExtraSkill";
            }
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
    public bool SkipAction = false;
    public List<KeyCode> finalSkillKeyCodes = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};

    public void FinalSkillCommand()
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
                    foreach(var specialAction in SpecialActionList)
                    {
                        if(specialAction.executeCharacter == player && specialAction.skill == player.skillFinal)
                        {
                            GameManager.Instance.ShowPanelText("casting finalSkill now!");
                            return;
                        }
                    }

                    if (skill.specialFinalSkill == Skill_SO.SpecialFinalSkill.FEIXIAO)
                    {
                        var skillPointStatus = player.currentStatus.Find(e => e.StatusName == "1051");
                        if (skillPointStatus.StatusLayer < 12)
                        {
                            GameManager.Instance.ShowPanelText("no enough energy!");
                            return;
                        }
                    }
                    else if (skill.energyConsumed > GameManager.Instance.players[i].characterData.currentEnergyValue)
                    {
                        GameManager.Instance.ShowPanelText("no enough energy!");
                        return;
                    }

                    CurrentInputState = CurrentInputStateEnum.R;
                    SpecialActionList.AddLast(new SpecialAction(player, skill, null, GameManager.Instance.GlobalFinalTimePoint));
                    GameManager.Instance.GlobalFinalTimePoint += 1;
                    FreshSpecialAction();
                    //TryCastFinalSkill(skill, player);
                }
            }
        }
        #endregion
    }

    public void PlayerActionCommand()
    {
        if (currentActionCharacter.type == Character.CharaterType.Player && SpecialActionList.Count == 0 && CurrentInputState != CurrentInputStateEnum.R)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Skill_SO skill = currentActionCharacter.skillQ;
                if (CurrentInputState == CurrentInputStateEnum.Q)
                {
                    SkillExecute(skill, currentActionCharacter, currentSelectEnemy);
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
                    SkillExecute(skill, currentActionCharacter, currentSelectEnemy);
                    CurrentInputState = CurrentInputStateEnum.None;
                }
                else
                {
                    CurrentInputState = CurrentInputStateEnum.E;
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
        }
    }

    public void ConfirmFinalSkill()
    {
        #region 当前特殊行动列表第一位是FinalSkill   输入进入"是否释放终结技"状态
        if (SpecialActionList.Count > 0 && SpecialActionList.First.Value.skill.damageType == Skill_SO.DamageType.FinalAttack && FeiXiaoFinalSkillState == false)
        {
            Character player = SpecialActionList.First.Value.executeCharacter;
            if (player.skillFinal.specialFinalSkill == Skill_SO.SpecialFinalSkill.None)
            {
                ChangeTargetFirstCharacter(player.skillFinal);
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SpecialActionDequeue(ESC: true);

                    FreshSpecialAction();
                }

                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    SpecialActionDequeue(player.skillFinal, player, currentSelectEnemy);

                    FreshSpecialAction();

                }
            }
            else if (player.skillFinal.specialFinalSkill == Skill_SO.SpecialFinalSkill.FEIXIAO)
            {
                ChangeTargetFirstCharacter(player.skillFinal);
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SpecialActionDequeue(ESC: true);

                    FreshSpecialAction();
                }

                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    SkillExecute(player.skillFinal, player, currentSelectEnemy);
                    FeiXiaoFinalSkillState = true;
                }
            }
        }

        if (FeiXiaoFinalSkillState == true)
        {
            Character player = SpecialActionList.First.Value.executeCharacter;
            DamageInfo totalInfo = new DamageInfo(player, currentSelectEnemy, player.skillFinal);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                FeiXiaoFinalSkillCount += 1;
                totalInfo.toughDamage += SpeicalSkillExecute(FeiXiaoSpecialE, player, currentSelectEnemy).toughDamage;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                FeiXiaoFinalSkillCount += 1;
                totalInfo.toughDamage += SpeicalSkillExecute(FeiXiaoSpecialE, player, currentSelectEnemy).toughDamage;
            }

            if (FeiXiaoFinalSkillCount >= 6)
            {
                SpecialActionDequeue(FeiXiaoFinalEnd, player, currentSelectEnemy);  //TODO 要释放终结技结尾
                FreshSpecialAction();
                FeiXiaoFinalSkillCount = 0;
                FeiXiaoFinalSkillState = false;
                Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, totalInfo);
                Messenger.Instance.BroadCast(Messenger.EventType.SettleDeath);
            }
        }
        #endregion
    }
    public void ActionExecute()
    {

        if (currentSelectEnemy == null && currentSelectPlayer == null)
        {
            FindFirstTarget();
        }


        //当额外行动条有行动时 先处理额外  若为追加则直接执行 若终结技则根据输入执行     要按队列顺序执行！
        #region 敌人回合 或者追加类型行动 (不受控制 程序自行执行)

        //SpecialActionList 下一个特殊行动是追加类型 
        if (SpecialActionList.Count != 0 && SpecialActionList.First.Value.skill.damageType == Skill_SO.DamageType.ExtraAttack && enemyActionCounterDown.currentTime <= 0)
        {
            if(SpecialActionList.First.Value.Target != null)
            {
                SpecialActionDequeue(SpecialActionList.First.Value.skill, SpecialActionList.First.Value.executeCharacter, SpecialActionList.First.Value.Target);
                FreshSpecialAction();
            }
            else
            {
                Debug.Log($"{SpecialActionList.First.Value.skill.skillID} 没有目标！ 确认该技能是否是随机目标");
                SpecialActionDequeue(SpecialActionList.First.Value.skill, SpecialActionList.First.Value.executeCharacter);
                FreshSpecialAction();

            }
        }

        //敌人的行动  也由程序自动执行
        else if (currentActionCharacter.type == Character.CharaterType.Enemy && enemyActionCounterDown.currentTime <= 0 && SpecialActionList.Count == 0)
        {
            if (SkipAction == false)
            {
                List<Character> player = new List<Character>();
                foreach (var p in GameManager.Instance.players)
                {
                    if (p != null)
                    {
                        player.Add(p);
                    }
                }
                GameManager.Instance.ShowPanelText($"Enemy {currentActionCharacter.CurrentIndex} Action");

                int randomTargetIndex = UnityEngine.Random.Range(0, player.Count);
                DamageAction.DealDamageAction(currentActionCharacter, player[randomTargetIndex]);
                Messenger.Instance.BroadCast(Messenger.EventType.SettleDeath);

            }

            enemyActionCounterDown.ResetTimer();
            enemyActionCounterDown.AddEndAction(()=>FreshAction());
            
            return;
        }

        #endregion



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
    public void FreshAction(float dis = distance)
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
        GameManager.Instance.TotalActionValuePass += passActionValue;
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].characterData.actionValue -= passActionValue;
            GameObject newActionBar = Instantiate(actionBar, ActionPanel.transform);
            newActionBar.transform.GetChild(0).GetComponent<Image>().sprite = characters[i].sprite;
            newActionBar.transform.GetChild(1).GetComponent<Text>().text = characters[i].characterData.actionValue.ToString("f2");
            Dict.Add(characters[i], newActionBar);
        }

        characters[0].characterData.actionValue = dis / characters[0].characterData.currentSpeed;    //某个角色开始行动前行动值就已经设置了  应该要改
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

        CSVToolKit.LoadFile(Application.streamingAssetsPath + "/brokenDamage.csv");   //读数据表



        if (currentGameState == CurrentGameState.Outside)
        {
            foreach (var player in GameManager.Instance.AllPlayers)
            {
                GameObject newImage = Instantiate(SpritePrefab, SelectPanel.GetChild(0));
                newImage.GetComponent<Image>().sprite = player.GetComponent<Player>().sprite;
                newImage.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Elements[(int)player.GetComponent<Player>().data.elementType].GetComponent<SpriteRenderer>().sprite;

                var component = newImage.GetComponent<SelectPlayer>();

                component.PlayerName = player.GetComponent<Player>().characterName;
                component.playerIndex = GameManager.Instance.AllPlayers.IndexOf(player);

            }

            foreach (var equip in GameManager.Instance.AllEquip)
            {
                GameObject newImage = Instantiate(EnemySlotPrefab, SelectPanel.GetChild(1));
                newImage.GetComponent<Image>().sprite = equip.GetComponent<SpriteRenderer>().sprite;
                //newImage.transform.GetChild(1).GetComponent<Image>().sprite = GameManager.Instance.Elements[(int)player.GetComponent<Player>().data.elementType].GetComponent<SpriteRenderer>().sprite;

                var component = newImage.GetComponent<SelectEquip>();

                component.equipIndex = GameManager.Instance.AllEquip.IndexOf(equip);
            }
            SelectPanel.SetAsLastSibling();


        }

        #region
        else
        {
            GameObject newEnmey = Instantiate(GameManager.Instance.AllEnemy[0]);
            newEnmey.GetComponent<Enemy>().currentIndex = 0;
            newEnmey.transform.position = GameManager.Instance.enemySlots[0].position;

            GameObject newEnmey2 = Instantiate(GameManager.Instance.AllEnemy[1]);
            newEnmey2.GetComponent<Enemy>().currentIndex = 1;
            newEnmey2.transform.position = GameManager.Instance.enemySlots[1].position;

            GameObject newEnmey3 = Instantiate(GameManager.Instance.AllEnemy[0]);
            newEnmey3.GetComponent<Enemy>().currentIndex = 2;
            newEnmey3.transform.position = GameManager.Instance.enemySlots[2].position;

            GameObject newEnmey4 = Instantiate(GameManager.Instance.AllEnemy[0]);
            newEnmey4.GetComponent<Enemy>().currentIndex = 3;
            newEnmey4.transform.position = GameManager.Instance.enemySlots[3].position;

            GameObject newEnmey5 = Instantiate(GameManager.Instance.AllEnemy[0]);
            newEnmey5.GetComponent<Enemy>().currentIndex = 4;
            newEnmey5.transform.position = GameManager.Instance.enemySlots[4].position;

            GameObject newEnmey6 = Instantiate(GameManager.Instance.AllPlayers[0]);
            newEnmey6.GetComponent<Player>().currentIndex = 0;
            newEnmey6.transform.position = GameManager.Instance.playerSlots[0].position;

            GameObject newEnmey7 = Instantiate(GameManager.Instance.AllPlayers[5]);
            newEnmey7.GetComponent<Player>().currentIndex = 1;
            newEnmey7.transform.position = GameManager.Instance.playerSlots[1].position;

            GameObject newEnmey8 = Instantiate(GameManager.Instance.AllPlayers[4]);
            newEnmey8.GetComponent<Player>().currentIndex = 2;
            newEnmey8.transform.position = GameManager.Instance.playerSlots[2].position;

            GameObject newEnmey9 = Instantiate(GameManager.Instance.AllPlayers[3]);
            newEnmey9.GetComponent<Player>().currentIndex = 3;
            newEnmey9.transform.position = GameManager.Instance.playerSlots[3].position;


            #endregion

            FreshAction();

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
                            if (player.currentStatus[j].fieldEffects[k].limit == Status.PlayerLimit.Element)
                            {
                                StatusAction.AddStatusAllFriend(player, player.currentStatus[j].fieldEffects[k].status, player.currentStatus[j].fieldEffects[k].ElementType);
                            }
                            else if (player.currentStatus[j].fieldEffects[k].limit == Status.PlayerLimit.None)
                            {
                                StatusAction.AddStatusAllFriend(player, player.currentStatus[j].fieldEffects[k].status);
                            }
                        }
                        else if (player.currentStatus[j].fieldEffects[k].field == Status.FieldTarget.Global)   
                        {
                            if (player.currentStatus[j].fieldEffects[k].status.StatusName == "1015")
                            {
                                GameManager.Instance.maxSkillPoint += (int)player.currentStatus[j].fieldEffects[k].status.StatusValue[0];    //这个Status 1015 花火战技点上限 只用了其status
                            }

                        }
                    }
                }
            }
            #endregion
        }
    }
    public void TryCastFinalSkill(Skill_SO skill, Character executeCharacter)
    {
        GameManager.Instance.ShowPanelText("Try Cast Final Skill");
        if (skill.damageType == Skill_SO.DamageType.FinalAttack)
        {
            enemyActionCounterDown.currentTime = 20f;
            enemyActionCounterDown.pauseFlag = true;
        }
    }
    
    private CharacterData_SO.weaknessType tempType;

    //暂时给FeiXiao设计的
    public DamageInfo SpeicalSkillExecute(Skill_SO skill, Character executeCharacter, Character targetCharacter)
    {
        if(skill.attackType == Skill_SO.TargetType.SingleTarget)
        {
            if (targetCharacter == null)
            {
                var enemy = GameManager.Instance.GetRandomEnemy();
                DamageInfo info = DamageAction.DealDamageAction(executeCharacter, skill, enemy, 0);

                return info;
            }
            else
            {
                DamageInfo info = DamageAction.DealDamageAction(executeCharacter, skill, targetCharacter, 0);

                return info;
            }
        }

        return null;
    }
    public void SkillExecute(Skill_SO skill, Character executeCharacter, Character targetCharacter)
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

        if(skill.damageType == Skill_SO.DamageType.FinalAttack)
        {
            if (skill.specialFinalSkill == Skill_SO.SpecialFinalSkill.FEIXIAO)
            {
                var skillPointStatus = executeCharacter.currentStatus.Find(e => e.StatusName == "1051");
                skillPointStatus.StatusLayer -= 12;
            }
            else if (executeCharacter.characterData.currentEnergyValue >= skill.energyConsumed)
            {
                executeCharacter.characterData.currentEnergyValue = 0f;
            }
            Messenger.Instance.BroadCast(Messenger.EventType.CastFinalSkill,executeCharacter);
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
            int randomIndex = UnityEngine.Random.Range(0, temp.Count);
            tempType = temp[randomIndex];
        }

        if (skill.target == Skill_SO.Target.Enemy)
        {
            #region 效果执行前Action
            foreach (var action in skill.addactions)
            {
                if (action.AfterDamage == false)
                {
                    ExecuteAction(action, executeCharacter, targetCharacter);
                }
            }
            #endregion

            #region 效果执行
            foreach (var skillType in skill.skillType)   //一个技能涉及优先级时(先添加buff还是先造成伤害  可以通过填表顺序实现)
            {
                if (skillType == Skill_SO.SkillType.DealDamage)   //造成伤害可能涉及多个倍率  索引与读表人为规定
                {
                    #region 不同伤害类型
                    if (skill.attackType == Skill_SO.TargetType.SingleTarget)    //飞霄追加 主目标不存在时 随机选取
                    {
                        if(targetCharacter == null)
                        {
                            var enemy = GameManager.Instance.GetRandomEnemy();
                            DamageInfo info = DamageAction.DealDamageAction(executeCharacter, skill, enemy, 0);

                            Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                            Messenger.Instance.BroadCast(Messenger.EventType.TargetAndAttackEnemy, executeCharacter, enemy, skill);   //技能攻击
                        }
                        else
                        {
                            DamageInfo info = DamageAction.DealDamageAction(executeCharacter, skill, targetCharacter, 0);

                            Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                            Messenger.Instance.BroadCast(Messenger.EventType.TargetAndAttackEnemy, executeCharacter, targetCharacter, skill);   //技能攻击
                        }

                    }
                    else if (skill.attackType == Skill_SO.TargetType.NeighborTarget)
                    {
                        DamageInfo info = new DamageInfo(executeCharacter, null, skill);
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, targetCharacter, 0).toughDamage;
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, targetCharacter.FindLeftTarget(), 1).toughDamage;
                        info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, targetCharacter.FindRightTarget(), 1).toughDamage;


                        Messenger.Instance.BroadCast(Messenger.EventType.TargetAndAttackEnemy, executeCharacter, targetCharacter, skill);
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
                        Messenger.Instance.BroadCast(Messenger.EventType.TargetAndAttackEnemy, executeCharacter, (Character)null, skill);  
                    }
                    else if (skill.attackType == Skill_SO.TargetType.RandomTarget)
                    {
                        DamageInfo info = new DamageInfo(executeCharacter, null, skill);
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

                            int randomIndex = UnityEngine.Random.Range(0, enemies.Count);
                            attackTarget += randomIndex.ToString() + "  ,";
                            info.toughDamage += DamageAction.DealDamageAction(executeCharacter, skill, enemies[randomIndex], 0).toughDamage;
                        }

                        Messenger.Instance.BroadCast(Messenger.EventType.ToughDamage, info);
                        Messenger.Instance.BroadCast(Messenger.EventType.TargetAndAttackEnemy, executeCharacter, targetCharacter, skill);

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
                            if (skill.basePercent[StatusIndex] * (currentActionCharacter.characterData.effectPercent + 1) * (1 - targetCharacter.characterData.effectDefend) > UnityEngine.Random.Range(0f, 1f))
                            {
                                var cloneStatus = Instantiate(s.status);
                                if (s.status.StatusName == "1035")
                                {
                                    cloneStatus.involvedElement = tempType;
                                }
                                StatusAction.AddStatusAction(currentActionCharacter, targetCharacter, cloneStatus);
                            }
                            else
                            {
                                GameManager.Instance.ShowPanelText("效果抵抗 !");
                            }
                        }

                    }
                }
            }

            #endregion

            #region 效果执行后Action
            foreach (var action in skill.addactions)
            {
                if (action.AfterDamage == true)
                {
                    ExecuteAction(action, executeCharacter, targetCharacter);
                    Debug.Log($"{skill.skillID}  {targetCharacter.name} ");
                }
            }
            #endregion
        }
        else if(skill.target == Skill_SO.Target.Friend)
        {
            #region 效果执行前Action
            foreach (var action in skill.addactions)
            {
                if (action.AfterDamage == false)
                {
                    ExecuteAction(action, executeCharacter, currentSelectPlayer);
                }
            }
            #endregion

            #region 效果执行
            foreach (var skillType in skill.skillType)
            {
                if (skillType == Skill_SO.SkillType.AddStatus)  //注意每个Status要先克隆出来(否则一会影响原始值  二回导致各角色之间Status混淆 变成一致)
                {
                    foreach(var statusStruct in skill.addStatus)  
                    {
                        int StatusIndex = skill.addStatus.IndexOf(statusStruct);
                        if (statusStruct.statusTarget == Skill_SO.TargetType.SingleTarget)
                        {
                            var cloneStatus = Instantiate(statusStruct.status);
                            StatusAction.AddStatusAction(currentActionCharacter, currentSelectPlayer, cloneStatus);
                        }
                        else if (statusStruct.statusTarget == Skill_SO.TargetType.AllTarget)   //如全队减伤
                        {
                            StatusAction.AddStatusAllFriend(currentActionCharacter, statusStruct.status);
                        }
                        else if(statusStruct.statusTarget == Skill_SO.TargetType.AllOtherFriend)  //符玄给队友加分摊伤害Status
                        {
                            StatusAction.AddStatusAllOtherFriend(currentActionCharacter, statusStruct.status);
                        }
                        else if(statusStruct.statusTarget == Skill_SO.TargetType.Self)
                        {
                            var cloneStatus = Instantiate(statusStruct.status);
                            StatusAction.AddStatusAction(currentActionCharacter, currentActionCharacter, statusStruct.status);
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
            #endregion 

            #region 效果执行后Action
            foreach (var action in skill.addactions)
            {
                if (action.AfterDamage)
                {
                    ExecuteAction(action, executeCharacter, currentSelectPlayer);
                }
            }
            #endregion
        }

        if (skill.damageType == Skill_SO.DamageType.SkillAttack)   
        {
            Debug.Log("行动条移动");
            enemyActionCounterDown.AddEndAction(() => FreshAction());
        }
        if(skill.skillQER == Skill_SO.SkillQER.E)
        {
            Messenger.Instance.BroadCast(Messenger.EventType.CastSkillE, executeCharacter);
        }
        enemyActionCounterDown.ResetTimer();
    }
    public void ExecuteAction(Skill_SO.Actions action, Character executeCharacter, Character target)
    {
        if (action.addaction == Skill_SO.AddAction.PushActon)
        {
            if (action.targetType == Skill_SO.TargetType.SingleTarget)
            {
                PushActionValueAction.PushActionValue(target, action.value);
            }
            else if (action.targetType == Skill_SO.TargetType.AllTarget)  //555拉条触发器
            {
                PushActionValueAction.PushAllActionValue(action.value);
            }
        }
        else if (action.addaction == Skill_SO.AddAction.GetSkillPoint)
        {
            GameManager.Instance.skillPoint = GameManager.Instance.skillPoint + 4;
            Messenger.Instance.BroadCast(Messenger.EventType.SkillPointChange, GameManager.Instance.skillPoint);
        }
        else if (action.addaction == Skill_SO.AddAction.AddWeakness)
        {
            if (target != null)
            {
                target.characterData.weakness.Add(tempType);
                target.ShowSelfWeakness();
            }
        }
        else if (action.addaction == Skill_SO.AddAction.AddStatusLayer)
        {
            Status s = executeCharacter.currentStatus.Find(e => e.StatusName == action.statusName);
            if (s != null)
            {
                StatusAction.AddStatusLayerAction(s.Caster, s.Owner, s, (int)action.value);
            }
        }
        else if (action.addaction == Skill_SO.AddAction.AddStatusDuration)
        {
            Status s = executeCharacter.currentStatus.Find(e => e.StatusName == action.statusName);
            if (s != null)
            {
                StatusAction.AddStatusDurationAction(s.Caster, s.Owner, s, (int)action.value);
            }
        }
        else if (action.addaction == Skill_SO.AddAction.GetEnergy)
        {
            if (action.targetType == Skill_SO.TargetType.Self)
            {
                EnergyChangeAction.AddEnergyAction(executeCharacter, action.value);
            }
        }
        else if (action.addaction == Skill_SO.AddAction.ExecuteSkill)    //例如飞霄E技能释放追加
        {
            if (action.targetType == Skill_SO.TargetType.SingleTarget)
            {
                SpecialActionList.AddFirst(new SpecialAction(executeCharacter, action.skill,target,GameManager.Instance.GlobalExtraTimePoint));
                GameManager.Instance.GlobalExtraTimePoint += 1;
                FreshSpecialAction();
                enemyActionCounterDown.ResetTimer();
            }
        }
        else if (action.addaction == Skill_SO.AddAction.DealBrokenDamage)
        {
            if(action.targetType == Skill_SO.TargetType.SingleTarget)
            {
                DamageAction.BrokenDamageAction(executeCharacter, target, action.value);
            }
        }
    }
    public void ChangeTargetFirstCharacter(Skill_SO skill)
    {
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

    /// <summary>
    /// 特殊行动退队列   一定是按照队列的顺序退  ESC是手动取消终结技传进来的参数 
    /// </summary>
    public void SpecialActionDequeue(Skill_SO skill = null,Character caster = null,Character target = null,bool ESC = false)  //退队列  技能释放(可能触发各种追加攻击)  然后根据队列最上方类型(追加 或者终结技)
    {
        if (ESC)
        {
            SpecialActionList.RemoveFirst();
            if (SpecialActionList.Count == 0)
            {
                CurrentInputState = CurrentInputStateEnum.None;
                enemyActionCounterDown.ResetTimer();
                enemyActionCounterDown.pauseFlag = false;
                return;
            }

            var next = SpecialActionList.First.Value;
            if (next.skill.damageType == Skill_SO.DamageType.ExtraAttack)
            {
                CurrentInputState = CurrentInputStateEnum.None;
                enemyActionCounterDown.ResetTimer();
                enemyActionCounterDown.pauseFlag = false;
            }
            else if (next.skill.damageType == Skill_SO.DamageType.FinalAttack)
            {
                CurrentInputState = CurrentInputStateEnum.R;
                enemyActionCounterDown.currentTime = 20f;
                enemyActionCounterDown.pauseFlag = true;
            }
        }
        else
        {
            SpecialActionList.RemoveFirst();

            SkillExecute(skill,caster,target);

            if (SpecialActionList.Count == 0)
            {
                CurrentInputState = CurrentInputStateEnum.None;
                enemyActionCounterDown.ResetTimer();
                enemyActionCounterDown.pauseFlag = false;
                return;
            }

            var next = SpecialActionList.First.Value;
            if (next.skill.damageType == Skill_SO.DamageType.ExtraAttack)
            {
                CurrentInputState = CurrentInputStateEnum.None;
                enemyActionCounterDown.ResetTimer();
                enemyActionCounterDown.pauseFlag = false;
            }
            else if (next.skill.damageType == Skill_SO.DamageType.FinalAttack)
            {
                CurrentInputState = CurrentInputStateEnum.R;
                enemyActionCounterDown.currentTime = 20f;
                enemyActionCounterDown.pauseFlag = true;
            }
        }
    }


}
