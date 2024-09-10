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
                DamageAction.context += $"{target.name} ���� ��ǰ����ֵ {target.characterData.currentHealth}\n";

                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth - caster.characterData.currentHealth) * skill.rates[rateIndex];

                DamageAction.context += $"������ {caster.characterData.maxHealth - caster.characterData.currentHealth * skill.rates[rateIndex]} ���������ֵ {target.characterData.currentHealth}\n";
            }
            else if(depend == Skill_SO.DependProperty.maxHealth)
            {
                DamageAction.context += $"{target.name} ���� ��ǰ����ֵ {target.characterData.currentHealth}\n";
                target.characterData.currentHealth = Mathf.Min(target.characterData.maxHealth, target.characterData.currentHealth + caster.characterData.maxHealth  * skill.rates[rateIndex]);

                DamageAction.context += $"������ {caster.characterData.maxHealth * skill.rates[rateIndex]} ���������ֵ {target.characterData.currentHealth}\n";
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
