using UnityEngine;
using System.Collections;

public class RotateClockWise : MonoBehaviour {
	public float speed = 180.0f;

	void Update () {
		float angleMoveY = Time.deltaTime*speed;
		transform.Rotate(0,angleMoveY,0);
	}
}
