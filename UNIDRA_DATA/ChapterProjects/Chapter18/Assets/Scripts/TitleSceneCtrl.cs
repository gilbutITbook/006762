using UnityEngine;
using System.Collections;

public class TitleSceneCtrl : MonoBehaviour {
    // 타이틀 화면 텍스처.
    public Texture2D bgTexture;

    void OnGUI()
    {
        // 스타일을 준비한다.
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

        // 해상도를 지원한다.
        GUI.matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.identity,
            new Vector3(Screen.width / 854.0f, Screen.height / 480.0f, 1.0f));
        // 타이틀 화면에 텍스처를 표시한다.
        GUI.DrawTexture(new Rect(0.0f, 0.0f, 854.0f, 480.0f), bgTexture);

        // 시작 버튼을 만든다.
        if (GUI.Button(new Rect(327, 290, 200, 54), "Start", buttonStyle))
        {
            // 씬 전환은 Application.LoadLevel를 사용한다.
            Application.LoadLevel("GameScene");
        }
    }
}
