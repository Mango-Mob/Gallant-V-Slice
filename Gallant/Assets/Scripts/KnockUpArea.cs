using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockUpArea : MonoBehaviour
{
    private float m_rangeDisplay = 1.0f;
    public void StartKnockUp(float radius, float damage, float delay)
    {
        m_rangeDisplay = radius;
        StartCoroutine(KnockUpInArea(radius, damage, delay));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_rangeDisplay);
    }

    public IEnumerator KnockUpInArea(float radius, float damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hit.GetComponent<Player_Controller>();
                if (player != null)
                {
                    player.DamagePlayer(damage);
                    Vector3 newPos = player.transform.position;
                    newPos.y += 3;
                    player.transform.position = newPos;
                    player.StunPlayer(0.5f, Vector3.zero);
                }
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            {
                //Damage shadow
                AdrenalineProvider provider = hit.GetComponent<AdrenalineProvider>();
                provider.GiveAdrenaline();
            }
        }
        Destroy(gameObject);
        yield return null;
    }
}
