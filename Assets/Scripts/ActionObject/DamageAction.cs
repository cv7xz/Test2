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

        float tempDamageIncrease = 0f;
        float tempSkillRateIncrease = 0f;
        float tempToughDamageEfficiency = 0f;

        #region ��������
        if (skill.hasDamageIncrease)
        {
            foreach (var increase in skill.otherDamageIncrease)
            {
                if (increase.damageIncreaseType == Skill_SO.DamageIncreaseType.DamageIncreaseProperty)
                {
                    if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.toughDamage)
                    {
                        tempDamageIncrease += realToughDamage * increase.rates[0] * 0.01f;
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetToughRate)
                    {
                        if (attackedData.currentToughShield >= attackedData.maxToughShield * increase.conditionValue)
                        {
                            tempDamageIncrease += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetBroken)
                    {
                        if (attackedData.currentToughShield <= 1e-6)
                        {
                            tempDamageIncrease += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetNotBroken)
                    {
                        if (attackedData.currentToughShield > 1e-6)
                        {
                            tempDamageIncrease += increase.rates[0];
                        }
                    }
                }
                else if (increase.damageIncreaseType == Skill_SO.DamageIncreaseType.SkillRate)
                {
                    if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.toughDamage)
                    {
                        tempSkillRateIncrease += realToughDamage * increase.rates[0] * 0.01f;
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetToughRate)
                    {
                        if (attackedData.currentToughShield >= attackedData.maxToughShield * increase.conditionValue)
                        {
                            tempSkillRateIncrease += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetBroken)
                    {
                        if (attackedData.currentToughShield <= 1e-6)
                        {
                            tempSkillRateIncrease += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetNotBroken)
                    {
                        if (attackedData.currentToughShield > 1e-6)
                        {
                            tempSkillRateIncrease += increase.rates[0];
                        }
                    }
                }
                else if (increase.damageIncreaseType == Skill_SO.DamageIncreaseType.ToughEfficiency)
                {
                    if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.toughDamage)
                    {
                        tempToughDamageEfficiency += realToughDamage * increase.rates[0] * 0.01f;
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetToughRate)
                    {
                        if (attackedData.currentToughShield >= attackedData.maxToughShield * increase.conditionValue)
                        {
                            tempToughDamageEfficiency += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetBroken)
                    {
                        if (attackedData.currentToughShield <= 1e-6)
                        {
                            tempToughDamageEfficiency += increase.rates[0];
                        }
                    }
                    else if (increase.damageIncreaseConditions == Skill_SO.DamageIncreaseCondition.targetNotBroken)
                    {
                        if (attackedData.currentToughShield > 1e-6)
                        {
                            tempToughDamageEfficiency += increase.rates[0];
                        }
                    }
                }
            }
        }
        #endregion

        #region  Status �ض����� ��׷�ӹ����ı�������
        float tempCriticalDamageIncrease = 0f;
        foreach(var status in attacker.currentStatus)
        {
            if (status.HasBonusLimit)
            {
                if(skill.damageType == status.limitType.BonusAttackType)
                {
                    if(status.statusType == Status.StatusType.CriticalDamageBonus)
                    {
                        tempCriticalDamageIncrease += status.StatusValue[0];
                    }
                }
            }
        }
        #endregion

        #region ���ͼ���
        if (attackedData.weakness.Contains(skill.elementType) || skill.ignoreWeakness)
        {
            toughDamage = skill.toughDamage[Index] * (1 + (attackerData.BrokenEfficiencyBonus + tempToughDamageEfficiency));
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

        float damageValue = attackerData.currentAttack;
        #region �˺�����
        if (skill.dependProperties.Count == 0)
        {
            context += "��ǰ������: " + attackerData.currentAttack.ToString() + "\n";
        }
        else if(skill.dependProperties[0] == Skill_SO.DependProperty.CurrentHealth)
        {
            damageValue = attackerData.currentHealth;
            context += "��ǰ����ֵ: " + attackerData.currentHealth.ToString() + "\n";
        }


        bool isCritical = attackerData.criticalPercent >= Random.Range(0f, 1f);
        if (isCritical)
        {
            damageValue *= (1 + attackerData.criticalDamage + tempCriticalDamageIncrease);
            context += "  �Ƿ񱩻�: " + "<color=red>" + isCritical.ToString() +"</color>" + "����: " + (attackerData.criticalDamage + tempCriticalDamageIncrease).ToString() + " ������˺�: " + damageValue.ToString() + "\n";
        }
        else
        {
            context += "  �Ƿ񱩻�: " + isCritical.ToString() + " ������˺�: " + damageValue.ToString() + "\n";
        }



       
        damageValue *= (skill.rates[Index] + tempSkillRateIncrease);
        context += "  ���ܱ���: " + (skill.rates[Index] + tempSkillRateIncrease).ToString() + " ������˺�: " + damageValue.ToString() + "\n";

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

        GameManager.Instance.FreshBattleInfor(context);
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

        DealDamageAddress(attacker, attacked, skill, damageValue);

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

        context += "<color=green>" + attacker.name + " Attack " + attacked.name + "</color>" + " !\n";

        float damageValue = attackerData.currentAttack;

        context += "��ǰ������: " + attackerData.currentAttack.ToString() + "\n";

        damageValue *= (1 + attackerData.damageIncrease);

        damageValue *= attackedData.currentDefend / (attackerData.Level * 10 + 200 + attackedData.currentDefend);

        context += "  ��������: " + attackedData.currentDefend / (attackerData.Level * 10 + 200 + attackedData.currentDefend) + " ������˺�: " + damageValue.ToString() + "\n";

        damageValue *= (1 - attackedData.damageDecrease);

        context += "  �ٷֱȼ���: " + attackedData.damageDecrease.ToString() + " ������˺�: " + damageValue.ToString() + "\n";

        damageValue = Mathf.Max(damageValue, 1f);

        #region ��̯�˺�
        foreach (var status in attacked.currentStatus)
        {
            if(status.statusType == Status.StatusType.ShareDamage)
            {
                attackedData.currentHealth -= damageValue * (1 - status.StatusValue[0]);
                GameManager.Instance.DamageAppearFunc(attacker, attacked, damageValue * (1 - status.StatusValue[0]));
                GameManager.Instance.DamageBallAppear(attacker, attacked, damageValue);
                Messenger.Instance.BroadCast(Messenger.EventType.DealDamage, attacker, attacked, damageValue);
                Messenger.Instance.BroadCast(Messenger.EventType.TakeDamage, damageValue,attacked);

                status.Caster.characterData.currentHealth -= damageValue * (status.StatusValue[0]);
                GameManager.Instance.DamageAppearFunc(attacker, status.Caster, damageValue * (status.StatusValue[0]));
                GameManager.Instance.DamageBallAppear(attacker, attacked, damageValue);
                Messenger.Instance.BroadCast(Messenger.EventType.DealDamage, attacker, status.Caster, damageValue);
                Messenger.Instance.BroadCast(Messenger.EventType.TakeDamage, damageValue, attacked);

                context += "��̯�˺�: " + attackedData.name + "takes " + (damageValue * (1 - status.StatusValue[0])).ToString() + "damage\n";
                context += "��̯�˺�: " + status.Caster.name + "takes " + (damageValue * (status.StatusValue[0])).ToString() + "damage\n";
                GameManager.Instance.FreshBattleInfor(context);
                return;   //��ʱֻ����һ���̯�˺�Status����
            }
        }
        #endregion
        GameManager.Instance.FreshBattleInfor(context);

        attackedData.currentHealth -= damageValue;
        Messenger.Instance.BroadCast(Messenger.EventType.DealDamage, attacker, attacked, damageValue);
        Messenger.Instance.BroadCast(Messenger.EventType.TakeDamage, damageValue, attacked);
        GameManager.Instance.DamageAppearFunc(attacker, attacked, damageValue);
        GameManager.Instance.DamageBallAppear(attacker, attacked, damageValue);
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


    public static void DealDamageAddress(Character attacker,Character attacked,Skill_SO skill,float damageValue)
    {
        GameManager.Instance.DamageAppearFunc(attacker, attacked, damageValue);
        GameManager.Instance.DamageBallAppear(attacker, attacked, damageValue);

        Messenger.Instance.BroadCast(Messenger.EventType.DealDamage, attacker, attacked, damageValue);      //����������˺�
        Messenger.Instance.BroadCast(Messenger.EventType.TakeDamage, damageValue, attacked);                //�����˺�
    }
}
