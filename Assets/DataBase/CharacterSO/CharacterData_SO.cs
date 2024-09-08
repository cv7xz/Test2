using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData")]
public class CharacterData_SO : ScriptableObject
{
    public float baseAttack, AttackPercentBonus, fixAttackBonus,currentAttack;
    public float baseHealth, healthPercentBonus, currentHealth, maxHealth,fixHealthBonus;
    public float baseDefend, DefendPercentBonus, currentDefend, fixDefendBonus;
    public float criticalPercent, criticalDamage;
    public float baseSpeed, speedPercentBonus, fixSpeedBonus, currentSpeed;

    public float effectPercent, effectDefend;

    #region Player


    public float damageIncrease;
    public float BrokensFocus;
    public float energyCollectEfficiency;
    public float currentEnergyValue;
    public float maxEnergyValue;
    public float BrokenEfficiencyBonus;
    public weaknessType elementType;
    #endregion

    public float currentBINGDefend, BINGDefend;  //¿¹ÐÔ
    public float currentHUODefend, HUODefend;
    public float currentFENGDefend, FENGDefend;
    public float currentLEIDefend, LEIDefend;
    public float currentLIANGZIDefend, LIANGZIDefend;
    public float currentXUSHUDefend, XUSHUDefend;
    public float currentWULIDefend, WULIDefend;

    public float maxToughShield;
    public float currentToughShield;
    public enum weaknessType
    {
        BING,
        HUO,
        FENG,
        LEI,
        WULI,
        LIANGZI,
        XUSHU,
        NONE,
    }
    public List<weaknessType> weakness = new List<weaknessType>();
    
    public float actionValue;
    public int Level;
}
