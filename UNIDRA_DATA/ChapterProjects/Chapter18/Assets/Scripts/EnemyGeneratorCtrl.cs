using UnityEngine;
using System.Collections;

public class EnemyGeneratorCtrl : MonoBehaviour {
	// 생겨날 적 프리팹.
	public GameObject enemyPrefab;
	// 적을 저장한다.
	GameObject[] existEnemys;
	// 액티브 최대 수.
	public int maxEnemy = 2;

	void Start()
	{
		// 배열 확보.
		existEnemys = new GameObject[maxEnemy];
		// 주기적으로 실행하고 싶을 때는 코루틴을 사용하면 쉽게 구현할 수 있다.
		StartCoroutine(Exec());
	}

	// 적을 생성한다.
	IEnumerator Exec()
	{
		while(true){ 
			if (Network.isServer)
				Generate();
			yield return new WaitForSeconds( 3.0f );
		}
	}

	void Generate()
	{
		for(int enemyCount = 0; enemyCount < existEnemys.Length; ++ enemyCount)
		{
			if( existEnemys[enemyCount] == null ){
				// 적 생성.
				existEnemys[enemyCount] = Network.Instantiate(enemyPrefab,transform.position,transform.rotation,0) as GameObject;
				return;
			}
		}
	}
	
}
