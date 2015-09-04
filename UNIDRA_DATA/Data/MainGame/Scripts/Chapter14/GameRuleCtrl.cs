using UnityEngine;
using System.Collections;

public class GameRuleCtrl : MonoBehaviour {
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
		// 게임 종료 조건 성립 후 씬 전환.
		if( gameOver || gameClear ){
			sceneChangeTime -= Time.deltaTime;
			if( sceneChangeTime <= 0.0f ){
				Application.LoadLevel("TitleScene");
			}
			return;
		}

		timeRemaining -= Time.deltaTime;
		// 남은 시간이 없으면 게임 오버.
		if(timeRemaining<= 0.0f ){
			GameOver();
		}

	}
	
	public void GameOver()
	{
		gameOver = true;
	}
	public void GameClear()
	{
		gameClear = true;
	}
}
