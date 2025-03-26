using UnityEngine;
public class CameraController : MonoBehaviour
{
    GameObject player;
    Vector3 offset;
    public void SetupPlayer(GameObject _player)
    {
        player = _player;
        offset = -player.transform.forward * 5f + player.transform.up * 2f;
    }

    void LateUpdate()
    {
        if (player is null) return;

        this.transform.position = player.transform.position + offset + Vector3.up * 2;
        this.transform.LookAt(player.transform);
    }
}
