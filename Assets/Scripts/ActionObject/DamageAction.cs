using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageAction
{
    public static string context = "";
    public static DamageInfo DealDamageAction(Character attacker, Skill_SO skill, Character attacked, int Index) //���������ĸ����ܹ����Է�
    {
        DamageInfo damageInfo = new DamageInfo(attacker, attacked, skill);
        if (attacker == null || attacked == null)
        {
            return damageInfo;
        }
        CharacterData_SO attackerData = attacker.characterData;
        CharacterData_SO attackedData = attacked.characterData;

        float toughDamage = 0;
        float realToughDamage = 0;
        float overToughDamage = 0;
        #region ���ͼ���
        if (attackedData.weakness.Contains(skill.elementType) || skill.ignoreWeakness)
        {
            toughDamage = skill.toughDamage[Index] * (1 + attackerData.BrokenEfficiencyBonus);
            realToughDamage = Mathf.Min(toughDamage, attackedData.currentToughShield);
            overToughDamage = toughDamage - realToughDamage;

            if (toughDamage >= attackedData.currentToughShield)
            {
                Messenger.Instance.BroadCast(Messenger.EventType.ToughShieldBroken, attacker, skill);
            }
            attackedData.currentToughShield -= realToughDamage;

        }
        #endregion
        context += "<color=blue>" + attacker.name + " Attack " + attacked.name + "</color>"+" !\n";
        #region �˺�����
        float damageValue = attackerData.currentAttack;
        context += "��ǰ������: " + attackerData.currentAttack.ToString()+ "\n";


        bool isCritical = attackerData.criticalPercent >= Random.Range(0f, 1f);
        if (isCritical)
        {
            damageValue *= (1 + attackerData.criticalDamage);
            context += "  �Ƿ񱩻�: " + "<color=red>" + isCritical.ToString() +"</color>" + " ������˺�: " + damageValue.ToString() + "\n";
        }
        else
        {
            context += "  �Ƿ񱩻�: " + isCritical.ToString() + " ������˺�: " + damageValue.ToString() + "\n";
        }

        damageValue *= skill.rates[Index];
        context += "  ���ܱ���: " + skill.rates[Index].ToString() + " ������˺�: " + damageValue.ToString() + "\n";

        float tempDamageIncrease = 0f;
        if (skill.hasDamageIncrease)
        {
            foreach(var increase in skill.otherDamageIncrease)
            {
                if(increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.toughDamage)
                {
                    tempDamageIncrease += realToughDamage * increase.rates[0];
                }
                else if(increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetToughRate)
                {
                    if(attackedData.currentToughShield >= attackedData.maxToughShield * increase.conditionValue)
                    {
                        tempDamageIncrease += increase.rates[0];
                    }
                }
            }
        }
        damageValue *= (1 + attackerData.damageIncrease + tempDamageIncrease);
        context += "  ���˴���: " + attackerData.damageIncrease.ToString() + "  ��ʱ����: " + tempDamageIncrease.ToString() + " ������˺�: " + damageValue.ToString() + "\n";


        float damageDecrease = attackedData.currentDefend / (attackerData.Level * 10 + 200 + attackedData.currentDefend);
        damageValue *= (1 - damageDecrease);
        context += "  ��������: " + damageDecrease + " ������˺�: " + damageValue.ToString() + "\n";


        if (attackedData.currentToughShield > 0)
        {
            damageValue *= 0.9f;
            context += "  ���Լ���: 10%" + " ������˺�: " + damageValue.ToString() + "\n";
        }
        float weaknessDefend;

        #region ����Ϳ��Էֱ��� ���Ǹ����Ƿ��������жϿ�����
        if (skill.elementType == CharacterData_SO.weaknessType.BING)
        {
            weaknessDefend = attackedData.currentBINGDefend;
        }
        else if (skill.elementType == CharacterData_SO.weaknessType.HUO)
        {
            weaknessDefend = attackedData.currentHUODefend;
        }
        else if (skill.elementType == CharacterData_SO.weaknessType.FENG)
        {
            weaknessDefend = attackedData.currentFENGDefend;
        }
        else if (skill.elementType == CharacterData_SO.weaknessType.LEI)
        {
            weaknessDefend = attackedData.currentLEIDefend;
        }
        else if (skill.elementType == CharacterData_SO.weaknessType.WULI)
        {
            weaknessDefend = attackedData.currentWULIDefend;
        }
        else if (skill.elementType == CharacterData_SO.weaknessType.LIANGZI)
        {
            weaknessDefend = attackedData.currentLIANGZIDefend;
        }
        else 
        {
            weaknessDefend = attackedData.currentXUSHUDefend;
        }
        #endregion

        damageValue *= (1 - weaknessDefend);
        context += "  ����: "+ weaknessDefend + " ������˺�: " + "<color=red>"+ damageValue.ToString()+ "</color>" + "\n"; ;
        context += "-------------------------------------\n";
        damageValue = Mathf.Max(damageValue, 1f);

        GameManager.Instance.statusText.text = context;
        attackedData.currentHealth -= damageValue;
        #endregion

        if(damageValue >= attackedData.currentHealth)
        {
            Messenger.Instance.BroadCast(Messenger.EventType.KillTarget, attacker,attacked);
        }
        damageInfo.damageValue = damageValue;
        damageInfo.isBrokenDamage = false;
        damageInfo.elementType = skill.elementType;
        damageInfo.skill = skill;
        damageInfo.toughDamage = realToughDamage;


        GameManager.Instance.DamageAppearFunc(attacker,attacked,damageValue);

       

        Messenger.Instance.BroadCast(Messenger.EventType.DealDamage,attacker,attacked, damageValue);
        return damageInfo;
    }

    public static void DealDamageAction(Character attacker, Character attacked)
    {
        if (attacker == null || attacked == null)
        {
            return;
        }
        CharacterData_SO attackerData = attacker.characterData;
        CharacterData_SO attackedData = attacked.characterData;

        float damageValue = attackerData.currentAttack;

        bool isCritical = attackerData.criticalPercent > Random.Range(0f, 1f);
        if (isCritical)
        {
            damageValue *= (1 + attackerData.criticalDamage);
        }
        damageValue *= (1 + attackerData.damageIncrease);

        damageValue = Mathf.Max(damageValue, 1f);
        attackedData.currentHealth -= damageValue;

        GameManager.Instance.DamageAppearFunc(attacker, attacked, damageValue);
    }
    public static float BrokenDamageAction(Character source, Character attacked)
    {
        CharacterData_SO attackerData = source.characterData;
        CharacterData_SO attackedData = attacked.characterData;

        float damageValue = 50;

        damageValue *= (1 + attackerData.BrokensFocus);
        if (attackerData.elementType == CharacterData_SO.weaknessType.HUO || attackerData.elementType == CharacterData_SO.weaknessType.WULI)
        {
            damageValue *= 4;
        }
        else if (attackerData.elementType == CharacterData_SO.weaknessType.LEI || attackerData.elementType == CharacterData_SO.weaknessType.BING)
        {
            damageValue *= 2;
        }
        else if (attackerData.elementType == CharacterData_SO.weaknessType.FENG)
        {
            damageValue *= 3;
        }

        //����������1��

        damageValue *= (attackedData.maxToughShield * 0.1f + 2f) / 4f;



        float damageDecrease = attackedData.currentDefend / (attackerData.Level * 10 + 200 + attackedData.currentDefend);
        damageValue *= (1 - damageDecrease);

        damageValue *= 0.9f;
        return damageValue;
    }
}
