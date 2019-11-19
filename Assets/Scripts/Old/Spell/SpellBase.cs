//#region Author
///////////////////////////////////////////
////   Yannig Smagghe --> #SpellManager#
////   https://gitlab.com/YannigSmagghe
///////////////////////////////////////////
//#endregion
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public abstract class SpellBase : NetworkBehaviour
//{/*
//    #region Variables
//    [SerializeField] private int m_spellStack = 1;
//    [SerializeField] protected int m_duration = 10;
//    [SerializeField] private int m_timerCD = 10;
//    [SerializeField] protected UISpellButton m_uiSpellButton;
//    [SerializeField] protected Sprite m_spellImage = null;
//    [SerializeField] protected Sprite m_spellImageActive = null;

//    private CooldownController m_couldownController;
//    protected int m_currentSpellStack = 0;
//    private bool m_onCD;

//    public enum SpellType
//    {
//        SpellScout,
//        SpellWarrior,
//        SpellTank,
//        SpellRange
//    }
//    #endregion

//    #region Unity's functions
//    public virtual void Start()
//    {
//        if (!GetComponent<NetworkIdentity>().hasAuthority)
//        {
//            return;
//        }

//        m_uiSpellButton = Instantiate(m_uiSpellButton);
//        m_uiSpellButton.transform.SetParent(GameObject.Find(Constant.ListOfUI.s_spellCanvas).transform);
//        m_uiSpellButton.transform.localScale = new Vector3(1, 1, 1);
//        m_uiSpellButton.SetParentSpell(this);
//        m_uiSpellButton.GetComponent<Image>().sprite = m_spellImage;
//        m_couldownController = m_uiSpellButton.transform.Find(Constant.ListOfUI.s_spellImageCD).GetComponent<CooldownController>();
//        m_currentSpellStack = m_spellStack;
//    }
//    #endregion

//    #region Functions
//    [TargetRpc]
//    public void TargetUseSpellStack(NetworkConnection target, SpellType type)
//    {
//        SpellBase tempSpell = null;
//        switch (type)
//        {
//            case SpellType.SpellRange:
//                tempSpell = GetComponent<SpellRange>(); ;
//                break;
//            case SpellType.SpellTank:
//                tempSpell = GetComponent<SpellTank>();
//                break;
//            case SpellType.SpellWarrior:
//                tempSpell = GetComponent<SpellWarrior>();
//                break;
//            case SpellType.SpellScout:
//                tempSpell = GetComponent<SpellScout>(); ;
//                break;

//        }
//        tempSpell.m_currentSpellStack--;
//        tempSpell.m_uiSpellButton.GetComponent<Image>().sprite = tempSpell.m_spellImageActive;
//        if (tempSpell.m_currentSpellStack < tempSpell.m_spellStack && !tempSpell.m_onCD)
//        {
//            StartCoroutine(tempSpell.CDSpell(tempSpell));
//        }
//    }

//    private IEnumerator CDSpell(SpellBase tempSpell)
//    {
//        m_onCD = true;
//        //Launch ACTIVE animation
//        float currentTimeBeforeLaunchCD = m_duration;
//        while (currentTimeBeforeLaunchCD > 0)
//        {
//            yield return new WaitForFixedUpdate();
//            currentTimeBeforeLaunchCD -= Time.fixedDeltaTime;
//        }
//        tempSpell.m_uiSpellButton.GetComponent<Image>().sprite = tempSpell.m_spellImage;
//        //Launch CD animation
//        float currentTime = m_timerCD;
//        m_couldownController.ActiveCooldown(m_timerCD);
//        while (currentTime > 0)
//        {
//            yield return new WaitForFixedUpdate();
//            currentTime -= Time.fixedDeltaTime;
//        }

//        m_currentSpellStack++;
//        m_onCD = false;
//        yield return null;

//    }
//    #endregion

//    public int GetCurrentStackSpell()
//    {
//        return m_currentSpellStack;
//    }*/
//}
