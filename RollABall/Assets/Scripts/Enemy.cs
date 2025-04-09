using UnityEngine;
using Mirror;
using System.Collections;
public class Enemy : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider collider;
    [SerializeField] AudioSource audioSource;

    void OnCllisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            NetworkServer.Destroy(collision.gameObject);
            RpcOnHit();
            StartCoroutine(DestoryAfterDelay());
        }
    }

    [ClientRpc]
    void RpcOnHit()
    {
        rb.isKinematic = true;
        collider.enabled = false;

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    IEnumerator DestoryAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
