using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CenterPlayer:NetworkBehaviour {
    public LayerMask floorMask;
    public Transform cardboardHead;

    public void Update() {
        if(!isServer)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, cardboardHead.rotation, Time.deltaTime * 10f);
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //
        //        if(Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, 100, floorMask.value)) {
        //            Vector3 lookPos = hit.point - transform.position;
        //            lookPos.y = 0;
        //            Quaternion targetRot = Quaternion.LookRotation(lookPos);
        //            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        //        }
    }

    public void OnTriggerStay(Collider other) {
        Player player = other.GetComponent<Player>();
        if(player != null) {
            player.Respawn();
        }
    }
}