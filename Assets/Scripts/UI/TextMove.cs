#region Author
/////////////////////////////////////////
//   Yannig Smagghe
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextMove : MonoBehaviour
{
    #region Variables
    private float m_duration = 1;
    private Vector3 m_endPos = new Vector3(0, 1, 0);
    private Vector3 canvasRotation;
    private GameObject canvas;
    #endregion

    #region Unity's function
    private void Update()
    {
        canvasRotation = new Vector3(CameraManager.Instance.transform.eulerAngles.x, CameraManager.Instance.transform.eulerAngles.y, 0);
        canvas.transform.eulerAngles = canvasRotation;
    }

    #endregion

    #region Functions
    public void MoveText(int value)
    {
        canvas = transform.parent.gameObject;
        StartCoroutine(ShowText(value));
    }

    private IEnumerator ShowText(int value)
    {
        GetComponent<Text>().text = "+ " + value;
        float currentTime = m_duration;

        while (currentTime > 0)
        {
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, m_endPos, 0.01f);
            yield return new WaitForSecondsRealtime(0.01f);
            currentTime -= 0.01f;
        }
        yield return null;
        Destroy(gameObject);
    }
    #endregion
}
