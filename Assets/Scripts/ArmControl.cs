using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : MonoBehaviour {

	public KeyCode waveKey = KeyCode.Mouse0;
	public float sensitivity = 2f;

	public GameObject arm;
	public GameObject elbow;

	private void Update () {
		if (Input.GetKey (waveKey)) {
			arm.SetActive (true);
			float xRot = Input.GetAxis ("Mouse Y") * sensitivity;
			float yRot = Input.GetAxis ("Mouse X") * sensitivity;
			elbow.transform.Rotate (new Vector3 (xRot, yRot, 0), Space.Self);
		} else {
			elbow.transform.localRotation = new Quaternion (0, 0, 0, 0);
			arm.SetActive (false);

		}
	}

}
