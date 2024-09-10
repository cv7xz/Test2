using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "SkillData",menuName = "Skill")]
public class Skill_SO : ScriptableObject
{
    public string skillID;

    public CharacterData_SO.weaknessType elementType;
    public enum Target
    {
        Enemy,
        Friend,
    }
    public Target target;
    public enum TargetType
    {
        SingleTarget,
        AllTarget,
        NeighborTarget,
        RandomTarget,
        AllOtherFriend,
        Self,
    }
    public TargetType attackType;
    
    public enum SkillType
    {
        DealDamage,
        Healing,
        AddStatus,
    }
    public enum DamageType
    {
        ExtraAttack,
        SkillAttack,
        FinalAttack,
    }
    public DamageType damageType;
    public List<SkillType> skillType;
    public enum DependProperty
    {
        CurrentAttack,
        CurrentHealth,
        CurrentDefend,
        CurrentLossHealh,
        maxHealth,
    }

    public List<DependProperty> dependProperties = new List<DependProperty>();

    //һ�����ܿ����漰���ֱ���������ֵ
    public List<float> toughDamage = new List<float>();
    public List<float> rates = new List<float>();

    public int skillPointConsumed;
    public int skillPointProvide;
    public float energyConsumed;
    public int RandomAttackNumber;  //������Ŀ�����͵���

    public float restoreEnergy;
    public bool ignoreWeakness;
    public List<float> basePercent;

    public CharacterData_SO.weaknessType BrokenEffect;

    public enum DamageIncreaseCondition
    {
        toughDamage,
        targetToughRate,

        XINGHUN,
    }

    public bool hasDamageIncrease;
    [System.Serializable]
    public struct OtherDamageIncrease
    {
        public DamageIncreaseCondition damageIncreaseConditions;
        public float conditionValue;
        public List<float> rates;
    }
    public List<OtherDamageIncrease> otherDamageIncrease = new List<OtherDamageIncrease>();


    [System.Serializable]
    public struct AddStatus
    {
        public Status status;
        public TargetType statusTarget;
    }
    public List<AddStatus> addStatus = new List<AddStatus>();

    public enum AddAction
    {
        PushActon,
        GetEnergy,
        GetSkillPoint,
        AddWeakness,
        AddStatusLayer,  //�����սἼ�Ӳ���
    }

    [System.Serializable]
    public struct Actions    //�ýṹ����ȫ����д��һ��ScirptableObject ��Ϊ���� Ӧ����: 555ȫ���Ѿ�����  ĳ�о����ĳ��������  
    {
        public AddAction addaction;
        public CharacterData_SO.weaknessType element;  //����ר�� �������Action 
        public float value;
        public string statusName;
        public TargetType targetType;
        public bool AfterDamage;  //һЩAction��Ҫ�����������˺�ǰ/��
    }
    public List<Actions> addactions = new List<Actions>();
}
