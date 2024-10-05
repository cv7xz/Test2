using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticNumber
{
    public static List<float> brokenBaseDamage = new List<float>();


    public static float ActionTime = 0.1f; //行动后 怪物下一次行动cd


    public static Dictionary<(string, string), float> propertyDict = new Dictionary<(string, string), float>()
    {
        {("XUEYI", "101_BD"),0 },     //攻击
        {("XUEYI", "102_BD"),0.18f }, //生命
        {("XUEYI", "103_BD"),0.08f }, //增伤
        {("XUEYI", "104_BD"),37.3f }, //击破特攻
        {("XUEYI", "105_BD"),0 },     //爆伤
        {("XUEYI", "106_BD"),0 },     //命中
        {("XUEYI", "107_BD"),0 },     //抵抗
        {("XUEYI", "108_BD"),0 },     //暴击
        {("XUEYI", "109_BD"),0 },     //防御
        {("XUEYI", "110_BD"),0 },     //速度 fix

        {("HUAHUO", "101_BD"),0 },
        {("HUAHUO", "102_BD"),0.28f },
        {("HUAHUO", "103_BD"),0 },
        {("HUAHUO", "104_BD"),0 },
        {("HUAHUO", "105_BD"),0.24f },
        {("HUAHUO", "106_BD"),0 },
        {("HUAHUO", "107_BD"),0.1f },
        {("HUAHUO", "108_BD"),0 },
        {("HUAHUO", "109_BD"),0 },
        {("HUAHUO", "110_BD"),0 },

        {("YINLANG", "101_BD"),0.28f },
        {("YINLANG", "102_BD"),0 },
        {("YINLANG", "103_BD"),0 },
        {("YINLANG", "104_BD"),0 },
        {("YINLANG", "105_BD"),0.08f },
        {("YINLANG", "106_BD"),0.18f },
        {("YINLANG", "107_BD"),0 },
        {("YINLANG", "108_BD"),0 },
        {("YINLANG", "109_BD"),0 },
        {("YINLANG", "110_BD"),0 },

        {("FUXUAN", "101_BD"),0 },
        {("FUXUAN", "102_BD"),0.18f },
        {("FUXUAN", "103_BD"),0 },
        {("FUXUAN", "104_BD"),0 },
        {("FUXUAN", "105_BD"),0 },
        {("FUXUAN", "106_BD"),0 },
        {("FUXUAN", "107_BD"),0.1f },
        {("FUXUAN", "108_BD"),0.187f },
        {("FUXUAN", "109_BD"),0 },
        {("FUXUAN", "110_BD"),0 },

        {("FEIXIAO", "101_BD"),0.28f },
        {("FEIXIAO", "102_BD"),0 },
        {("FEIXIAO", "103_BD"),0 },
        {("FEIXIAO", "104_BD"),0 },
        {("FEIXIAO", "105_BD"),0 },
        {("FEIXIAO", "106_BD"),0 },
        {("FEIXIAO", "107_BD"),0 },
        {("FEIXIAO", "108_BD"),0.12f },
        {("FEIXIAO", "109_BD"),0.125f },
        {("FEIXIAO", "110_BD"),0 },

        {("RUANMEI", "101_BD"),0f },
        {("RUANMEI", "102_BD"),0 },
        {("RUANMEI", "103_BD"),0 },
        {("RUANMEI", "104_BD"),37.3f },
        {("RUANMEI", "105_BD"),0 },
        {("RUANMEI", "106_BD"),0 },
        {("RUANMEI", "107_BD"),0 },
        {("RUANMEI", "108_BD"),0f },
        {("RUANMEI", "109_BD"),0.225f },
        {("RUANMEI", "110_BD"),5f },

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
