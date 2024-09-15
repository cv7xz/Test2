using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEquip : MonoBehaviour
{
    public int equipIndex;
    public string equipName;


    public void ShowCurrent()
    {
        SelectManager.Instance.selectEquip = this;
        SelectManager.Instance.ShowCurrentSelect();
    }
}
