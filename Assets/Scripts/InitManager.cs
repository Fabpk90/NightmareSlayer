using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = false;
    }
    
    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        while(!FMODUnity.RuntimeManager.HasBanksLoaded)
            yield return new WaitForSeconds(0.2f);
        
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
        
    }
}
