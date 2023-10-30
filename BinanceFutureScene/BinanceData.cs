using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using TMPro;
using System;
using System.Linq;
public class BinanceData : MonoBehaviour//마지막 인덱스가 가장 최근 캔들이다.
{
    [SerializeField] private GameObject indicatorCalculation;
    [SerializeField] private TMP_Text candle_Futre_BTC;
    
    public static List<float> closingPrices_1m = new List<float>();
    public static List<float> closingPrices_3m = new List<float>();
    public static List<float> closingPrices_5m = new List<float>();
    public static List<float> closingPrices_15m = new List<float>();
    public static List<float> closingPrices_1h = new List<float>();
    public static List<float> closingPrices_4h = new List<float>();
    public static List<float> closingPrices_1d = new List<float>();

    public static float currentClose = 0f;
    public static float currentHigh = 0f;
    public static float currentLow = 0f;

    void Start()
    {
        StartCoroutine(GetCandleData(2f, "1m", 205));
        StartCoroutine(GetCandleData(2f, "3m", 205));
        StartCoroutine(GetCandleData(2f, "5m", 205));
        StartCoroutine(GetCandleData(2f, "15m", 205));
        StartCoroutine(GetCandleData(2f, "1h", 205));
        StartCoroutine(GetCandleData(2f, "4h", 205));
        StartCoroutine(GetCandleData(2f, "1d", 205));

        StartCoroutine(StartIndicators());
    }
    IEnumerator StartIndicators()
    {
        WaitForSeconds delay = new WaitForSeconds(2f);
        while (true)
        {
            yield return delay;
            if (closingPrices_1m != null && closingPrices_3m != null && closingPrices_5m != null && closingPrices_15m != null && closingPrices_1h != null && closingPrices_4h != null && closingPrices_1d != null)
            {
                indicatorCalculation.SetActive(true);
                break;
            } 
        }
    }
    IEnumerator GetCandleData(float delayTime, string _interval, int _limit)
    {
        string candleUrl = $"https://fapi.binance.com/fapi/v1/klines?symbol=BTCUSDT&interval={_interval}&limit={_limit}";
        UnityWebRequest request;
        WaitForSeconds delay = new WaitForSeconds(delayTime);
        string getString = "";

        List<List<string>> candleData = new List<List<string>>();
        List<float> closingPricesList = new List<float>();

        while (true)
        {
            request = UnityWebRequest.Get(candleUrl);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }

            else
            {
                getString = request.downloadHandler.text;
                ParsingCandleData(getString, candleData, closingPricesList, _interval);
                AssigningValueToSpecificLists(closingPricesList, _interval);
            }
            yield return delay;
        }
    }
    private void ParsingCandleData(string json, List<List<string>> candleData, List<float> closingPrices, string _interval)
    {
        candleData.Clear();
        closingPrices.Clear();

        string newJson = json.Substring(2, json.Length - 4);// 문자열 양 옆의 [[ 와 ]] 제거
        candleData.AddRange(newJson.Split("],[").Select(x => x.Split(",").ToList()));
        foreach (var candle in candleData)
        {
            closingPrices.Add(float.Parse(candle[4].Substring(1, candle[4].Length - 2)));//종가데이터 추출
        }
        if(_interval == "1m")
        {
            currentHigh = float.Parse(candleData[candleData.Count - 1][2].Substring(1, candleData[candleData.Count - 1][2].Length - 2));
            currentLow = float.Parse(candleData[candleData.Count - 1][3].Substring(1, candleData[candleData.Count - 1][3].Length - 2));
            currentClose = float.Parse(candleData[candleData.Count - 1][4].Substring(1, candleData[candleData.Count - 1][4].Length - 2));
            candle_Futre_BTC.text = currentClose.ToString();
            //Debug.Log($"{currentHigh}, {currentLow}, {currentClose}");
        }
    }
    private void AssigningValueToSpecificLists(List<float> list, string _interval)
    {
        if(_interval == "1m")
        {
            closingPrices_1m.Clear();
            closingPrices_1m.AddRange(list);
        }
        else if(_interval == "3m")
        {
            closingPrices_3m.Clear();
            closingPrices_3m.AddRange(list);
        }
        else if (_interval == "5m")
        {
            closingPrices_5m.Clear();
            closingPrices_5m.AddRange(list);
        }
        else if (_interval == "15m")
        {
            closingPrices_15m.Clear();
            closingPrices_15m.AddRange(list);
        }
        else if (_interval == "1h")
        {
            closingPrices_1h.Clear();
            closingPrices_1h.AddRange(list);
        }
        else if (_interval == "4h")
        {
            closingPrices_4h.Clear();
            closingPrices_4h.AddRange(list);
        }
        else if (_interval == "1d")
        {
            closingPrices_1d.Clear();
            closingPrices_1d.AddRange(list);
        }
    }
}
[System.Serializable]
public class CandleData
{//[1693180800000,"26087.60","26244.60","25850.40","26106.50","219992.051",1693267199999,"5728245334.03454",2034199,"109259.071","2844967503.22500","0"]
    
    public long open_time;
    public string open_price;
    public string high_price;
    public string low_price;
    public string close_price;
    public string volume;
    public long close_time;
    public string quote_asset_volume;
    public int number_of_trades;
    public string taker_buy_base_asset_volume;
    public string taker_buy_quote_asset_volume;
    public string ignore;
}
[System.Serializable]
public class CandleDataArray
{
    public List<List<string>> list;
}
