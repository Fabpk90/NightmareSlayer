using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player;
    public bool gameHasStarted = false;
    public GameObject titleScreenUi;
    public CameraManager camera;
    public GameObject Door;
    public static GameManager instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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

    public void GiveControls(bool hasControl)
    {
        player.rigidBody.velocity = Vector2.zero;
        player.hasControl = hasControl;
    }

    public void LockBossRoom()
    {
        Door.SetActive(true);
        camera.FixPositionForBossFight();
    }
}
