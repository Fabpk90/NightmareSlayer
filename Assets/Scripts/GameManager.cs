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
    public Boss boss;
    public float bossNightmareAmount = 1;
    public SpriteRenderer nightmareOpacityRenderer;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        FMODUnity.RuntimeManager.SetListenerLocation(camera.gameObject);

        Cursor.visible = false;
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
                FMODUnity.RuntimeManager.SetListenerLocation(player.gameObject);
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
        StartCoroutine(BossRoomAnimation());
    }

    private IEnumerator BossRoomAnimation()
    {
        Door.SetActive(true);
        camera.FixPositionForBossFight();
        yield return new WaitForSeconds(3);
        boss.gameObject.SetActive(true);
    }

    // Between 0 and 1
    public void SetNightmareAmount(float ratio)
    {
        Debug.Log(ratio);
        Color oldColor = nightmareOpacityRenderer.color;
        nightmareOpacityRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, ratio);
        Debug.Log(nightmareOpacityRenderer.color);
    }
}
