using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectiles : MonoBehaviour
{
    private Transform m_targetProjectile;
    public float speed= 7f;

    public void SeekTarget(Transform _target)
    {
        m_targetProjectile = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_targetProjectile == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = m_targetProjectile.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Destroy(gameObject);
    }
}
