using UnityEngine;
using System.Collections;

public class EnemyCtrl : MonoBehaviour {
	CharacterStatus status;
	CharaAnimation charaAnimation;
    CharacterMove characterMove;
	Transform attackTarget;
    // 대기 시간은 2초로 설정한다.
    public float waitBaseTime = 2.0f;
    // 남은 대기 시간.
    float waitTime;
    // 이동 범위 5미터.
    public float walkRange = 5.0f;
    // 초기 위치를 저장해 둘 변수.
    public Vector3 basePosition;
    // 복수의 아이템을 저장할 수 있는 배열로 한다.
    public GameObject[] dropItemPrefab;

	// 게임 규칙.
	GameRuleCtrl gameRuleCtrl;

	public GameObject hitEffect;

	// 스테이트 종류.
	enum State {
        Walking,	// 탐색.
        Chasing,	// 추적.
        Attacking,	// 공격.
        Died,		// 사망.
    };
	
	State state = State.Walking;		// 현재 스테이트.
	State nextState = State.Walking;	// 다음 스테이트.

	public AudioClip deathSeClip;
	public AudioClip attackSeClip;

	// Use this for initialization
	void Start () {
        status = GetComponent<CharacterStatus>();
        charaAnimation = GetComponent<CharaAnimation>();
    	characterMove = GetComponent<CharacterMove>(); 
        // 초기 위치를 저장한다.
        basePosition = transform.position;
        // 대기 시간.
        waitTime = waitBaseTime;

		gameRuleCtrl = FindObjectOfType<GameRuleCtrl>();
    }
	
	// Update is called once per frame
	void Update () {
		if (!networkView.isMine)
			return;

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
				Died();
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
        // 대기 시간이 아직 남았다면.
        if (waitTime > 0.0f)
        {
            // 대기 시간을 줄인다.
            waitTime -= Time.deltaTime;
            // 대기 시간이 없어지면.
            if (waitTime <= 0.0f)
            {
                // 범위 내의 어딘가.
                Vector2 randomValue = Random.insideUnitCircle * walkRange;
                // 이동할 곳을 설정한다.
                Vector3 destinationPosition = basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
                // 목적지를 지정한다.
                SendMessage("SetDestination", destinationPosition);
            }
        }
        else
        {
            // 목적지에 도착한다.
            if (characterMove.Arrived())
            {
                // 대기 상태로 전환한다.
                waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
            }
            // 타겟을 발견하면 추적한다.
            if (attackTarget)
            {
                ChangeState(State.Chasing);
            }
        }
    }
    // 추적 시작.
    void ChaseStart()
    {
        StateStartCommon();
    }
    // 추적 중.
    void Chasing()
    {
	    // 이동할 곳을 플레이어에 설정한다.
	    SendMessage("SetDestination", attackTarget.position);
	    // 2미터 이내로 접근하면 공격한다.
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
		
		// 이동을 멈춘다.
		SendMessage("StopMove");

		// 공격 사운드 효과 재생.
		AudioSource.PlayClipAtPoint(attackSeClip,transform.position);

	}
	
	// 공격 중 처리.
	void Attacking()
	{
		if (charaAnimation.IsAttacked())
			ChangeState(State.Walking);
        // 대기 시간을 다시 설정한다.
        waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
        // 타겟을 리셋한다.
        attackTarget = null;

    }

    void dropItem()
    {
        if (dropItemPrefab.Length == 0) { return; }
        GameObject dropItem = dropItemPrefab[Random.Range(0, dropItemPrefab.Length)];
		Network.Instantiate(dropItem, transform.position, transform.rotation,0);
    }

	void Died()
	{
		status.died = true;
		dropItem();
		AudioSource.PlayClipAtPoint(deathSeClip,transform.position);
		if( gameObject.tag == "Boss" ) {
			gameRuleCtrl.GameClear();
		}
		Network.Destroy(gameObject);
		Network.RemoveRPCs(networkView.viewID);
	}

	void Damage(AttackArea.AttackInfo attackInfo)
	{
		// 이펙트 발생.
		GameObject effect = Instantiate ( hitEffect, transform.position,Quaternion.identity ) as GameObject;
		effect.transform.localPosition = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
		Destroy(effect, 0.3f);

		if (networkView.isMine)
			DamageMine(attackInfo.attackPower);
		else
			networkView.RPC("DamageMine",networkView.owner,attackInfo.attackPower);
	}
	
	[RPC]
	void DamageMine(int damage)
	{
		status.HP -= damage;
		if (status.HP <= 0) {
			status.HP = 0;
			// 체력이 ０이므로 사망 스테이트로 전환한다.
			ChangeState(State.Died);
		}
	}

	// 스테이트가 시작되기 전에 스테이터스를 초기화한다.
	void StateStartCommon()
	{
		status.attacking = false;
		status.died = false;
	}
    // 공격 대상을 설정한다.
    public void SetAttackTarget(Transform target)
    {
        attackTarget = target;
    }

	void OnNetworkInstantiate(NetworkMessageInfo info) {
		if (!networkView.isMine) {
			CharacterMove move = GetComponent<CharacterMove>();
			Destroy(move);
			
			AttackArea[] attackAreas = GetComponentsInChildren<AttackArea>();
			foreach (AttackArea attackArea in attackAreas) {
				Destroy(attackArea);
			}
			
			AttackAreaActivator attackAreaActivator = GetComponent<AttackAreaActivator>();
			Destroy(attackAreaActivator);
		}
	}
}
