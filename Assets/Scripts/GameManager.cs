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
    public Image lifeImage;
    public List<Sprite> lifeSpriteList;
    public GameObject earlyAnimation;
    public GameObject bossUI;
    public Slider healthSlider;



    
    [Header("Prefabs")]
    public GameObject bossRoomTriggerPrefab;
    public GameObject playerPrefab;
    public GameObject bossPrefab;
    public GameObject smokeParticle;
    
    
    [Header("Title and win screen")]
    public bool hasTitleScreenLoaded = false;
    public EventInstance musicManager;
    public EventInstance ambianceManager;
    public Image title;
    public Image pressStart;
    public Image titleBackground;
    public Image background;
    public Image winScreen;
    public Image storyImage;
    
    
    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        FMODUnity.RuntimeManager.SetListenerLocation(camera.gameObject);

        musicManager = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Menu_Music");

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
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Menu_Validation", transform.position);
                StartCoroutine(StartGameAnimation());
            }
        }
    }

    private IEnumerator StartGameAnimation()
    {
        gameHasStarted = true;
        StartCoroutine(FadeOut(2, title));
        StartCoroutine(FadeOut(2, titleBackground));
        StartCoroutine(FadeOut(2, pressStart));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(1, storyImage));
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeOut(1.5f, storyImage));
        yield return new WaitForSeconds(3f);
        earlyAnimation.SetActive(true);
        titleScreenUi.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Char/Char_Spawn_Early", transform.position);
    }

    public void SpawnPlayer()
    {
        musicManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        player.hasControl = true;
        player.lifeImage.gameObject.SetActive(true);
        FMODUnity.RuntimeManager.SetListenerLocation(player.gameObject);
        ambianceManager = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Ambiant_Nightmare");
        ambianceManager.start();
        player.gameObject.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/Menu_Validation", transform.position);
        earlyAnimation.SetActive(false);
    }

    public void GiveControls(bool hasControl)
    {
        player.rigidBody.velocity = Vector2.zero;
        player.hasControl = hasControl;
    }

    public void LockBossRoom()
    {
        ambianceManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicManager = FMODUnity.RuntimeManager.CreateInstance("event:/Music/InGame_Music");
        musicManager.start();
        boss.gameObject.SetActive(true);
        Door.SetActive(true);
        camera.FixPositionForBossFight();
        smokeParticle.SetActive(true);
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
        musicManager.start();
        StartCoroutine(FadeOut(1, background));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(2, title));
        StartCoroutine(FadeIn(2, titleBackground));
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(0.5f, pressStart));
        hasTitleScreenLoaded = true;
    }

    public void RetryBoss()
    {
        smokeParticle.SetActive(false);
        StartCoroutine(OnDeathRetryBoss());
    }
    
    private IEnumerator OnDeathRetryBoss()
    {
        Destroy(boss.gameObject);
        bossUI.SetActive(false);
        healthSlider.value = 1;
        player.lifeImage.gameObject.SetActive(false);
        Destroy(player.gameObject);
        Door.SetActive(false);
        musicManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambianceManager.start();
        StartCoroutine(FadeIn(2, background));
        yield return new WaitForSeconds(2);
        Instantiate(bossRoomTriggerPrefab);
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.gameObject.SetActive(true);
        player.hasControl = true;
        player.lifeImage = lifeImage;
        player.lifeSpriteList = lifeSpriteList;
        player.lifeImage.sprite = lifeSpriteList[0];
        player.lifeImage.gameObject.SetActive(true);
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
        smokeParticle.SetActive(false);
        StartCoroutine(WinAnimation());
    }

    private IEnumerator WinAnimation()
    {
        player.hasControl = false;
        bossUI.SetActive(false);
        player.lifeImage.gameObject.SetActive(false);
        winParticles.SetActive(true);
        yield return new WaitForSeconds(2);
        StartCoroutine(FadeIn(4, winScreen));
        yield return new WaitForSeconds(4);
        StartCoroutine(FadeIn(2, background));
        yield return new WaitForSeconds(2.5f);
        musicManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance = null;
        SceneManager.LoadScene(1);
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
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
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
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
}
