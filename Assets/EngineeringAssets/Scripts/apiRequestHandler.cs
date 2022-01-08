using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class apiRequestHandler : MonoBehaviour
{
    private const string loginURL = "https://us-central1-coinracer-stagging.cloudfunctions.net/Login";
    private const string firebaseLoginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string firebaseApiKey = "AIzaSyBpdWOUj1_7iN3F3YBYetCONjMwVCVAIGE";
    public static apiRequestHandler Instance;

    private void OnEnable()
    {
        
    }

    public void onClick()
    {
        apiRequestHandler.Instance.getLoginDetails("naeQzZ6LI0P5MAz4wNsVoozA93p2");
    }
    public void Start()
    {
        Instance = this;
    }

    public void signInWithEmail(string _email,string _pwd)
    {
        StartCoroutine(processTokenRequest(_email, _pwd));
    }

    public void getLoginDetails(string _token)
    {
       // StartCoroutine(processRequest(_token, "loginDetails"));
    }

    private IEnumerator processTokenRequest(string _email, string _pwd)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _pwd);
        form.AddField("returnSecureToken", "true");
        using UnityWebRequest request = UnityWebRequest.Post(firebaseLoginUrl+firebaseApiKey,form);
        


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Result is: ");
            Debug.Log(request.result);
            Debug.Log(request.downloadHandler.text);
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            StartCoroutine(processRequest(tID));
            Debug.Log(tID);
        }
    }


    private IEnumerator processRequest(string _token)
    {
        using UnityWebRequest request = UnityWebRequest.Get(loginURL);
        request.SetRequestHeader("Authorization","Bearer "+ _token);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Result is: ");
            Debug.Log(request.result);
            Debug.Log(request.downloadHandler.text);
            FirebaseManager.Instance.SetPlayerData(request.downloadHandler.text);
            //UserData _player;
            //_player.UserName = 
        }
    }

    private void Update()
    {
     
    }
}
