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
    public enum SkillQER   //强行枚举出 普攻战技终结技
    {
        Q,
        E,
        R,
        other,
    }
    public SkillQER skillQER;
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

    //一个技能可能涉及多种倍率与削韧值
    public List<float> toughDamage = new List<float>();
    public List<float> rates = new List<float>();

    public int skillPointConsumed;
    public int skillPointProvide;
    public float energyConsumed;
    [HideInInspector]
    public int RandomAttackNumber;  //适用于目标类型弹射

    public float restoreEnergy;
    public bool ignoreWeakness;
    public List<float> basePercent;

    public CharacterData_SO.weaknessType BrokenEffect;

    public enum DamageIncreaseCondition
    {
        toughDamage,
        targetToughRate,

        targetBroken,
        targetNotBroken,
        XINGHUN,
    }

    public bool hasDamageIncrease;

    public enum DamageIncreaseType
    {
        DamageIncreaseProperty,
        SkillRate,
        ToughEfficiency,
    }
    [System.Serializable]
    public struct OtherDamageIncrease
    {
        public DamageIncreaseCondition damageIncreaseConditions;
        public DamageIncreaseType damageIncreaseType;
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
        AddStatusLayer,  //符玄终结技加层数
        ExecuteSkill,   //飞霄技能 跟一次追加 (子技能)
        AddStatusDuration,
    }

    [System.Serializable]
    public struct Actions    //该结构体完全可以写成一个ScirptableObject 较为复杂 应用有: 555全体友军拉条  某敌军添加某属性弱点  
    {
        public AddAction addaction;
        public CharacterData_SO.weaknessType element;  //银狼专用 添加弱点Action 
        public float value;
        public string statusName;
        public Skill_SO skill;
        public TargetType targetType;
        public bool AfterDamage;  //一些Action需要具体作用于伤害前/后
    }
    public List<Actions> addactions = new List<Actions>();

    public enum SpecialFinalSkill
    {
        None,
        FEIXIAO,
    }
    public SpecialFinalSkill specialFinalSkill;
}
