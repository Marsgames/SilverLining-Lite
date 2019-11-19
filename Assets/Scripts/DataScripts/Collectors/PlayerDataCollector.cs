#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("DataCollector/PlayerDataCollector")]
public class PlayerDataCollector : DataCollector
{
    #region Variables
    PlayerTakeDamageDataSet m_playerTakeDamageDataSet;
    PlayerIncomeDataSet m_playerIncomeDataSet;
    PlayerUnitNumberDataSet m_playerUnitNumberDataSet;
    PlayerEntity m_player;
    #endregion
    #region Unity's functions
    protected override void Start()
    {
        base.Start();

        m_player = GetComponent<PlayerEntity>();
    }

    public void InitAndStartCollect()
    {        
        m_playerTakeDamageDataSet = new PlayerTakeDamageDataSet(m_player.GetPlayerNumber());
        m_playerIncomeDataSet = new PlayerIncomeDataSet(m_player.GetPlayerNumber());
        m_playerUnitNumberDataSet = new PlayerUnitNumberDataSet(m_player.GetPlayerNumber());
        m_player.TakeDamageEvents.AddListener(OnDamageTaken);

        StartCoroutine(CollectEveryXSec(5));
    }
    #endregion

    #region Functions
    private void OnDamageTaken(int amount)
    {
        m_playerTakeDamageDataSet.ColledData(amount);
    }

    private IEnumerator CollectEveryXSec(int x)
    {
        while (true)
        {
            yield return new WaitForSeconds(x);
            m_playerIncomeDataSet.CollectData(m_player.GetUnitGold(), m_player.GetBuildGold());
            int i = 0;
            foreach (var units in FindObjectsOfType<UnitController>())
            {
                if (units.GetPlayerNumber() == m_player.GetPlayerNumber())
                {
                    i++;
                }
            }
            m_playerUnitNumberDataSet.ColledData(i);
        }
    }
    #endregion





}
