//#pragma warning disable CS0618 // type deprecated
//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #UiSpawnUnit#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class UISpawn : UIBaseIcon
//{
//    #region Variables
//    public enum UISpawnType
//    {
//        Unit,
//        Obstacle
//    }    
//    #endregion

//    #region Functions
//    public override void SetShowUi(GameObject parentPrefab, PlayerEntity.Player playerNumber)
//    {
//        base.SetShowUi(parentPrefab, playerNumber);
//        if (parentPrefab.GetComponent<SpawnUnits>())
//        {
//            m_parentPrefab = parentPrefab.GetComponent<SpawnUnits>().gameObject;

//            SetupPlayerUnitsIcon(playerNumber);
//            SetChildrenSpawnUnit(m_parentPrefab, UISpawnType.Unit);        
//        }

//        if (parentPrefab.GetComponent<ObstacleConstructor>())
//        {
//            m_parentPrefab = parentPrefab.GetComponent<ObstacleConstructor>().gameObject;
//            SetupPlayerObstaclesIcon(playerNumber);
//            SetChildrenSpawnUnit(m_parentPrefab, UISpawnType.Obstacle);
//        }
//    }

//    private void SetChildrenSpawnUnit(GameObject spawnUnitParent, UISpawnType typeSpawn)
//    {
//        gameObject.SetActive(true);
//        ToggleChildActive(true);
//        switch (typeSpawn)
//        {
//            case UISpawnType.Unit:
//                Transform rangeUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_rangeUnit);
//                rangeUnitTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform normalUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_warriorUnit);
//                normalUnitTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform tankUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_tankUnit);
//                tankUnitTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform scoutUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_scoutUnit);
//                scoutUnitTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);
//                break;
//            case UISpawnType.Obstacle:
//                Transform simpleWallTransform = gameObject.transform.Find(UIBase.ListOfChild.s_simpleWall);
//                simpleWallTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform pillarTransform = gameObject.transform.Find(UIBase.ListOfChild.s_pillar);
//                pillarTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform slopeWallRightTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallRight);
//                slopeWallRightTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform slopeWallLeftTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallLeft);
//                slopeWallLeftTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                Transform trapTransform = gameObject.transform.Find(UIBase.ListOfChild.s_trap);
//                trapTransform.GetComponent<UISpawnButton>().SetParentSpawnUnit(spawnUnitParent);

//                break;
//        }
//    }
//    #endregion
//}
