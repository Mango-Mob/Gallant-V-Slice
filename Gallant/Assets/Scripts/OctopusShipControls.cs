using UnityEngine;

public class OctopusShipControls : MonoBehaviour
{
    private Animator m_animator;

    public void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Slam(bool isFront)
    {
        m_animator.Play((isFront) ? "SlamFront" : "SlamBack");
    }
}
