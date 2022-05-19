using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DamageDisplay : UI_Element
{
    public GameObject phyDamagePrefab;
    public GameObject magDamagePrefab;
    public GameObject trueDamagePrefab;

    public float noise;

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        return;
    }

    public override void OnMouseUpEvent()
    {
        return;
    }

    public void DisplayDamage(Transform source, CombatSystem.DamageType dType, float damage)
    {
        if (damage < 0.5)
            return;

        GameObject objInWorld = null;
        
        switch (dType)
        {
            case CombatSystem.DamageType.Physical:
                objInWorld = Instantiate(phyDamagePrefab, transform);
                break;
            case CombatSystem.DamageType.Ability:
                objInWorld = Instantiate(magDamagePrefab, transform);
                break;
            case CombatSystem.DamageType.True:
                objInWorld = Instantiate(trueDamagePrefab, transform);
                break;
            default:
                break;
        }

        if(objInWorld != null)
        {
            objInWorld.SetActive(true);
            objInWorld.GetComponentInChildren<WorldToCanvas>().m_anchorTransform = source;
            objInWorld.GetComponentInChildren<WorldToCanvas>().m_offset = Random.insideUnitSphere * noise;
            objInWorld.GetComponentInChildren<TMP_Text>().SetText(Mathf.RoundToInt(damage).ToString());
        }
    }
}
