using ActorSystem.AI.Bosses;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI.Other
{
    public class CastleWallController : MonoBehaviour
    {

        public void SetEnabledStatus(bool status)
        {
            GetComponent<NavMeshObstacle>().carving = status;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                collision.collider.gameObject.GetComponent<Boss_Castle>().HitWall();
            }
        }
    }
}
