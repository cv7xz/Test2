using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{


    [System.Serializable]
    public struct playerTalentData
    {

    }
    [System.Serializable]
    public struct playerLevelData
    {
        public float baseHealth;
        public float baseAttack;
        public float baseDefend;

    }
    public List<playerLevelData> XUEYILevelDATA = new List<playerLevelData>();

}
