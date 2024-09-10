using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnergyChangeAction 
{
    public static void AddEnergyAction(Character character,float value)
    {
        DamageAction.context += $"{character.name} 能量增加  {value}";
        character.characterData.currentEnergyValue += (1 + character.characterData.energyCollectEfficiency) * value;
        if(character.characterData.currentEnergyValue > 1e-6)
        {
            DamageAction.context += $"回复效率: {character.characterData.energyCollectEfficiency}  处理后 {(1 + character.characterData.energyCollectEfficiency) * value}\n";
        }

        if (character.characterData.currentEnergyValue > character.characterData.maxEnergyValue)
        {
            DamageAction.context += "能量回复满\n";
            character.characterData.currentEnergyValue = character.characterData.maxEnergyValue;
        }


        GameManager.Instance.statusText.text = DamageAction.context;
    }
}
