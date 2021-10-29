using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardWindow : MonoBehaviour
{
    public GameObject m_window;

    public GameObject m_weaponRewardOption;
    public GameObject m_itemRewardOption;

    private void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.I))
        {
            Show(true);
        }
        if (InputManager.instance.IsKeyDown(KeyType.O))
        {
            Show();
        }
    }

    public void Show(bool generate = false)
    {
        m_window.SetActive(true);

        if(generate)
        {
            for (int i = ((m_window.transform as RectTransform).childCount) - 1; i >= 0; i--)
            {
                Destroy((m_window.transform as RectTransform).GetChild(i).gameObject);
            }
            
        }
    }

    public void Hide()
    {
        m_window.SetActive(false);
    }
}
