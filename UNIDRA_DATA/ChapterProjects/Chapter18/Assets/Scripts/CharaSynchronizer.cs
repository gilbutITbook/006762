using UnityEngine;
using System.Collections;

public class CharaSynchronizer : MonoBehaviour {
	// 스테이터스를 패킹할 때 비트 필드.
	enum  BitField {
		Attacking = 0,
		Died,
	};
	
	// 송수신 정보.
	Vector3 position;
	Quaternion rotation;

	// 캐릭터 스테이터스.
	CharacterStatus status;
	
	// Use this for initialization
	void Start () {
		position = transform.position;
		rotation = transform.rotation;
		status = GetComponent<CharacterStatus>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!networkView.isMine) {
			transform.position = Vector3.Lerp(transform.position,position,Time.deltaTime * 5.0f);
			transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime * 5.0f);
		}	
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			// 송신.
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			stream.Serialize(ref pos );
			stream.Serialize(ref rot );
			if (status != null) {
				int hp = status.HP;
				int packedFlags = PackStatusFlags();
				stream.Serialize(ref hp);
				stream.Serialize(ref packedFlags);
			}
			
		} else {
			// 수신.
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
			if (status != null) {
				int hp = 0;
				int flags = 0;
				stream.Serialize(ref hp);
				stream.Serialize(ref flags);
				status.HP = hp;
				UnpackStatusBit(flags);
			}
		}
	}
	
	int PackStatusFlags()
	{
		int packed = 0;
		packed |= status.attacking ? (1 << (int)BitField.Attacking) : 0;
		packed |= status.died ? (1 << (int)BitField.Died) : 0;
		return packed;
	}
	
	void UnpackStatusBit(int bit)
	{
		status.attacking = (bit & (1 << (int)BitField.Attacking)) != 0;
		status.died = (bit & (1 << (int)BitField.Died)) != 0;
	}
	
}
