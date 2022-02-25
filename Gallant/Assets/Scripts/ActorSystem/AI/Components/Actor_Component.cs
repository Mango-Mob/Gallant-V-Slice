using UnityEngine;

namespace ActorSystem.AI.Components
{
    public abstract class Actor_Component : MonoBehaviour
    {
        public abstract void SetEnabled(bool status);
    }
}
