using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Head : Actor_Component
    {
        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }

        public void SetLookDirection(Vector3 forward)
        {
            transform.forward = forward;
        }
    }
}

