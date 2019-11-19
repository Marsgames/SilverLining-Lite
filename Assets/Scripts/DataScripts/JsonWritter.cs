#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Guillaume Quiniou
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonWritter
{
    #region Variables
    private List<string> m_allData;

    public List<string> GetAlldata()
    {
        return m_allData;
    }
    #endregion
    #region Functions

    public void WriteJson()
    {
        string datastring = string.Join(",", m_allData.ToArray());
        string path = Application.persistentDataPath + @"/"+ System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm")+".json";
        File.WriteAllText(path, "[" + datastring + "]");
        Debug.Log("Json writed at "+ path);
    }

    public JsonWritter()
    {
        m_allData = new List<string>();        
    }

    public void AddAllData(string json)
    {
        m_allData.Add(json);
    }
    #endregion
}
