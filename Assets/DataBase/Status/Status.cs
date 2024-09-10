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
        DefendPercentBonus,   //����ǿ���
        SpeedPercentBonus,

        DefendValueBonus,  //������
        CriticalPercentBonus,
        TriggerComponent,  //����������

        ShareDamage,  //����ר��
        DamageDecreseBonus,  //�ٷֱȼ���
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

        PureDefendValue,
        CriticalPercentValue,

        DamageDecreseValue,
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

    public bool hasCounter;
    [System.Serializable]
    public struct Counter
    {
        public Messenger.EventType counterType; //������ʽ����
        public List<int> addLayer;  //��Ҫ�����ŵ�һλ
    }
    public Counter counter;


    public enum TriggerCondition
    {
        LayerEnough,
        HealthLimit,
        CastFinalSkill,
        DamageDefendLessEnemy,
    }

    public enum TriggerEffect
    {
        ExecuteSkill,
        AddStatus,
        ExecuteAction,
    }

    public bool hasTrigger;
    [System.Serializable]
    public struct Trigger   //������ 
    {
        public TriggerCondition triggerCondition;
        public int triggerLayer;

        public Compare limitRelation;
        public float limitValue;  //Ѫ��С��50%ʱ
        public TriggerEffect triggerEffect;

        public List<Status> triggerStatus;   //�������»�ɱ�����ӱ�����Ч��
        public Skill_SO triggerSkill;
        public Skill_SO.Actions triggerAction;
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
    //��������Ability�� ĳЩStatus�Ĳ�������Ϊ �¼� ���䶯�����⣬����һЩStatus�� �ﵽĳ������ʱ����Ч��������  ����Ability��������¼�  (ӵ�м����򴥷�Ч��������)
    //�м�����������Status  ���漰�����ı䶯  ����һ���ᴥ���¼�  (����ʹ��ս���� ȫ�Ӽ�����buff��������)

    //�д�����������Status  ��һ���м����� (������Ѫ��Ѫ������  �������Skill������AddActionʵ��)
    public void AddCounterAbility()   
    {
        AddTriggerAbility();  //������û�м�����  �������AddCounterAbility���� �Ƿ���Ӽ���������hasCounter�ж�

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
                    Messenger.Instance.AddListener(Messenger.EventType.CastFinalSkill, CheckCondition);
                    break;
                case TriggerCondition.CastFinalSkill:
                    Messenger.Instance.AddListener(Messenger.EventType.CastFinalSkill, CheckCondition);
                    break;
                case TriggerCondition.DamageDefendLessEnemy:
                    Messenger.Instance.AddListener<Character, Character, float>(Messenger.EventType.DealDamage, CheckCondition);
                    break;
            }
        }
    }

    public void ApplyTrigger()
    {
        if (trigger.triggerEffect == TriggerEffect.ExecuteSkill)
        {
            InputManager.Instance.SpecialActionQueue.Enqueue(new InputManager.SpecialAction(Owner, trigger.triggerSkill));
            InputManager.Instance.FreshSpecialAction();
            InputManager.Instance.enemyActionCounterDown.ResetTimer();
        }
        else if (trigger.triggerEffect == TriggerEffect.AddStatus)   //�������»�ɱ�����ӱ�����Ч��
        {
            foreach (var status in trigger.triggerStatus)
            {
                StatusAction.AddStatusAction(Caster, Owner, status);
            }
        }
        else if(trigger.triggerEffect == TriggerEffect.ExecuteAction)
        {
            InputManager.Instance.ExecuteAction(trigger.triggerAction, Owner, null);
        }
    }
    public void CheckCondition()
    {
        if(hasTrigger == false)
        {
            return;
        }
        if(trigger.triggerLayer == 0)
        {
            if (trigger.triggerCondition == TriggerCondition.HealthLimit)   //������Ѫ������
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
        }
    }

    public void CheckCondition(float _,Character a)
    {
        CheckCondition();
    }
    public void CheckCondition(Character attacker,Character attacked,float damageValue)  //��������ʼǰ  װ���߹������������͵ĵз�Ŀ���ָ�����
    {
        foreach(var status in attacked.currentStatus)
        {
            if(status.statusType == StatusType.DefendValueBonus && status.StatusValue[0] < 0)
            {
                ApplyTrigger();
            }
        }
    }

    #region �������ص�
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

        CheckCondition();
    }

    private void ConsumeSkillPointAction(int changeNumber)  //�����츳����Status
    {
        if(changeNumber < 0)   //С��0˵��������ս����
        {
            StatusAction.AddStatusLayerAction(Caster, Owner, this, -changeNumber);
        }
        Owner.FreshProperty(this);
    }

    private void KillTargetAction(Character attacter,Character killed)  //��������
    {
        if (attacter.currentStatus.Contains(this))
        {
            StatusLayer = 1;
        }

        CheckCondition();
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
