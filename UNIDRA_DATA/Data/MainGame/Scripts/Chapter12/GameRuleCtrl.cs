using UnityEngine;
using System.Collections;

public class GameRuleCtrl : MonoBehaviour {
	// 남은 시간.
	public float timeRemaining = 5.0f * 60.0f;
	// 게임 오버 플래그. 
	public bool gameOver = false;
	// 게임 클리어 플래그. 
    public bool gameClear = false;

	void Update()
	{
		timeRemaining -= Time.deltaTime;
		// 남은 시간이 없으면 게임 오버.
		if(timeRemaining<= 0.0f ){
			GameOver();
		}
	}
	
	public void GameOver()
	{
		gameOver = true;
        Debug.Log("GameOver");
	}
	public void GameClear()
	{
		gameClear = true;
        Debug.Log("GameClear");
    }
}
