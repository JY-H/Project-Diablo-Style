using UnityEngine;

public class RotateAround : MonoBehaviour {

	public Vector3 rotation;

	void Update () {
		transform.Rotate(rotation * Time.deltaTime);
	}
}
