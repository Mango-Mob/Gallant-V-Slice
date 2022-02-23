using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    public class Boss_Actor : Actor
    {
        protected override void Awake()
        {
            m_myBrain = GetComponent<Actor_Brain>();
            m_mySpawn = GetComponent<Actor_SpawnMethod>();

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
    }
}

