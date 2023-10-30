using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationObjectScripts : MonoBehaviour
{
    private TMP_Text indicatorText;
    private TMP_Text timeText;

    public void StartNotification(string indicator, string timeFrame, int baseTime)
    {
        indicatorText = transform.GetChild(0).GetComponent<TMP_Text>();
        timeText = transform.GetChild(1).GetComponent<TMP_Text>();
        indicatorText.transform.name = $"{indicator}_{timeFrame}";
        indicatorText.text = $"{indicator}_{timeFrame}";
        FCM_Manager.instance.ReadRealTime(indicator, timeFrame);
        StartCoroutine(Timer(indicator, baseTime));
    }
    IEnumerator Timer(string indicator, int baseTime)
    {
        WaitForSeconds delay = new WaitForSeconds(1f);
        int timeLimit = baseTime;
        while (true)
        {
            yield return delay;
            timeLimit--;
            if(timeLimit > 0)
                timeText.text = Mathf.FloorToInt(timeLimit).ToString();
            else
            {
                timeText.text = 0.ToString();
                break;
            }
        }
        Destroy(gameObject);
    }
}
