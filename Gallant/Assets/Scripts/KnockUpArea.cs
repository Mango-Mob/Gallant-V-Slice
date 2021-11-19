using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockUpArea : MonoBehaviour
{
    public GameObject m_knockUpVFX;
    public float m_previewRadius = 1.0f;

    public void StartKnockUp(float radius, float damage, float delay)
    {
        m_previewRadius = radius;
        StartCoroutine(KnockUpInArea(radius, damage, delay));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_previewRadius);
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
                    player.StunPlayer(0.8f, Vector3.up * 20f);
                }
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            {
                //Damage shadow
                AdrenalineProvider provider = hit.GetComponent<AdrenalineProvider>();
                provider.GiveAdrenaline();
            }
        }
        Instantiate(m_knockUpVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield return null;
    }
}
