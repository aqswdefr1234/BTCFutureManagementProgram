using System;
using System.Collections;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine;
using TMPro;
public class RSI : MonoBehaviour
{
    [SerializeField] private Transform rsiPanel;
    private List<float> closing14 = new List<float>();//14��
    private List<float> gains = new List<float>();
    private List<float> losses = new List<float>();
    private string[] intervalToUse;
    private List<float> rsi = new List<float>();

    public static List<TMP_Text> rsi14Text = new List<TMP_Text>();

    void Start()
    {
        int count = rsiPanel.childCount;
        intervalToUse = new string[count];
        if (count == 0)
        {
            Debug.Log("interval�� ��� �ֽ��ϴ�.");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            intervalToUse[i] = rsiPanel.GetChild(i).name;
            rsi14Text.Add(rsiPanel.GetChild(i).GetChild(1).GetComponent<TMP_Text>());
        }
        StartCoroutine(RSICalculation(intervalToUse));
    }
    IEnumerator RSICalculation(string[] intervals)
    {
        WaitForSeconds delay = new WaitForSeconds(5f);
        while (true)
        {
            rsi.Clear();
            Calculation(intervals);
            UI_InsertValue(rsi14Text);
            yield return delay;
        }
    }
    private void UI_InsertValue(List<TMP_Text> panelText)
    {
        for (int i = 0; i < rsi.Count; i++)
        {
            panelText[i].text = rsi[i].ToString();
        }
    }
    private void Calculation(string[] intervals)
    {
        foreach (string interval in intervals)
        {
            RequiredDataSetting(interval, 200, closing14);
            CurrentRSI(gains, losses, closing14, 14, 200);
        }
    }
    private void RequiredDataSetting(string _interval, int period, List<float> closingList)
    {
        closingList.Clear();
        float price = 0f;

        if (_interval == "1m")
        {
            for (int i = BinanceData.closingPrices_1m.Count - period; i < BinanceData.closingPrices_1m.Count; i++)
            {
                price = BinanceData.closingPrices_1m[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "3m")
        {
            for (int i = BinanceData.closingPrices_3m.Count - period; i < BinanceData.closingPrices_3m.Count; i++)
            {
                price = BinanceData.closingPrices_3m[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "5m")
        {
            for (int i = BinanceData.closingPrices_5m.Count - period; i < BinanceData.closingPrices_5m.Count; i++)
            {
                price = BinanceData.closingPrices_5m[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "15m")
        {
            for (int i = BinanceData.closingPrices_15m.Count - period; i < BinanceData.closingPrices_15m.Count; i++)
            {
                price = BinanceData.closingPrices_15m[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "1h")
        {
            for (int i = BinanceData.closingPrices_1h.Count - period; i < BinanceData.closingPrices_1h.Count; i++)
            {
                price = BinanceData.closingPrices_1h[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "4h")
        {
            for (int i = BinanceData.closingPrices_4h.Count - period; i < BinanceData.closingPrices_4h.Count; i++)
            {
                price = BinanceData.closingPrices_4h[i];
                closingList.Add(price);
            }
        }
        else if (_interval == "1d")
        {
            for (int i = BinanceData.closingPrices_1d.Count - period; i < BinanceData.closingPrices_1d.Count; i++)
            {
                price = BinanceData.closingPrices_1d[i];
                closingList.Add(price);
            }
        }
    }
    private void CurrentRSI(List<float> gain, List<float> loss, List<float> closingList, int period, int limit)
    {
        gain.Clear();
        loss.Clear();

        for (int i = 1; i < closingList.Count; i++)//14���� ����Ʈ�� �ִٸ� 13�� �ݺ�
        {
            float delta = closingList[i] - closingList[i - 1];
            if (delta > 0)
            {
                gain.Add(delta);
                loss.Add(0);
            }
            else
            {
                gain.Add(0);
                loss.Add(-delta);
            }
        }

        float AU = CalculateEWMA(gain, period);
        float AD = CalculateEWMA(loss, period);

        float rs = AU / AD;

        rsi.Add(100 - (100 / (1 + rs)));
    }
    
    float CalculateEWMA(List<float> data, int period)
    {
        float alpha = 1f / period;
        float ewm = data[0];

        for (int i = 1; i < data.Count; i++)
        {
            ewm = alpha * data[i] + (1 - alpha) * ewm;
        }

        return ewm;
    }
}
/*
 ������ ���� ���ݺ��� ����� ���� ��º��� U(up) ���̶�� �ϰ�,
������ ���� ���ݺ��� �϶��� ���� �϶����� D(down) ���̶�� �Ѵ�.
U���� D���� ��հ��� ���Ͽ� �װ��� ���� AU(average ups)�� AD(average downs)�� �Ѵ�.
AU�� AD������ ���� ���� RS(relative strength) ���̶�� �Ѵ�. 

RS ���� ũ�ٴ� ���� ���� �Ⱓ �϶��� ������ ����� ���� ũ�ٴ� ���� �ǹ��Ѵ�.

 

- ���� ��꿡 ���Ͽ� RSI ���� ���Ѵ�.

RSI ��� ���� :
RSI = RS / (1 + RS)
�Ǵ�, ������ ���� ���ص� ����� �����ϴ�.
RSI = AU / (AU + AD)
 */