using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlayer : MonoBehaviour
{
    public int playerIndex;
    public string PlayerName;


    public void ShowCurrent()
    {
        SelectManager.Instance.selectPlayer = this;
        SelectManager.Instance.ShowCurrentSelect();
    }
}
