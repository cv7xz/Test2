using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public static SelectManager Instance;
    public SelectPlayer selectPlayer;
    public SelectEquip selectEquip;

    public GameObject showPlayer, showEquip;

    public Slider playerSlider, equipSlider;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowCurrentSelect()
    {
        if(selectPlayer != null)
        {
            showPlayer.GetComponent<Image>().sprite = GameManager.Instance.AllPlayers[selectPlayer.playerIndex].GetComponent<Character>().sprite;
        }
        if(selectEquip != null)
        {
            showEquip.GetComponent<Image>().sprite = GameManager.Instance.AllEquip[selectEquip.equipIndex].GetComponent<EquipGO>().sprite;
        }
    }

}
