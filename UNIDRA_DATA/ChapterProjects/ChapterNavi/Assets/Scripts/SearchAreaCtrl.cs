using UnityEngine;
using System.Collections;

public class SearchAreaCtrl : MonoBehaviour {

	void OnTriggerStay( Collider other )
	{
		// Player를 확인한다.
		PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
		if( playerCtrl == null ){ return; }

		transform.parent.gameObject.GetComponent<EnemyCtrl>().SetAttackTarget(other.transform);
	}
}
