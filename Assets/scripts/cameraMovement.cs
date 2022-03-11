using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public Transform follow;

    private void FixedUpdate()
    {
        this.transform.position = new Vector3(follow.position.x, follow.position.y, this.transform.position.z);
    }
}
