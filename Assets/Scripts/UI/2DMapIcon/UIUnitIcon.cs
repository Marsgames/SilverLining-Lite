using System.Collections;
using UnityEngine;

public class UIUnitIcon : UI2DMapIcon
{
    #region 
    private void LateUpdate()
    {
        m_spriteRenderer.transform.position = new Vector3(transform.parent.position.x, 120f, transform.parent.position.z);
    }
    #endregion

    #region Functions
    public override void ShowIcon()
    {
        base.ShowIcon();
        StartCoroutine(RotateIcon());
    }

    public override void SelectIconToDisplay()
    {
        return;
    }


    IEnumerator RotateIcon()
    {
        while(true)
        {
            m_spriteRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
            yield return null;
        }
    }
    #endregion

}
