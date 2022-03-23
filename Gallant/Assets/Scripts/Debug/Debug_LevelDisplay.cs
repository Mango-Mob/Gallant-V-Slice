using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exceed.Debug
{
    public class Debug_LevelDisplay : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            GetComponent<Text>().text = GameManager.Instance.currentLevel.ToString();
        }
    }

}