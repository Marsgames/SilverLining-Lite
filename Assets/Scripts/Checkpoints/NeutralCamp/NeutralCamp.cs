#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public abstract class NeutralCamp : NetworkBehaviour
{
    #region Variables
    [SerializeField] protected List<GameObject> m_golemsList = new List<GameObject>();
    [SerializeField] protected List<Transform> m_golemsSpawnPoints = new List<Transform>();
    [SerializeField, Tooltip("Time in seconds")] protected int m_timeBeforeRespawn = 30;
    [SerializeField, Tooltip("How many hp are regen each second")] private int m_hpRegen = 50;
    [SerializeField] private Text m_countdownText = null;

    protected int m_nbGolemsToSpawn;
    private List<CombatUnit> m_enemies = new List<CombatUnit>();
    private bool m_isInRegen = true;
    private float m_currentTime = 0f;
    #endregion

    #region Unity's functions
    [ServerCallback]
    protected virtual void Start()
    {
        CheckIfOk();
        m_nbGolemsToSpawn = m_golemsList.Count;
        SpawnGolems();
    }

    [ServerCallback]
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(Constant.ListOfTag.s_unit))
        {
            return;
        }
        CombatUnit enemy = other.GetComponent<CombatUnit>();
        m_enemies.Add(enemy);
        SetIsInRegen(false);

        AttackInRangeOfCheckpoint(enemy);

    }
    [ServerCallback]
    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.tag.Equals(Constant.ListOfTag.s_unit))
        {
            return;
        }
        m_enemies.Remove(other.GetComponent<CombatUnit>());
        SetIsInRegen(m_enemies.Count <= 0);
    }
    #endregion


    #region Functions
    [Server]
    public abstract void DecrementGolemsNumber(PlayerEntity.Player killer);

    [Server]
    protected void SpawnGolems()
    {
        for (int i = 0; i < m_nbGolemsToSpawn; i++)
        {
            GameObject theSpawnPoint = m_golemsSpawnPoints[i].gameObject;

            GameObject theGolem = Instantiate(m_golemsList[i], theSpawnPoint.transform.position, theSpawnPoint.transform.rotation);

            theGolem.transform.SetParent(transform);
            theGolem.tag = Constant.ListOfTag.s_neutralUnit;

            NeutralUnit golemScript = theGolem.GetComponent<NeutralUnit>();
            golemScript.SetCamp(this);
            golemScript.SetSpawnPosition(theSpawnPoint.transform);

            NetworkServer.Spawn(theGolem);
        }
        m_nbGolemsToSpawn = 0;
    }

    [Server]
    public void AttackInRangeOfCheckpoint(CombatUnit enemyCombatUnit)
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.tag.Equals(Constant.ListOfTag.s_neutralUnit))
            {
                continue;
            }
            child.GetComponent<CombatUnit>().UnitEnterAggroRange(enemyCombatUnit);
            enemyCombatUnit.UnitEnterAggroRange(child.GetComponent<CombatUnit>());
        }
    }

    [Server]
    private void RegenGolems()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.tag.Equals(Constant.ListOfTag.s_neutralUnit))
            {
                continue;
            }
            child.GetComponent<NeutralUnit>().AddCurrentHealth(m_hpRegen);
        }
    }

    [Server]
    public virtual void SetIsInRegen(bool value)
    {
        m_isInRegen = value;

        if (m_isInRegen)
        {
            CancelInvoke("RegenGolems");
            InvokeRepeating("RegenGolems", 1, 1);

        }
        else
        {
            CancelInvoke("RegenGolems");
        }
    }

    [Server]
    public void CleanNullInEnemyList()
    {
        if (m_enemies.Exists(x => (x.Equals(null) || !x.isActiveAndEnabled || x.GetCurrentState() == CombatUnit.State.Dead)))
        {
            m_enemies.RemoveAll(x => (x.Equals(null) || !x.isActiveAndEnabled || x.GetCurrentState() == CombatUnit.State.Dead));
        }
        SetIsInRegen(m_enemies.Count <= 0);
    }

    [ClientRpc]
    protected void RpcActiveTimer()
    {
        StartCoroutine(ActiveTimer());
    }

    protected IEnumerator ActiveTimer()
    {
        // TODO: /!\ l'utilisation du script LookAtCamera sur le Canvas fait buguer l'UI... Je ne sais pas comment régler ce problème, BON CHANCE

        m_currentTime = m_timeBeforeRespawn;
        transform.Find(Constant.ListOfUI.s_canvas).transform.gameObject.SetActive(true);
        transform.Find(Constant.ListOfUI.s_canvas).Find(Constant.ListOfUI.s_cd).GetComponent<CooldownController>().ActiveCooldown((int)m_currentTime);
        m_countdownText.text = ((int)m_currentTime).ToString();
        while (m_currentTime > 0)
        {
            m_countdownText.text = ((int)m_currentTime).ToString();
            yield return new WaitForSecondsRealtime(1);
            m_currentTime -= 1;
        }

        transform.Find(Constant.ListOfUI.s_canvas).Find(Constant.ListOfUI.s_cd).GetComponent<CooldownController>().StopCooldown();
        m_countdownText.transform.parent.gameObject.SetActive(false);
        yield return null;

    }



    #endregion

    private void CheckIfOk()
    {
        if (m_golemsSpawnPoints.Count != m_golemsList.Count)
        {
            Debug.LogError("Il n'y a pas autant de points de spawn de golems que de golems... -->" + name, this);
#if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

}
