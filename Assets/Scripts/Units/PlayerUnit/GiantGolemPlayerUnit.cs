#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion

public class GiantGolemPlayerUnit : CombatUnit
{
    public override UnitType GetUnitType()
    {
        return UnitType.GiantGolem;
    }
}
