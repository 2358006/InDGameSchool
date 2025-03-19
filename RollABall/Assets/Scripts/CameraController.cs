using UnityEngine;
public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Vector3 offset;
    void Start()
    {
        // player > camera Vector
        offset = this.transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        this.transform.position = player.transform.position + offset + Vector3.up * 2;
        this.transform.LookAt(player.transform);
    }
}
