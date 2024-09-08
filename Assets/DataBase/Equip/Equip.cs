using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "new Equip", fileName = "Equip")]
public class Equip : ScriptableObject
{
    public float baseAttackValue, baseHealthValue, baseDefendValue;

    public List<Status> initStatus = new List<Status>();
}
