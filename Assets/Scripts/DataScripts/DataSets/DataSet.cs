#pragma warning disable CS0414 // The private field is assigned but its value is never used
#region Author
///////////////////////////////////////////////
//   Guillaume Quiniou 
/////////////////////////////////////////
#endregion

using UnityEngine;
public abstract class DataSet
{
    [SerializeField] protected string eventName;

    protected void SaveData()
    {
        string json = JsonUtility.ToJson(this, true);
        if (null != GameManager.Instance)
        {
            GameManager.Instance.jsonWritter.AddAllData(json);
        }
    }
}
