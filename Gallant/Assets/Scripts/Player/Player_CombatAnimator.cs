﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_CombatAnimator : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
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
            playerController.animator.CrossFade(_animName, 0.1f);

            if (_animName[0] == 'L')
                playerController.animator.SetBool("UsingLeft", true);
            if (_animName[0] == 'R')
                playerController.animator.SetBool("UsingRight", true);
        }
    }
}
