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
    }

    public List<DependProperty> dependProperties = new List<DependProperty>();

    //一个技能可能涉及多种倍率与削韧值
    public List<float> toughDamage = new List<float>();
    public List<float> rates = new List<float>();

    public int skillPointConsumed;
    public int skillPointProvide;
    public float energyConsumed;
    public int RandomAttackNumber;  //适用于目标类型弹射

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
    public List<Status> addStatus = new List<Status>();

    public enum AddAction
    {
        PushActon,
        GetEnergy,
        GetSkillPoint,
        AddWeakness,
    }

    [System.Serializable]
    public struct Actions
    {
        public AddAction addaction;
        public CharacterData_SO.weaknessType element;  //银狼专用 添加弱点Action 
        public float value;
        public bool AfterDamage;  //一些Action需要具体作用于伤害前/后

        public Actions(AddAction a, CharacterData_SO.weaknessType b, float c,bool d)
        {
            addaction = a;
            element = b;
            value = c;
            AfterDamage = d;
        }
    }
    public List<Actions> addactions = new List<Actions>();
}
