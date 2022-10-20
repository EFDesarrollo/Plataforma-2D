using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI powerUpsText;

    public void SetTimeText(int time)
    {
        int minute = time / 60;
        int seconds = time % 60;
        timeText.text = minute.ToString("00") + ":" + seconds.ToString("00");
    }
    public void SetLives(int lives)
    {
        livesText.text = "Lives: " + lives.ToString();
    }
    public void SetPowerUps(int powerUps)
    {
        powerUpsText.text = "PowerUps: " + powerUps.ToString();
    }
}
