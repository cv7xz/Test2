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
                DamageAction.context += $"{target.name} 治疗 当前生命值 {target.characterData.currentHealth}\n";

                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth - caster.characterData.currentHealth) * skill.rates[rateIndex];

                DamageAction.context += $"治疗量 {caster.characterData.maxHealth - caster.characterData.currentHealth * skill.rates[rateIndex]} 处理后生命值 {target.characterData.currentHealth}\n";
            }
            else if(depend == Skill_SO.DependProperty.maxHealth)
            {
                DamageAction.context += $"{target.name} 治疗 当前生命值 {target.characterData.currentHealth}\n";
                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth  * skill.rates[rateIndex]);

                DamageAction.context += $"治疗量 {caster.characterData.maxHealth * skill.rates[rateIndex]} 处理后生命值 {target.characterData.currentHealth}\n";
            }
        }
        GameManager.Instance.statusText.text = DamageAction.context;
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
