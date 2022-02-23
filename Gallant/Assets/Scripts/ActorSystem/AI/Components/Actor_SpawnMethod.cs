using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public abstract class Actor_SpawnMethod : MonoBehaviour
    {
        public abstract void StartSpawn(Vector3 start, Vector3 end, Vector3 finalForward);

        protected virtual void Update() { }

        protected void EndSpawn()
        {
            GetComponent<Actor_Brain>().enabled = true;
        }
    }
}
