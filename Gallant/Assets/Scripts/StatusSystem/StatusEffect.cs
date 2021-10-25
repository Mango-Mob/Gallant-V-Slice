using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * StatusEffect : A parent class template for any future creations of status effects.
 * @author : Michael Jordan
 * @file : StatusEffect.cs
 * @year : 2021
 */
public abstract class StatusEffect
{
    //Display image for UI
    public Sprite m_displayImage { get; private set; }

    //Display vfx for world
    public GameObject m_vfxDisplayPrefab { get; private set; }

    //Strength which is abstractly defined by each status effect.
    public float m_strength;

    //Duration in seconds
    public float m_duration;

    //Duration used for UI display
    public float m_startDuration { get; protected set; }

    //Duration initialised as a negative value are perceived as forever.
    public bool m_lastsForever = false;

    //Compute variable of if this status effect has expired.
    public bool HasExpired { get { return m_duration <= 0 && !m_lastsForever; } }

    public Color m_displayColor { get; protected set; }

    public StatusEffect(float str, float dur)
    {
        m_strength = str;
        m_duration = dur;
        m_startDuration = dur;
        m_lastsForever = dur < 0;
        
        LoadDisplayImage();
        LoadDisplayVFX();
    }

    public abstract void StartActor(Actor _actor);
    public abstract void StartPlayer(Player_Controller _player);

    public abstract void UpdateOnActor(Actor _actor, float dt);
    public abstract void UpdateOnPlayer(Player_Controller _player, float dt);

    public abstract void EndActor(Actor _actor);
    public abstract void EndPlayer(Player_Controller _player);

    public abstract bool ReactTo(StatusEffect other);

    protected abstract void LoadDisplayImage();
    protected abstract void LoadDisplayVFX();
}
