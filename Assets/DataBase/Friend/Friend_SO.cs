using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Friend", menuName = "Friend")]
public class Friend_SO : ScriptableObject
{
    public string CharacterName;
    public List<Status> InitStatus = new List<Status>();

    public List<Status> InitStatusObject = new List<Status>();
}
