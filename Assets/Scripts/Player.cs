using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player:NetworkBehaviour {
    public LayerMask floorMask;
    private Vector3 _spawnPos;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        if(isServer) {
            Destroy(gameObject);
        } else {
            _spawnPos = transform.position;
        }
    }

    public void Update() {
        if(!isLocalPlayer)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, 100, floorMask.value)) {
            Vector3 tarPos = hit.point - transform.position;
            tarPos.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, tarPos, Time.deltaTime * 5f);
        }
    }

    public void Respawn() {
        transform.position = _spawnPos;
    }
}