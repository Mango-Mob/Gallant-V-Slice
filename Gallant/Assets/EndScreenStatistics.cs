using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenStatistics : MonoBehaviour
{
    public static float elapsedTimeInSeconds = 0;
    public static int roomsCleared = 0;
    public static float damageDealt = 0;
    public static int levelReached = 1;

    public TextMeshProUGUI m_elapsedTimeText;
    public TextMeshProUGUI m_levelReachedText;
    public TextMeshProUGUI m_roomsClearedText;
    public TextMeshProUGUI m_damageDealtText;

    // Start is called before the first frame update
    void Start()
    {
        m_elapsedTimeText.text = CaluclateTime();
        m_levelReachedText.text = (levelReached).ToString();
        m_roomsClearedText.text = (roomsCleared).ToString();
        m_damageDealtText.text = CalculateDamage();
    }

    private string CalculateDamage()
    {
        int multiples = 0;
        float tempVal = damageDealt;

        while(tempVal > 1000f)
        {
            tempVal /= 1000f;
            multiples++;
        }

        switch (multiples)
        {
            default:
            case 0:
                return $"{damageDealt.ToString()}";
            case 1:
                return $"{damageDealt.ToString()}k";
            case 2:
                return $"{damageDealt.ToString()}M";
            case 3:
                return $"{damageDealt.ToString()}B";
        }
    }

    private string CaluclateTime()
    {
        float minutes;
        float hours;

        if (elapsedTimeInSeconds > 60)
        {
            if(elapsedTimeInSeconds > 60 * 60)
            {
                hours = elapsedTimeInSeconds / (60 * 60);
                return $"{hours.ToString().Substring(0, 4)} hours";
            }
            minutes = elapsedTimeInSeconds / 60;
            return $"{minutes.ToString().Substring(0, 4)} minutes";
        }
        return $"{elapsedTimeInSeconds.ToString().Substring(0, 4)} seconds";
    }

    public static void Restart()
    {
        elapsedTimeInSeconds = 0;
        roomsCleared = 0;
        damageDealt = 0;
        levelReached = 1;
    }
}
