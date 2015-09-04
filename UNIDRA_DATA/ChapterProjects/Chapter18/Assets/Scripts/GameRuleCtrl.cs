using UnityEngine;
using System.Collections;

public class GameRuleCtrl : MonoBehaviour {
	// 자신의 컴퓨터에 있는 플레이어 게임 오브젝트.
	public GameObject player;
	// 실체화되는 플레이어의 프리팹.
	public GameObject playerPrefab;
	// 시작 위치.
	public Transform startPoint;
	// FollowCamera 스크립트.
	public FollowCamera followCamera;

	// 남은 시간.
	public float timeRemaining = 5.0f * 60.0f;
	// 게임 오버 플래그.
	public bool gameOver = false;
	// 게임 클리어 플래그.
    public bool gameClear = false;
	// 씬 전환 시간.
	public float sceneChangeTime = 3.0f;

	void Update()
	{
		// 플레이어 생성.
		if (player == null && (Network.isServer || Network.isClient)) {
			Vector3 shiftVector = new Vector3(Network.connections.Length*1.5f,0,0);
			player = Network.Instantiate(playerPrefab,startPoint.position+shiftVector,startPoint.rotation,0) as GameObject;
			followCamera.lookTarget = player.transform;
			// 이름 송신.
			NetworkManager networkManager = FindObjectOfType(typeof(NetworkManager)) as NetworkManager;
			player.networkView.RPC ("SetName",RPCMode.AllBuffered,networkManager.GetPlayerName());
		}

		// 게임 종료 조건 성립 후 씬 전환.
		if( gameOver || gameClear ){
			sceneChangeTime -= Time.deltaTime;
			if( sceneChangeTime <= 0.0f ){
				Application.LoadLevel("TitleScene");
			}
			return;
		}

		// 게임이 시작되면 타이머를 움직인다.
		if (Network.isServer || Network.isClient) {
			timeRemaining -= Time.deltaTime;
			// 남은 시간이 없으면 게임 오버.
			if(timeRemaining<= 0.0f ){
				GameOver();
			}
		}
	}

	// 게임 오버.
	void GameOver()
	{
		if (!gameOver && Network.isServer)
			networkView.RPC("GameOverOnNetwork",RPCMode.AllBuffered);
	}
	
	[RPC]
	void GameOverOnNetwork()
	{
		gameOver = true;
	}

	// 게임 클리어.

	public void GameClear()
	{
		if (!gameClear && Network.isServer)
			networkView.RPC("GameClearOnNetwork",RPCMode.AllBuffered);
	}
	
	[RPC]
	void GameClearOnNetwork(){
		gameClear = true;
	}

	// 남은 시간 설정.
	[RPC]
	void SetRemainTime(float time)
	{
		timeRemaining = time;
	}
	
	// 다른 플레이어가 접속하면 호출된다.
	// 이 함수가 호출되는 것은 서버의 컴퓨터뿐이다.
	void OnPlayerConnected(NetworkPlayer player) 
	{
		networkView.RPC("SetRemainTime",player,timeRemaining);
	}
}
