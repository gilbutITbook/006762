using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {
	public enum ItemKind
	{
		Attack,
		Speed,
	};

	public ItemKind kind;

	// Use this for initialization
	void Start () {
		Vector3 velocity = Random.insideUnitSphere * 5.0f + Vector3.up * 10.0f;
		rigidbody.velocity = velocity;
	}
	
	void OnTriggerEnter(Collider other)
	{	
		PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();	
		if (playerCtrl == null)
			return;
		CharacterStatus aStatus = other.GetComponent<CharacterStatus>();
		aStatus.GetItem(kind);
		Destroy(gameObject);
	}
}
