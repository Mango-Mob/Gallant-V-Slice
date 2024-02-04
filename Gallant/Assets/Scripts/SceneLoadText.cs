using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadText : MonoBehaviour
{
    public void Update()
    {
        if(InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsGamepadButtonDown(ButtonType.START, 0))
        {
            End();
        }
    }

    public void End()
    {
        Destroy(gameObject);
    }
}
