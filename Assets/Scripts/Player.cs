using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player:NetworkBehaviour {
    public LayerMask floorMask;
    private Vector3 _spawnPos;
    private Rigidbody rgdBdy;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        if(isServer) {
            Destroy(gameObject);
        } else {
            _spawnPos = transform.position;
        }
    }

    public void Awake() {
        rgdBdy = GetComponent<Rigidbody>();
    }

    public void Update() {
        if(!isLocalPlayer)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Input.GetMouseButton(0) && Physics.Raycast(ray, out hit, 100, floorMask.value)) {
            Vector3 tarVector = hit.point - transform.position;
            tarVector.y = transform.position.y;
            rgdBdy.MovePosition(Vector3.Lerp(transform.position, transform.position + tarVector.normalized * 20f, Time.deltaTime));
        }
    }

    [ClientRpc]
    public void RpcRespawn() {
        if(isLocalPlayer)
            transform.position = _spawnPos;
    }
}