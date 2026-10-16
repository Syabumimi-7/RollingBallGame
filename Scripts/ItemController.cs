using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameDirector gameDirector;

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
            Destroy(gameObject);
            gameDirector.Items += 1;
            Debug.Log("Get Item!");
        }
    }
}
