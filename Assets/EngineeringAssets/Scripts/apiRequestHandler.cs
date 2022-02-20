using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
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

public class LoginDataBO
{
    public string walletAddress { get; set; }
}

public class LoginDataBOPayload
{
    public LoginDataBO data { get; set; }
}

public class LeaderboardCounter
{
    public int number { get; set; }
}

public class LeaderboardPayload
{
    public LeaderboardCounter data { get; set; }
}

public class apiRequestHandler : MonoBehaviour
{
    //Staging : https://us-central1-coinracer-stagging.cloudfunctions.net/
    //Production : https://us-central1-coinracer-alpha-tournaments.cloudfunctions.net/

    private string BaseURL;
    private string loginURL;
    private const string firebaseLoginUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
    private const string firebaseSignupUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=";

    private  string forgetPassword = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";
    private  string emailVerification ="https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";

    //Staging : AIzaSyBpdWOUj1_7iN3F3YBYetCONjMwVCVAIGE
    //Production : AIzaSyDcLz0eTFpmf7pksItUB_AQ6YA2SNErx_8
    private string firebaseApiKey ;

    private  string signupBOUserURL ;
    private  string updateUserBoURL;
    private  string leaderboardBOURL ;
      
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
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        if (Constants.IsStagging)
        {
            BaseURL = "https://us-central1-coinracer-stagging.cloudfunctions.net/";
            firebaseApiKey = "AIzaSyBpdWOUj1_7iN3F3YBYetCONjMwVCVAIGE";
        }
        else //Production
        {
            BaseURL = "https://us-central1-coinracer-alpha-tournaments.cloudfunctions.net/";
            firebaseApiKey = "AIzaSyDcLz0eTFpmf7pksItUB_AQ6YA2SNErx_8";
        }

        loginURL = BaseURL + "Login";
        signupBOUserURL = BaseURL+"SignUp"; 
        updateUserBoURL = BaseURL+"UpdateUserBO";
        leaderboardBOURL = BaseURL+"Leaderboard";

        forgetPassword = forgetPassword + firebaseApiKey;
        emailVerification = emailVerification + firebaseApiKey;

    }

    public void updatePlayerData()
    {
        StartCoroutine(processTokenRequest(FirebaseManager.Instance.Credentails.Email,
            FirebaseManager.Instance.Credentails.Password, true));
    }

    public void signInWithEmail(string _email,string _pwd)
    {
        StartCoroutine(processTokenRequest(_email, _pwd,false));
    }
    public void signUpWithEmail(string _email,string _pwd,string _username)
    {
        FirebaseManager.Instance.Credentails.Email = _email;
        FirebaseManager.Instance.Credentails.Password = _pwd;
        FirebaseManager.Instance.Credentails.UserName = _username;
      //  //Debug.Log("In Signup Email");
        StartCoroutine(processSignUpRequest(_email,_pwd,_username));
      //  //Debug.Log(_email);
      //  //Debug.Log(_pwd);
      //  //Debug.Log(_username);
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

        //Debug.Log("Processing Request");
        //Debug.Log(request.result);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            //Debug.LogError(request.result);
            //Debug.Log(request.error);
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
         //Debug.Log("Result is: ");
                    //Debug.Log(request.result);
                    //Debug.Log(request.downloadHandler.text);
                    JToken token = JObject.Parse(request.downloadHandler.text);
                    string tID = (string)token.SelectToken("idToken");
                    StartCoroutine(signupBORequest(_email,_username,_pwd,tID));
                  //Debug.Log(tID);
        }
        else
        {
         //Debug.Log("Result is: ");
         //Debug.Log(request.result);
         JToken res = JObject.Parse(request.downloadHandler.text);
         //Debug.Log((string)res.SelectToken("error").SelectToken("message"));
         if ((string) res.SelectToken("error").SelectToken("message") == "EMAIL_EXISTS")
         {
             MainMenuViewController.Instance.ErrorMessage("Email already associated with another account");
             //MainMenuViewController.Instance.EmailAlreadyExisted();
         }
         else if ((string) res.SelectToken("error").SelectToken("message") == "EMAIL_NOT_FOUND")//2nd error
         {
             MainMenuViewController.Instance.SomethingWentWrong();
         }
         else
         {
             MainMenuViewController.Instance.SomethingWentWrongMessage();
         }
           
        }
    }

    private IEnumerator signupBORequest(string _email, string _username,string _pwd,string _BOtoken)
    {
        string _walletAddress = "";
        //Debug.Log(WalletManager.Instance.GetAccount());
        //if (Constants.IsTest)
        //{
        //    _walletAddress = "0xD4d844C5A1cFAB13A8Ab252E466188d";
        //}
        //else
        _walletAddress = Constants.WalletAddress;

        UserDataBO userDataObj = new UserDataBO();
        userDataObj.userName = _username;
        userDataObj.email = _email;
        userDataObj.walletAddress = _walletAddress;

        userDataPayload obj = new userDataPayload();
        obj.data = userDataObj;
        //Debug.Log(JsonConvert.SerializeObject(obj));
       // //Debug.Log(_BOtoken);
        string req = JsonConvert.SerializeObject(obj);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"SignUp", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _BOtoken;
        //Debug.Log(_reqToken);
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //Debug.Log(request.error);
        }
        else
        {
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken res = JObject.Parse(request.downloadHandler.text);
           
            if (request.result == UnityWebRequest.Result.Success)
            {
               // string tID = (string)res.SelectToken("idToken");
                //StartCoroutine(processTokenRequest(_email,_pwd,false));
                //Debug.Log(_BOtoken);
                StartCoroutine(sendVerificationLink(_BOtoken));
               
            }
            else if ((string) res.SelectToken("message") == "Same WalletAddress already in Use")
            {
                MainMenuViewController.Instance.ErrorMessage("Same WalletAddress already in Use");
            }
            
            else if ((string) res.SelectToken("message") == "No User Found.")
            {
                MainMenuViewController.Instance.SomethingWentWrong();
            }
            else if ((string) res.SelectToken("message") == "Unauthorized")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Required parameters are missing")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Invalid request.")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            
            
            
        }
    }

    public void sendVerificationAgain()
    {
        string _token = Constants.ResendTokenID;
        StartCoroutine(sendVerificationLink(_token,true));
       
    }

    private IEnumerator sendVerificationLink(string _tokenId, bool resendAgain = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "VERIFY_EMAIL");
        form.AddField("idToken", _tokenId);
        using UnityWebRequest request = UnityWebRequest.Post(emailVerification, form);

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            //Debug.Log(request.error);
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            // FirebaseManager.Instance.ResendEmailSent("");
            if (resendAgain)
            {
                FirebaseManager.Instance.ResendEmailSent("");
            }
            else
            {
                MainMenuViewController.Instance.ErrorMessage("Verification link sent to Email");

            }
        }
        else
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            if(resendAgain)
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        }
    }

    private IEnumerator processTokenRequest(string _email, string _pwd, bool flag = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _pwd);
        form.AddField("returnSecureToken", "true");
        using UnityWebRequest request = UnityWebRequest.Post(firebaseLoginUrl+firebaseApiKey,form);
        


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            //Debug.Log(request.error);
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            if (flag)
            {
                //TODO: Update Request
                StartCoroutine(processUpdateRequest(tID));
            }
            else
            {
                StartCoroutine(processRequest(tID)); //Login Request
            }
           // Debug.Log(tID);
        }
        else
        {
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            JToken res = JObject.Parse(request.downloadHandler.text);
            //Debug.Log((string)res.SelectToken("error").SelectToken("message"));
            if ((string) res.SelectToken("error").SelectToken("message") == "INVALID_PASSWORD")
            {
                MainMenuViewController.Instance.ErrorMessage("Invalid Credentials");
            }
            else if ((string) res.SelectToken("error").SelectToken("message") == "EMAIL_NOT_FOUND")
            {
                MainMenuViewController.Instance.ErrorMessage("Email does not exists");
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
        }
    }

    private IEnumerator processUpdateRequest(string _tID)
    {
        FirebaseManager.Instance.updatePlayerDataPayload();
        string req = JsonConvert.SerializeObject(FirebaseManager.Instance.PlayerDataPayload);
        //Debug.Log(req);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"UpdateUserBO", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _tID;
        //Debug.Log(_reqToken);
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            //Debug.Log(request.error);
        }
        else
        {
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken res = JObject.Parse(request.downloadHandler.text);
            // //Debug.Log((string)res.SelectToken("error").SelectToken("message"));
            if (request.result == UnityWebRequest.Result.Success)
            {
                FirebaseManager.Instance.OnDocUpdate("");
            }
            else if ((string) res.SelectToken("message") == "No User Found.")
            {
                MainMenuViewController.Instance.SomethingWentWrong();
            }
            else if ((string) res.SelectToken("message") == "Unauthorized")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Required parameters are missing")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Invalid request.")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            //JToken token = JObject.Parse(request.downloadHandler.text);
            // string tID = (string)token.SelectToken("idToken");
            // //Debug.Log(tID);
        }
    }
    

    private IEnumerator processRequest(string _token)//Login API
    {
        LoginDataBO loginData = new LoginDataBO();
        //if (Constants.IsTest)
        //{
        //    loginData.walletAddress = "0xAE79Dc61917d0de544db72C75de727421AcD7566";
        //}
        //else
        //{
        loginData.walletAddress = Constants.WalletAddress;
       
        LoginDataBOPayload loginDataPayload = new LoginDataBOPayload();
        loginDataPayload.data = loginData;
        string req = JsonConvert.SerializeObject(loginDataPayload);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"Login", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _token;
        //Debug.Log(_token);
        request.SetRequestHeader("Authorization", _reqToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            Debug.Log(request.error);
        }
        else
        {
            //"message": "Unauthorized" //wrong password
            // JToken response = JObject.Parse(request.downloadHandler.text);
            // string reqResponse = (string)response.SelectToken("data").SelectToken("Email");

            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken res = JObject.Parse(request.downloadHandler.text);
            // //Debug.Log((string)res.SelectToken("error").SelectToken("message"));
            if (request.result == UnityWebRequest.Result.Success)
            {
                FirebaseManager.Instance.SetPlayerData(request.downloadHandler.text);
            }
            else if ((string) res.SelectToken("message") == "Email is not verified")
            {
                Constants.ResendTokenID = _token;
                FirebaseManager.Instance.showVerificationScreen();
                
                //MainMenuViewController.Instance.ErrorMessage("Email is not verified");

                //  MainMenuViewController.Instance.sendEmailConfirmationAgain(_token);
            }
            else if ((string) res.SelectToken("message") == "No User Found.")
            {
                MainMenuViewController.Instance.SomethingWentWrong();
            }
            else if ((string) res.SelectToken("message") == "Unauthorized")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Required parameters are missing")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            else if ((string) res.SelectToken("message") == "Invalid request.")
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
             else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
        //UserData _player;
        //_player.UserName = 
        }
    }

    public void getLeaderboard()
    {
        StartCoroutine(processLeaderboardToken(FirebaseManager.Instance.Credentails.Email,
            FirebaseManager.Instance.Credentails.Password));
    }

    private IEnumerator processLeaderboardToken(string _email, string _password)
    {
        if(LeaderboardManager.Instance)
            LeaderboardManager.Instance.ClearLeaderboard();

        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _password);
        form.AddField("returnSecureToken", "true");
        using UnityWebRequest request = UnityWebRequest.Post(firebaseLoginUrl+firebaseApiKey,form);
        


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrong();
            //Debug.Log(request.error);
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            StartCoroutine(processLeaderBoardRequest(tID));
            //Debug.Log(tID);
        }
        else
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
        }

    }

    private IEnumerator processLeaderBoardRequest(string _tID)
    {
        LeaderboardCounter _count = new LeaderboardCounter();
        _count.number = Constants.LeaderboardCount;
        LeaderboardPayload obj = new LeaderboardPayload();
        obj.data = _count;
        string req = JsonConvert.SerializeObject(obj);
        //Debug.Log(req);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"Leaderboard", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _tID;
        //Debug.Log(_reqToken);
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //Debug.Log(request.error);
        }
        else
        {
            //"message": "Unauthorized" //wrong password
            // JToken response = JObject.Parse(request.downloadHandler.text);
            // string reqResponse = (string)response.SelectToken("data").SelectToken("Email");
            
            //Debug.Log("LeaderBoard Result is: ");
            //Debug.Log(request.result);
            //Debug.Log(request.downloadHandler.text);
           
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);
                FirebaseManager.Instance.OnQueryUpdate(request.downloadHandler.text);
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            }
            //  FirebaseManager.Instance.SetPlayerData(request.downloadHandler.text);
            //UserData _player;
            //_player.UserName = 
        }   
    }

    public void onForgetPassword(string _email)
    {
        StartCoroutine(processForgetRequest(_email));
    }

    private IEnumerator processForgetRequest(string _email)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "PASSWORD_RESET");
        form.AddField("email", _email);
        using UnityWebRequest request = UnityWebRequest.Post(forgetPassword,form);
        
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrong();
            ////Debug.Log(request.error);
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            FirebaseManager.Instance.OnPassEmailSent("");
        }
        else
        {
            FirebaseManager.Instance.OnPassEmailSentError("");
        }

    }

   
    
    
    // private void Update()
    // {
    //  
    // }
}
