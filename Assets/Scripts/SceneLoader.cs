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
        SceneManager.LoadScene("Settings");
    }

    public void LoadInstructionsScene()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("DevWithUI");
    }

    public void LoadEndScene()
    {
        SceneManager.LoadScene("End");
    }
}
