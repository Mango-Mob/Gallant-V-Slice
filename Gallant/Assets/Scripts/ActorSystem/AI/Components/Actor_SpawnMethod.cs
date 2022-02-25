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
        public abstract void StartSpawn(Vector3 start, Vector3 end, Vector3 finalForward);

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
