using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Actor_MiniMapIcon : Actor_Component
    {
        public Quaternion m_idealRotation;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = m_idealRotation;
        }
        public void OnEnable()
        {
            GetComponent<Renderer>().enabled = true;
        }

        public void OnDisable()
        {
            GetComponent<Renderer>().enabled = false;
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }
    }

}
