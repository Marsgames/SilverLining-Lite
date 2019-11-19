//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #SpellTank#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class SpellTank : SpellBase
//{/*
//    #region Variables
//    [SerializeField] private int m_malusAttack = 5;
//    #endregion

//    #region Unity's functions
//    public override void Start()
//    {
//        base.Start();
//        m_uiSpellButton.GetComponent<Button>().onClick.AddListener(TaskOnClickTank);
//    }

//    private void Update()
//    {
//        if (Input.GetButtonDown(Constant.ListOfShortcut.s_spellTank))
//        {
//            CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(), m_currentSpellStack);
//        }
//    }

//    public void TaskOnClickTank()
//    {
//        CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(), m_currentSpellStack);
//    }

//    [Command]
//    private void CmdSendSpellOnServer(Constant.Player playerNumber, int currentSpellStack)
//    {
//        if (currentSpellStack == 0)
//        {
//            return;
//        }
//        TargetUseSpellStack(connectionToClient, SpellType.SpellTank);
       
//        foreach (TankUnit tank in FindObjectsOfType<TankUnit>())
//        {
//            if (tank.GetComponent<UnitController>().GetPlayerNumber() == playerNumber)
//            {
//                tank.BoostAttack(-m_malusAttack, m_duration);
//                tank.Invulnerability(true, m_duration);
//            }
//        }
//    }


//    #endregion*/
//}
