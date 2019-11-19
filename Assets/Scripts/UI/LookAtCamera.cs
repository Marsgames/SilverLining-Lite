using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        transform.eulerAngles = new Vector3(CameraManager.Instance.transform.eulerAngles.x, CameraManager.Instance.transform.eulerAngles.y, 0);
    }
}
