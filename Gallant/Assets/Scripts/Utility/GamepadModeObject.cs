using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadModeObject : MonoBehaviour
{
    public GameObject m_targetObject;
    void Update()
    {
        m_targetObject.SetActive(InputManager.Instance.isInGamepadMode);
    }
}
