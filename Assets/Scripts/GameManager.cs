using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public bool gameHasStarted = false;
    public GameObject titleScreenUi;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameHasStarted)
        {
            if (Input.anyKey)
            {
                gameHasStarted = true;
                Debug.Log("Game Started");
                titleScreenUi.SetActive(false);
                player.hasControl = true;
            }
        }
    }
}
