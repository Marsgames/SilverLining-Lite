#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RemoveGoldsUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject m_textPrefab = null;
    private Vector3 m_endPos = new Vector3(0, 1, 0);
    #endregion

    #region Function
    public void SpawnText(string textToShow, float duration = 1)
    {
        m_endPos = transform.position + new Vector3(0, 30, 0);
        StartCoroutine(ShowText(textToShow, duration));
    }

    private IEnumerator ShowText(string textToShow, float duration)
    {
        GameObject go = Instantiate(m_textPrefab, transform);
        go.name = "GoldSubstract" + textToShow;
        Text uiText = go.GetComponent<Text>();

        go.transform.SetParent(transform);
        go.transform.position = transform.position;


        uiText.text = textToShow;

        float currentTime = duration;

        go.GetComponent<Text>().CrossFadeAlpha(0, 1.0f, false);

        while (currentTime > 0)
        {
            go.transform.position = Vector3.MoveTowards(go.transform.position, m_endPos, 1);
            yield return new WaitForSecondsRealtime(0.01f);
            currentTime -= 0.01f;
        }
        yield return null;
        Destroy(go);
    }

    #endregion
}
