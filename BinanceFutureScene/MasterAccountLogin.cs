using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class MasterAccountLogin : MonoBehaviour
{
    private FirebaseAuth master;
    private string email = "yourEmail@naver.com";
    private string password = "yourSecret";

    public void MasterLogin()
    {
        master = FirebaseAuth.DefaultInstance;

        master.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("LoginSuccess");
        });
    }
}
