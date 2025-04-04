using UnityEngine;
using Mirror;
using System.Collections;
public class Enemy : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider collider;

    void OnCllisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            RpcOnHit();
            StartCoroutine(DestoryAfterDelay());
        }
    }

    [ClientRpc]
    void RpcOnHit()
    {
        rb.isKinematic = true;
        collider.enabled = false;
    }

    IEnumerator DestoryAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        NetworkServer.Destroy(this.gameObject);
    }
}
