using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    // GetÇµÇΩItemÇÃêî
    private int itemCollected = 0;

    // UI
    GameObject score;

    // Item
    public int Items
    {
        get 
        {
            return itemCollected;
        }
        set
        {
            itemCollected = value;
            Debug.LogFormat("Item: {0}", itemCollected);
        }
    }

    void Start()
    {
        this.score = GameObject.Find("Score");
    }

    void Update()
    {
        this.score.GetComponent<TextMeshProUGUI>().text = "score:" + itemCollected.ToString("D2");
    }
}
