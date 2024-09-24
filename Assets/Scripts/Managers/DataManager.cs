using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Dictionary<string, DataClass> TotalData = new Dictionary<string, DataClass>();

    public Dictionary<string, CurrentPlayerData> currentTotalData = new Dictionary<string, CurrentPlayerData>();
    public enum PropertyType
    {
        MainDamage,
        BaseEffectPercent,
    }
    [System.Serializable]
    public struct playerPropertyData
    {
        public Status.StatusType propertyType;
        public float Value;
    }

    
    [System.Serializable]
    public struct playerLevelData
    {
        public float baseHealth;
        public float baseAttack;
        public float baseDefend;

    }
    public enum RelicType
    {

    }

    public class DataClass
    {
        public List<playerLevelData> LevelData = new List<playerLevelData>();

        public playerPropertyData L1P, L2P, L3P, R1P, R2P, R3P, M1P, M2P, M3P, M4P;
        public List<Dictionary<PropertyType,List<float>>> LB1,MB1,RB1,Talent,Attack,FinalSkill;   //LB1[2][XXX] = value

        
    }

    public class CurrentPlayerData
    {
        public int Level;

        public int constellation;

        public int L1P, L2P, L3P, R1P, R2P, R3P, M1P, M2P, M3P, M4P;
    }
}

