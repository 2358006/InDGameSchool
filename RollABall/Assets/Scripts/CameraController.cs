using UnityEngine;
public class CameraController : MonoBehaviour
{
    //[SerializeField]
    GameObject player;
    Vector3 offset;


    public void SetupPlayer(GameObject _player)
    {
        // player > camera Vector
        player = _player;
        //offset = this.transform.position - player.transform.position;

        offset = -player.transform.forward * 5f + player.transform.up * 2f;
    }

    void LateUpdate()
    {
        if (player is null) return;

        this.transform.position = player.transform.position + offset + Vector3.up * 2;
        this.transform.LookAt(player.transform);
    }
}
