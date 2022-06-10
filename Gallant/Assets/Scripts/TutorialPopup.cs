using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    public GameObject KeyboardDisplay;
    public GameObject GamepadDisplay;

    // Update is called once per frame
    void Update()
    {
        if(NavigationManager.Instance.IsVisible)
        {
            Destroy(gameObject);
        }
        GamepadDisplay.SetActive(InputManager.Instance.isInGamepadMode);
        KeyboardDisplay.SetActive(!InputManager.Instance.isInGamepadMode);
    }
}
