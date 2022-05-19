using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Indicator : Actor_Component
    {
        public override void SetEnabled(bool status)
        {
            this.gameObject.SetActive(status);
        }
    }
}
