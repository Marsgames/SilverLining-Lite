#pragma warning disable CS0618 // type deprecated
#region Author
/////////////////////////////////////////
//   Yannig Smagghe --> #Bomb#
//   https://gitlab.com/YannigSmagghe
/////////////////////////////////////////
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    #region Variables

    [SerializeField] private int m_bombStack = 10;
    [SerializeField] private int m_timerCD = 10;
    private Text m_uiBombStack;
    private int m_currentbombStack;
    private bool m_onCD;
    #endregion

    #region Unity's functions
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(GameManager.Instance.GetAllPlayersReady);
        GameObject[] uiBombStack = GameObject.FindGameObjectsWithTag(Constant.ListOfTag.s_bombCD);
        if (uiBombStack[0])
        {
            m_uiBombStack = uiBombStack[0].GetComponent<Text>();
        }

        m_currentbombStack = m_bombStack;
    }

    void Update()
    {
        if (!GetComponent<NetworkIdentity>().hasAuthority)
        {
            return;
        }
        if (null != m_uiBombStack && !GetComponent<AIControl>())
        {
            m_uiBombStack.text = m_currentbombStack.ToString();
        }

    }
    #endregion

    #region Accessors
    public int GetBombStack()
    {
        return m_currentbombStack;
    }
    #endregion

    public void UseBombStack()
    {
        m_currentbombStack--;
        if (m_currentbombStack < m_bombStack && !m_onCD)
        {
            StartCoroutine(CDBomb());
        }
    }

    private IEnumerator CDBomb()
    {
        m_onCD = true;
        GameObject.Find(Constant.ListOfUI.s_bombImageCD).GetComponent<CooldownController>().ActiveCooldown(m_timerCD);
        float currentTime = m_timerCD;
        while (currentTime > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            currentTime -= 1;
        }


        m_currentbombStack++;
        if (m_currentbombStack < m_bombStack)
        {
            StartCoroutine(CDBomb());
        }
        else
        {
            m_onCD = false;
            yield return null;
        }

        yield return null;

    }
}
