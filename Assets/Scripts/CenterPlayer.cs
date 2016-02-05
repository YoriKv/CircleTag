using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CenterPlayer:NetworkBehaviour {
    public LayerMask floorMask;
    public LayerMask allButEye;
    public Transform cardboardHead;
    public GameObject coneGraphic;
    private bool server;

    public override void OnStartClient() {
        coneGraphic.GetComponent<Renderer>().enabled = !isServer;
    }

    public void Update() {
        if(!isServer)
            return;

        Vector3 targetRot = cardboardHead.eulerAngles;
        targetRot.z = targetRot.x = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), Time.deltaTime * 10f);
    }

    public void OnTriggerStay(Collider other) {
        if(!isServer)
            return;

        // Raycast to check hit
        RaycastHit hit;
        if(Physics.Raycast(transform.position, (other.transform.position - transform.position), out hit, 150f, allButEye)) {
            Player player = other.GetComponent<Player>();
            if(player != null && other == hit.collider && hit.distance > 8f) {
                player.RpcRespawn();
            }
        }
    }
}