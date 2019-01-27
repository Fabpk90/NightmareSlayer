using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}
