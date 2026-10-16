using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameDirector gameDirector;

    // アイテムの個数
    public int maxItems = 40;

    // Gameover
    bool showGameoverScreen = false;
    public string gameoverText = "Gameover";

    // Score
    bool scoreScreen = false;
    public string scoreText;

    // UIのstyle
    private GUIStyle gameoverStyle;
    private GUIStyle scoreStyle;

    // UIの初期化
    private bool gameoverstyleInitialized = false;
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
            showGameoverScreen = true;
            scoreScreen = true;
            Time.timeScale = 0f;
            Debug.Log("GameOver");
        }
    }

    private void OnGUI()
    {
        // 解像度スケーリング（基準: 960x540）
        float referenceWidth = 960f;
        float referenceHeight = 540f;
        float scaleX = Screen.width / referenceWidth;
        float scaleY = Screen.height / referenceHeight;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scaleX, scaleY, 1f));

        // GameOverのUI
        if (!gameoverstyleInitialized)
        {
            gameoverStyle = new GUIStyle(GUI.skin.label);
            gameoverStyle.fontSize = 96;
            gameoverStyle.alignment = TextAnchor.MiddleCenter;
            gameoverStyle.normal.textColor = Color.red;
            gameoverStyle.fontStyle = FontStyle.Bold;
            gameoverstyleInitialized = true;
        }

        if (showGameoverScreen)
        {
            // 中央に Game Over テキスト
            GUI.Label(new Rect(230, 70, 500, 150), gameoverText, gameoverStyle);

            // Title ボタン（中央下寄り）
            if (GUI.Button(new Rect(280, 400, 100, 50), "Title"))
            {
                SceneManager.LoadScene("TitleScene");
                Time.timeScale = 1.0f;
            }

            // Retry ボタン（右寄り）
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

            // 中央にスコア表示
            GUI.Label(new Rect(230, 200, 500, 150), scoreText, scoreStyle);
        }
    }

}
