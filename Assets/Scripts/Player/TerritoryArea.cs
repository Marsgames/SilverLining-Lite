using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TerritoryArea : MonoBehaviour
{
    [SerializeField] private GameObject m_occludeesObject = null;
    private Territory m_territory;

    private void Start()
    {
        CheckIfOk();

        m_territory = m_occludeesObject.GetComponent<Territory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        m_territory.TriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        m_territory.TriggerExit(other);
    }

    /// <summary>
    /// Checks if all required conditions are filled, otherwise crash
    /// </summary>
    private void CheckIfOk()
    {
#if UNITY_EDITOR
        if (null == m_occludeesObject)
        {
            Debug.LogError("Occludees Object ne peut pas être null dans " + name);
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
    }
}
