using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimator : MonoBehaviour
{
    public void EndAnimation()
    {
        GameManager.instance.SpawnPlayer();
    }
}
