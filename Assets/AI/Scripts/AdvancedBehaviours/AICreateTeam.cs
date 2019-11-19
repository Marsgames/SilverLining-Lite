#region Authors
/*****************************
 * Corentin Couderc
 * Dylan Tosti
 * ***************************/
#endregion
using System.Collections.Generic;
using UnityEngine;

public class AICreateTeam : StateMachineBehaviour {
    #region Variables
    private AISpawnUnit m_aiSpawnUnit;
    private List<Constant.SpawnTypeUnit> m_listeUnits = new List<Constant.SpawnTypeUnit> ();
    private List<Constant.SpawnTypeUnit> m_listeTypeUnits;
    private List<int> m_listeNbUnits;
    private List<SpawnUnits> m_listeSpawnUnits;
    private int m_indexSpanwUnits;
    #endregion

    #region main function
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_aiSpawnUnit = animator.GetBehaviour<AISpawnUnit> ();
        m_listeUnits.Clear ();
        for (int i = 0; i < m_listeTypeUnits.Count; i++) {
            for (int j = 0; j < m_listeNbUnits[i]; j++) {
                m_listeUnits.Add(m_listeTypeUnits[i]);
            }
        }
        m_listeSpawnUnits[m_indexSpanwUnits].SetUnitsQueue(m_listeUnits);
        m_aiSpawnUnit.SetIndexSpawUnits(m_indexSpanwUnits);
        m_aiSpawnUnit.SetListeSpawnUnits(m_listeSpawnUnits);
        animator.SetTrigger(Constant.BotTransition.s_spawnUnit);
        m_aiSpawnUnit = animator.GetBehaviour<AISpawnUnit>();
    }
    #endregion

    #region setters
    public void SetSpawnList (List<Constant.SpawnTypeUnit> listeTypeUnits, List<int> listeNbUnits)
    {
        m_listeTypeUnits = listeTypeUnits;
        m_listeNbUnits = listeNbUnits;
    }

    public void SetSpawnUnitsIndex (int ind)
    {
        m_indexSpanwUnits = ind;
    }

    public void SetBotSpawnUnitsList (List<SpawnUnits> list)
    {
        m_listeSpawnUnits = list;
    }

    #endregion
}