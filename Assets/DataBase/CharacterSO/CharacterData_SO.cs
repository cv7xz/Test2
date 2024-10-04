using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Attack Field")]
    public float baseAttack;
    public float AttackPercentBonus, fixAttackBonus,currentAttack;

    [Header("Health Field")]
    public float baseHealth;
    public float healthPercentBonus, currentHealth, maxHealth,fixHealthBonus;
    [Header("Defend Field")]
    public float baseDefend;
    public float DefendPercentBonus, currentDefend, fixDefendBonus;
    [Header("Critical Field")]
    public float criticalPercent;
    public float criticalDamage;
    [Header("Speed Field")]
    public float baseSpeed;
    public float speedPercentBonus, fixSpeedBonus, currentSpeed;
    [Header("Other Field")]
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

    public float damageDecrease;

    public float currentAllPentration, AllPentration;
    public float currentBINGPentration, BINGPentration;  //¿¹ÐÔ´©Í¸
    public float currentHUOPentration, HUOPentration;
    public float currentFENGPentration, FENGPentration;
    public float currentLEIPentration, LEIPentration;
    public float currentLIANGZIPentration, LIANGZIPentration;
    public float currentXUSHUPentration, XUSHUPentration;
    public float currentWULIPentration, WULIPentration;
    [Header("Enemy ElementDefend")]
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
