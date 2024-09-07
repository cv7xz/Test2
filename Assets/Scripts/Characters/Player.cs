using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override void Awake()
    {
        base.Awake();
        characterData = Instantiate(data);
        skillQ = Instantiate(QSKILLObject);
        skillE = Instantiate(ESKILLObject);
        skillFinal = Instantiate(SKILLFINALObject);

        friend_SO = Instantiate(FriendObject);
        for (int i = 0; i < friend_SO.InitStatusObject.Count; i++)
        {
            friend_SO.InitStatus.Add(Instantiate(friend_SO.InitStatusObject[i]));
        }

        if (friend_SO != null)
        {
            foreach (var status in friend_SO.InitStatus)
            {
                if (status.StatusName.Contains("BD"))
                {
                    if (status.StatusValue.Count == 0)
                    {
                        status.StatusValue.Add(StaticNumber.propertyDict[(characterName, status.StatusName)]);
                    }
                    else
                    {
                        status.StatusValue[0] = StaticNumber.propertyDict[(characterName, status.StatusName)];
                    }
                }
                StatusAction.AddStatusAction(this, this, status);
            }
        }


        characterData.currentSpeed = characterData.baseSpeed * (1 + characterData.speedPercentBonus);
        characterData.currentHealth = characterData.baseHealth * (1 + characterData.healthPercentBonus);
        characterData.maxHealth = characterData.baseHealth * (1 + characterData.healthPercentBonus);
        characterData.currentDefend = characterData.baseDefend * (1 + characterData.DefendPercentBonus);
        characterData.currentAttack = characterData.baseAttack * (1 + characterData.AttackPercentBonus) + characterData.fixAttackBonus;
        characterData.actionValue = InputManager.Instance.distance / characterData.currentSpeed;


        FreshProperty(Status.InvolvedProperty.AttackValue);
        FreshProperty(Status.InvolvedProperty.HealthValue);
        FreshProperty(Status.InvolvedProperty.DamageIncreaseValue);
        FreshProperty(Status.InvolvedProperty.BrokenFocus);
        FreshProperty(Status.InvolvedProperty.CriticalDamageValue);
    }
}
