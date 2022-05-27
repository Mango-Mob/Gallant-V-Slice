using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent : SceneEvent
{
    [Header("Dialogue")]
    public string m_eventDialog = "While adventuring, you spot enemies ahead! You have a choice...";
    public string m_probText = "movement speed";
    public string m_successText = "You sneak around them!";
    public string m_failureText = "The enemies have spotted you!";

    [Header("Button Text")]
    public string m_confirmText = "Fight";
    public string m_declineText = "Sneak Past";

    public SceneData[] m_combatScenarios;
    public ItemEffect m_probVariable;
    public AnimationCurve m_probCurve;

    private int m_prob;
    protected override void Start()
    {
        base.Start();

        DialogManager.Instance.SetCharacter(null);
        m_prob = Mathf.FloorToInt(m_probCurve.Evaluate(GameManager.Instance.m_player.GetComponent<Player_Controller>().playerStats.GetEffectQuantity(m_probVariable)));
        DialogManager.Instance.SetDialogText(m_eventDialog + $"({m_prob}% chance based on {m_probText}).");

        DialogManager.Instance.SetButtonOption(0, m_confirmText, Fight);
        DialogManager.Instance.SetButtonOption(1, m_declineText, Sneak);
        DialogManager.Instance.SetButtonOption(2, "", null);
        DialogManager.Instance.SetButtonOption(3, "", null);
    }

    public void Fight()
    {
        //Start immediately
        int select = Random.Range(0, m_combatScenarios.Length);
        Instantiate(m_combatScenarios[select].prefabToLoad, Vector3.zero, Quaternion.identity);

        if(m_combatScenarios[select].prefabPropsToLoad.Count > 0)
        {
            int selectProps = Random.Range(0, m_combatScenarios[select].prefabPropsToLoad.Count);
            Instantiate(m_combatScenarios[select].prefabPropsToLoad[selectProps], Vector3.zero, Quaternion.identity);
        }
        DialogManager.Instance.Hide();

        Destroy(gameObject);
    }

    public void Sneak()
    {
        if(Random.Range(0, 10000) < 10000 * Mathf.Clamp(m_prob/100f, 0f, 1f))
        {
            DialogManager.Instance.SetDialogText(m_successText);
            DialogManager.Instance.SetButtonOption(0, "Continue", Leave);
            DialogManager.Instance.SetButtonOption(1, "", null);
        }
        else
        {
            DialogManager.Instance.SetDialogText(m_failureText);
            DialogManager.Instance.SetButtonOption(0, "Continue", Fight);
            DialogManager.Instance.SetButtonOption(1, "", null);
        }
    }

    public void Leave()
    {
        EndEvent();
    }
}