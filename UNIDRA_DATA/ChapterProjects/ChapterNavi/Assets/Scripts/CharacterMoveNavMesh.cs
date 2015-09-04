using UnityEngine;
using System.Collections;

public class CharacterMoveNavMesh : MonoBehaviour {
	public NavMeshAgent agent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// 목적지를 설정한다. 인수 destination은 목적지.
	public void SetDestination(Vector3 destination)
	{
		agent.SetDestination( destination );
	}
}
