using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public List<SpriteRenderer> nightmareSprites = new List<SpriteRenderer>();
    public GameObject winParticles;
    public GameObject bossRightSide;
    public GameObject bossLeftSide;

    
    [Header("Prefabs")]
    public GameObject bossRoomTriggerPrefab;
    public GameObject playerPrefab;
    public GameObject bossPrefab;
    
    
    [Header("Title and win screen")]
    public bool hasTitleScreenLoaded = false;
    public EventInstance musicManager;
    public Image title;
    public Image pressStart;
    public Image background;
    public Image winScreen;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        FMODUnity.RuntimeManager.SetListenerLocation(camera.gameObject);

        //TODO : Mettre la musique
        musicManager = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Ambiant_Nightmare");
        musicManager.start();

        Cursor.visible = false;
        

    }

    void Start()
    {
        StartCoroutine(TitleScreenAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameHasStarted && hasTitleScreenLoaded)
        {
            if (Input.anyKey)
            {
                player.gameObject.SetActive(true);
                gameHasStarted = true;
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

    public void SetNightmareAmount(float ratio)
    {
        foreach (var spriteColor in nightmareSprites)
        {
            Color oldColor = spriteColor.color;
            spriteColor.color = new Color(oldColor.r, oldColor.g, oldColor.b, ratio);
        }
    }

    private IEnumerator TitleScreenAnimation()
    {
        StartCoroutine(FadeOut(1, background));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(2, title));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(2, pressStart));
        hasTitleScreenLoaded = true;
    }

    public void RetryBoss()
    {
        StartCoroutine(OnDeathRetryBoss());
    }
    
    private IEnumerator OnDeathRetryBoss()
    {
        Destroy(boss.gameObject);
        Destroy(player.gameObject);
        Door.SetActive(false);
        musicManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        StartCoroutine(FadeIn(2, background));
        yield return new WaitForSeconds(2);
        Instantiate(bossRoomTriggerPrefab);
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.hasControl = true;
        boss = Instantiate(bossPrefab).GetComponent<Boss>();
        boss.gameObject.SetActive(false);
        boss.player = player;
        boss.left = bossLeftSide;
        boss.right = bossRightSide;
        SetNightmareAmount(1);
        StartCoroutine(FadeOut(2, background));
        camera.playerPosition = player.transform;
        camera.isFollowingPlayer = true;
    }

    public void OnWin()
    {
        StartCoroutine(WinAnimation());
    }

    private IEnumerator WinAnimation()
    {
        winParticles.SetActive(true);
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(4, winScreen));
        yield return new WaitForSeconds(4);
        StartCoroutine(FadeIn(2, background));
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(0);
    }
    
    
    private IEnumerator FadeIn(float fadeDuration, Image image)
    {
        float progress = 0f;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        while (progress < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, progress));
            progress += Time.deltaTime / fadeDuration;

            yield return null;
        }
    }

    private IEnumerator FadeOut(float fadeDuration, Image image)
    {
        float progress = 0f;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        while (progress < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1, 0, progress));
            progress += Time.deltaTime / fadeDuration;

            yield return null;
        }
    }
}
