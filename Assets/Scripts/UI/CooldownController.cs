#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS 
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownController : MonoBehaviour
{
    #region Variables
    private Image m_cooldownImage;
    private bool m_isCooldown;
    #endregion

    #region Unity's function
    private void Start()
    {
        m_cooldownImage = GetComponent<Image>();

        CheckIfOk();
    }
    #endregion

    #region Function
    /// <summary>
    /// Play cooldown animation
    /// </summary>
    public void ActiveCooldown(int timer)
    {
        if (null == m_cooldownImage)
        {
            m_cooldownImage = GetComponent<Image>();
            CheckIfOk();
        }
        if (m_isCooldown)
        {
            return;
        }

        m_cooldownImage.fillAmount = 1;
        StartCoroutine(FillCooldown(timer));
    }
    public void StopCooldown()
    {
        m_isCooldown = false;
        StopAllCoroutines();
    }

    private IEnumerator FillCooldown(float timer)
    {
        m_isCooldown = true;
        while (m_cooldownImage.fillAmount > 0)
        {
            m_cooldownImage.fillAmount -= 1 / timer * Time.deltaTime;

            if (m_cooldownImage.fillAmount <= 0)
            {
                m_isCooldown = false;
                yield return null;
            }
            yield return null;
        }
        m_isCooldown = false;
    }


    /// <summary>
    /// Checks if all required conditions are filled, otherwise crash
    /// </summary>
    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_cooldownImage)
        {
            Debug.LogError("Problème avec l'ajout de m_cooldownImage sur l'objet " + name, this);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
    #endregion
}
