using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public static bool IsStillPlaying = false;
    public void Update()
    {
        if(InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsGamepadButtonDown(ButtonType.START, 0))
        {
            EndCredits();
        }
    }
    // Start is called before the first frame update
    public void EndCredits()
    {
        if(IsStillPlaying)
        {
            LevelManager.Instance.LoadHubWorld(false);
        }
        else
        {
            LevelManager.Instance.LoadNewLevel("MainMenu");
        }
        CreditsScript.IsStillPlaying = false;
    }
}
