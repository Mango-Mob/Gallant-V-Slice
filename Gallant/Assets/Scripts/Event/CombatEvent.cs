using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEvent : SceneEvent
{
    public SceneData[] m_combatScenarios;

    protected override void Start()
    {
        base.Start();

        DialogManager.Instance.SetCharacter(null);

        DialogManager.Instance.SetDialogText("While adventuring, you spot enemies ahead! You have a choice...");

        DialogManager.Instance.SetButtonOption(0, "Fight", Fight);
        DialogManager.Instance.SetButtonOption(1, "Sneak Past", Sneak);
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
        if()
        {

        }
        DialogManager.Instance.Hide();

        Destroy(gameObject);
    }

    public void Sneak()
    {
        if(Random.Range(0, 10000) < 10000 * 0.5f)
        {
            DialogManager.Instance.SetDialogText("You sneak around them!");
            DialogManager.Instance.SetButtonOption(0, "Continue", Leave);
            DialogManager.Instance.SetButtonOption(1, "", null);
        }
        else
        {
            DialogManager.Instance.SetDialogText("The enemies have spotted you!");
            DialogManager.Instance.SetButtonOption(0, "Continue", Fight);
            DialogManager.Instance.SetButtonOption(1, "", null);
        }
    }

    public void Leave()
    {
        EndEvent();
    }
}