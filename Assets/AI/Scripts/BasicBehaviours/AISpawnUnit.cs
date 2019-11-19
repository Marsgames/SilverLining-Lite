#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AISpawnUnit : StateMachineBehaviour
{
    #region Variables
    private int m_indexSpawnUnits;
    private List<SpawnUnits> m_aiListeSpawnUnits;
    private PlayerEntity m_playerEntity;
    private List<int> m_pricesList;
    private List<Constant.SpawnTypeUnit> m_unitsType;
    #endregion

    #region main functions
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!m_aiListeSpawnUnits[m_indexSpawnUnits].IsInvoking())
        {
            SubstractGold();
            m_aiListeSpawnUnits[m_indexSpawnUnits].MakeSpawnUnitsFromQueue();
        }
        animator.SetTrigger(Constant.BotTransition.s_return);

    }

    #endregion

    #region setters
    public void SetIndexSpawUnits(int ind)
    {
        m_indexSpawnUnits = ind;
    }

    public void SetListeSpawnUnits(List<SpawnUnits> listeSpawnUnits)
    {
        m_aiListeSpawnUnits = listeSpawnUnits;
    }

    public void SetPlayerEntity(PlayerEntity playerEntity)
    {
        m_playerEntity = playerEntity;
    }

    private void SubstractGold()
    {
        int m_totalPrice = 0;
        List<Constant.SpawnTypeUnit> queue = m_aiListeSpawnUnits[m_indexSpawnUnits].GetUnitsQueue();
        for (int j = 0; j < queue.Count; j++)
        {
            int price = m_pricesList[m_unitsType.IndexOf(queue[j])];
            m_totalPrice += price;
        }
        m_playerEntity.SubstractUnitGold(m_totalPrice);
    }

    public void SetPricesList(List<int> listPrices)
    {
        m_pricesList = listPrices;
    }

    public void SetTypesList(List<Constant.SpawnTypeUnit> listeTypes)
    {
        m_unitsType = listeTypes;
    }
    #endregion
}
