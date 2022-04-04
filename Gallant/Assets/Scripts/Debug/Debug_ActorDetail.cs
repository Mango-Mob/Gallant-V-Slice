using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Exceed.Debug
{
    public class Debug_ActorDetail : MonoBehaviour
    {
        private Actor m_ref;

        [SerializeField] private Text m_level;
        [SerializeField] private Text m_type;
        [SerializeField] private Text m_state;
        [SerializeField] private Text m_target;

        [SerializeField] private Slider m_hpSlider;
        [SerializeField] private Slider m_phySlider;
        [SerializeField] private Slider m_abilSlider;
        [SerializeField] private Slider m_damageSlider;

        [SerializeField] private GameObject m_classDisplayPrefab;
        [SerializeField] private GameObject m_classDisplayParent;
        private List<GameObject> m_ActorComponents;

        public void Update()
        {
            if (m_ref != null)
            {
                m_level.text = m_ref.m_myLevel.ToString();
                m_state.text = m_ref.m_activeStateText;
                m_target.text = (m_ref.m_target != null) ? m_ref.m_target.name : "Null";

                m_hpSlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currHealth);
                m_phySlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currPhyResist);
                m_abilSlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currAbilResist);
                m_damageSlider.SetValueWithoutNotify((m_ref.m_myBrain.m_arms != null) ? m_ref.m_myBrain.m_arms.m_baseDamageMod : 0);
            }
        }

        public void SetReference(Actor _ref)
        {
            m_ref = _ref;

            m_hpSlider.maxValue = m_ref.m_myBrain.m_startHealth * 2f;
            m_hpSlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currHealth);

            m_phySlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currPhyResist);
            m_abilSlider.SetValueWithoutNotify(m_ref.m_myBrain.m_currAbilResist);

            m_damageSlider.maxValue = (m_ref.m_myBrain.m_arms != null) ? m_ref.m_myBrain.m_arms.m_baseDamageMod * 2f : 0;
            m_damageSlider.SetValueWithoutNotify((m_ref.m_myBrain.m_arms != null) ? m_ref.m_myBrain.m_arms.m_baseDamageMod : 0);
            
            if (m_ActorComponents != null && m_ActorComponents.Count > 0)
            {
                for (int i = 0; i < m_ActorComponents.Count; i++)
                {
                    Destroy(m_ActorComponents[i]);
                }
            }
            m_ActorComponents = new List<GameObject>();
            foreach (var comp in _ref.GetComponentsInChildren<Actor_Component>())
            {
                m_ActorComponents.Add(Instantiate(m_classDisplayPrefab, m_classDisplayParent.transform));
                m_ActorComponents[m_ActorComponents.Count - 1].GetComponent<UI_ScriptDisplay>().m_reference = comp;
                m_ActorComponents[m_ActorComponents.Count - 1].SetActive(true);
            }
            return;
        }

        public void Change(int slider)
        {
            if(m_ref != null)
            {
                switch (slider)
                {
                    default:
                    case 0: //Health
                        m_ref.m_myBrain.m_currHealth = m_hpSlider.value;
                        break;
                    case 1: //Phy
                        m_ref.m_myBrain.m_currPhyResist = m_phySlider.value;
                        break;
                    case 2: //Abil
                        m_ref.m_myBrain.m_currAbilResist = m_abilSlider.value;
                        break;
                    case 3: //Damage
                        if (m_ref.m_myBrain.m_arms != null)
                            m_ref.m_myBrain.m_arms.m_baseDamageMod = m_damageSlider.value;
                        break;
                }
            }
        }
    }
}
