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

    public bool HasBonusLimit;
    [System.Serializable]
    public struct BonusLimitType
    {
        public Skill_SO.DamageType BonusAttackType;
    }
    public BonusLimitType limitType;   //����     ׷�ӹ���  �б�������
 
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

        SpeedFixBouns,  //�츳���������ٶ�fix����
        BrokenEfficiencyBouns,

        ExistFieldStatus,   //��Status����ʱ�й⻷Ч��
        DefendPenetration,

        AttackTriggerEffect,

        ContinueDamageStatus,   
        ControlStatus,    //������Status

        EnergyRestore,
    }
    public bool TurnStartTiming;
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

    public int LayerLimited,DuartionLimited;
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

        public DependTpye dependType;
        public float minLimit;      //����minLimit��  ÿ����everyStep  ����ֵ����stepValue   ���Ƴ���120%ʱ ÿ����10% Ч�����6%
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
        DefendValue,  //����
        SpeedValue,

        PureDefendValue,  //����
        CriticalPercentValue,

        DamageDecreseValue,
        BrokenEfficiencyValue,

        DefendPenetrationValue,
    }
    public List<InvolvedProperty> InvolvedName;

    public bool isField;   //FieldStatus��StatusType ���� �� ���������
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
    public bool IsAttached;  //ĳ��Status������һ��������   �绨������buff�����Թ�buff�������ֵ����  ����buff�ĸ�boolֵ��true ��Fresh����ʱ����

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
    public bool IsAttachSkill;  //��Status��һ��Skill������  ������ͷ��սἼ����Ϊ׷�ӹ���
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
    public enum SpecialType   //������ʾ��ɫ���ϵ�����Status ��ѩ��׷�ӹ��������� ������Ѫ����
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
        public Messenger.EventType counterType; //������ʽ����
        public List<int> addLayer;  //��Ҫ�����ŵ�һλ
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
        AttackEnemy,    //�������� DealDamage�ź�  ���ܻ����˽���Status���
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

        public bool SelfLimit;  //�źŵļ��������ͷ��սἼ����ȫ�ֵ�  һЩ���������������޶�Ϊ�Լ�
        public Compare limitRelation;
        public float limitValue;  //Ѫ��С��50%ʱ
        public TriggerEffect triggerEffect;

        public List<Status> triggerStatus;   //�������»�ɱ�����ӱ�����Ч��
        public Skill_SO triggerSkill;
        public Skill_SO.Actions triggerAction;
    }
    [HideInInspector]
    public Trigger trigger;
    #endregion

    public bool StatusGroup;
    public List<Status> childStatus = new List<Status>();

    public List<Status> fieldStatus = new List<Status>();  //Type �ض�Ϊ ExistFieldStatusר��  �⻷����StatusЧ��

    public bool FeiXiaoExtraSkillPoint;   //��������֮����  ÿ�غϴ���һ�� �غϿ�ʼʱ���ô�������

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

    //��������Ability�� ĳЩStatus�Ĳ�������Ϊ �¼� ���䶯�����⣬����һЩStatus�� �ﵽĳ������ʱ����Ч��������  ����Ability��������¼�  (ӵ�м����򴥷�Ч��������)
    //�м�����������Status  ���漰�����ı䶯  ����һ���ᴥ���¼�  (����ʹ��ս���� ȫ�Ӽ�����buff��������)

    //�д�����������Status  ��һ���м����� (������Ѫ��Ѫ������  �������Skill������AddActionʵ��)
    public void AddCounterAbility()   
    {
        AddTriggerAbility();  //������û�м�����  �������AddCounterAbility���� �Ƿ���Ӽ���������hasCounter�ж�

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
                case Messenger.EventType.TargetAndAttackEnemy:                                        //�����������ɻƲ���  DealDamageһ���ж����ܶ�δ���  ���ź���һ��SKill�㲥һ��
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
        else if (trigger.triggerEffect == TriggerEffect.AddStatus)   //�������»�ɱ�����ӱ�����Ч��
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

    #region �������ص�
    public void CheckCondition()
    {
        if(hasTrigger == false)
        {
            return;
        }

        if (trigger.triggerLayer == 0)
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
    public void CheckCondition(Character attacker,Character attacked,float damageValue)  //��������ʼǰ  װ���߹������������͵ĵз�Ŀ���ָ�����
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
    private void DealDamageToEnemyTriggerAction(Character attacker, Character attacked, float damageValue)   //һ�غ�һ��  ���ѹ�������׷�ӹ���
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

    private void ConsumeSkillPointAction(int changeNumber)  //�����츳����Status  ����ս����
    {
        if(changeNumber < 0)   //С��0˵��������ս����
        {
            StatusAction.AddStatusLayerAction(Caster, Owner, this, -changeNumber);
        }
        Owner.FreshProperty(this);
    }

    private void KillTargetAction(Character attacter,Character killed)     //�������� ��ɱ
    {
        if (attacter.currentStatus.Contains(this))
        {
            StatusLayer = 1;
        }

        CheckCondition();
    }

    private void DealDamageToEnemyCounterAction(Character attacker,Character attacked,Skill_SO skill)   //���� �Ѿ������Ӳ���
    {
        Debug.Log($"{this}");
        if(attacker.type == Character.CharaterType.Player)
        {
            StatusLayer += 1;
        }
    }

    #endregion

    public void FreshStatusTurnStart(Character player)   //����׷�ӹ��� �غ�ˢ�»ص�
    {
        if(player.characterName != "FEIXIAO")   //��Ϊ�źŵ�ȫ�ֻ���  û��Status�� ����Ҳ�����ûص�
        {
            return;
        }
        if (FeiXiaoExtraSkillPoint)
        {
            if(StatusLayer == 1)   //�츳ҳ��Ч��
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
