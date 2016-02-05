using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CenterPlayer:NetworkBehaviour {
    public LayerMask floorMask;
    public Transform cardboardHead;
    public GameObject coneGraphic;

    public void Update() {
        if(!isServer)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, cardboardHead.rotation, Time.deltaTime * 10f);
    }

    public void OnTriggerStay(Collider other) {
        if (!isServer)
            return;

        Player player = other.GetComponent<Player>();
        if(player != null) {
            player.RpcRespawn();
        }
    }
}