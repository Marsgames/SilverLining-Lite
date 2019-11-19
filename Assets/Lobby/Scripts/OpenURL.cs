#region Author
/*********************
 * Corentin Couderc
 *********************/
#endregion
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    #region Functions
    public void OpenFacebookURL()
    {
        Application.OpenURL("https://www.facebook.com/SilverLiningGGS/");
    }

    public void OpenWebsiteURL()
    {
        Application.OpenURL("https://silverlininggame.wixsite.com/home");
    }

    public void OpenTwitterURL()
    {
        Application.OpenURL("https://twitter.com/SilverLiningGGS");
    }

    public void OpenYoutubeURL()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCD56BRx9XFvyyYJ5rWMA3bw?");
    }
    
    #endregion
}
