using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnergyChangeAction 
{
    public static void AddEnergyAction(Character character,float value)
    {
        DamageAction.context += $"{character.name} ��������  {value}";
        character.characterData.currentEnergyValue += (1 + character.characterData.energyCollectEfficiency) * value;
        if(character.characterData.currentEnergyValue > 1e-6)
        {
            DamageAction.context += $"�ظ�Ч��: {character.characterData.energyCollectEfficiency}  ����� {(1 + character.characterData.energyCollectEfficiency) * value}\n";
        }

        if (character.characterData.currentEnergyValue > character.characterData.maxEnergyValue)
        {
            DamageAction.context += "�����ظ���\n";
            character.characterData.currentEnergyValue = character.characterData.maxEnergyValue;
        }


        GameManager.Instance.statusText.text = DamageAction.context;
    }
}
