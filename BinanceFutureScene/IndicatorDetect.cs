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
        StartCoroutine(GoldenDeadCross5_20Detect(10f));
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
                    CreateNotification("BollingerBandUpper", upText.transform.parent.name);
                }
            }
            foreach (TMP_Text downText in BollingerBand.bollingerLowerText)
            {
                down = float.Parse(downText.text);
                if (BinanceData.currentLow < down)
                {
                    CreateNotification("BollingerBandLower", downText.transform.parent.name);
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
                    CreateNotification("RSIUpper", rsiText.transform.parent.name);
                }
                else if(30.1f > rsi)
                    CreateNotification("RSILower", rsiText.transform.parent.name);
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
                    CreateNotification("GoldenCross5_20", goldenText.transform.parent.name);
                }
            }
            foreach (TMP_Text deadText in GoldenCross.deadCross5_20Text)
            {
                if (deadText.text == "True")
                {
                    CreateNotification("DeadCross5_20", deadText.transform.parent.name);
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
                    CreateNotification("GoldenCross50_200", goldenText.transform.parent.name);
                }
            }
            foreach (TMP_Text deadText in GoldenCross.deadCross50_200Text)
            {
                if (deadText.text == "True")
                {
                    CreateNotification("DeadCross50_200", deadText.transform.parent.name);
                }
            }
            yield return delay;
        }
    }
    private void CreateNotification(string indicator, string timeFrame)
    {
        bool isExists = false;
        foreach(Transform obj in notificationPanel)
        {
            if(obj.GetChild(0).name == $"{indicator}_{timeFrame}")
                isExists = true;
        }
        if (isExists == false) 
        {
            Instantiate(notificationPrefab, notificationPanel).GetComponent<NotificationObjectScripts>().StartNotification(indicator, timeFrame, BaseTimeCalculation(indicator, timeFrame));
        }
    }
    private int BaseTimeCalculation(string indicatorType, string timeFrame)
    {
        if (indicatorType == "GoldenCross5_20" || indicatorType == "GoldenCross50_200" || indicatorType == "DeadCross5_20" || indicatorType == "DeadCross50_200")//다음 캔들까지는 상태가 변할일이 없으니 타임 프레임 만큼 타이머를 준다.
        {
            if (timeFrame == "1m")
                return 61;
            else if (timeFrame == "3m")
                return 181;
            else if (timeFrame == "5m")
                return 301;
            else if (timeFrame == "15m")
                return 901;
            else if (timeFrame == "1h")
                return 3601;
            else if (timeFrame == "4h")
                return 14401;
            else if (timeFrame == "1d")
                return 86401;
            else
                return 1000;
        }
        else if (indicatorType == "BollingerBandUpper" || indicatorType == "BollingerBandLower")
        {
            if (timeFrame == "1m")
                return 120;
            else if (timeFrame == "3m")
                return 360;
            else if (timeFrame == "5m")
                return 600;
            else if (timeFrame == "15m")
                return 1800;
            else if (timeFrame == "1h")
                return 3600;
            else if (timeFrame == "4h")
                return 7200;
            else if (timeFrame == "1d")
                return 7200;
            else
                return 1000;
        }
        else if (indicatorType == "RSIUpper" || indicatorType == "RSILower")
        {
            if (timeFrame == "1m")
                return 120;
            else if (timeFrame == "3m")
                return 360;
            else if (timeFrame == "5m")
                return 600;
            else if (timeFrame == "15m")
                return 1800;
            else if (timeFrame == "1h")
                return 3600;
            else if (timeFrame == "4h")
                return 7200;
            else if (timeFrame == "1d")
                return 14400;
            else
                return 1000;
        }
        else
            return 1000;
    }
}
