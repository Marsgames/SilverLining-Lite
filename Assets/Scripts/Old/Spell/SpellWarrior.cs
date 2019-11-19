//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #SpellWarrior#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class SpellWarrior : SpellBase
//{/*
//    #region Unity's functions
//    public override void Start()
//    {
//        base.Start();
//        m_uiSpellButton.GetComponent<Button>().onClick.AddListener(TaskOnClickWar);
//    }

//    private void Update()
//    {
//        if (Input.GetButtonDown(Constant.ListOfShortcut.s_spellWarrior))
//        {
//            CmdSendSpellOnServer(GameManager.Instance.GetLocalPlayer(), m_currentSpellStack);
//        }
//    }

//    public void TaskOnClickWar()
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
//        TargetUseSpellStack(connectionToClient,SpellType.SpellWarrior);
//        foreach (WarriorUnit war in FindObjectsOfType<WarriorUnit>())
//        {
//            if (war.GetComponent<UnitController>().GetPlayerNumber() == playerNumber)
//            {
//                war.DyingLater(true, m_duration);
//            }
//        }
//    }
//    #endregion*/
//}
