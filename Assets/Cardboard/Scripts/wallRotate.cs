using UnityEngine;
using System.Collections;

public class wallRotate : MonoBehaviour {
	public float turnSpeed = 10f;

	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
	}
}
