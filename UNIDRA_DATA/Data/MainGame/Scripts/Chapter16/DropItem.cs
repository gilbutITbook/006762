using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {
	public enum ItemKind
	{
		Attack,
		Heal,
	};
	public ItemKind kind;

	public AudioClip itemSeClip;
	
	void OnTriggerEnter(Collider other)
	{	
		// Player인지 판정.
		if( other.tag == "Player" ){
			// 아이템 획득.
			CharacterStatus aStatus = other.GetComponent<CharacterStatus>();
			aStatus.GetItem(kind);
			// 획득했으면 아이템을 삭제.
			Destroy(gameObject);

			// 오디오 재생. 
			AudioSource.PlayClipAtPoint(itemSeClip, transform.position);
		}
	}

	// Use this for initialization
	void Start () {
		Vector3 velocity = Random.insideUnitSphere * 2.0f + Vector3.up * 8.0f;
		rigidbody.velocity = velocity;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
