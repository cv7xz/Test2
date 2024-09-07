using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnergyChangeAction 
{
    public static void AddEnergyAction(Character character,float value)
    {
        character.characterData.currentEnergyValue += (1 + character.characterData.energyCollectEfficiency) * value;

        if(character.characterData.currentEnergyValue > character.characterData.maxEnergyValue)
        {
            character.characterData.currentEnergyValue = character.characterData.maxEnergyValue;
        }

    }
}
