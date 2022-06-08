using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Head_SpawnMethod : Actor_SpawnMethod
    {
        public float SpawnSpeed = 2f;
        private Actor m_myActor;
        private GameObject m_myHead;
        private float dt = 0;
        private bool m_hasResentlySpawnned = false;

        private Vector3 localStartPosition;
        private Quaternion localStartRotation;
        public void Start()
        {
            m_myActor = GetComponent<Actor>();
            m_myHead = GetComponentInChildren<Renderer>().gameObject;
        }

        public override void StartSpawn(Vector3 spawnLoc, Quaternion rotation)
        {
            dt = 0;
            Actor_Brain myBrain = null;
            if (m_myActor != null)
                myBrain = m_myActor.m_myBrain;
            else
            {
                myBrain = GetComponent<Actor_Brain>();
                m_myActor = GetComponent<Actor>();
                m_myHead = GetComponentInChildren<Renderer>().gameObject;
            }

            myBrain.SetEnabled(false);
            myBrain.GetComponent<Rigidbody>().isKinematic = true;
            myBrain.transform.position = spawnLoc;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(spawnLoc, out hit, 3, ~0))
            {
                Vector3 diff = spawnLoc - hit.position;
                transform.position = hit.position;
                m_myHead.transform.localPosition = diff;
            }
            m_myHead.transform.rotation = rotation;

            localStartPosition = m_myHead.transform.localPosition;
            localStartRotation = m_myHead.transform.localRotation;
            m_hasResentlySpawnned = true;
        }

        protected override void Update()
        {
            base.Update();
            if(m_hasResentlySpawnned)
            {
                m_myHead.transform.localPosition = Vector3.Lerp(localStartPosition, Vector3.up, dt);
                m_myHead.transform.localRotation = Quaternion.Slerp(localStartRotation, Quaternion.identity, dt);

                dt += SpawnSpeed * Time.deltaTime;

                if(dt > 1)
                {
                    m_myHead.transform.localPosition = Vector3.up;
                    m_myHead.transform.localRotation = Quaternion.identity;
                    StopSpawning();
                }
            }
        }

        public override void Respawn()
        {
            m_myActor.DestroySelf();
        }

        public override void StopSpawning()
        {
            m_hasResentlySpawnned = false;
            m_spawnning = false;
            m_myActor.m_myBrain.SetEnabled(true);
            m_myActor.m_myBrain.m_legs.SetTargetLocation(transform.position);
            m_myActor.m_myBrain.m_legs.SetTargetRotation(transform.rotation);
            m_myActor.SetTarget(GameManager.Instance.m_player);
            GetComponent<Collider>().enabled = true;
        }
    }

}

