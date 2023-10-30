using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.Networking;
using System.Linq;
using System;
using System.Text;

public class FCM_Manager : MonoBehaviour
{
    private string fcmToken = "";
    private DatabaseReference reference;
    private const string FCM_URL = "https://sendfcmmessaging.netlify.app/.netlify/functions/fcmmessaging";
    public List<string> readTokenList = new List<string>();
    public static FCM_Manager instance;

    void Awake()
    {
        instance = this;
        FirebaseInitialization();
    }
    private void FirebaseInitialization()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                GetComponent<MasterAccountLogin>().MasterLogin();
                reference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
    public void ReadRealTime(string indicator, string timeFrame)
    {
        readTokenList.Clear();
        if (indicator == "BollingerBandUpper" || indicator == "BollingerBandLower")
            indicator = indicator.Substring(0, indicator.Length - 5);
        else if (indicator == "RSIUpper" || indicator == "RSILower")
            indicator = indicator.Substring(0, indicator.Length - 5);
        Debug.Log("ReadRealTime : " + indicator);
        reference.Child($"{indicator}_{timeFrame}").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("리얼타임 데이터베이스 읽어오기 실패!");
                return;
            }
            DataSnapshot snapshot = task.Result;
            Debug.Log("type : " + snapshot.Value.GetType().Name);
                
            // Convert the DataSnapshot to a list

            if (snapshot != null && snapshot.Value is Dictionary<string, object> dictionary)
            {
                foreach (var item in dictionary)
                {
                    fcmToken = item.Value.ToString();
                    readTokenList.Add(fcmToken);
                }
                Debug.Log("스타트 코루틴");
                StartCoroutine(SendToTokens(readTokenList, indicator, timeFrame));
            }
        });
    }
    IEnumerator SendToTokens(List<string> tokens, string indicator, string timeFrame)
    {
        Debug.Log($"tokens Count : {tokens.Count}");//500개 토큰 제한 있음. 사용하는 사람 많이지면 리스트를 몇번에 걸쳐 500개씩 request보내는 코드로 수정해아함.
        WWWForm form = new WWWForm();
        string jsonMessage = "[\"" + string.Join("\",\"", tokens) + "\"]";
        Debug.Log(jsonMessage);
        form.AddField("tokenList", jsonMessage);
        form.AddField("indicator", indicator);
        form.AddField("timeFrame", timeFrame);

        UnityWebRequest request = UnityWebRequest.Post(FCM_URL, form);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {/*
            */
            Debug.Log(request.error);
        }
        else
        {
            string responseString = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);

            // 문자열을 JSON 객체로 파싱
            var responseObject = JsonUtility.FromJson<ResponseObject>(responseString);

            // 'message' 속성 가져오기
            string message = responseObject.message;
            Debug.Log(message);
        }
        request.Dispose();
    }
}
[System.Serializable]
public class ResponseObject
{
    public string message;
    public string results;
}