using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	public GameObject enemyPrefab;
	public float size = 3.0f;
	
	GameObject[] existEnemys ;
	public int maxEnemy = 1;
	public int totalEnemy = 1000000000;
	
	// Use this for initialization
	void Start () {
		existEnemys = new GameObject[maxEnemy];
		StartCoroutine(Exec());
	}


	IEnumerator Exec()
	{
		while(true){
			// yield라는 키워드로 중단시킬 수 있다.
			// 재개할 때는 yield 뒤부터 실행된다.
			// 3초에 한 번 이 프로그램이 실행된다.
			if( Generator() )
				yield break;

            yield return new WaitForSeconds( 3.0f );
		}
	}

    bool Generator()
    {
        for (int enemyCnt = 0; enemyCnt < existEnemys.Length; ++enemyCnt)
        {
            if (existEnemys[enemyCnt] == null)
            {
                Vector3 pos = (Vector3)(Random.insideUnitCircle * size) + transform.position;
                // Raycast하여 지면에 흡착시킨다.
                RaycastHit hitInfo;
                if (Physics.Raycast(pos,Vector3.down,out hitInfo,1000.0f,(1 << LayerMask.NameToLayer("Ground"))))
                    pos = hitInfo.point;
                // 적을 생성한다.
                existEnemys[enemyCnt] = Instantiate(enemyPrefab,pos,Quaternion.Euler(0, Random.Range(-180.0f, 180.0f), 0)) as GameObject;
                // 최대 수를 줄인다.
                totalEnemy--;
                return false;
            }
        }
        // 최대 수가 없어지면 코루틴 종료.
        return (totalEnemy <= 0);
    }
}

