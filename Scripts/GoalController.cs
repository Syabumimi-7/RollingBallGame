using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public GameDirector gameDirector;

    // アイテムの個数
    public int maxItems = 40;

    // Goal
    bool showGoalScreen = false;
    public string goalText = "GOAL!";

    // Score
    bool scoreScreen = false;
    public string scoreText;

    // UIのstyle
    private GUIStyle goalStyle;
    private GUIStyle scoreStyle;

    // UIの初期化
    private bool goalstyleInitialized = false;
    private bool scorestyleInitialized = false;

    void Start()
    {
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    void Update()
    {
        
    }
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            showGoalScreen = true;
            scoreScreen = true;
            Time.timeScale = 0f;
            Debug.Log("Goal!");
        }
    }

    private void OnGUI()
    {
        // 解像度に応じたスケーリング行列
        float referenceWidth = 960f;
        float referenceHeight = 540f;
        float scaleX = Screen.width / referenceWidth;
        float scaleY = Screen.height / referenceHeight;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1f));

        // GoalのUI
        if (!goalstyleInitialized)
        {
            goalStyle = new GUIStyle(GUI.skin.label);
            goalStyle.fontSize = 96;
            goalStyle.alignment = TextAnchor.MiddleCenter;
            goalStyle.normal.textColor = Color.red;
            goalStyle.fontStyle = FontStyle.Bold;
            goalstyleInitialized = true;
        }

        if (showGoalScreen)
        {
            // 960×540基準で中央にラベル配置
            GUI.Label(new Rect(230, 70, 500, 150), goalText, goalStyle); // 中央寄せ

            // ボタンの位置（画面中央下）
            if (GUI.Button(new Rect(280, 400, 100, 50), "Title"))
            {
                SceneManager.LoadScene("TitleScene");
                Time.timeScale = 1.0f;
            }
            if (GUI.Button(new Rect(580, 400, 100, 50), "Retry"))
            {
                UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
                Time.timeScale = 1.0f;
            }
        }

        // ScoreのUI
        if (!scorestyleInitialized)
        {
            scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = 32;
            scoreStyle.alignment = TextAnchor.MiddleCenter;
            scoreStyle.normal.textColor = Color.red;
            scoreStyle.fontStyle = FontStyle.Bold;
            scorestyleInitialized = true;
        }

        if (scoreScreen)
        {
            if (gameDirector.Items < maxItems)
            {
                scoreText = "score:" + gameDirector.Items.ToString("D2") + "/40";
            }
            else
            {
                scoreText = "score:" + gameDirector.Items.ToString("D2") + "/40\n" + "PERFECT!";
            }

            // スコア表示を中央に
            GUI.Label(new Rect(230, 200, 500, 150), scoreText, scoreStyle);
        }
    }
}
