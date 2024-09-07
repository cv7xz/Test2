using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticNumber
{
    public static List<float> brokenBaseDamage = new List<float>();

    public static float ActionTime = 0.5f; //行动后 怪物下一次行动cd

    //攻击 生命 增伤 击破特攻 爆伤 命中 抵抗
    public static Dictionary<(string, string), float> propertyDict = new Dictionary<(string, string), float>()
    {
        {("XUEYI", "101_BD"),0 },
        {("XUEYI", "102_BD"),0.18f },
        {("XUEYI", "103_BD"),0.08f },
        {("XUEYI", "104_BD"),37.3f },
        {("XUEYI", "105_BD"),0 },
        {("XUEYI", "106_BD"),0 },
        {("XUEYI", "107_BD"),0 },

        {("HUAHUO", "101_BD"),0 },
        {("HUAHUO", "102_BD"),0.28f },
        {("HUAHUO", "103_BD"),0 },
        {("HUAHUO", "104_BD"),0 },
        {("HUAHUO", "105_BD"),0.24f },
        {("HUAHUO", "106_BD"),0 },
        {("HUAHUO", "107_BD"),0.1f },

        {("YINLANG", "101_BD"),0.28f },
        {("YINLANG", "102_BD"),0 },
        {("YINLANG", "103_BD"),0 },
        {("YINLANG", "104_BD"),0 },
        {("YINLANG", "105_BD"),0.08f },
        {("YINLANG", "106_BD"),0.18f },
        {("YINLANG", "107_BD"),0 },

        //{(XUEYI, "101_BD"),0 },
        //{(XUEYI, "102_BD"),0 },
        //{(XUEYI, "103_BD"),0 },
        //{(XUEYI, "104_BD"),0 },
    };
    public static int GetPlayerNumber(CharacterData_SO.weaknessType Type)
    {
        int number = 0;
        foreach(var player in GameManager.Instance.players)
        {
            if(player != null)
            {
                if(player.characterData.elementType == Type)
                {
                    number += 1;
                }
            }
        }
        return number;
    }
}
