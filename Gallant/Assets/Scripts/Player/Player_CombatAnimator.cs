using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSystem
{
    public enum InputType
    {
        LeftAttack,
        RightAttack,
        LeftCast,
        RightCast,
        Roll,
        Heal,
    }

    public class Player_CombatAnimator : MonoBehaviour
    {
        public Player_Controller playerController { private set; get; }
        public float m_durationTransition = 0.4f;
        private Queue<KeyValuePair<InputType, string>> m_inputQueue = new Queue<KeyValuePair<InputType, string>>();

        // Start is called before the first frame update
        void Awake()
        {
            playerController = GetComponent<Player_Controller>();
        }


        // Update is called once per frame
        void Update()
        {
            // Check if no conflicting animation

            bool baseLayerReady = playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Base")).IsName("Run Blend Tree") ||
                playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Base")).IsName("Rotate Blend Tree");

            bool attackLayerReady = playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Arm")).IsName("No Attack") && // Check if not attack
                !playerController.animator.IsInTransition(playerController.animator.GetLayerIndex("Arm")); // Check if not transitioning

            bool animReady = baseLayerReady && attackLayerReady && m_inputQueue.Count != 0;

            if (!animReady)
                return;

            switch (m_inputQueue.Peek().Key)
            {
                case InputType.LeftAttack:
                case InputType.RightAttack:
                    if (playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Arm")).IsName("No Attack") &&
                !playerController.animator.IsInTransition(playerController.animator.GetLayerIndex("Arm")))
                    {
                        float transitionDuration = m_durationTransition;
                        string animName = m_inputQueue.Peek().Value;

                        switch (animName)
                        {
                            case "Left Hammer":
                                playerController.animator.SetBool("CanRotate", false);

                                playerController.playerMovement.QuickSetAttackMoveSpeedLerp(0.0f);
                                playerController.InstantRunStop();
                                break;
                            case "Left Greatsword":
                                playerController.playerMovement.QuickSetAttackMoveSpeedLerp(0.0f);
                                playerController.InstantRunStop();
                                break;
                            case "Left Brick":
                                transitionDuration = 0.2f;
                                playerController.animator.SetBool("CanRotate", false);
                                break;
                            default:
                                break;
                        }

                        Debug.Log("Playing Attack Anim");
                        playerController.animator.CrossFade(animName, transitionDuration, playerController.animator.GetLayerIndex("Arm"));

                        if (animName[0] == 'L')
                        {
                            playerController.animator.SetBool("UsingLeft", true);
                            playerController.m_lastAttackHand = Hand.LEFT;
                        }
                        if (animName[0] == 'R')
                        {
                            playerController.animator.SetBool("UsingRight", true);
                            playerController.m_lastAttackHand = Hand.RIGHT;
                        }
                    }
                    break;
                case InputType.LeftCast:
                case InputType.RightCast:
                case InputType.Heal:
                case InputType.Roll:

                    break;
                default:
                    Debug.LogError("Unconfigured player animation queued");
                    return;
            }
            m_inputQueue.Dequeue();
        }

        public void AddAction(InputType _type, string _animName)
        {
            foreach (var input in m_inputQueue)
            {
                if (input.Key == _type)
                    return;
            }

            m_inputQueue.Enqueue(new KeyValuePair<InputType, string>(_type, _animName));
            Debug.Log($"{_animName} added to queue ({_type}).");
        }


        //public void PlayAttack(string _animName)
        //{
        //    if (playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Arm")).IsName("No Attack") &&
        //        !playerController.animator.IsInTransition(playerController.animator.GetLayerIndex("Arm")))
        //    {
        //        float transitionDuration = m_durationTransition;
        //        if (_animName == "Left Hammer")
        //        {
        //            playerController.animator.SetBool("CanRotate", false);
        //        }
        //        if (_animName == "Left Brick")
        //        {
        //            transitionDuration = 0.2f;
        //            playerController.animator.SetBool("CanRotate", false);
        //        }
        //        if (_animName == "Left Hammer" || _animName == "Left Greatsword")
        //        {
        //            playerController.playerMovement.QuickSetAttackMoveSpeedLerp(0.0f);
        //            playerController.InstantRunStop();
        //        }
        //        playerController.animator.CrossFade(_animName, transitionDuration, playerController.animator.GetLayerIndex("Arm"));

        //        if (_animName[0] == 'L')
        //        {
        //            playerController.animator.SetBool("UsingLeft", true);
        //            playerController.m_lastAttackHand = Hand.LEFT;
        //        }
        //        if (_animName[0] == 'R')
        //        {
        //            playerController.animator.SetBool("UsingRight", true);
        //            playerController.m_lastAttackHand = Hand.RIGHT;
        //        }
        //    }
        //}

        public void SetIdleAnimation(Weapon _weapon, Hand _hand)
        {
            int layerIndex = 0;
            switch (_hand)
            {
                case Hand.LEFT:
                    layerIndex = playerController.animator.GetLayerIndex("IdleArmL");
                    break;
                case Hand.RIGHT:
                    layerIndex = playerController.animator.GetLayerIndex("IdleArmR");
                    break;
                default:
                    return;
            }

            string animName;
            switch (_weapon)
            {
                case Weapon.BOW:
                    animName = "Bow";
                    break;
                case Weapon.GREATSWORD:
                    animName = "Greatsword";
                    break;
                case Weapon.CROSSBOW:
                    animName = "Crossbow";
                    break;
                case Weapon.SPEAR:
                    animName = "Spear";
                    break;
                default:
                    animName = "Default";
                    break;
            }

            if (!playerController.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animName))
            {
                playerController.animator.Play(animName, layerIndex);
            }
        }
        public void SetRunAnimation(Weapon _weapon, Hand _hand)
        {
            int layerIndex = 0;
            switch (_hand)
            {
                case Hand.LEFT:
                    layerIndex = playerController.animator.GetLayerIndex("RunArmL");
                    break;
                case Hand.RIGHT:
                    layerIndex = playerController.animator.GetLayerIndex("RunArmR");
                    break;
                default:
                    return;
            }

            string animName;
            switch (_weapon)
            {
                case Weapon.BOW:
                    animName = "Bow";
                    break;
                case Weapon.GREATSWORD:
                    animName = "Greatsword";
                    break;
                case Weapon.SPEAR:
                    animName = "Spear";
                    break;
                default:
                    animName = "Default";
                    break;
            }

            if (!playerController.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(animName))
            {
                playerController.animator.Play(animName, layerIndex);
            }
        }
    }
}