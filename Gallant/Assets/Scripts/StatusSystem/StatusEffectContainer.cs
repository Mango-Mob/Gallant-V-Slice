
using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectContainer : MonoBehaviour
{
    public GameObject m_statusPrefab;

    public Transform m_headLoc;

    public bool IsImmuneToFrost = false;
    public bool IsImmuneToFlame = false;
    public bool IsImmuneToStun = false;
    public bool IsImmuneToWeaken = false;

    private Actor m_actor = null;
    private Player_Controller m_player = null;

    private struct StatusDisplay
    {
        public StatusDisplay(StatusEffect _effect, UI_StatusEffectBar _display) { effect = _effect; element = _display; }

        public StatusEffect effect;
        public UI_StatusEffectBar element;
    }

    private List<StatusDisplay> m_currentEffects = new List<StatusDisplay>();

    // Start is called before the first frame update
    void Start()
    {
        m_actor = this.GetComponent<Actor>();
        m_player = this.GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        //Loop through all effects and update them.
        foreach (var current in m_currentEffects)
        {
            if (m_actor != null)
                current.effect.UpdateOnActor(m_actor, Time.deltaTime);
            else
                current.effect.UpdateOnPlayer(m_player, Time.deltaTime);

            if (current.element)
                current.element.SetValue(current.effect.m_startDuration, current.effect.m_duration);
        }
    }

    //LateUpdate is called at the end of each frame
    private void LateUpdate()
    {
        for (int i = m_currentEffects.Count - 1; i >= 0; i--)
        {
            if(m_currentEffects[i].effect.HasExpired)
            {
                RemoveEffect(m_currentEffects[i]);
            }
        }
    }

    /*******************
     * AddStatusEffect : Checks all existing effects for a reaction, if none are effected then it is added to the list.
     * @author : Michael Jordan
     * @param : (StatusEffect) Effect to add to the list.
     */
    public void AddStatusEffect(StatusEffect effect)
    {
        if (IsImmuneToFlame && effect is BurnStatus)
            return;
        if (IsImmuneToFrost && effect is SlowStatus)
            return;
        if (IsImmuneToWeaken && effect is WeakenStatus)
            return;
        if (IsImmuneToStun && effect is StunStatus)
            return;

        foreach (var current in m_currentEffects)
        {
            if (current.effect.ReactTo(effect))
            {
                //Reaction occured, ignore this effect.
                return;
            }
        }

        UI_StatusEffectBar bar = null;
        if (m_actor != null)
        {
            if(m_actor.m_myBrain.m_ui != null)
            {
                bar = m_actor.m_myBrain.m_ui.GetElement<UI_List>("StatusList")?.Instantiate(m_statusPrefab) as UI_StatusEffectBar;
                bar.SetImage(effect.m_displayImage);
            }
            effect.StartActor(m_actor, m_headLoc);
        }
        else
        {
            //Add with player context
            effect.StartPlayer(m_player);
        }
        m_currentEffects.Add(new StatusDisplay(effect, bar));
    }

    private void RemoveEffect(StatusDisplay display)
    {
        m_currentEffects.Remove(display);
        if (m_actor != null)
        {
            if (m_actor.m_myBrain.m_ui != null)
            {
                m_actor.m_myBrain.m_ui.GetElement<UI_List>()?.RemoveElement(display.element);
                Destroy(display.element.gameObject);
            }
            //Remove vfx
            if(display.effect.m_vfxInWorld != null)
            {
                Destroy(display.effect.m_vfxInWorld);
            }
            display.effect.EndActor(m_actor);
        }
        else
        {
            //Remove vfx
            if (display.effect.m_vfxInWorld != null)
            {
                Destroy(display.effect.m_vfxInWorld);
            }
            display.effect.EndPlayer(m_player);
        }
    }
}
