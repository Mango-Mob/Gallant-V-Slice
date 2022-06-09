using ActorSystem.AI.Bosses;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI.Other
{
    public class CastleWallController : MonoBehaviour
    {

        public void SetEnabledStatus(bool status)
        {
            GetComponent<NavMeshObstacle>().enabled = status;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                other.gameObject.GetComponent<Boss_Castle>()?.HitWall();
            }
        }
    }
}
