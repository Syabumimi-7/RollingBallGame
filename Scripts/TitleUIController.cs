using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        float referenceWidth = 960f;
        float referenceHeight = 540f;

        float scaleX = Screen.width / referenceWidth;
        float scaleY = Screen.height / referenceHeight;

        // 解像度に応じたスケーリングを適用
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1f));

        // 以下は 960x540 を基準にしたボタンの位置とサイズ
        if (GUI.Button(new Rect(320, 330, 100, 50), "stage1"))
        {
            SceneManager.LoadScene("GameScene_1");
            Time.timeScale = 1.0f;
        }
        if (GUI.Button(new Rect(530, 330, 100, 50), "stage2"))
        {
            SceneManager.LoadScene("GameScene_2");
            Time.timeScale = 1.0f;
        }
    }
}
