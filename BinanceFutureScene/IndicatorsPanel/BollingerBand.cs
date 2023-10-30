using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;

public class BollingerBand : MonoBehaviour
{
    [SerializeField] private Transform bollingerPanel;
    private List<float> closing20 = new List<float>();//20개
    public static List<TMP_Text> bollingerUpperText = new List<TMP_Text>();
    public static List<TMP_Text> bollingerLowerText = new List<TMP_Text>();
    //private string[] intervalToUse = { "15m", "5m", "1h", "4h", "1d" };//시간 작은 순 부터 작성.ui오브젝트도 시간 작은 순으로 위에서 아래로 배치해야함.
    //List<TMP_Text>에 순서대로 넣어야 안꼬이기 때문임.
    private string[] intervalToUse;
    public static List<Vector2> bollinger = new List<Vector2>();

    void Start()
    {
        int count = bollingerPanel.childCount;
        intervalToUse = new string[count];

        if(count == 0)
        {
            Debug.Log("interval이 비어 있습니다.");
            return;
        }
        for(int i = 0; i < count; i++)
        {
            intervalToUse[i] = bollingerPanel.GetChild(i).name;
            bollingerUpperText.Add(bollingerPanel.GetChild(i).GetChild(1).GetComponent<TMP_Text>());
            bollingerLowerText.Add(bollingerPanel.GetChild(i).GetChild(2).GetComponent<TMP_Text>());
        }
        
        StartCoroutine(BollingerBandCalculation(intervalToUse));
    }
    IEnumerator BollingerBandCalculation(string[] intervals)
    {
        WaitForSeconds delay = new WaitForSeconds(5f);
        while (true)
        {
            bollinger.Clear();
            Calculation(intervals);
            UI_InsertValue();
            yield return delay;
        }
    }
    private void UI_InsertValue()
    {
        for(int i = 0; i < bollinger.Count; i++)
        {
            bollingerUpperText[i].text = bollinger[i].x.ToString();
            bollingerLowerText[i].text = bollinger[i].y.ToString();
        }
    }
    private void Calculation(string[] intervals)
    {
        foreach (string interval in intervals)
        {
            closing20.Clear();
            float ma = MaCalculation(interval, 20);
            bollinger.Add(UpperLowerBand(closing20, ma, interval));
        }
    }
    private float MaCalculation(string _interval, int baseLength)//항상 20일 선 기준으로 계산한다.
    {
        float price = 0f;
        float sum = 0f;
        float ma = 0f;
        if (_interval == "1m")
        {
            for (int i = BinanceData.closingPrices_1m.Count - baseLength; i < BinanceData.closingPrices_1m.Count; i++)
            {
                price = BinanceData.closingPrices_1m[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "3m")
        {
            for (int i = BinanceData.closingPrices_3m.Count - baseLength; i < BinanceData.closingPrices_3m.Count; i++)
            {
                price = BinanceData.closingPrices_3m[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "5m")
        {
            for (int i = BinanceData.closingPrices_5m.Count - baseLength; i < BinanceData.closingPrices_5m.Count; i++)
            {
                price = BinanceData.closingPrices_5m[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "15m")
        {
            for (int i = BinanceData.closingPrices_15m.Count - baseLength; i < BinanceData.closingPrices_15m.Count; i++)
            {
                price = BinanceData.closingPrices_15m[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "1h")
        {
            for (int i = BinanceData.closingPrices_1h.Count - baseLength; i < BinanceData.closingPrices_1h.Count; i++)
            {
                price = BinanceData.closingPrices_1h[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "4h")
        {
            for (int i = BinanceData.closingPrices_4h.Count - baseLength; i < BinanceData.closingPrices_4h.Count; i++)
            {
                price = BinanceData.closingPrices_4h[i];
                closing20.Add(price);
                sum += price;
            }
        }
        else if (_interval == "1d")
        {
            for (int i = BinanceData.closingPrices_1d.Count - baseLength; i < BinanceData.closingPrices_1d.Count; i++)
            {
                price = BinanceData.closingPrices_1d[i];
                closing20.Add(price);
                sum += price;
            }
        }
        ma = Mathf.Round(sum * 100.0f) / 20 / 100.0f;
        return ma;
    }
    private Vector2 UpperLowerBand(List<float> closinglist, float ma, string _interval)
    {
        float standardDeviationMultiplier = 2.0f;
        float sumOfSquares = 0;

        for (int i = 0; i < 20; i++)
        {
            
            float diff = closinglist[i] - ma;
            sumOfSquares += Mathf.Pow(diff, 2);
        }
        float standardDeviation = Mathf.Sqrt(sumOfSquares / 20f);
        // 상단 및 하단 밴드 계산
        float upperBand = ma + (standardDeviation * standardDeviationMultiplier);
        float lowerBand = ma - (standardDeviation * standardDeviationMultiplier);
        Vector2 bollingerBand = new Vector2(Mathf.Round(upperBand * 10f) / 10f, Mathf.Round(lowerBand * 10f) / 10f);
        return bollingerBand;
    }
}
