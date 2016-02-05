using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CenterPlayerCollider:MonoBehaviour {
    private CenterPlayer _cp;

    public void Awake() {
        _cp = GetComponentInParent<CenterPlayer>();
    }

    public void OnTriggerStay(Collider other) {
        _cp.OnTriggerStay(other);
    }
}