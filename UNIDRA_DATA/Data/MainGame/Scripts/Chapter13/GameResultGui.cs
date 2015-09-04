using UnityEngine;
using System.Collections;

public class GameResultGui : MonoBehaviour
{
	GameRuleCtrl gameRuleCtrl;

	float baseWidth = 854f;
	float baseHeight = 480f;

	public Texture2D gameOverTexture;
	public Texture2D gameClearTexture;
	
	void Awake()
	{
		gameRuleCtrl = GameObject.FindObjectOfType(typeof(GameRuleCtrl)) as GameRuleCtrl;
	}
	
	void OnGUI()
	{
		Texture2D aTexture;
		if( gameRuleCtrl.gameClear )
		{
			aTexture = gameClearTexture;
		}
		else if( gameRuleCtrl.gameOver )
		{
			aTexture = gameOverTexture;
		}
		else
		{
			return;
		}

		// 해상도 대응.
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.identity,
			new Vector3(Screen.width / baseWidth, Screen.height / baseHeight, 1f));
		
		// 결과.
		GUI.DrawTexture(new Rect(0.0f, 208.0f, 854.0f, 64.0f), aTexture);
	}
}
