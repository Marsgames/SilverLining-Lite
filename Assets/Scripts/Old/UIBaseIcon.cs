//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #UiIconManager#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion

//using UnityEngine;
//using UnityEngine.UI;

//public abstract class UIBaseIcon : UIBase
//{
//    #region Variables
//    [SerializeField] protected Sprite m_forestNormalIcon;
//    [SerializeField] protected Sprite m_forestRangeIcon;
//    [SerializeField] protected Sprite m_forestTankIcon;
//    [SerializeField] protected Sprite m_forestScoutIcon;
//    [SerializeField] protected Sprite m_arcaNormalIcon;
//    [SerializeField] protected Sprite m_arcaRangeIcon;
//    [SerializeField] protected Sprite m_arcaTankIcon;
//    [SerializeField] protected Sprite m_arcaScoutIcon;
//    [SerializeField] protected Sprite m_forestSimpleWallIcon;
//    [SerializeField] protected Sprite m_forestPillarIcon;
//    [SerializeField] protected Sprite m_forestSlopeLeftIcon;
//    [SerializeField] protected Sprite m_forestSlopeRightIcon;
//    [SerializeField] protected Sprite m_forestTrapIcon;
//    [SerializeField] protected Sprite m_arcaSimpleWallIcon;
//    [SerializeField] protected Sprite m_arcaPillarIcon;
//    [SerializeField] protected Sprite m_arcaSlopeLeftIcon;
//    [SerializeField] protected Sprite m_arcaSlopeRightIcon;
//    [SerializeField] protected Sprite m_arcaTrapIcon;

//    #endregion

//    #region Function
//    protected void SetupPlayerUnitsIcon(PlayerEntity.Player playerNumber)
//    {
//        if (playerNumber == PlayerEntity.Player.Player1)
//        {
//            Transform rangeUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_rangeUnit).Find("Image");
//            Image rangeImage = rangeUnitTransform.gameObject.GetComponent<Image>();
//            rangeImage.sprite = m_forestRangeIcon;

//            Transform normalImageTransform = gameObject.transform.Find(UIBase.ListOfChild.s_warriorUnit).Find("Image");
//            Image normalImage = normalImageTransform.gameObject.GetComponent<Image>();
//            normalImage.sprite = m_forestNormalIcon;

//            Transform tankUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_tankUnit).Find("Image");
//            Image tankImage = tankUnitTransform.gameObject.GetComponent<Image>();
//            tankImage.sprite = m_forestTankIcon;

//            Transform scoutUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_scoutUnit).Find("Image");
//            Image scoutImage = scoutUnitTransform.gameObject.GetComponent<Image>();
//            scoutImage.sprite = m_forestScoutIcon;
//        }
//        else if (playerNumber == PlayerEntity.Player.Player2)
//        {
//            Transform rangeUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_rangeUnit).Find("Image");
//            Image rangeImage = rangeUnitTransform.gameObject.GetComponent<Image>();
//            rangeImage.sprite = m_arcaRangeIcon;

//            Transform normalImageTransform = gameObject.transform.Find(UIBase.ListOfChild.s_warriorUnit).Find("Image");
//            Image normalImage = normalImageTransform.gameObject.GetComponent<Image>();
//            normalImage.sprite = m_arcaNormalIcon;

//            Transform tankUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_tankUnit).Find("Image");
//            Image tankImage = tankUnitTransform.gameObject.GetComponent<Image>();
//            tankImage.sprite = m_arcaTankIcon;

//            Transform scoutUnitTransform = gameObject.transform.Find(UIBase.ListOfChild.s_scoutUnit).Find("Image");
//            Image scoutImage = scoutUnitTransform.gameObject.GetComponent<Image>();
//            scoutImage.sprite = m_arcaScoutIcon;
//        }
//    }

//    protected void SetupPlayerObstaclesIcon(PlayerEntity.Player playerNumber)
//    {
//        if (playerNumber == PlayerEntity.Player.Player1)
//        {
//            Transform simpleWallTransform = gameObject.transform.Find(UIBase.ListOfChild.s_simpleWall).Find("Image");
//            Image simpleWallImage = simpleWallTransform.gameObject.GetComponent<Image>();
//            simpleWallImage.sprite = m_forestSimpleWallIcon;

//            Transform pillarTransform = gameObject.transform.Find(UIBase.ListOfChild.s_pillar).Find("Image");
//            Image pillarImage = pillarTransform.gameObject.GetComponent<Image>();
//            pillarImage.sprite = m_forestPillarIcon;

//            Transform slopeLeftTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallLeft).Find("Image");
//            Image slopeLeftImage = slopeLeftTransform.gameObject.GetComponent<Image>();
//            slopeLeftImage.sprite = m_forestSlopeLeftIcon;

//            Transform slopeRightTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallRight).Find("Image");
//            Image slopeRightImage = slopeRightTransform.gameObject.GetComponent<Image>();
//            slopeRightImage.sprite = m_forestSlopeRightIcon;

//            Transform trapTransform = gameObject.transform.Find(UIBase.ListOfChild.s_trap).Find("Image");
//            Image trapImage = trapTransform.gameObject.GetComponent<Image>();
//            trapImage.sprite = m_forestTrapIcon;
//        }
//        else if (playerNumber == PlayerEntity.Player.Player2)
//        {
//            Transform simpleWallTransform = gameObject.transform.Find(UIBase.ListOfChild.s_simpleWall).Find("Image");
//            Image simpleWallImage = simpleWallTransform.gameObject.GetComponent<Image>();
//            simpleWallImage.sprite = m_arcaSimpleWallIcon;

//            Transform pillarTransform = gameObject.transform.Find(UIBase.ListOfChild.s_pillar).Find("Image");
//            Image pillarImage = pillarTransform.gameObject.GetComponent<Image>();
//            pillarImage.sprite = m_arcaPillarIcon;

//            Transform slopeLeftTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallLeft).Find("Image");
//            Image slopeLeftImage = slopeLeftTransform.gameObject.GetComponent<Image>();
//            slopeLeftImage.sprite = m_arcaSlopeLeftIcon;

//            Transform slopeRightTransform = gameObject.transform.Find(UIBase.ListOfChild.s_slopeWallRight).Find("Image");
//            Image slopeRightImage = slopeRightTransform.gameObject.GetComponent<Image>();
//            slopeRightImage.sprite = m_arcaSlopeRightIcon;

//            Transform trapTransform = gameObject.transform.Find(UIBase.ListOfChild.s_trap).Find("Image");
//            Image trapImage = trapTransform.gameObject.GetComponent<Image>();
//            trapImage.sprite = m_arcaTrapIcon;
//        }
//    }
//    #endregion
//}
