//#region Author
///////////////////////////////////////////
////   RAPHAËL DAUMAS
////   https://raphdaumas.wixsite.com/portfolio
////   https://github.com/Marsgames
///////////////////////////////////////////

//#endregion
//using UnityEngine;

//public class BoostPointSpeed : MonoBehaviour
//{
//    #region Variables
//    [SerializeField] private int m_effectDuration = 10;
//    [SerializeField, Tooltip("Speed that will be add to the current speed")] private int m_speedBonus = 2;
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

//        unit.BoostSpeed(m_speedBonus, m_effectDuration);
//    }
//    #endregion
//}