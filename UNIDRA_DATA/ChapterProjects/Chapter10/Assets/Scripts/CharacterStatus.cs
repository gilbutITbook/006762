using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {
	
	//---------- 공격 장에서 사용한다. ----------
	// 체력.
	public int HP = 100;
	public int MaxHP = 100;
	
	// 공격력.
	public int Power = 10;
	
	// 마지막에 공격한 대상.
	public GameObject lastAttackTarget = null;
	
	//---------- GUI 및 네트워크 장에서 사용한다. ----------
	// 플레이어 이름.
	public string characterName = "Player";
	
	//--------- 애니메이션 장에서 사용한다. -----------
	// 상태.
	public bool attacking = false;
	public bool died = false;
	
}
