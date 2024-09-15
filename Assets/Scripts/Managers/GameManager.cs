using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform canvas;
    public List<GameObject> AllEnemy = new List<GameObject>(); //预制体

    public List<GameObject> AllPlayers = new List<GameObject>();
    
    public List<GameObject> AllEquip = new List<GameObject>();
    public List<GameObject> AllEquip_HUIMIE = new List<GameObject>();
    public List<GameObject> AllEquip_ZHISHI = new List<GameObject>();
    public List<GameObject> AllEquip_XUWU = new List<GameObject>();
    public List<GameObject> AllEquip_XUNLIE = new List<GameObject>();
    public List<GameObject> AllEquip_CUNHU = new List<GameObject>();

    public List<Status> AllStatus = new List<Status>();

    public List<Character> enemies = new List<Character>(5);
    public List<Character> players = new List<Character>(5);

    public List<Transform> enemySlots = new List<Transform>();
    public List<Transform> playerSlots = new List<Transform>();

    #region UI
    public GameObject TipsPanel;
    public Text tipsText;
    public float textShowTime;

    public GameObject StatusPanel;
    public Text statusText;

    public GameObject InfomationPanel;
    public Text CharacterInfomText;
    public GameObject InformingCharacterFlag;
    public Character mouseDownCharacter;


    public Text DamageTextPrefab, damageText;
    #endregion

    public Transform skillPointTransform;
    public int skillPoint;

    public List<GameObject> Elements = new List<GameObject>();
    public int maxSkillPoint = 5;

    public float TotalActionValuePass = 0f;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private void Update()
    {
        textShowTime -= Time.deltaTime;
        TipsPanel.gameObject.SetActive(textShowTime > 0);

        if(InfomationPanel.activeSelf == true)
        {
            mouseDownCharacter?.FreshInformation();
        }

        FreshSpecialPanel();
    }
    private void Start()
    {
        Messenger.Instance.AddListener<int>(Messenger.EventType.SkillPointChange, FreshSkillPoint);
        Messenger.Instance.AddListener<Character, Character>(Messenger.EventType.ChangeTarget, SetFlag);

        skillPoint = 3;
        Messenger.Instance.BroadCast(Messenger.EventType.SkillPointChange, 0);
    }
    public void SetFlag(Character cancelFlag,Character addFlag)
    {
        if(cancelFlag != null)
        {
            cancelFlag.CancelTargeted();
        }
        if(addFlag != null)
        {
            addFlag.Targeted();
        }
    }

    public void ShowPanelText(string text,float time = 3)
    {
        tipsText.text = text;
        textShowTime = time;
    }
    
    public void FreshSkillPoint(int changeNumebr)
    {
        skillPoint = Mathf.Min(maxSkillPoint, skillPoint);
        for (int i = 0; i < skillPointTransform.childCount; i++)
        {
            skillPointTransform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < skillPoint; i++)
        {
            skillPointTransform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public GameObject SpecialStatusPanel;
    public Text SpecialStatusText;
    public List<Status> SpecialStatus = new List<Status>();
    public void FreshSpecialPanel()
    {
        SpecialStatusText.text = "";
        foreach (var status in SpecialStatus)
        {
            if(status.specialType == Status.SpecialType.LimitedLayer)   //一些Status按层数限制显示
            {
                SpecialStatusText.text += $"{status.name}  {status.StatusLayer}/{status.LayerLimited}\n";
            }
            else if(status.specialType == Status.SpecialType.TriggerLayer)   //一些Status显示触发层数
            {
                SpecialStatusText.text += $"{status.name}  {status.StatusLayer}/{status.trigger.triggerLayer}\n";
            }
        }
    }

    public void FreshBattleInfor(string context)
    {
        GameManager.Instance.statusText.text = context;
    }

    public Character GetRandomEnemy()
    {
        List<Character> enemies = new List<Character>();
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.characterData.currentHealth > 0)
            {
                enemies.Add(enemy);
            }
        }

        int randomIndex = Random.Range(0, enemies.Count);
        return enemies[randomIndex];
    }

    #region 伤害显示函数
    public void DamageAppearFunc(Character source, Character target,float damage)
    {
        float rx = Random.Range(1.5f, 5f);
        float ry = Random.Range(4f, 6f);

        damageText = Instantiate(DamageTextPrefab, canvas.transform);
        damageText.transform.position = target.gameObject.transform.position + new Vector3(rx, ry, 0);
        damageText.text = damage.ToString();

        StartCoroutine(DamageAppear(damageText,1.5f));
    }

    public IEnumerator DamageAppear(Text text, float time)
    {
        while (time >= 0)
        {
            text.transform.Translate(new Vector3(0, 15f, 0) * Time.deltaTime, Space.World);
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Destroy(text.gameObject);
            }
            yield return null;
        }
    }

    public GameObject PlayerDamageBall,EnemyDamageBall;
    public void DamageBallAppear(Character source, Character target, float damage)
    {
        GameObject newBall;
        if (source.type == Character.CharaterType.Player)
        {
            newBall = Instantiate(PlayerDamageBall);
        }
        else
        {
            newBall = Instantiate(EnemyDamageBall);
        }
        newBall.transform.position = source.transform.position;
        StartCoroutine(DamageBallMoving(source, target, newBall));
    }
    public IEnumerator DamageBallMoving(Character source, Character target, GameObject GO)
    {
        Vector3 endpos = new Vector3(target.transform.position.x, target.transform.position.y, 0);
        while (GO != null && Vector3.Distance(GO.transform.position, endpos) >= 1f)
        {
            if(target == null)
            {
                Destroy(GO.gameObject);
            }
            GO.transform.position = Vector3.Lerp(GO.transform.position, endpos, 4f * Time.deltaTime);
            yield return null;
        }
        yield return null; 
    }
    #endregion
}
