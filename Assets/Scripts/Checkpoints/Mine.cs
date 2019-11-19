#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
//  Guillaume Quiniou
/////////////////////////////////////////
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Mine : CheckpointBase
{

    #region Variables
    [SerializeField] private int[] m_generateGold = null;
    [SerializeField] private GameObject m_canvasTextGold = null;

    private Animator m_animator;
    private Coroutine m_coroutineGoldText;
    [SyncVar] private int m_goldGenerateNow = 0;
    private bool m_coroutineGoldRun = false;

    #endregion

    #region Unity's function

    protected override IEnumerator Start()
    {
        yield return base.Start();
        m_animator = GetComponent<Animator>();
    }

    #endregion

    #region Functions
    [Server]
    protected override void FightIsOver()
    {
        base.FightIsOver();
        StartGoldProduction();
    }

    [Server]
    protected override void AttackCheckpoint(UnitController unit)
    {
        base.AttackCheckpoint(unit);
        m_goldGenerateNow = 0;
        PlayerEntity owner = GameManager.Instance.GetPlayer(m_playerOwner);
        owner.ChangeMineIncome(this, m_goldGenerateNow);
        RpcAnimationChart(false);
        if (m_playerOwner != PlayerEntity.Player.Bot)
        {
            TargetGoldText(owner.connectionToClient, false);
        }
    }

    [Server]
    public override void ChangeNbUnitsStocked(bool add, GameObject unit)
    {
        base.ChangeNbUnitsStocked(add, unit);

        if (m_enemiesUnits.transform.childCount > 0)
        {
            return;
        }
        StartGoldProduction();
    }

    [Server]
    private void StartGoldProduction()
    {
        m_goldGenerateNow = m_generateGold[m_stockUnits.transform.childCount];
        PlayerEntity owner = GameManager.Instance.GetPlayer(m_playerOwner);
        owner.ChangeMineIncome(this, m_goldGenerateNow);
        if (m_goldGenerateNow > 0)
        {
            RpcAnimationChart(true);
            if (m_playerOwner != PlayerEntity.Player.Bot)
            {
                TargetGoldText(owner.connectionToClient, true);
            }
        }
        else
        {
            RpcAnimationChart(false);
            if (m_playerOwner != PlayerEntity.Player.Bot)
            {
                TargetGoldText(owner.connectionToClient, false);
            }
        }
    }

    [ClientRpc]
    private void RpcAnimationChart(bool active)
    {
        m_animator.SetBool(Constant.ListOfAnimTrigger.s_isActive, active);

    }

    [TargetRpc]
    private void TargetGoldText(NetworkConnection target, bool active)
    {
        if (!m_coroutineGoldRun && active)
        {
            m_coroutineGoldRun = true;
            m_coroutineGoldText = StartCoroutine(ShowText());
        }
        else if (!active)
        {
            m_coroutineGoldRun = false;
        }
    }

    [Client]
    private IEnumerator ShowText()
    {
        while (m_coroutineGoldRun)
        {
            GameObject goldTextGO = Instantiate(m_canvasTextGold, transform.position + new Vector3(0, 2.1f, -0.5f), Quaternion.identity, transform).gameObject;
            goldTextGO.transform.eulerAngles = new Vector3(CameraManager.Instance.transform.eulerAngles.x, CameraManager.Instance.transform.eulerAngles.y, 0);
            goldTextGO.GetComponentInChildren<TextMove>().MoveText(m_goldGenerateNow);
            yield return new WaitForSecondsRealtime(1);
            Destroy(goldTextGO);
        }
    }

    [TargetRpc]
    public override void TargetPlaySoundCapture(NetworkConnection target)
    {
        m_alertManager.TimerVerificationCapturePIGenerator();
    }

    [ClientRpc]
    public override void RpcAllPlaySoundCapture()
    {
        m_soundEmitter.PlaySound();
    }
    #endregion

}