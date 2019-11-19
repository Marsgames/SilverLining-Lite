using UnityEngine;

public class UITutorial : MonoBehaviour
{
    [SerializeField] private string m_videoAdress = "";
    [SerializeField] private GameObject m_canvas = null;

    public void PlayVideo()
    {
        m_canvas.SetActive(true);
        StreamVideo stream = m_canvas.GetComponent<StreamVideo>();
        stream.SetAdress(m_videoAdress);
        stream.PlayVideo();
    }
}
