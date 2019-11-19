//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #SpellRange#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class SpellRange : SpellBase
//{/*
//    #region Variables
//    [SerializeField] private int m_malusSpeed = 5;
//    [SerializeField] private int m_initSpeed = 3;
//    [SerializeField] private int m_boostRange = 5;
//    #endregion

//    #region Unity's functions
//    public override void Start()
//    {
//        base.Start();
//        m_uiSpellButton.GetComponent<Button>().onClick.AddListener(TaskOnClickRange);
//    }

//    private void Update()
//    {
//        if (Input.GetButtonDown(Constant.ListOfShortcut.s_spellRange))
//        {
//            CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(), m_currentSpellStack);
//        }
//    }

//    public void TaskOnClickRange()
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
//        TargetUseSpellStack(connectionToClient, SpellType.SpellRange);
//        foreach (RangeUnit range in FindObjectsOfType<RangeUnit>())
//        {
//            if (range.GetComponent<UnitController>().GetPlayerNumber() == playerNumber)
//            {
//                range.BoostSpeed(-m_malusSpeed, m_duration,m_initSpeed);
//                range.BoostRange(m_boostRange, m_duration);
//            }
//        }
//    }
//    #endregion*/
//}
