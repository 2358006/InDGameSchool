using UnityEngine;
public class CameraController : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset;
    public void SetupPlayer(GameObject _player)
    {
        // 플레이어에서 카메라로 가는 벡터
        player = _player;
        // offset = this.transform.position - player.transform.position;

        offset = -player.transform.forward * 5f + player.transform.up * 2f;
    }

    private void LateUpdate()
    {
        if (player is null) return;

        this.transform.position = player.transform.position + offset + Vector3.up * 2;
        this.transform.LookAt(player.transform);
    }
}
