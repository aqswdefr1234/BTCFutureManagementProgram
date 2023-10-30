using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;

public class IndicatorDetect : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private Transform notificationPanel;
    void Start()
    {
        StartCoroutine(BollingerBandDetect(1f));
        StartCoroutine(rsiDetect(1f));
        StartCoroutine(GoldenDeadCross5_20Detect(1f));
        StartCoroutine(GoldenDeadCross50_200Detect(10f));
    }
    IEnumerator BollingerBandDetect(float delayTime)
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);
        float up = 0f;
        float down = 0f;
        while (true)
        {
            foreach (TMP_Text upText in BollingerBand.bollingerUpperText)
            {
                up = float.Parse(upText.text);
                if (BinanceData.currentHigh > up)
                {
                    CreateNotification("BollingerBandUpper", upText.transform.parent.name, 100);
                }
            }
            foreach (TMP_Text downText in BollingerBand.bollingerLowerText)
            {
                down = float.Parse(downText.text);
                if (BinanceData.currentLow < down)
                {
                    CreateNotification("BollingerBandLower", downText.transform.parent.name, 100);
                }
            }
            yield return delay;
        }
    }
    IEnumerator rsiDetect(float delayTime)
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);
        float rsi = 0f;
        while (true)
        {
            foreach (TMP_Text rsiText in RSI.rsi14Text)
            {
                rsi = float.Parse(rsiText.text);
                if (69.9f < rsi)
                {
                    CreateNotification("RSIUpper", rsiText.transform.parent.name, 100);
                }
                else if(30.1f > rsi)
                    CreateNotification("RSILower", rsiText.transform.parent.name, 100);
            }
            
            yield return delay;
        }
    }
    IEnumerator GoldenDeadCross5_20Detect(float delayTime)
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);
        while (true)
        {
            foreach (TMP_Text goldenText in GoldenCross.goldenCross5_20Text)
            {
                if (goldenText.text == "True")
                {
                    CreateNotification("GoldenCross5_20", goldenText.transform.parent.name, 100);
                }
            }
            foreach (TMP_Text deadText in GoldenCross.deadCross5_20Text)
            {
                if (deadText.text == "True")
                {
                    CreateNotification("DeadCross5_20", deadText.transform.parent.name, 100);
                }
            }
            yield return delay;
        }
    }
    IEnumerator GoldenDeadCross50_200Detect(float delayTime)
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);
        while (true)
        {
            foreach (TMP_Text goldenText in GoldenCross.goldenCross50_200Text)
            {
                if (goldenText.text == "True")
                {
                    CreateNotification("GoldenCross50_200", goldenText.transform.parent.name, 100);
                }
            }
            foreach (TMP_Text deadText in GoldenCross.deadCross50_200Text)
            {
                if (deadText.text == "True")
                {
                    CreateNotification("DeadCross50_200", deadText.transform.parent.name, 100);
                }
            }
            yield return delay;
        }
    }
    private void CreateNotification(string indicator, string timeFrame, int baseTime)
    {
        bool isExists = false;
        foreach(Transform obj in notificationPanel)
        {
            if(obj.GetChild(0).name == $"{indicator}_{timeFrame}")
                isExists = true;
        }
        if (isExists == false)
            Instantiate(notificationPrefab, notificationPanel).GetComponent<NotificationObjectScripts>().StartNotification(indicator, timeFrame, baseTime);
    }
}
