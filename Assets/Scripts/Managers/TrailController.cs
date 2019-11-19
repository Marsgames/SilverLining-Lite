#region Author
/////////////////////////////////////////
//   RAPHAËL DAUMAS
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
#endregion
using System.Collections;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float speed = 15;
    [SerializeField] private bool m_moveTrailInEditor = true;

    private Vector3[] m_waypoints = new Vector3[0];

    public double index = 0;
    public int rayon = 30;
    #endregion

    #region Unity's functions
    private void Start()
    {
        var position = transform.position;
        position.y += 1.5f;

        transform.position = position;
    }
    #endregion

    #region Unity's functions
    private void OnDrawGizmos()
    {
        if (!m_moveTrailInEditor)
        {
            return;
        }

        var position = transform.position;

        position.x = rayon * Mathf.Cos((float)((2 * Mathf.PI * index) / 360));
        position.z = rayon * Mathf.Sin((float)((2 * Mathf.PI * index) / 360));

        index += 1;

        if (index >= 360)
        {
            index = 0;
        }

        transform.position = position;
    }
    #endregion Unity's functions

    #region Functions
    /// <summary>
    /// Set the array of positions that the trail will reach then send the trail
    /// </summary>
    /// <param name="waypoints">Array of positions</param>
    public void SetWaypoints(Vector3[] waypoints)
    {
        m_waypoints = waypoints;

        if (null == waypoints)
        {
            return;
        }

        for (int i = 0; i < m_waypoints.Length; i++)
        {
            m_waypoints[i].y += 1.5f;
        }

        StartCoroutine(GoToPosition());
    }

    /// <summary>
    /// Move the trail to each waypoint
    /// </summary>
    private IEnumerator GoToPosition()
    {
        foreach (Vector3 waypoint in m_waypoints)
        {
            while (transform.position != waypoint)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint, Time.deltaTime * speed);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
    #endregion

}