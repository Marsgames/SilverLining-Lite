//#region Author
///////////////////////////////////////////
////   RAPHAËL DAUMAS
////   https://raphdaumas.wixsite.com/portfolio
////   https://github.com/Marsgames
///////////////////////////////////////////

//#endregion
//using UnityEngine;

//public class BoostPointAttack : MonoBehaviour
//{
//    #region Variables
//    [SerializeField] private int m_effectDuration = 10;
//    [SerializeField] private int m_attackBonus = 150;
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

//        unit.BoostAttack(m_attackBonus, m_effectDuration);
//    }
//    #endregion
//}
