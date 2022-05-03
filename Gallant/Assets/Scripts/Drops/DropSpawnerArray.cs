using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawnerArray : MonoBehaviour
{
    [SerializeField] private GameObject spawnerPrefab;

    [SerializeField] private int m_weaponLevel = 1;
    [Range(1, 3)] [SerializeField] private int m_abilityPowerLevel = 1;
    [SerializeField] private float m_spacing = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        CreateSpawners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateSpawners()
    {
        int row = 0;
        int col = 0;


        foreach (Weapon weapon in System.Enum.GetValues(typeof(Weapon)))
        {
            foreach (Ability ability in System.Enum.GetValues(typeof(Ability)))
            {
                GameObject gameObject = Instantiate(spawnerPrefab, transform.position + new Vector3(row * m_spacing, 0.0f, col * m_spacing), Quaternion.identity);
                gameObject.GetComponent<DropSpawner>().Configure(m_weaponLevel, weapon, ability, m_abilityPowerLevel);
                row++;
            }
            row = 0;
            col++;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        int width = System.Enum.GetValues(typeof(Weapon)).Length - 1;
        int height = System.Enum.GetValues(typeof(Ability)).Length - 1;
        Vector3 size = new Vector3(height, 0, width) * m_spacing;

        Gizmos.DrawWireCube(transform.position + size * 0.5f, size);
    }
}
