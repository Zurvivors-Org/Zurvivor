using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadStartScene()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void LoadSettingsScene()
    {
       // SceneManager.LoadScene("Blank");
    }

    public void LoadInstructionsScene()
    {
        //SceneManager.LoadScene("Blank");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Developer Scene");
    }

    public void LoadEndScene()
    {
       // SceneManager.LoadScene("Blank");
    }
}
