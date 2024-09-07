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
        DefendPercentBonus,
        SpeedPercentBonus,


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
    }
    [System.Serializable]
    public struct Trigger
    {
        public bool hasTrigger;

        
        public TriggerCondition triggerCondition;
        public int triggerLayer;

        public Skill_SO triggerSkill;
    }
    public Trigger trigger;

    public bool StatusGroup;
    public List<Status> childStatus = new List<Status>();
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
            }
        }
    }

    public void TryApplyTrigger()
    {
        if(trigger.triggerCondition == TriggerCondition.LayerEnough)
        {
            if(StatusLayer >= trigger.triggerLayer)
            {
                InputManager.Instance.SkillExecute(trigger.triggerSkill, Owner);
                StatusLayer -= trigger.triggerLayer;
            }
        }
    }

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

    private void ConsumeSkillPointAction(int changeNumber)
    {
        if(changeNumber < 0)   //小于0说明消耗了战技点
        {
            StatusAction.AddStatusLayerAction(Caster, Owner, this, -changeNumber);
        }
        Owner.FreshProperty(this);
    }
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
