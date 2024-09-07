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

    public bool turnStartDuration;  //buff����ʧʱ���Ϊ�غϿ�ʼʱ
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
    public CharacterData_SO.weaknessType involvedElement; //�����漰��
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

    public enum PlayerLimit   //�ܳԵ��⻷Ч��������
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

    public bool IsAttached;  //ĳ��Status������һ��������   �绨������buff�����Թ�buff�������ֵ����  ����buff�ĸ�boolֵ��true ��Fresh����ʱ����
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

        public Messenger.EventType counterType; //������ʽ����
        public List<int> addLayer;  //��Ҫ�����ŵ�һλ
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
            return;   //ѩ�±���׷�ӹ����޷���������
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
        if(changeNumber < 0)   //С��0˵��������ս����
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
