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
    }
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
    public int LayerLimited;
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
        public float correctValues;
    }
    public List<DependValue> dependValues;

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
        DefendValue,
        SpeedValue,

        PureDefendValue,
        CriticalPercentValue,
    }
    public List<InvolvedProperty> InvolvedName;

    public bool isField;

    public enum FieldTarget
    {
        AllFriends,
        Global,
    }
    [System.Serializable]
    public struct FieldEffect
    {
        public FieldTarget field;
        public PlayerLimit limit;
        public CharacterData_SO.weaknessType ElementType;
        public Status status;
    }
    public List<FieldEffect> fieldEffects;

    public bool IsAttached;  //某个Status与另外一个有联动   如花火增伤buff在有迷诡buff情况下数值增加  增伤buff的该bool值设true 在Fresh属性时处理
    [System.Serializable]
    public struct AttachOtherStatus
    {
        public string StatusName;
        public float AddValue;
    }
    public AttachOtherStatus attachOtherStatus;

    [System.Serializable]
    public struct Counter
    {
        public bool hasCounter;

        public Messenger.EventType counterType; //计数方式类型
        public List<int> addLayer;  //主要层数放第一位
    }
    public Counter counter;


    public enum TriggerCondition
    {
        LayerEnough,
        HealthLimit,
    }

    public enum TriggerEffect
    {
        ExecuteSkill,
        AddStatus,
    }
    [System.Serializable]
    public struct Trigger   //触发器 
    {
        public bool hasTrigger;

        public TriggerCondition triggerCondition;
        public int triggerLayer;

        public Compare limitRelation;
        public float limitValue;  //血量小于50%时
        public TriggerEffect triggerEffect;
        public List<Status> triggerStatus;
        public Skill_SO triggerSkill;
    }
    public Trigger trigger;

    public bool StatusGroup;
    public List<Status> childStatus = new List<Status>();



    public enum Compare
    {
        Less,
        LessOrEqual,
        Equal,
        MoreOrEqual,
        More,
    }
    public void AddCounterAbility()
    {
        if (this.counter.hasCounter)
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
                case Messenger.EventType.DealDamage:
                    Messenger.Instance.AddListener<Character, Character, float>(Messenger.EventType.DealDamage, HealthCheckAction);
                    break;
            }
        }
    }

    public void ApplyTrigger()
    {
        if (trigger.triggerEffect == TriggerEffect.ExecuteSkill)
        {
            InputManager.Instance.SkillExecute(trigger.triggerSkill, Owner);
        }
        else if (trigger.triggerEffect == TriggerEffect.AddStatus)
        {
            foreach (var status in trigger.triggerStatus)
            {
                StatusAction.AddStatusAction(Caster, Owner, status);
            }
        }
    }
    public void TryApplyTrigger()
    {
        if(trigger.hasTrigger && trigger.triggerCondition == TriggerCondition.LayerEnough)
        {
            if(StatusLayer >= trigger.triggerLayer)
            {
                ApplyTrigger();
                StatusLayer -= trigger.triggerLayer;
            }
        }
        else if(trigger.hasTrigger && trigger.triggerCondition == TriggerCondition.HealthLimit)   //符玄半血触发器
        {
            if (StatusLayer >= trigger.triggerLayer)
            {
                if(trigger.limitRelation == Compare.Less && Owner.characterData.currentHealth < Owner.characterData.maxHealth * trigger.limitValue)
                {
                    ApplyTrigger(); 
                    StatusLayer -= trigger.triggerLayer;
                }
            }
        }
    }

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

        TryApplyTrigger();
    }

    private void ConsumeSkillPointAction(int changeNumber)  //花火天赋被动Status
    {
        if(changeNumber < 0)   //小于0说明消耗了战技点
        {
            StatusAction.AddStatusLayerAction(Caster, Owner, this, -changeNumber);
        }
        Owner.FreshProperty(this);
    }

    private void KillTargetAction(Character attacter,Character killed)  //在蓝天下
    {
        if (attacter.currentStatus.Contains(this))
        {
            StatusLayer = 1;
        }

        TryApplyTrigger();
    }

    private void HealthCheckAction(Character a, Character b,float c)
    {
        TryApplyTrigger();
    }
    #endregion
    public void OnDestroy()
    {

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
}
