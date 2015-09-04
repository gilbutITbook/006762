using UnityEngine;
using System.Collections;

public class Nameplate : MonoBehaviour {
	public Vector3 offset = new Vector3(0, 2.0f, 0);
	public CharacterStatus status;
	TextMesh textMesh;
	
	// Use this for initialization
	void Start () {
		// 컴포넌트 캐시.
		textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		// 이름 갱신.
		if (textMesh.text != status.characterName)
			textMesh.text =  status.characterName;
		// 머리 위로 이동.
		transform.position = status.transform.position + offset;
		// 항상 카메라와 같은 방향으로.
		transform.rotation = Camera.main.transform.rotation;
		// 크기 조정.
		float scale = Camera.main.transform.InverseTransformPoint(transform.position).z / 30.0f;
		transform.localScale = Vector3.one * scale;
	}
}