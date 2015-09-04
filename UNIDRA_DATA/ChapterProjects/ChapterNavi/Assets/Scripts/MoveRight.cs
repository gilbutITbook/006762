using UnityEngine;
using System.Collections;

public class MoveRight : MonoBehaviour {
	public float speed = 1.0f;

	void Update () {
		Vector3 moveVector =  Vector3.right * speed * Time.deltaTime;
		transform.Translate(moveVector);

	}
}
