#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public abstract class DataCollector : NetworkBehaviour
{
    #region Unity's functions
    protected virtual void Start()
    {
        if (!isServer)
        {
            Destroy(this);
        }
    }
    #endregion

}
