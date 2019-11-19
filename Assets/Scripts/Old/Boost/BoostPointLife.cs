//#region Author
///////////////////////////////////////////
////   RAPHAËL DAUMAS
////   https://raphdaumas.wixsite.com/portfolio
////   https://github.com/Marsgames
///////////////////////////////////////////

//#endregion
//using UnityEngine;

//public class BoostPointLife : MonoBehaviour
//{
//    #region Variables
//    [SerializeField] private int m_effectDuration = 10;
//    [SerializeField] private int m_healthBonus = 20;
//    #endregion

//    #region Unity's functions
//    private void OnTriggerEnter(Collider other)
//    {
//        if (!other.tag.Equals(Constant.ListOfTag.s_unit))
//        {
//            return;
//        }

//        CombatUnit unit = other.GetComponent<CombatUnit>();
//        if (null == unit)
//        {
//            return;
//        }

//        unit.BoostHealth(m_healthBonus, m_effectDuration);
//    }
//    #endregion
//}