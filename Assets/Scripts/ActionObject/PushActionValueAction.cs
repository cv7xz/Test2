using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PushActionValueAction
{
    public static void PushActionValue(Character character,float distance)
    {
        character.characterData.actionValue -= distance / character.characterData.currentSpeed;
        
        if(character.characterData.actionValue < 0)
        {
            character.characterData.actionValue = 0;
        }
    }
}
