using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy :Character
{
    public GameObject ToughBarPrefab, ToughBar;
    public Transform ToughPos;

    public override void Awake()
    {
        base.Awake();

        characterData = Instantiate(data);
        characterData.baseDefend = characterData.Level * 10 + 200;

        characterData.currentSpeed = characterData.baseSpeed * (1 + characterData.speedPercentBonus);
        characterData.currentHealth = characterData.baseHealth * (1 + characterData.healthPercentBonus);
        characterData.maxHealth = characterData.baseHealth * (1 + characterData.healthPercentBonus);
        characterData.currentDefend = characterData.baseDefend * (1 + characterData.DefendPercentBonus);
        characterData.currentAttack = characterData.baseAttack * (1 + characterData.AttackPercentBonus) + characterData.fixAttackBonus;

        characterData.currentBINGDefend = characterData.BINGDefend;
        characterData.currentHUODefend = characterData.HUODefend;
        characterData.currentFENGDefend = characterData.FENGDefend;
        characterData.currentLEIDefend = characterData.LEIDefend;
        characterData.currentWULIDefend = characterData.WULIDefend;
        characterData.currentLIANGZIDefend = characterData.LIANGZIDefend;
        characterData.currentXUSHUDefend = characterData.XUSHUDefend;

        characterData.actionValue = InputManager.Instance.distance / characterData.currentSpeed;
    }
    public override void Start()
    {
        base.Start();
        ToughBar = Instantiate(ToughBarPrefab, GameManager.Instance.canvas.transform);
        ToughBar.transform.position = ToughPos.transform.position;
        ShowSelfWeakness();

        characterData.currentToughShield = characterData.maxToughShield;
        
        Messenger.Instance.AddListener<Character,Skill_SO>(Messenger.EventType.ToughShieldBroken, ShieldBroken);
    }
   
    public override void Update()
    {
        base.Update();
        ShowToughBar();
    }
    public void ShieldBroken(Character source,Skill_SO skill)
    {
        DamageAction.BrokenDamageAction(source, this);
    }

    public void ShowToughBar()
    {
        ToughBar.transform.GetChild(0).GetComponent<Image>().fillAmount = characterData.currentToughShield / characterData.maxToughShield;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(ToughBar);
        Messenger.Instance.RemoveListener<Character,Skill_SO>(Messenger.EventType.ToughShieldBroken, ShieldBroken);
    }
}
