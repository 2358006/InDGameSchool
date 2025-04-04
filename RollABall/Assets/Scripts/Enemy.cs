using UnityEngine;
using Mirror;
using System.Collections;
public class Enemy : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider collider;
    [SerializeField] AudioSource audioSouce;

    void OnCllisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            RpcOnHit();
            Debug.Log("나 맞았어유");
            StartCoroutine(DestoryAfterDelay());
        }
    }

    [ClientRpc]
    void RpcOnHit()
    {
        rb.isKinematic = true;
        collider.enabled = false;
        audioSouce.PlayOneShot(audioSouce.clip);
    }

    IEnumerator DestoryAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(this.gameObject);
    }
}
