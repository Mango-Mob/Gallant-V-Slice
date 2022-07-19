using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public class Player_ClassArmour : MonoBehaviour
    {
        public Player_Controller playerController { private set; get; }

        [Header("Slots")]
        public GameObject m_knightClass;
        public GameObject m_mageClass;
        public GameObject m_hunterClass;

        // Start is called before the first frame update
        private void Awake()
        {
            playerController = GetComponent<Player_Controller>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetClassArmour(InkmanClass _class)
        {
            m_knightClass.SetActive(false);
            m_mageClass.SetActive(false);
            m_hunterClass.SetActive(false);

            switch (_class)
            {
                case InkmanClass.KNIGHT:
                    m_knightClass.SetActive(true);
                    break;
                case InkmanClass.MAGE:
                    m_mageClass.SetActive(true);
                    break;
                case InkmanClass.HUNTER:
                    m_hunterClass.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}