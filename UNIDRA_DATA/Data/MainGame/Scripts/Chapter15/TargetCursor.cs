using UnityEngine;
using System.Collections;

public class TargetCursor : MonoBehaviour {
	// 반지름.
	public float radius = 1.0f;
	// 회전 속도.
	public float angularVelocity = 480.0f;
	// 목적지.
	public Vector3 destination = new Vector3( 0.0f, 0.5f, 0.0f );
	// 위치.
	Vector3 position = new Vector3( 0.0f, 0.5f, 0.0f );
	// 각도.
	float angle = 0.0f;
	
	// 위치를 설정한다.
	public void SetPosition(Vector3 iPosition)
	{
		destination = iPosition;
		// 높이는 고정한다.
		destination.y = 0.5f;
	}
	
	void Start()
	{
		// 초기 위치를 목적지로 설정한다.
		SetPosition( transform.position );
		position = destination;
	}

	// Update is called once per frame
	void Update () {
		position += ( destination - position ) / 10.0f;
		// 회전 각도.
		angle += angularVelocity * Time.deltaTime;
		// 오프셋 위치.
		Vector3 offset = Quaternion.Euler( 0.0f, angle, 0.0f ) * new Vector3( 0.0f, 0.0f, radius );
		// 이펙트 위치.
		transform.localPosition =  position + offset;
	}
}