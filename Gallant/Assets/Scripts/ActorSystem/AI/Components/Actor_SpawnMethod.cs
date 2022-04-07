using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public abstract class Actor_SpawnMethod : Actor_Component
    {
        public bool m_spawnning = false;

        public abstract void StartSpawn(Vector3 spawnLoc);
        public abstract void StopSpawning();
        protected virtual void Update() { }

        protected void EndSpawn()
        {
            GetComponent<Actor_Brain>().SetEnabled(true);
        }
        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }
    }
}
