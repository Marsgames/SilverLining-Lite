//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #SpellScout#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class SpellScout : SpellBase
//{/*
//    #region Variables
//    [SerializeField] private int m_malusSpeed = 1;
//    [SerializeField] private int m_initSpeed = 5;
//    [SerializeField] private int m_boostAttack = 5;
//    #endregion

//    #region Unity's functions

//    public override void Start()
//    {
//        base.Start();
//        m_uiSpellButton.GetComponent<Button>().onClick.AddListener(TaskOnClickScout);
//    }

//    private void Update()
//    {
//        if (Input.GetButtonDown(Constant.ListOfShortcut.s_spellScout))
//        {
//            CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(), m_currentSpellStack);
//        }
//    }

//    public void TaskOnClickScout()
//    {
//        CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(),m_currentSpellStack);
//    }

//    [Command]
//    private void CmdSendSpellOnServer(Constant.Player playerNumber, int currentSpellStack)
//    {
//        if (currentSpellStack == 0)
//        {
//            return;
//        }
//        TargetUseSpellStack(connectionToClient, SpellType.SpellScout);
//        foreach (ScoutUnit scout in FindObjectsOfType<ScoutUnit>())
//        {
//            if (scout.GetComponent<UnitController>().GetPlayerNumber() == playerNumber)
//            {
//                scout.BoostSpeed(-m_malusSpeed, m_duration, m_initSpeed);
//                scout.BoostAttack(m_boostAttack, m_duration);
//            }
//        }
//    }
//    #endregion*/
//}
