#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion

using UnityEngine;

public class MediumGolemPlayerUnit : CombatUnit
{
    public override UnitType GetUnitType()
    {
        return UnitType.MediumGolem;
    }
    
}
