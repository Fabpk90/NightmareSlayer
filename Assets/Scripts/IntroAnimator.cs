using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimator : MonoBehaviour
{
    public void EndAnimation()
    {
        GameManager.instance.SpawnPlayer(GameManager.instance.playerInitialPosition);
        GameManager.instance.SpawnBoss();
        gameObject.SetActive(false);
        GameManager.instance.musicManager.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        GameManager.instance.ambianceManager = FMODUnity.RuntimeManager.CreateInstance("event:/Amb/Ambiant_Nightmare");
        GameManager.instance.ambianceManager.start();
    }
}
