using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UserDataBO
{

    public string userName { get; set; }

    public string email { get; set; }
    public string walletAddress { get; set; }
}
public class userDataPayload
{
    public UserDataBO data { get; set; }
}
public class apiRequestHandler : MonoBehaviour
{
    private const string loginURL = "https://us-central1-coinracer-stagging.cloudfunctions.net/Login";
    private const string firebaseLoginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string firebaseSignupUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=";

    private const string firebaseApiKey = "AIzaSyBpdWOUj1_7iN3F3YBYetCONjMwVCVAIGE";
    private const string signupBOUserURL = "https://us-central1-coinracer-stagging.cloudfunctions.net/SignUp";
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
    public void signUpWithEmail(string _email,string _pwd,string _username)
    {
      //  Debug.Log("In Signup Email");
        StartCoroutine(processSignUpRequest(_email,_pwd,_username));
      //  Debug.Log(_email);
      //  Debug.Log(_pwd);
      //  Debug.Log(_username);
    }

    public void getLoginDetails(string _token)
    {
       // StartCoroutine(processRequest(_token, "loginDetails"));
    }
    private IEnumerator processSignUpRequest(string _email, string _pwd, string _username)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _pwd);
        form.AddField("returnSecureToken", "true");
        using UnityWebRequest request = UnityWebRequest.Post(firebaseSignupUrl + firebaseApiKey, form);



        yield return request.SendWebRequest();

        Debug.Log("Processing Request");
        Debug.Log(request.result);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.result);
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Result is: ");
            Debug.Log(request.result);
            Debug.Log(request.downloadHandler.text);
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            StartCoroutine(signupBORequest(_email,_username,_pwd,tID));
            Debug.Log(tID);
        }
    }

    private IEnumerator signupBORequest(string _email, string _username,string _pwd,string _BOtoken)
    {
        string _walletAddress = "";
        Debug.Log(WalletManager.Instance.GetAccount());
        if (Constants.IsTest)
        {
            _walletAddress = "12347985";
        }
        else
            _walletAddress = WalletManager.Instance.GetAccount();

        UserDataBO userDataObj = new UserDataBO();
        userDataObj.userName = _username;
        userDataObj.email = _email;
        userDataObj.walletAddress = _walletAddress;

        userDataPayload obj = new userDataPayload();
        obj.data = userDataObj;
        Debug.Log(JsonConvert.SerializeObject(obj));
       // Debug.Log(_BOtoken);
        string req = JsonConvert.SerializeObject(obj);
        using UnityWebRequest request = UnityWebRequest.Put(signupBOUserURL, req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _BOtoken;
        Debug.Log(_reqToken);
        request.SetRequestHeader("Authorization", _reqToken);

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
            StartCoroutine(processTokenRequest(_email,_pwd));
            Debug.Log(tID);
        }
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
