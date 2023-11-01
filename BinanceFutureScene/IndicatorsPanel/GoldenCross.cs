using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;
using System;

public class GoldenCross : MonoBehaviour
{   //5-20
    [SerializeField] private Transform goldenDeadCrossPanel5_20;
    private List<float> closing5_20 = new List<float>();
    private string[] intervalToUse5_20;
    private List<bool> goldenCross5_20 = new List<bool>();
    private List<bool> deadCross5_20 = new List<bool>();

    public static List<TMP_Text> goldenCross5_20Text = new List<TMP_Text>();
    public static List<TMP_Text> deadCross5_20Text = new List<TMP_Text>();
    
    //50-200
    [SerializeField] private Transform goldenDeadCrossPanel50_200;
    private List<float> closing50_200 = new List<float>();
    private string[] intervalToUse50_200;
    private List<bool> goldenCross50_200 = new List<bool>();
    private List<bool> deadCross50_200 = new List<bool>();

    public static List<TMP_Text> goldenCross50_200Text = new List<TMP_Text>();
    public static List<TMP_Text> deadCross50_200Text = new List<TMP_Text>();
    
    void Start()
    {   //5-20
        int count = goldenDeadCrossPanel5_20.childCount;
        intervalToUse5_20 = new string[count];
        if(count == 0)
        {
            Debug.Log("interval이 비어 있습니다.");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            intervalToUse5_20[i] = goldenDeadCrossPanel5_20.GetChild(i).name;
            goldenCross5_20Text.Add(goldenDeadCrossPanel5_20.GetChild(i).GetChild(1).GetComponent<TMP_Text>());
            deadCross5_20Text.Add(goldenDeadCrossPanel5_20.GetChild(i).GetChild(2).GetComponent<TMP_Text>());
        }
        //50-200
        int count2 = goldenDeadCrossPanel50_200.childCount;
        intervalToUse50_200 = new string[count2];
        if (count2 == 0)
        {
            Debug.Log("interval이 비어 있습니다.");
            return;
        }
        for (int i = 0; i < count2; i++)
        {
            intervalToUse50_200[i] = goldenDeadCrossPanel50_200.GetChild(i).name;
            goldenCross50_200Text.Add(goldenDeadCrossPanel50_200.GetChild(i).GetChild(1).GetComponent<TMP_Text>());
            deadCross50_200Text.Add(goldenDeadCrossPanel50_200.GetChild(i).GetChild(2).GetComponent<TMP_Text>());
        }
        
        StartCoroutine(GoldenCrossCalculation(intervalToUse5_20, goldenCross5_20, deadCross5_20, closing5_20, goldenCross5_20Text, deadCross5_20Text, 5, 20));
        StartCoroutine(GoldenCrossCalculation(intervalToUse50_200, goldenCross50_200, deadCross50_200, closing50_200, goldenCross50_200Text, deadCross50_200Text, 50, 200));
    }
    IEnumerator GoldenCrossCalculation(string[] intervals, List<bool> goldenCross, List<bool> deadCross, List<float> closing, List<TMP_Text> goldenCrossText, List<TMP_Text> deadCrossText, int termShort, int termLong)
    {
        WaitForSeconds delay = new WaitForSeconds(5f);
        while (true)
        {
            goldenCross.Clear();
            deadCross.Clear();
            Calculation(intervals, closing, goldenCross, deadCross, termShort, termLong);
            UI_InsertValue(goldenCrossText, deadCrossText, goldenCross, deadCross);
            yield return delay;
        }
    }
    private void UI_InsertValue(List<TMP_Text> panelText, List<TMP_Text> panelText2, List<bool> goldenCross, List<bool> deadCross)
    {
        for (int i = 0; i < goldenCross.Count; i++)
        {
            panelText[i].text = goldenCross[i].ToString();
            panelText2[i].text = deadCross[i].ToString();
        }
    }
    private void Calculation(string[] intervals, List<float> closing, List<bool> goldenCross, List<bool> deadCross, int termShort, int termLong)
    {
        foreach (string interval in intervals)
        {
            RequiredDataSetting(interval, closing);
            CurrentMaCross(closing, goldenCross, deadCross, termShort, termLong);
        }
    }
    private void RequiredDataSetting(string _interval, List<float> closingList)
    {
        closingList.Clear();

        if (_interval == "1m")
        {
            closingList.AddRange(BinanceData.closingPrices_1m);
        }
        else if (_interval == "3m")
        {
            closingList.AddRange(BinanceData.closingPrices_3m);
        }
        else if (_interval == "5m")
        {
            closingList.AddRange(BinanceData.closingPrices_5m);
        }
        else if (_interval == "15m")
        {
            closingList.AddRange(BinanceData.closingPrices_15m);
        }
        else if (_interval == "1h")
        {
            closingList.AddRange(BinanceData.closingPrices_1h);
        }
        else if (_interval == "4h")
        {
            closingList.AddRange(BinanceData.closingPrices_4h);
        }
        else if (_interval == "1d")
        {
            closingList.AddRange(BinanceData.closingPrices_1d);
        }
    }
    private void CurrentMaCross(List<float> closingList, List<bool> goldenCross, List<bool> deadCross, int shortTerm, int longTerm)
    {
        Vector3 maLong = MaCalculation(closingList, longTerm);
        Vector3 maShort = MaCalculation(closingList, shortTerm);
        
        if (maShort.x <= maLong.x && maShort.y >= maLong.y && maShort.z >= maLong.z)//GoldenCross
        {
            goldenCross.Add(true);
            deadCross.Add(false);
        }
        else if (maShort.x >= maLong.x && maShort.y <= maLong.y && maShort.z <= maLong.z)//DeadCross
        {
            goldenCross.Add(false);
            deadCross.Add(true);
        }
        else
        {
            goldenCross.Add(false);
            deadCross.Add(false);
        }
    }
    private Vector3 MaCalculation(List<float> closingList, int term)
    {
        float ma = 0f;
        float past_ma = 0f;
        float past_past_ma = 0f;
        float sum = 0f;
        //Debug.Log($"count : {closingList.Count}");
        for (int i = closingList.Count - term; i < closingList.Count; i++)//ma
        {
            sum += closingList[i];
        }
        ma = sum / term;
        sum = 0;

        for (int i = closingList.Count - term - 1; i < closingList.Count - 1; i++)//past ma
        {
            sum += closingList[i];
        }
        past_ma = sum / term;
        sum = 0;

        for (int i = closingList.Count - term - 2; i < closingList.Count - 2; i++)//past past ma
        {
            sum += closingList[i];
        }
        past_past_ma = sum / term;
        Vector3 maVec3 = new Vector3(past_past_ma, past_ma, ma);
        return maVec3;
    }
}
