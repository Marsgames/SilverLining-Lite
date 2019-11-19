#region Author
/////////////////////////////////////////
//  Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIGlobal : MonoBehaviour
{
    #region Variables
    public static UIGlobal Instance;

    [SerializeField] private Text m_timer = null;
    [SerializeField] private Text m_nbUnit = null;
    [SerializeField] private Text m_gold = null;
    [SerializeField] private Text m_income = null;
    [SerializeField] private Text m_currentBuildGold = null;
    [SerializeField] private Text m_maxBuildGold = null;
    [SerializeField] private Image m_buildGoldReload = null;
    [SerializeField] private Text m_buildGoldHardcap = null;
    [SerializeField] private Text m_playerHealth = null;
    [SerializeField] private Text m_playerMaxHealth = null;
    [SerializeField] private Text m_enemyHealth = null;
    [SerializeField] private Text m_enemyMaxHealth = null;
    [SerializeField] private Image m_imagePlayerHP = null;
    [SerializeField] private Image m_imageEnemyHP = null;
    [SerializeField] private Text m_warningText = null;
    [SerializeField] private UISpawnUnit m_spawnUI = null;
    [SerializeField] private Canvas m_selectspawnCanvas = null;
    [SerializeField] private RemoveGoldsUI m_removeGoldUI = null;

    private Coroutine m_warningCoroutine;
    private int m_timeInSecondGenerateBuildGold;
    private int m_maxHealthPlayers;
    #endregion

    #region Unity's functions

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        GetComponent<Image>().enabled = true;
    }

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(GameManager.Instance.GetAllPlayersReady);
        CheckIfOk();
        m_warningText.text = "";
        m_warningText.gameObject.SetActive(false);
        GetComponent<Image>().enabled = false;
    }

    protected void FixedUpdate()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        m_timer.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
    #endregion

    #region Function

    public void ShowWarning(string warning, float time = 3f)
    {
        if (null != m_warningCoroutine)
        {
            StopCoroutine(m_warningCoroutine);
        }
        m_warningCoroutine = StartCoroutine(ShowWarningCoroutine(warning, time));
    }

    private IEnumerator ShowWarningCoroutine(string warning, float time)
    {
        m_warningText.gameObject.SetActive(true);
        m_warningText.CrossFadeAlpha(255f, 0, false);
        m_warningText.text = warning;
        yield return new WaitForSeconds(time / 2);

        m_warningText.CrossFadeAlpha(0f, time / 2, false);
        yield return new WaitForSeconds(time / 2);

        m_warningText.text = "";
        m_warningText.gameObject.SetActive(false);
    }

    public void SetEndGame()
    {
        foreach (Transform child in transform)
        {
            if (child.name == Constant.ListOfUI.s_healthBarsCanvas || child.name == Constant.ListOfUI.s_timerCanvas)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void CheckIfOk()
    {
#if UNITY_EDITOR

        if (null == m_warningText)
        {
            Debug.LogError("WarningText n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_timer)
        {
            Debug.LogError("Timer n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_nbUnit)
        {
            Debug.LogError("NbUnit n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_gold)
        {
            Debug.LogError("Gold n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_income)
        {
            Debug.LogError("Income n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_currentBuildGold)
        {
            Debug.LogError("CurrentBuildGold n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_maxBuildGold)
        {
            Debug.LogError("MaxBuildGold n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_buildGoldReload)
        {
            Debug.LogError("BuildGoldReload n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_buildGoldHardcap)
        {
            Debug.LogError("BuildGoldHardcap n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_playerHealth)
        {
            Debug.LogError("PlayerHealth n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_playerMaxHealth)
        {
            Debug.LogError("PlayerMaxHealth n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_enemyHealth)
        {
            Debug.LogError("EnemyHealth n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_enemyMaxHealth)
        {
            Debug.LogError("EnemyMaxHealth n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_imagePlayerHP)
        {
            Debug.LogError("Image PlayerHP n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_imageEnemyHP)
        {
            Debug.LogError("Image EnemyHP n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_spawnUI)
        {
            Debug.LogError("SpawnUI n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
        if (null == m_removeGoldUI)
        {
            Debug.LogError("RemoveGoldUI n'est pas set sur " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }

#endif
    }

    #endregion

    #region Accessors

    public void InitLocalPlayerUI(PlayerEntity player)
    {
        if (player.GetPlayerNumber() == PlayerEntity.Player.Player2)
        {
            Text tempText = m_playerHealth;
            m_playerHealth = m_enemyHealth;
            m_enemyHealth = tempText;

            Image tempImage = m_imagePlayerHP;
            m_imagePlayerHP = m_imageEnemyHP;
            m_imageEnemyHP = tempImage;
        }

        m_playerHealth.text = player.GetHealthPoint().ToString();
        m_playerMaxHealth.text = player.GetMaxHealthPoint().ToString();
        m_timeInSecondGenerateBuildGold = player.GetTimeInSecondGenerateBuildGold();
        m_maxHealthPlayers = player.GetMaxHealthPoint();
        m_currentBuildGold.text = player.GetBuildGold().ToString();
        m_maxBuildGold.text = player.GetMaxBuildGold().ToString();
        m_buildGoldReload.fillAmount = 0;
        m_income.text = player.GetIncomeUnitGold().ToString();
    }

    public void InitEnemyPlayerUI(PlayerEntity player)
    {
        m_enemyHealth.text = player.GetHealthPoint().ToString();
        m_enemyMaxHealth.text = player.GetMaxHealthPoint().ToString();
    }

    public UISpawnUnit GetUISpawnUnit()
    {
        return m_spawnUI;
    }

    public Canvas GetUISelectSpawnUnit()
    {
        return m_selectspawnCanvas;
    }

    public RemoveGoldsUI GetRemoveGoldsUI()
    {
        return m_removeGoldUI;
    }

    public void SetCurrentBuildGold(int currentBuildGold)
    {
        m_currentBuildGold.text = currentBuildGold.ToString();
    }

    public void SetMaxBuildGold(int maxBuildGold)
    {
        m_maxBuildGold.text = maxBuildGold.ToString();
    }

    public void SetBuildGoldReload(int reloadBuildGold)
    {
        m_buildGoldReload.fillAmount = (float)reloadBuildGold / m_timeInSecondGenerateBuildGold;
    }

    public void BuildGoldHardcap()
    {
        m_buildGoldReload.gameObject.SetActive(true);
        m_buildGoldHardcap.gameObject.SetActive(false);
    }

    public void SetNbUnitInGame(int nbUnitGame)
    {
        m_nbUnit.text = nbUnitGame.ToString();
    }

    public void SetIncomeGold(int regularGold)
    {
        m_income.text = regularGold.ToString();
    }

    public void SetUnitGold(int unitGold)
    {
        m_gold.text = unitGold.ToString();
    }

    public void SetLocalHealthPoint(int healthPoint)
    {
        m_playerHealth.text = healthPoint.ToString();
        m_imagePlayerHP.fillAmount = (float)healthPoint / m_maxHealthPlayers;
    }

    public void SetEnemyHealthPoint(int healthPoint)
    {
        m_enemyHealth.text = healthPoint.ToString();
        m_imageEnemyHP.fillAmount = (float)healthPoint / m_maxHealthPlayers;
    }
    #endregion


}
