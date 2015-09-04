using UnityEngine;
using System.Collections;

public class EnemyCtrl : MonoBehaviour {
	CharacterStatus status;
	CharaAnimation charaAnimation;
	CharacterMove characterMove;
	Transform attackTarget;
	public GameObject[] dropItemPrefab;

    public float waitBaseTime = 2.0f;
    float waitTime;
	public float walkRange = 5.0f;
    // 초기 위치를 저장해 둘 변수.
    public Vector3 basePosition;

	// 스테이트 종류.
	enum State {
		Walking,
		Attacking,
		Chasing,
		Died,
	} ;
	
	State state = State.Walking;		// 현재 스테이트.
	State nextState = State.Walking;	// 다음 스테이트.

	// Use this for initialization
	void Start () {
		status = GetComponent<CharacterStatus>();
		charaAnimation = GetComponent<CharaAnimation>();
		characterMove = GetComponent<CharacterMove>();
        basePosition = transform.position;
        waitTime = waitBaseTime;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.Walking:
			Walking();
			break;
		case State.Chasing:
			Chasing();
			break;
		case State.Attacking:
			Attacking();
			break;
		case State.Died:
			Died();
			break;
		}
		
		if (state != nextState)
		{
			state = nextState;
			switch (state) {
			case State.Walking:
				WalkStart();
				break;
			case State.Chasing:
				ChaseStart();
				break;
			case State.Attacking:
				AttackStart();
				break;
			case State.Died:
				DiedStart();
				break;
			}
		}
	}
	
	
	// 스테이트를 변경한다.
	void ChangeState(State nextState)
	{
		this.nextState = nextState;
	}
	
	void WalkStart()
	{
		StateStartCommon();
	}
	
	void Walking()
	{
		if( waitTime > 0.0f){
			waitTime -= Time.deltaTime;
			if (waitTime <= 0.0f){
				// 범위 내의 어딘가.
				Vector2 randomValue = Random.insideUnitCircle * walkRange;
				// 이동할 장소를 설정한다.
                Vector3 destinationPosition = basePosition + (Vector3)(randomValue);

				SendMessage("SetDestination",destinationPosition);
			}
		}else{
			// 목적지에 도착한다.
			if (characterMove.Arrived())
			{
				// 대기 상태로 전환한다.
				waitTime = Random.Range(waitBaseTime, waitBaseTime*2.0f);
			}
            if (attackTarget)
            {
                ChangeState(State.Chasing);
            }
        }
	}

	void ChaseStart()
	{
		StateStartCommon();
	}

	void Chasing()
	{
        // 목적지를 지정한다.
        SendMessage("SetDestination", attackTarget.position);
        // 목적지에 도착한다.
        if (Vector3.Distance( attackTarget.position, transform.position ) <= 2.0f)
		{
			ChangeState(State.Attacking);
		}
	}

	// 공격 스테이트가 시작되기 전에 호출된다.
	void AttackStart()
	{
		StateStartCommon();
		status.attacking = true;
		
		// 적이 있는 방향으로 돌아본다.
		Vector3 targetDirection = (attackTarget.position-transform.position).normalized;
		SendMessage("SetDirection",targetDirection);
		
	}
	
	// 공격 중 처리.
	void Attacking()
	{
        if (charaAnimation.IsAttackFinished())
        {
            ChangeState(State.Walking);
            // 대기 시간을 다시 설정한다.
            waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
            // 타겟을 리셋한다.
            attackTarget = null;
        }
	}

	void DiedStart()
	{
		status.died = true;
		dropItem();
		Destroy (gameObject,3.0f);		
	}
	
	void Died()
	{

	}
	
	void Damage(AttackArea.AttackInfo attackInfo)
	{
		status.HP -= attackInfo.attackPower;
		if (status.HP <= 0) {
			status.HP = 0;
			// 체력이 0이므로 사망 스테이트로 전환한다.
			ChangeState(State.Died);
		}
	}
	
	// 스테이트가 시작되기 전에 스테이터스를 초기화한다.
	void StateStartCommon()
	{
		status.attacking = false;
		status.died = false;
	}

	void dropItem()
	{
		if (dropItemPrefab.Length == 0)
			return;
		GameObject dropItem = dropItemPrefab[ Random.Range(0, dropItemPrefab.Length) ];
		Instantiate(dropItem, transform.position, transform.rotation);
	}

	public void SetAttackTarget(Transform target)
	{
		attackTarget = target;
	}
}
