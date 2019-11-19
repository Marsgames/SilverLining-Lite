#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> #SCRIPTNAME#
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SmallGolemUnit : NeutralUnit
{
    public override UnitType GetUnitType()
    {
        return UnitType.SmallGolem;
    }
}
