using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusAction
{
    public static void AddStatusAction(Character source,Character target,Status status)
    {
        Status originStatus = target.currentStatus.Find(e => e.StatusName == status.StatusName);
        if(originStatus == null)
        {
            status.Caster = source;
            status.Owner = target;
            if (status.StatusGroup)
            {
                int randomIndex = Random.Range(0, status.childStatus.Count);
                var cloneChildStatus = GameObject.Instantiate(status.childStatus[randomIndex]);
                target.currentStatus.Add(cloneChildStatus);
                target.FreshProperty(cloneChildStatus);
                cloneChildStatus.AddCounterAbility();
            }
            else
            {
                target.currentStatus.Add(status);
                target.FreshProperty(status);
                status.AddCounterAbility();
            }
            return;
        }
        //重复添加刷新规则
        if (status.repeatRule == Status.RepeatRule.LayerStack)
        {
            originStatus.StatusLayer += status.StatusLayer;
        }
        else if(status.repeatRule == Status.RepeatRule.Replace)
        {
            originStatus = status;
        }
        //重复是否刷新持续时间
        if (status.ReapetFreshDuration)
        {
            originStatus.duration = status.duration;
        }
        //层数限制
        originStatus.StatusLayer = Mathf.Min(originStatus.StatusLayer, originStatus.LayerLimited);


        status.Caster = source;
        status.Owner = target;
        if (status.StatusGroup)
        {
            int randomIndex = Random.Range(0, status.childStatus.Count);
            var cloneChildStatus = GameObject.Instantiate(status.childStatus[randomIndex]);
            //target.currentStatus.Add(cloneChildStatus);
            target.FreshProperty(cloneChildStatus);
            cloneChildStatus.AddCounterAbility();
        }
        else
        {
            //target.currentStatus.Add(status);
            target.FreshProperty(status);
            status.AddCounterAbility();
        }
        //Messenger.Instance.BroadCast(Messenger.EventType.StatusChange, status);
    }

    public static void AddStatusLayerAction(Character source, Character target, Status status, int addLayer)
    {
        Status originStatus = target.currentStatus.Find(e => e.StatusName == status.StatusName);
        if (originStatus != null)
        {
            originStatus.StatusLayer = Mathf.Min(originStatus.StatusLayer + addLayer, originStatus.LayerLimited);
            if (originStatus.ReapetFreshDuration)   
            {
                Status AssetStatus = GameManager.Instance.AllStatus.Find(e => e.StatusName == status.StatusName);
                if(AssetStatus == null)
                {
                    Debug.LogError("游戏中不存在该Status!");
                }
                originStatus.duration = AssetStatus.duration;
            }
            target.FreshProperty(status);
            return;
        }
        else
        {
            Debug.LogError("尝试添加层数 但Buff不存在");
        }
    }

    public static void AddStatusAllFriend(Character source, Status status,CharacterData_SO.weaknessType type = CharacterData_SO.weaknessType.NONE)
    {
        foreach(var player in GameManager.Instance.players)
        {
            if(player != null)
            {
                if(player.characterData.elementType == type || type == CharacterData_SO.weaknessType.NONE)
                {
                    var cloneStatus = GameObject.Instantiate(status);
                    AddStatusAction(source, player, cloneStatus);
                }
            }
        }
    }


}
