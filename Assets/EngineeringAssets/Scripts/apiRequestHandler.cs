using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#region SuperClasses
[System.Serializable]
public class StoreResult
{
    public int Id { get; set; }
    public string name { get; set; }
    public string mechanics { get; set; }
    public string metadata { get; set; }
}


[System.Serializable]
public class
    MoralisBuyCarRequest
{
    public string id { get; set; }
    public string ownerWalletAddress { get; set; }
    public string uID { get; set; }
}

[System.Serializable]
public class MoralisConsumablePurchaseRequest
{
    public string id { get; set; }
    public string uID { get; set; }
    public string myNftID { get; set; }
    public string nlp { get; set; }
    public string ownerWalletAddress { get; set; }
}

[System.Serializable]
public class
    MoralisConsumablesRequest
{
    public string id { get; set; }
    public string uID { get; set; }
}


[System.Serializable]
public class MoralisStoreResponse
{
    public List<StoreResult> result { get; set; }
}


[System.Serializable]
public class MoralisNFTArrayRequest
{
    public List<string> tokenId { get; set; }
    public string uID { get; set; }
}

[System.Serializable]
public class MoralisUpdateRequest
{
    public string tokenId { get; set; }
    public string name { get; set; }
    public string mechanics { get; set; }
    public string uID { get; set; }
    public string metadata { get; set; }
    public string ownerWalletAddress { get; set; }
}


[System.Serializable]
public class MoralisNFTData
{
    public string tokenId { get; set; }
    public string ownerWallet { get; set; }
    public string name { get; set; }
    public string mechanics { get; set; }
    public string metadata { get; set; }
}

[System.Serializable]
public class MoralisNFTArrayResponse
{
    public List<MoralisNFTData> result { get; set; }
}
public class SignatureResponse
{
    public string signature { get; set; }
    public int nonce { get; set; }
}

public class UserDataBO
{
    public string userName { get; set; }

    public string email { get; set; }
    public string walletAddress { get; set; }

    public int AvatarID { get; set; }
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
#endregion

public class apiRequestHandler : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetStorage(string key, string ObjectName, string callback);
#endif
    public static apiRequestHandler Instance;

    #region Firebase datamembers
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
    #endregion

    #region Moralis datamembers
    private string m_BaseURL = "https://v7f3czkmtmmf.usemoralis.com:2053/server/functions/";
    private string m_AppID = "?_ApplicationId=8QhdEDdOy6fgMp0Bu6bUXmCU91VSvF1SDJOXbZAQ";
    private string m_GetNFTDataFunc = "getNFTDetails";
    private string m_GetNFTArrayDataFunc = "getNFTsDetails";
    private string m_UpdateNFTDataFunc = "updateNFTData";
    private string m_GetAllStoreDetails = "getAllNFTSMetaData";
    private string m_BuyCarFunc = "buyGameNFT";
    private string m_GetAllMyNFTFunc = "getAllMyNFTsData";
    private string m_PurchaseCarServerFunc = "purchaseCar";
    private string m_PurchaseConsumablesFunc = "consumablePurchases";
    private string m_uID = "";
    #endregion

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnEnable()
    {
        //StartCoroutine(ProcessNFTDataRequest("5063"));

        //string[] str_arr = new string[3]{ "5063", "5106", "20011" };
        //StartCoroutine(ProcessNFTDataArrayRequest(str_arr));
        Instance = this;

        if (Constants.IsTest)
        {
            m_uID = "ncnG7qYCEZezFYlKdBHrdiV9";
            Constants.GetSecKey = true;
        }

        //GetSecureKey();
        //ProcessAllStoreRequest();
    }

    #region Firebase
    public void onClick()
    {
        apiRequestHandler.Instance.getLoginDetails("naeQzZ6LI0P5MAz4wNsVoozA93p2");
    }

    public void Start()
    {
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
        StartCoroutine(processTokenRequest(FirebaseMoralisManager.Instance.Credentails.Email,
            FirebaseMoralisManager.Instance.Credentails.Password, true));
    }

    public void signInWithEmail(string _email,string _pwd)
    {
        StartCoroutine(processTokenRequest(_email, _pwd,false));
    }

    public void signUpWithEmail(string _email,string _pwd,string _username)
    {
        FirebaseMoralisManager.Instance.Credentails.Email = _email;
        FirebaseMoralisManager.Instance.Credentails.Password = _pwd;
        FirebaseMoralisManager.Instance.Credentails.UserName = _username;
        StartCoroutine(processSignUpRequest(_email,_pwd,_username));
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

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");

            FirebaseMoralisManager.Instance.Credentails.Email = _email;
            FirebaseMoralisManager.Instance.Credentails.Password = _pwd;

            StartCoroutine(signupBORequest(_email, _username, _pwd, tID));
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if ((string)res.SelectToken("error").SelectToken("message") == "EMAIL_EXISTS")
            {
                MainMenuViewController.Instance.ErrorMessage("Email already associated with another account");
            }
            else if ((string)res.SelectToken("error").SelectToken("message") == "EMAIL_NOT_FOUND")//2nd error
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

        _walletAddress = Constants.WalletAddress;

        UserDataBO userDataObj = new UserDataBO();
        userDataObj.userName = _username;
        userDataObj.email = _email;
        userDataObj.walletAddress = _walletAddress;
        userDataObj.AvatarID = Constants.FlagSelectedIndex;

        userDataPayload obj = new userDataPayload();
        obj.data = userDataObj;

        string req = JsonConvert.SerializeObject(obj);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"SignUp", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _BOtoken;
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            //Debug.Log(request.error);
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
           
            if (request.result == UnityWebRequest.Result.Success)
                StartCoroutine(sendVerificationLink(_BOtoken));
            else if ((string) res.SelectToken("message") == "Same WalletAddress already in Use")
                MainMenuViewController.Instance.ErrorMessage("Same WalletAddress already in Use");
            else if ((string) res.SelectToken("message") == "No User Found.")
                MainMenuViewController.Instance.SomethingWentWrong();
            else if ((string) res.SelectToken("message") == "Unauthorized")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else if ((string) res.SelectToken("message") == "Required parameters are missing")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else if ((string) res.SelectToken("message") == "Invalid request.")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else
                MainMenuViewController.Instance.SomethingWentWrongMessage();    
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
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            if (resendAgain)
            {
                FirebaseMoralisManager.Instance.ResendEmailSent("");
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
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            if (flag)
                StartCoroutine(processUpdateRequest(tID));
            else
                StartCoroutine(processRequest(tID)); //Login Request
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if ((string) res.SelectToken("error").SelectToken("message") == "INVALID_PASSWORD")
                MainMenuViewController.Instance.ErrorMessage("Invalid Credentials");
            else if ((string) res.SelectToken("error").SelectToken("message") == "EMAIL_NOT_FOUND")
                MainMenuViewController.Instance.ErrorMessage("Email does not exists");
            else
                MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
    }

    private IEnumerator processUpdateRequest(string _tID)
    {
        FirebaseMoralisManager.Instance.updatePlayerDataPayload();
        string req = JsonConvert.SerializeObject(FirebaseMoralisManager.Instance.PlayerDataPayload);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+ "GUpdateUserBO", req);//GUpdateUserBO//UpdateUserBO
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _tID;
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.Success)
                FirebaseMoralisManager.Instance.OnDocUpdate("");
            else if ((string) res.SelectToken("message") == "No User Found.")
                MainMenuViewController.Instance.SomethingWentWrong();
            else if ((string) res.SelectToken("message") == "Unauthorized")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else if ((string) res.SelectToken("message") == "Required parameters are missing")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else if ((string) res.SelectToken("message") == "Invalid request.")
                MainMenuViewController.Instance.SomethingWentWrongMessage();
            else
                MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
    }

    private IEnumerator processRequest(string _token)//Login API
    {
        LoginDataBO loginData = new LoginDataBO();

        loginData.walletAddress = Constants.WalletAddress;
       
        LoginDataBOPayload loginDataPayload = new LoginDataBOPayload();
        loginDataPayload.data = loginData;
        string req = JsonConvert.SerializeObject(loginDataPayload);
        using UnityWebRequest request = UnityWebRequest.Put(BaseURL+"Login", req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _token;
        request.SetRequestHeader("Authorization", _reqToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            Debug.Log(request.error);
        }
        else
        {
            JToken res = JObject.Parse(request.downloadHandler.text);
            if (request.result == UnityWebRequest.Result.Success)
            {
                FirebaseMoralisManager.Instance.SetPlayerData(request.downloadHandler.text);
            }
            else if ((string) res.SelectToken("message") == "Email is not verified")
            {
                Constants.ResendTokenID = _token;
                FirebaseMoralisManager.Instance.showVerificationScreen();
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

    public void getLeaderboard(bool IsSecondTour)
    {
        StartCoroutine(processLeaderboardToken(FirebaseMoralisManager.Instance.Credentails.Email,
            FirebaseMoralisManager.Instance.Credentails.Password, IsSecondTour));
    }

    private IEnumerator processLeaderboardToken(string _email, string _password, bool IsSecondTour)
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
        }
        else if(request.result == UnityWebRequest.Result.Success)
        {
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            StartCoroutine(processLeaderBoardRequest(tID, IsSecondTour));
        }
        else
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
    }

    private IEnumerator processLeaderBoardRequest(string _tID, bool IsSecondTour)
    {
        LeaderboardCounter _count = new LeaderboardCounter();

        if(IsSecondTour)
            _count.number = Constants.GLeaderboardCount;
        else
            _count.number = Constants.LeaderboardCount;

        LeaderboardPayload obj = new LeaderboardPayload();
        obj.data = _count;
        string req = JsonConvert.SerializeObject(obj);

        string _mainURL = BaseURL + "Leaderboard";

        if (IsSecondTour)
            _mainURL = BaseURL + "GLeaderboard";

        using UnityWebRequest request = UnityWebRequest.Put(_mainURL, req);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _tID;
        request.SetRequestHeader("Authorization", _reqToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
                FirebaseMoralisManager.Instance.OnQueryUpdate(request.downloadHandler.text, IsSecondTour);
            else
                MainMenuViewController.Instance.SomethingWentWrongMessage();
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
            MainMenuViewController.Instance.SomethingWentWrong();
        else if(request.result == UnityWebRequest.Result.Success)
            FirebaseMoralisManager.Instance.OnPassEmailSent("");
        else
            FirebaseMoralisManager.Instance.OnPassEmailSentError("");
    }

    async public Task<string> GetSignature(string _pid, string _winner, string _score)
    {
        string _token = await processSignatureToken(FirebaseMoralisManager.Instance.Credentails.Email, FirebaseMoralisManager.Instance.Credentails.Password);
        if (!string.IsNullOrEmpty(_token))
        {
            string signature = await processSignatureRequest(_token, _pid, _winner, _score);

            if (!string.IsNullOrEmpty(signature))
            {
                return signature;
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";
        }

    }

    async public Task<string> processSignatureToken(string _email, string _password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", _email);
        form.AddField("password", _password);
        form.AddField("returnSecureToken", "true");
        using UnityWebRequest request = UnityWebRequest.Post(firebaseLoginUrl + firebaseApiKey, form);

       await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrong();
            return "";
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            JToken token = JObject.Parse(request.downloadHandler.text);
            string tID = (string)token.SelectToken("idToken");
            return tID;
        }
        else
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
    }

    async public Task<string> processSignatureRequest(string _tID,string _pid,string _winner,string _score)
    {
        string _mainURL = BaseURL + "getSignature"+ "?params="+ _pid+"," + _winner + ","+ _score;
        Debug.Log(_mainURL);  

        using UnityWebRequest request = UnityWebRequest.Get(_mainURL);
        request.SetRequestHeader("Content-Type", "application/json");
        string _reqToken = "Bearer " + _tID;
        request.SetRequestHeader("Authorization", _reqToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }
    #endregion

    #region Moralis
    public void GetSecureKey()
    {
#if UNITY_WEBGL
        GetStorage(Constants.SecureKey, this.gameObject.name, "OnGetSec");
#endif
    }

    public void OnGetSec(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty)
        {
            string[] mainID = info.Split('"');
            m_uID = mainID[1];
            Debug.Log("received : " + m_uID);
            Constants.GetSecKey = true;
        }
        else
        {
            m_uID = "";
            Debug.Log("key was null, getting again....");
            Invoke("GetSecureKey", 0.5f);
        }
    }
    private IEnumerator ProcessNFTDataRequest(string _token)
    {            
        WWWForm form = new WWWForm();
        form.AddField("tokenId", _token);
        form.AddField("uID", m_uID);
        using UnityWebRequest request = UnityWebRequest.Post(m_BaseURL + m_GetNFTDataFunc+ m_AppID, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
        }
        else if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(request.downloadHandler.text);
        }
    }

    async public Task<string> ProcessNFTDataArrayRequest(List<string> _token)
    {
        MoralisNFTArrayRequest _dataNew = new MoralisNFTArrayRequest();
        _dataNew.tokenId = _token;
        _dataNew.uID = m_uID;

        string reqNew = JsonConvert.SerializeObject(_dataNew);
        byte[] rawBody = System.Text.Encoding.UTF8.GetBytes(reqNew);


        UnityWebRequest request = new UnityWebRequest(m_BaseURL + m_GetNFTArrayDataFunc + m_AppID, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            Debug.Log("data against token ids : "+request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    async public Task<string> ProcessAllMyNFTRequest(string _address)
    {
        WWWForm form = new WWWForm();
        form.AddField("uID", _address);
        using UnityWebRequest request = UnityWebRequest.Post(m_BaseURL + m_GetAllMyNFTFunc + m_AppID, form);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    async public Task<bool> ProcessNFTUpdateDataRequest(string _tokenid,string _name,string _mechanics,string _meta)
    {
        MoralisUpdateRequest _dataNEW = new MoralisUpdateRequest();
        _dataNEW.tokenId = _tokenid;
        _dataNEW.name = _name;
        _dataNEW.mechanics = _mechanics;
        _dataNEW.uID = m_uID;
        _dataNEW.ownerWalletAddress = Constants.WalletAddress;
        _dataNEW.metadata = _meta;

        string reqNew = JsonConvert.SerializeObject(_dataNEW);
        byte[] rawBody = System.Text.Encoding.UTF8.GetBytes(reqNew);

        UnityWebRequest request = new UnityWebRequest(m_BaseURL + m_UpdateNFTDataFunc + m_AppID, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();
         Debug.Log(request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return false;
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return false;
            }
        }
    }

    async public Task<string> ProcessAllStoreRequest()
    {
        WWWForm form = new WWWForm();
        form.AddField("uID", m_uID);
        using UnityWebRequest request = UnityWebRequest.Post(m_BaseURL + m_GetAllStoreDetails + m_AppID, form);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    async public Task<string> ProcessBuyCarRequest(string _metaID, string _owneraddress)
    {
        MoralisBuyCarRequest _dataNEW = new MoralisBuyCarRequest();
        _dataNEW.id = _metaID;
        _dataNEW.ownerWalletAddress = _owneraddress;
        _dataNEW.uID = m_uID;

        string reqNew = JsonConvert.SerializeObject(_dataNEW);
        byte[] rawBody = System.Text.Encoding.UTF8.GetBytes(reqNew);

        UnityWebRequest request = new UnityWebRequest(m_BaseURL + m_BuyCarFunc + m_AppID, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    async public Task<string> ProcessPurchaseCarServerRequest(string _metaID, string _owneraddress)
    {
        MoralisBuyCarRequest _dataNEW = new MoralisBuyCarRequest();
        _dataNEW.id = _metaID;
        _dataNEW.uID = m_uID ;
        _dataNEW.ownerWalletAddress = _owneraddress;

        string reqNew = JsonConvert.SerializeObject(_dataNEW);
        byte[] rawBody = System.Text.Encoding.UTF8.GetBytes(reqNew);

        UnityWebRequest request = new UnityWebRequest(m_BaseURL + m_PurchaseCarServerFunc + m_AppID, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    async public Task<string> ProcessPurchaseConsumableRequest(string _metaID, string _owneraddress,string nlp,string nftID)
    {
        MoralisConsumablePurchaseRequest _dataNEW = new MoralisConsumablePurchaseRequest();
        _dataNEW.id = _metaID;
        _dataNEW.ownerWalletAddress = _owneraddress;
        _dataNEW.nlp = nlp;
        _dataNEW.myNftID = nftID;
        _dataNEW.uID = m_uID;

        string reqNew = JsonConvert.SerializeObject(_dataNEW);
        Debug.LogError(reqNew);
        byte[] rawBody = System.Text.Encoding.UTF8.GetBytes(reqNew);

        UnityWebRequest request = new UnityWebRequest(m_BaseURL + m_PurchaseConsumablesFunc + m_AppID, "POST");
        request.uploadHandler = new UploadHandlerRaw(rawBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        Debug.Log(request.downloadHandler.text);
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            MainMenuViewController.Instance.SomethingWentWrongMessage();
            return "";
        }
        else
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                return request.downloadHandler.text;
            }
            else
            {
                MainMenuViewController.Instance.SomethingWentWrongMessage();
                return "";
            }
        }
    }

    #endregion
}
