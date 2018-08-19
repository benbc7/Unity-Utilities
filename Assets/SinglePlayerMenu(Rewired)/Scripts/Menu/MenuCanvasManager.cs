using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasManager : MonoBehaviour
{

    public GameObject titlePanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public List<GameObject> settingsTabPanels;

    public void PlayButton()
    {

    }

    public void SettingsButton()
    {
        titlePanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsTabPanels[0].SetActive(true);
    }

    public void CreditsButton()
    {

    }

    public void ExitButton()
    {

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#endif

        Application.Quit();
    }

    [System.Serializable]
    public class TitleScreenComponents {
        
    }

	[System.Serializable]
    public class SettingsComponents {

	}
}
