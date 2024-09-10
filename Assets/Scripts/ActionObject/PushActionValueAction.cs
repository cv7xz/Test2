using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PushActionValueAction
{
    public static void PushActionValue(Character character, float distance)
    {
        DamageAction.context += $"{character.name} �ж���ǰ {distance} �ж�ֵ: {character.characterData.actionValue}->\n";
        character.characterData.actionValue -= distance / character.characterData.currentSpeed;

        if (character.characterData.actionValue < 0)
        {
            character.characterData.actionValue = 0;
        }
        DamageAction.context += $"{character.characterData.actionValue}\n";

        GameManager.Instance.statusText.text = DamageAction.context;
    }
    public static void PushAllActionValue(float distance)
    {
        foreach (var character in GameManager.Instance.players)
        {
            if (character != null)
            {
                PushActionValue(character, distance);
            }
        }
    }
}
