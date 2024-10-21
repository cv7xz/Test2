using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status",menuName = "Status")]
public class Status : ScriptableObject
{
    public Character Caster;
    public Character Owner;

    public string StatusName;
    public string description;
    public int duration;
    public bool NotAutoDelete;

    public bool turnStartDuration;  //buff的消失时间变为回合开始时

    public bool HasBonusLimit;
    [System.Serializable]
    public struct BonusLimitType
    {
        public Skill_SO.DamageType BonusAttackType;
    }
    public BonusLimitType limitType;   //飞霄     追加攻击  有爆伤提升
 
    public enum StatusType
    {
        AttackPercentBonus,
        HealthPercentBonus,
        DamageIncreaseBonus,
        BrokenFocusBonus,
        TriggerAttack,
        CriticalDamageBonus,
        AttachOtherStatus,
        FieldStatus,
        ChangeSkillPointLimited,
        EffectPercentBonus,
        EffectDefendBonus,
        DefendPercentBonus,   //这个是抗性
        SpeedPercentBonus,

        DefendValueBonus,  //防御力
        CriticalPercentBonus,
        TriggerComponent,  //做触发器用

        ShareDamage,  //符玄专属
        DamageDecreseBonus,  //百分比减伤

        SpeedFixBouns,  //天赋被动就有速度fix上升
        BrokenEfficiencyBouns,

        ExistFieldStatus,   //当Status存在时有光环效果
        DefendPenetration,

        AttackTriggerEffect,

        ContinueDamageStatus,   
        ControlStatus,    //控制类Status

        EnergyRestore,
    }
    public bool TurnStartTiming;
    public StatusType statusType;
    public List<float> StatusValue = new List<float>();
    public CharacterData_SO.weaknessType involvedElement; //减抗涉及的
    public enum RepeatRule
    {
        LayerStack,
        Replace,
    }

    public RepeatRule repeatRule;
    public bool ReapetFreshDuration;
    public int StatusLayer;

    public int LayerLimited,DuartionLimited;
    public float ValueLimited;


    public bool IsDepend;

    public enum PlayerLimit   //能吃到光环效果的条件
    {
        None,
        Element,
    }

    [System.Serializable]
    public struct DependValue
    {
        public InvolvedProperty property;
        public DependTarget dependTarget;

        public CharacterData_SO.weaknessType certainElement;
        public List<float> values;

        public DependTpye dependType;
        public float minLimit;      //超过minLimit后  每经过everyStep  依存值增加stepValue   击破超过120%时 每超过10% 效果提高6%
        public float maxValue;
        public float everyStep;
        public float stepValue;

        public float correctValues;
    }
    
    public List<DependValue> dependValues;
    public enum DependTpye
    {
        Normal,
        MinLimit,
    }
    public enum DependTarget
    {
        Owner,
        Caster,
        Global,
    }
    public enum InvolvedProperty
    {
        AttackValue,
        HealthValue,
        DamageIncreaseValue,
        BrokenFocus,
        CriticalDamageValue,
        CertainElementPlayerNumber,
        EffectPercentValue,
        EffectDefendValue,
        DefendValue,  //抗性
        SpeedValue,

        PureDefendValue,  //防御
        CriticalPercentValue,

        DamageDecreseValue,
        BrokenEfficiencyValue,

        DefendPenetrationValue,
    }
    public List<InvolvedProperty> InvolvedName;

    public bool isField;   //FieldStatus的StatusType 会用 如 物体呼吸中
    public enum FieldTarget
    {
        AllFriends,
        Global,
    }
    #region FieldEffect
    [System.Serializable]
    public struct FieldEffect
    {
        public FieldTarget field;
        public PlayerLimit limit;
        public CharacterData_SO.weaknessType ElementType;
        public Status status;
    }
    public List<FieldEffect> fieldEffects;
    #endregion

    #region AttachStatus
    public bool IsAttached;  //某个Status与另外一个有联动   如花火增伤buff在有迷诡buff情况下数值增加  增伤buff的该bool值设true 在Fresh属性时处理

    public enum AttachTarget
    {
        Self,
        Caster,
    }
    [System.Serializable]
    public struct AttachOtherStatus
    {
        public AttachTarget attachTargte;
        public string StatusName;
        public float AddValue;
        public bool hasDepend;
        public List<DependValue> dependValues;
    }
    public AttachOtherStatus attachOtherStatus;
    #endregion

    #region AttachSkill
    public bool IsAttachSkill;  //该Status与一个Skill有联动  如飞霄释放终结技被视为追加攻击
    [System.Serializable]
    public struct AttachOtherSkill
    {
        public List<string> SkillName;
        public Skill_SO.DamageType damageType;
    }
    public AttachOtherSkill attachOtherSkill;
    #endregion

    #region SpecialStatus
    public bool isSpecialStatus;
    public enum SpecialType   //用于显示角色身上的特殊Status 如雪衣追加攻击触发器 符玄回血次数
    {
        LimitedLayer,
        TriggerLayer,
        LimitedDuration,
    }
    public SpecialType specialType;
    #endregion

    #region Counter
    public bool hasCounter;
    [System.Serializable]
    public struct Counter
    {
        public Messenger.EventType counterType; //计数方式类型
        public List<int> addLayer;  //主要层数放第一位
    }
    public Counter counter;
    #endregion

    #region Trigger
    public enum TriggerCondition
    {
        LayerEnough,
        HealthLimit,
        CastFinalSkill,
        DamageDefendLessEnemy,
        FriendDealDamage,
        CastSkillE,
        AttackEnemy,    //攻击敌人 DealDamage信号  对受击敌人进行Status添加
    }

    public enum TriggerEffect
    {
        ExecuteSkill,
        AddStatus,
        ExecuteAction,
    }

    public bool hasTrigger;

    [System.Serializable]
    public struct Trigger   //触发器 
    {
        public TriggerCondition triggerCondition;
        public int triggerLayer;

        public bool SelfLimit;  //信号的监听（如释放终结技）是全局的  一些触发器触发条件限定为自己
        public Compare limitRelation;
        public float limitValue;  //血量小于50%时
        public TriggerEffect triggerEffect;

        public List<Status> triggerStatus;   //在蓝天下击杀触发加暴击率效果
        public Skill_SO triggerSkill;
        public Skill_SO.Actions triggerAction;
    }
    [HideInInspector]
    public Trigger trigger;
    #endregion

    public bool StatusGroup;
    public List<Status> childStatus = new List<Status>();

    public List<Status> fieldStatus = new List<Status>();  //Type 特定为 ExistFieldStatus专用  光环产生Status效果

    public bool FeiXiaoExtraSkillPoint;   //超级特判之飞霄  每回合触发一次 回合开始时重置触发次数

    public enum actionEffect
    {
        NotFreshBroken,
        PushActionValue,
    }
    [System.Serializable]
    public struct Action
    {
        public actionEffect effect;
        public float value;


    }
    public List<Action> controlActions = new List<Action>();
    public enum Compare
    {
        Less,
        LessOrEqual,
        Equal,
        MoreOrEqual,
        More,
    }

    public enum DependCalculate
    {
        Multi,
        Divide,
        Add,
    }

    //以下两个Ability是 某些Status的层数会因为 事件 而变动，此外，还有一些Status有 达到某个条件时触发效果的能力  两个Ability都是添加事件  (拥有计数或触发效果的能力)
    //有计数器能力的Status  会涉及层数的变动  但不一定会触发事件  (花火使用战技点 全队加增伤buff层数增加)

    //有触发器能力的Status  不一定有计数器 (符玄半血回血触发器  其层数是Skill中配置AddAction实现)
    public void AddCounterAbility()   
    {
        AddTriggerAbility();  //无论有没有计数器  都会进入AddCounterAbility函数 是否添加计数器是由hasCounter判断

        if (FeiXiaoExtraSkillPoint)
        {
            Messenger.Instance.AddListener<Character>(Messenger.EventType.TurnStart, FreshStatusTurnStart);
        }

        if (this.hasCounter)
        {
            switch (this.counter.counterType)
            {
                case Messenger.EventType.ToughDamage:
                    Messenger.Instance.AddListener<DamageInfo>(Messenger.EventType.ToughDamage, ToughDamageAction);
                    break;
                case Messenger.EventType.SkillPointChange:
                    Messenger.Instance.AddListener<int>(Messenger.EventType.SkillPointChange, ConsumeSkillPointAction);
                    break;
                case Messenger.EventType.KillTarget:
                    Messenger.Instance.AddListener<Character,Character>(Messenger.EventType.KillTarget, KillTargetAction);
                    break;
                case Messenger.EventType.TargetAndAttackEnemy:                                        //飞霄被动叠飞黄层数  DealDamage一次行动可能多次触发  该信号是一次SKill广播一次
                    Messenger.Instance.AddListener<Character, Character, Skill_SO>(Messenger.EventType.TargetAndAttackEnemy, DealDamageToEnemyCounterAction);
                    break;
            }
        }
    }
    public void AddTriggerAbility()
    {
        if (this.hasTrigger)
        {
            switch (this.trigger.triggerCondition)
            {
                case TriggerCondition.HealthLimit:
                    Messenger.Instance.AddListener<float,Character>(Messenger.EventType.TakeDamage, CheckCondition);
                    Messenger.Instance.AddListener<Character>(Messenger.EventType.CastFinalSkill, CheckCondition);
                    break;
                case TriggerCondition.CastFinalSkill:
                    Messenger.Instance.AddListener<Character>(Messenger.EventType.CastFinalSkill, CheckCondition);
                    break;
                case TriggerCondition.DamageDefendLessEnemy:
                    Messenger.Instance.AddListener<Character, Character, float>(Messenger.EventType.DealDamage, CheckCondition);
                    break;
                case TriggerCondition.FriendDealDamage:
                    Messenger.Instance.AddListener<Character, Character, float>(Messenger.EventType.DealDamage, DealDamageToEnemyTriggerAction);
                    break;
                case TriggerCondition.CastSkillE:
                    Messenger.Instance.AddListener(Messenger.EventType.CastSkillE,CheckCondition);
                    break;
                case TriggerCondition.AttackEnemy:
                    Messenger.Instance.AddListener<Character, Character, float>(Messenger.EventType.DealDamage, AttackTriggerAction);
                    break;
            }
        }
    }

    public void ApplyTrigger()
    {
        if (trigger.triggerEffect == TriggerEffect.ExecuteSkill)
        {
            InputManager.Instance.SpecialActionList.AddFirst(new InputManager.SpecialAction(Owner, trigger.triggerSkill));
            InputManager.Instance.FreshSpecialAction();
            InputManager.Instance.enemyActionCounterDown.ResetTimer();
        }
        else if (trigger.triggerEffect == TriggerEffect.AddStatus)   //在蓝天下击杀触发加暴击率效果
        {
            foreach (var status in trigger.triggerStatus)
            {
                var cloneStatus = Instantiate(status);
                StatusAction.AddStatusAction(Caster, Owner, cloneStatus);
            }
        }
        else if(trigger.triggerEffect == TriggerEffect.ExecuteAction)
        {
            InputManager.Instance.ExecuteAction(trigger.triggerAction, Owner, null);
        }
    }

    #region 触发器回调
    public void CheckCondition()
    {
        if(hasTrigger == false)
        {
            return;
        }

        if (trigger.triggerLayer == 0)
        {
            if (trigger.triggerCondition == TriggerCondition.HealthLimit)   //符玄半血触发器
            {
                if (trigger.limitRelation == Compare.Less && Owner.characterData.currentHealth < Owner.characterData.maxHealth * trigger.limitValue)
                {
                    ApplyTrigger();
                }
            }
            else if (trigger.triggerCondition == TriggerCondition.CastFinalSkill)
            {
                ApplyTrigger();
            }
            else if (trigger.triggerCondition == TriggerCondition.FriendDealDamage)
            {
                ApplyTrigger();
            }
            else if(trigger.triggerCondition == TriggerCondition.CastSkillE)
            {
                ApplyTrigger();
            }
        }
        else
        {
            if (trigger.triggerCondition == TriggerCondition.LayerEnough)
            {
                if (StatusLayer >= trigger.triggerLayer)
                {
                    ApplyTrigger();
                    StatusLayer -= trigger.triggerLayer;
                }
            }
            else if (trigger.triggerCondition == TriggerCondition.HealthLimit)  
            {
                if (StatusLayer >= trigger.triggerLayer)
                {
                    if (trigger.limitRelation == Compare.Less && Owner.characterData.currentHealth < Owner.characterData.maxHealth * trigger.limitValue)
                    {
                        ApplyTrigger();
                        StatusLayer -= trigger.triggerLayer;
                    }
                }
            }
            else if (trigger.triggerCondition == TriggerCondition.CastFinalSkill)
            {
                if (StatusLayer >= trigger.triggerLayer)
                {
                    ApplyTrigger();
                    StatusLayer -= trigger.triggerLayer;
                }
            }
            else if (trigger.triggerCondition == TriggerCondition.FriendDealDamage)
            {
                if (StatusLayer >= trigger.triggerLayer)
                {
                    ApplyTrigger();
                    StatusLayer -= trigger.triggerLayer;
                }
            }
        }
    }
    public void CheckCondition(Character a)
    {
        if(this.trigger.SelfLimit)
        {
            if(a == this.Owner)
            {
                CheckCondition();
            }
        }
        else
        {
            CheckCondition();
        }

    }
    public void CheckCondition(float _,Character a)
    {
        if (this.trigger.SelfLimit)
        {
            if (a == this.Owner)
            {
                CheckCondition();
            }
        }
        else
        {
            CheckCondition();
        }
    }
    public void CheckCondition(Character attacker,Character attacked,float damageValue)  //新手任务开始前  装备者攻击防御被降低的敌方目标后恢复能量
    {
        if(attacker != this.Owner)
        {
            return;
        }
        foreach(var status in attacked.currentStatus)
        {
            if(status.statusType == StatusType.DefendValueBonus && status.StatusValue[0] < 0)
            {
                ApplyTrigger();
            }
        }
    }
    private void DealDamageToEnemyTriggerAction(Character attacker, Character attacked, float damageValue)   //一回合一次  队友攻击触发追加攻击
    {
        if(attacker.type == Character.CharaterType.Player && attacker != Owner)
        {
            CheckCondition();
        }
    }
    private void AttackTriggerAction(Character attacker,Character attacked,float damageValue)
    {
        if(attacker == this.Owner)
        {
            foreach(var status in this.trigger.triggerStatus)
            {
                StatusAction.AddStatusAction(attacker, attacked, status);
            }
        }
    }

    #endregion

    #region 计数器回调
    private void ToughDamageAction(DamageInfo damage)
    {
        if(damage.skill.skillID == "1013")
        {
            return;   //雪衣本身追加攻击无法叠层特判
        }
        if (damage.attacker.currentStatus.Contains(this))
        {
            StatusLayer += counter.addLayer[0] * (int)(damage.toughDamage / 10f);
        }
        else
        {
            StatusLayer += counter.addLayer[1] * (damage.toughDamage > 0 ? 1 : 0);
        }
        CheckCondition();
    }

    private void ConsumeSkillPointAction(int changeNumber)  //花火天赋被动Status  消耗战技点
    {
        if(changeNumber < 0)   //小于0说明消耗了战技点
        {
            StatusAction.AddStatusLayerAction(Caster, Owner, this, -changeNumber);
        }
        Owner.FreshProperty(this);
    }

    private void KillTargetAction(Character attacter,Character killed)     //在蓝天下 击杀
    {
        if (attacter.currentStatus.Contains(this))
        {
            StatusLayer = 1;
        }

        CheckCondition();
    }

    private void DealDamageToEnemyCounterAction(Character attacker,Character attacked,Skill_SO skill)   //飞霄 友军攻击加层数
    {
        Debug.Log($"{this}");
        if(attacker.type == Character.CharaterType.Player)
        {
            StatusLayer += 1;
        }
    }

    #endregion

    public void FreshStatusTurnStart(Character player)   //飞霄追加攻击 回合刷新回调
    {
        if(player.characterName != "FEIXIAO")   //因为信号的全局机制  没有Status的 敌人也会进入该回调
        {
            return;
        }
        if (FeiXiaoExtraSkillPoint)
        {
            if(StatusLayer == 1)   //天赋页中效果
            {
                var skillPointStatus = player.currentStatus.Find(e => e.StatusName == "1051");
                skillPointStatus.StatusLayer = Mathf.Min(skillPointStatus.LayerLimited, skillPointStatus.StatusLayer + 1);
            }
            StatusLayer = 1;
        }  
    }
}


public class DamageInfo
{
    public Character attacker;
    public Character target;

    public Skill_SO skill;
    public float damageValue;
    public float toughDamage;
    public CharacterData_SO.weaknessType elementType;

    public bool isBrokenDamage;

    public DamageInfo(Character Attacker, Character Target,Skill_SO Skill)
    {
        attacker = Attacker;
        target = Target;
        skill = Skill;
    }
    public DamageInfo()
    {
        toughDamage = 0f;
        damageValue = 0f;
    }
}
