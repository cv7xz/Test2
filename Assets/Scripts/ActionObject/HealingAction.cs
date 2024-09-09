using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HealingAction
{
    public static void DealHealingAction(Character caster,Character target, Skill_SO skill = null)
    {
        foreach(var depend in skill.dependProperties)
        {
            int rateIndex = skill.dependProperties.IndexOf(depend);
            if(depend == Skill_SO.DependProperty.CurrentLossHealh)
            {
                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth - caster.characterData.currentHealth) * skill.rates[rateIndex];
            }
            else if(depend == Skill_SO.DependProperty.maxHealth)
            {
                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth - caster.characterData.currentHealth) * skill.rates[rateIndex];
            }
        }
    }

    public static void DealHealingAllAction(Character caster, Skill_SO skill = null)
    {
        foreach(var player in GameManager.Instance.players)
        {
            if (player != null)
            {
                DealHealingAction(caster, player, skill);
            }
        }
    }
}
