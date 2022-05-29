using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CombatAnimator : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    public float m_durationTransition = 0.4f;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }

    public void PlayAttack(string _animName)
    {
        if (playerController.animator.GetCurrentAnimatorStateInfo(playerController.animator.GetLayerIndex("Arm")).IsName("No Attack") &&
            !playerController.animator.IsInTransition(playerController.animator.GetLayerIndex("Arm")))
        {
            float transitionDuration = m_durationTransition;
            //if (_animName == "Left Hammer")
            //{
            //    transitionDuration = 0.2f;
            //}
            if (_animName == "Left Hammer" || _animName == "Left Greatsword")
            {
                playerController.animator.SetLayerWeight(playerController.animator.GetLayerIndex("Arm"), 0.0f);
                playerController.animator.SetLayerWeight(playerController.animator.GetLayerIndex("StandArm"), 1.0f);

                playerController.playerMovement.QuickSetAttackMoveSpeedLerp(1.0f);
            }
            playerController.animator.CrossFade(_animName, transitionDuration, playerController.animator.GetLayerIndex("Arm"));

            if (_animName[0] == 'L')
            {
                playerController.animator.SetBool("UsingLeft", true);
                playerController.m_lastAttackHand = Hand.LEFT;
            }
            if (_animName[0] == 'R')
            {
                playerController.animator.SetBool("UsingRight", true);
                playerController.m_lastAttackHand = Hand.RIGHT;
            }
        }
    }

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
