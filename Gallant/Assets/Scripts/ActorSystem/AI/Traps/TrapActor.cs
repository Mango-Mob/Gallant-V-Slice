using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class TrapActor : Actor
    {
        protected override void Awake()
        {
            m_myBrain = GetComponent<Actor_Brain>();

            if (m_toReserveOnLoad)
            {
                ActorManager.Instance.ReserveMe(this);
                m_myBrain.enabled = false;
            }
            else
            {
                ActorManager.Instance.Subscribe(this);
                m_myBrain.enabled = true;
            }

        }
        protected override void Start()
        {
            //Don't start statemachine
        }

        protected override void Update()
        {
            //Don't start statemachine
            m_myBrain.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            if((m_myBrain.m_arms.m_targetMask & (1 << other.gameObject.layer)) != 0)
            {
                m_myBrain.BeginAttack(0);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if ((m_myBrain.m_arms.m_targetMask & (1 << other.gameObject.layer)) != 0)
            {
                m_myBrain.BeginAttack(0);
            }
        }
    }
}