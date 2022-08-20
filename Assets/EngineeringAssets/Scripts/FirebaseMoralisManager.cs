using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Numerics;

#region SuperClasses
[System.Serializable]
public class MechanicsData
{
    public string CarName;
    public int CarHealth;
    public float Tyre_Laps;
    public float EngineOil_Laps;
    public float Gas_Laps;

    public MechanicsData(string _carName,int carHealth,float tyre_Laps,float engineOil_Laps,float gas_Laps)
    {
        this.CarName = _carName;
        this.CarHealth = carHealth;
        this.Tyre_Laps = tyre_Laps;
        this.EngineOil_Laps = engineOil_Laps;
        this.Gas_Laps = gas_Laps;
    }
}

[System.Serializable]
public class NFTMehanicsData
{
    public string PlayerName;
    public string OwnerWalletAddress;
    public MechanicsData mechanicsData;
}
public class UserData
{
    public string UserName { get; set; }
    public string WalletAddress { get; set; }
    public double TimeSeconds { get; set; }
    public string UID { get; set; }
    public double NumberOfTries { get; set; }
    public double NumberOfTriesPractice { get; set; }
    public bool PassBought { get; set; }
    public string Email { get; set; }
    public string StalkedNFT { get; set; }
    public int AvatarID { get; set; }
    public EndDate TournamentEndDate { get; set; }
    public Timestamp ProfileCreated { get; set; }
    public int TotalWins { get; set; }
    public double GNumberOfTries { get; set; }
    public bool GPassBought { get; set; }
    public double GTimeSeconds { get; set; }
    public EndDate GTournamentEndDate { get; set; }
    public double VC_Amount { get; set; }
}

public class AuthCredentials
{
    public string Email { get; set; }
    public string Password { get; set; }

    public string UserName { get; set; }

    public string WalletAddress { get; set; }
}
public class updateDataPayload
{
    public UserData data { get; set; }
}
#endregion
public class FirebaseMoralisManager : MonoBehaviour
{
    #region DataMembers
    [DllImport("__Internal")]
    private static extern void SetStorage(string key, string val);

    private int key = 129;
    private string UID = "";
    public UserData PlayerData;
    public updateDataPayload PlayerDataPayload;


    public UserData[] PlayerDataArray;
    public static FirebaseMoralisManager Instance;
    public Dictionary<int, NFTMehanicsData> NFTMehanics = new Dictionary<int, NFTMehanicsData>();
    public Dictionary<int, List<StatSettings>> CarDealer = new Dictionary<int, List<StatSettings>>();

    [HideInInspector]
    public AuthCredentials Credentails;

    string DocPath = "users";
    [HideInInspector]
    public bool DocFetched = false;
    [HideInInspector]
    public bool ResultFetched = false;
    bool DataFetchError = false;
    bool FetchUserData = false;
    bool UserDataFetched = false;

    private List<string> TokenPayload = new List<string>();
    private Dictionary<int, string> TokenName = new Dictionary<int, string>();
    #endregion

    #region StartFunctionality

    private void Awake()
    {
        //GetNFTData();
    }
    void Start()
    {
        
        Credentails = new AuthCredentials();

        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }

        //AuthenticateFirebase();
        if (Constants.isUsingFirebaseSDK)
            OnAuthChanged();

        GetAssignStoreData();
    }

    public void updatePlayerDataPayload()
    {
        PlayerDataPayload = new updateDataPayload();
        PlayerDataPayload.data = PlayerData;
    }
    #endregion

    #region Firebase
    public void SetPlayerData(string _response)
    {
        JToken response = JObject.Parse(_response);
        Debug.Log(response);
        PlayerData = new UserData();
        PlayerData.Email = (string)response.SelectToken("data").SelectToken("Email");
        PlayerData.UserName = (string)response.SelectToken("data").SelectToken("UserName");
        PlayerData.WalletAddress = (string)response.SelectToken("data").SelectToken("WalletAddress");
        PlayerData.TimeSeconds = (double)response.SelectToken("data").SelectToken("TimeSeconds");
        PlayerData.UID = (string)response.SelectToken("data").SelectToken("UID");
        PlayerData.NumberOfTries = (double)response.SelectToken("data").SelectToken("NumberOfTries");
        PlayerData.NumberOfTriesPractice = (double)response.SelectToken("data").SelectToken("NumberOfTriesPractice");
        PlayerData.PassBought = (bool)response.SelectToken("data").SelectToken("PassBought");
        PlayerData.AvatarID = (int)response.SelectToken("data").SelectToken("AvatarID");
        PlayerData.TotalWins = (int)response.SelectToken("data").SelectToken("TotalWins");
        PlayerData.StalkedNFT = (string)response.SelectToken("data").SelectToken("StalkedNFT");
        Constants.TotalWins = PlayerData.TotalWins;
        PlayerData.TournamentEndDate = new EndDate();
        PlayerData.TournamentEndDate.nanoseconds = (double)response.SelectToken("data").SelectToken("TournamentEndDate").SelectToken("nanoseconds");
        PlayerData.TournamentEndDate.seconds = (double)response.SelectToken("data").SelectToken("TournamentEndDate").SelectToken("seconds");
        PlayerData.ProfileCreated = new Timestamp();
        PlayerData.ProfileCreated.nanoseconds = (double)response.SelectToken("data").SelectToken("ProfileCreated").SelectToken("nanoseconds");
        PlayerData.ProfileCreated.seconds = (double)response.SelectToken("data").SelectToken("ProfileCreated").SelectToken("seconds");

        PlayerData.GTimeSeconds = (double)response.SelectToken("data").SelectToken("GTimeSeconds");
        PlayerData.GNumberOfTries = (double)response.SelectToken("data").SelectToken("GNumberOfTries");
        PlayerData.GPassBought = (bool)response.SelectToken("data").SelectToken("GPassBought");
        PlayerData.GTournamentEndDate = new EndDate();
        PlayerData.GTournamentEndDate.nanoseconds = (double)response.SelectToken("data").SelectToken("GTournamentEndDate").SelectToken("nanoseconds");
        PlayerData.GTournamentEndDate.seconds = (double)response.SelectToken("data").SelectToken("GTournamentEndDate").SelectToken("seconds");
        PlayerData.VC_Amount = (double)response.SelectToken("data").SelectToken("VC_Amount");
        //PlayerData.Mechanics.CarHealth = response.SelectToken("data").SelectToken("Mechanics").SelectToken("CarHealth") != null ? (int)response.SelectToken("data").SelectToken("Mechanics").SelectToken("CarHealth") : 100;
        //PlayerData.Mechanics.Tyre_Laps = response.SelectToken("data").SelectToken("Mechanics").SelectToken("Tyre_Laps") != null ? (float)response.SelectToken("data").SelectToken("Mechanics").SelectToken("Tyre_Laps") : 0;
        //PlayerData.Mechanics.EngineOil_Laps = response.SelectToken("data").SelectToken("Mechanics").SelectToken("EngineOil_Laps") != null ? (float)response.SelectToken("data").SelectToken("Mechanics").SelectToken("EngineOil_Laps") : 0;
        //PlayerData.Mechanics.Gas_Laps = response.SelectToken("data").SelectToken("Mechanics").SelectToken("Gas_Laps") != null ? (float)response.SelectToken("data").SelectToken("Mechanics").SelectToken("Gas_Laps") : 0;

        //Debug.Log(PlayerData.Mechanics.VC_Amount);
        //Debug.Log(PlayerData.Mechanics.CarHealth);
        //Debug.Log(PlayerData.Mechanics.Tyre_Laps);
        //Debug.Log(PlayerData.Mechanics.EngineOil_Laps);
        //Debug.Log(PlayerData.Mechanics.Gas_Laps);

        Constants.UserName = PlayerData.UserName;
        Constants.FlagSelectedIndex = PlayerData.AvatarID;
        
        Constants.VirtualCurrencyAmount = PlayerData.VC_Amount;
        //Constants.StoredCarHealth = PlayerData.Mechanics.CarHealth;

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);
        if (Constants.PushingTime)
        {
            Constants.PushingTime = false;
            GamePlayUIHandler.Instance.SubmitTime();
        }
        string message = (string)response.SelectToken("message");
        if (message == "User BO")
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.OnLoginSuccess(false);
        }
    }
    public void SetStalkedNFT(string _key)
    {
        PlayerData.StalkedNFT = _key;
        UpdatedFireStoreData(PlayerData);
    }
    public string GetStalkedNFT()
    {
        return PlayerData.StalkedNFT;
    }
    public void SetLocalStorage(string key,string data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetStorage(key, data);
#endif
    }
    public void AuthenticateFirebase()
    {
        FirebaseAuth.AuthenticateAnonymous(gameObject.name, "OnAuthSuccess", "OnAuthError");
    }
    public void OnAuthSuccess(string info)
    {
        //Debug.Log(info);        
    }
    public void OnAuthError(string error)
    {
        Debug.LogError(error);
    }
    public void CheckEmailForAuth(string _email,string _pass,string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;
        FirebaseAuth.CheckEmail(_email, gameObject.name, "OnEmailCheck", "OnEmailCheckError");
    }
    public void OnEmailCheck(string info)
    {
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.EmailAlreadyExisted();
    }
    public void OnEmailCheckError(string error)
    {
        if (error.Contains("Email Not Registered"))
        {
            CreateNewUser(Constants.SavedEmail, Constants.SavedPass);
        }else
        {
            Debug.LogError("Email check error : "+error);
        }
    }
    public void CreateNewUser(string _email,string _pass)
    {
        FirebaseAuth.CreateUserWithEmailAndPassword(_email,_pass, gameObject.name, "OnCreateUser", "OnCreateUserError");
    }
    public void OnCreateUser(string info)
    {
        if(MainMenuViewController.Instance)
        {
            UID = info;
            StartCoroutine(CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
            SendVerEmail();
            MainMenuViewController.Instance.ShowToast(4f, "Verification link sent to entered email address, please check inbox (or spam) and click on link to verify then login.");
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            Invoke("CallWithDelay", 3f);
        }else
        {
            Debug.LogError("MMVC is null for OnCreateUser");
        }
    }
    public void CallWithDelay()
    {
        MainMenuViewController.Instance.DisableRegisterScreen();
    }
    public void OnCreateUserError(string error)
    {
        Debug.LogError("Create user error : "+error);
    }
    public void LoginUser(string _email, string _pass,string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;

        //Login with Firebase SDK and API
        if(Constants.isUsingFirebaseSDK)
            FirebaseAuth.SignInWithEmailAndPassword(_email, _pass, gameObject.name, "OnLoginUser", "OnLoginUserError");
        else
            apiRequestHandler.Instance.signInWithEmail(_email, _pass);
    }
    public void CheckVerification()
    {
        FirebaseAuth.CheckEmailVerification(gameObject.name, "OnCheckEmail", "OnCheckEmail");
    }
    public void OnCheckEmail(string info)
    {
        if (info == "true")
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.OnLoginSuccess(false);
        }
        else
        {
            MainMenuViewController.Instance.ShowResendScreen(5f);
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ResetRegisterFields();
        }
    }
    public void OnLoginUser(string info)
    { 
        CheckVerification();
    }
    public void OnLoginUserError(string error)
    {
        Debug.LogError("Login User error: "+error);
        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.SomethingWentWrong();
    }
    public void LogoutUser()
    {
        ResetStorage();

        if (MainMenuViewController.Instance)
        {
            MainMenuViewController.Instance.EnableRegisterLogin();
            MainMenuViewController.Instance.ToggleMainMenuScreen(false);
        }

        if(Constants.isUsingFirebaseSDK)
            FirebaseAuth.SignOut(gameObject.name, "OnSignOut", "OnSignOutError");
    }
    public void ResetStorage()
    {
        string _json = "";
        SetLocalStorage(Constants.CredKey, _json);
        SetLocalStorage(Constants.WalletAccoutKey, _json);
    }
    public void OnSignOut(string info)
    {
        //Debug.Log(info);
    }
    public void OnSignOutError(string info)
    {
        Debug.LogError("Logout User error : "+info);
    }
    public void OnAuthChanged()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        FirebaseAuth.OnAuthStateChanged(gameObject.name, "OnAuthChangedSuccess", "OnAuthChangedError");
        #endif
    }
    public void OnAuthChangedSuccess(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
        UID = parsedUser.uid;
    }
    public void OnAuthChangedError(string info)
    {
        UID = "";
        Debug.LogError("Auth change error : "+info);
    }
    public void AddFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.SetDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnAddData", "OnAddDataError");
    }
    public void OnAddData(string info)
    {
       // Debug.Log("Data successfully added on firestore");
    }
    public void OnAddDataError(string error)
    {
        Debug.LogError("firestore data add error: "+error);
    }
    public void GetFireStoreData(string _collectionID,string _docID)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;
        FirebaseFirestore.GetDocument(_collectionID, _docID, gameObject.name, "OnDocGet", "OnDocGetError");
    }
    public void OnDocGet(string info)
    {
        if (info == null || info=="null")
        {
            UserDataFetched = false;
            DataFetchError = true;
            DocFetched = false;
            ResultFetched = true;
            FetchUserData = true;
            Debug.LogError("doc was fetched but is null");
        }
        else
        {
            Debug.Log("doc was fetched successfully from firestore");
            DataFetchError = false;
            PlayerData = JsonConvert.DeserializeObject<UserData>(info);
            UserDataFetched = true;
            DocFetched = true;
            ResultFetched = true;
            FetchUserData = true;
        }
    }
    public void OnDocGetError(string error)
    {
        UserDataFetched = false;
        DataFetchError = true;
        DocFetched = false;
        ResultFetched = true;
        FetchUserData = true;
        Debug.Log("Doc fetching error : "+error);
    }
    public IEnumerator FetchUserDB(string _walletID, string _username)
    {
        if (Constants.isUsingFirebaseSDK)
        {
            UserDataFetched = false;
            FetchUserData = false;
            GetFireStoreData(DocPath, _walletID);
        }
        else
        {
            UserDataFetched = true;
            FetchUserData = true;
        }

        yield return new WaitUntil(() => FetchUserData == true);
        if (UserDataFetched)
        {
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;
            Constants.TotalWins = PlayerData.TotalWins;

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);

            if (Constants.PushingTime)
            {
                Constants.PushingTime = false;
                GamePlayUIHandler.Instance.SubmitTime();
            }
        }
        else
        {
            Debug.LogError("something went wrong with user data fetching, trying again");
            StartCoroutine(FetchUserDB(PlayerPrefs.GetString("Account"), ""));
            yield return null;
        }
    }
    public IEnumerator CheckCreateUserDB(string _walletID,string _username)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;

        if (TournamentManager.Instance)
            TournamentManager.Instance.GetTournamentDataDB();

        if(Constants.isUsingFirebaseSDK)
            GetFireStoreData(DocPath, _walletID);

        yield return new WaitUntil(() => ResultFetched == true);

        if (DocFetched == true) //document existed
        {
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;
            Constants.TotalWins = PlayerData.TotalWins;

        }
        else
        {
            PlayerData = new UserData();
            PlayerData.WalletAddress = _walletID;
            PlayerData.UserName = Constants.SavedUserName;
            Constants.UserName = PlayerData.UserName;
            PlayerData.TotalWins = Constants.TotalWins;
            PlayerData.PassBought = false;
            PlayerData.TimeSeconds = 0;
            PlayerData.UID = UID;
            PlayerData.NumberOfTries = 0;
            PlayerData.NumberOfTriesPractice = 0;
            PlayerData.Email = Constants.SavedEmail;
            PlayerData.TournamentEndDate = null;
            PlayerData.AvatarID = Constants.FlagSelectedIndex;

            PlayerData.GNumberOfTries = 0;
            PlayerData.GPassBought = false;
            PlayerData.GTimeSeconds = 0;
            PlayerData.GTournamentEndDate = null;
            PlayerData.VC_Amount = 0;

            if(Constants.isUsingFirebaseSDK)
                AddFireStoreData(PlayerData);
        }

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);

        if(Constants.PushingTime)
        {
            Constants.PushingTime = false;
            GamePlayUIHandler.Instance.SubmitTime();
        }
    }
    public void SendVerEmail()
    {
        FirebaseAuth.SendEmailVerification(gameObject.name, "OnEmailSent", "OnEmailSentError");
    }
    public void OnEmailSent(string info)
    {  
    }
    public void OnEmailSentError(string info)
    {
        Debug.LogError("Sending Verfication email error: "+info);
        //Invoke("SendVerEmail", 1f);
    }
    public IEnumerator CheckWalletDB(string _walletID)
    {
        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);

        if (MainMenuViewController.Instance)
        {
            if (DocFetched == true) //document existed
                MainMenuViewController.Instance.DBChecked(true);
            else
                MainMenuViewController.Instance.DBChecked(false);
        }
        else
        {
            Debug.LogError("MMVC is null for CheckWalletDB");
        }
    }
    public void UpdatedFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        if (Constants.isUsingFirebaseSDK)
        {
            FirebaseFirestore.UpdateDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnDocUpdate",
                "OnDocUpdateError");
        }
        else
        {
            apiRequestHandler.Instance.updatePlayerData();
        }
    }
    public void OnDocUpdate(string info)
    {
        if(Constants.PushingTries)
        {
            Constants.PushingTries = false;
            return;
        }

        if (Constants.PushingWins)
        {
            Constants.PushingWins = false;
            return;
        }

        if (RaceManager.Instance)
            RaceManager.Instance.RaceEnded();
    }
    public void OnDocUpdateError(string error)
    {
        Debug.LogError("Doc update error : "+error);
    }
    public void QueryDB(string _field, string _type,bool IsSecondTour)
    {
        if (Constants.isUsingFirebaseSDK)
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            FirebaseFirestore.QueryDB(DocPath, _field, _type, gameObject.name, "OnQueryUpdate", "OnQueryUpdateError");
        #endif
        }
        else
        {
            //Send Leaderboard request to api
            apiRequestHandler.Instance.getLeaderboard(IsSecondTour);
        }
    }
    public void OnQueryUpdate(string info,bool IsSecondTour)
    {
        PlayerDataArray = JsonConvert.DeserializeObject<UserData[]>(info);
        LeaderboardManager.Instance.PopulateLeaderboardData(PlayerDataArray, IsSecondTour);
    }
    public void OnQueryUpdateError(string error)
    {
        Debug.LogError("Leaderboard query error : "+error);
    }
    public string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }
    public void SendPasswordResetEmail(string _email)
    {
        Constants.EmailSent = _email;
        if(Constants.isUsingFirebaseSDK)
            FirebaseAuth.SendPasswordResetEmail(_email,gameObject.name, "OnPassEmailSent", "OnPassEmailSentError");
        else
        {
            apiRequestHandler.Instance.onForgetPassword(_email);
        }
    }
    public void OnPassEmailSent(string info)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f,"Reset password link sent to entered email address, please click the link in inbox to reset password.");
    }
    public void OnPassEmailSentError(string info)
    {
        Debug.LogError("Password resent sending error : "+info);
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending password reset link, please try again later.");
    }
    public void CallOnError()
    {
        SendPasswordResetEmail(Constants.EmailSent);
    }
    public void showVerificationScreen()
    {
        MainMenuViewController.Instance.ShowResendScreen(5f);
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.ResetRegisterFields();
    }
    public void ResendVerificationEmail()
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        if(Constants.isUsingFirebaseSDK)
            FirebaseAuth.SendEmailVerification(gameObject.name, "ResendEmailSent", "ResendEmailSentError");
        else
        {
            apiRequestHandler.Instance.sendVerificationAgain();
        }
    }
    public void ResendEmailSent(string info)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.DisableResendScreen();
        MainMenuViewController.Instance.ShowToast(4f, "Confirmation link sent again, please click the link in inbox (or spam) to confirm.");
    }
    public void ResendEmailSentError(string info)
    {
        Debug.LogError("Resend verification email error : "+info);
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.DisableResendScreen();
        MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending confirmation link, please try again later.");
    }
    #endregion

    #region Mechanics

    public void ResetNFTData()
    {
        PlayerPrefs.DeleteKey("NFTLocalData");
        GetNFTData();
        MainMenuViewController.Instance.ShowToast(3f, "NFT data was reset, game will be restarted.", false);
        Invoke("RestartGame", 3.1f);
    }

    public void RestartGame()
    {
        WalletManager.Instance.RestartGame();
    }
    public void StoreNFTLocally(string _data)
    {
        PlayerPrefs.SetString("NFTLocalData", _data);
    }
    public string GetNFTLocally()
    {
        return PlayerPrefs.GetString("NFTLocalData", "");
    }
    public NFTMehanicsData GetMechanics(int key)
    {
        return NFTMehanics[key];
    }
    public void UpdateMechanics(int key, NFTMehanicsData _data)
    {
        if (key == 0)
            return;

        if (NFTMehanics.ContainsKey(key))
        {
            NFTMehanics[key] = _data;
            SaveNFTData(key, _data);
        }
        else
        {
            Debug.Log("NO data for NDT was updated as key dpes not exist");
        }
    }
    async public void SaveNFTData(int key=0, NFTMehanicsData _data=null)
    {
        if (Constants.DebugAllCars)
        {
            string _json = JsonConvert.SerializeObject(NFTMehanics);
            StoreNFTLocally(_json);
        }
        else
        {
            string _json = JsonConvert.SerializeObject(_data.mechanicsData);
            bool isDone = await apiRequestHandler.Instance.ProcessNFTUpdateDataRequest(key.ToString(), _data.PlayerName, _json);

            if (isDone)
                Debug.Log("updated successfully");
            else
                Debug.Log("updating failed");
        }
    }
    async public void GetNFTData()
    {
        Constants.GetMoralisData = false;
        NFTMehanics.Clear();
        NFTMehanicsData _data = new NFTMehanicsData();
        _data.OwnerWalletAddress = "testone";
        _data.PlayerName = "rider";
        _data.mechanicsData = new MechanicsData("Bolt",100, 0, 0, 0);
        NFTMehanics.Add(0, _data);

        //this is function that will be used to populate data from moralis
        if (Constants.DebugAllCars)
        {
            if (GetNFTLocally() == "")
            {
                for (int i = 0; i < NFTGameplayManager.Instance.DataNFTModel.Count; i++)
                {
                    NFTMehanicsData _newData = new NFTMehanicsData();
                    _newData.OwnerWalletAddress = "testone";
                    _data.PlayerName = PlayerData.UserName;
                    _newData.mechanicsData = new MechanicsData(NFTGameplayManager.Instance.DataNFTModel[i].name,100, 0, 0, 0);
                    NFTMehanics.Add(i + 1, _newData);
                }

                SaveNFTData();
                Debug.Log("NFT DATA STORED");

                foreach (var item in NFTMehanics)
                {
                    Debug.Log(item.Key);
                    Debug.Log(item.Value.mechanicsData.CarName);
                }
            }
            else
            {
                Debug.Log("NFT DATA Retrieved");
                NFTMehanics = JsonConvert.DeserializeObject<Dictionary<int, NFTMehanicsData>>(GetNFTLocally());
            }
        }
        else
        {
            if (Constants.CheckAllNFT)
            {
                TokenPayload.Clear();
                TokenName.Clear();
                for (int i = 0; i < Constants.TokenNFT.Count; i++)
                {
                    for (int j = 0; j < Constants.TokenNFT[i].ID.Count; j++)
                    {
                        TokenPayload.Add(Constants.TokenNFT[i].ID[j].ToString());
                        TokenName.Add(Constants.TokenNFT[i].ID[j], Constants.TokenNFT[i].Name);
                    }
                }

                string _response = await apiRequestHandler.Instance.ProcessNFTDataArrayRequest(TokenPayload);

                if (_response != "" && !string.IsNullOrEmpty(_response))
                {
                    MoralisNFTArrayResponse _dataNEW = new MoralisNFTArrayResponse();
                    _dataNEW = JsonConvert.DeserializeObject<MoralisNFTArrayResponse>(_response);

                    for (int i = 0; i < _dataNEW.result.Count; i++)
                    {
                        NFTMehanicsData _newData = new NFTMehanicsData();
                        _newData.OwnerWalletAddress = _dataNEW.result[i].ownerWallet;
                        _newData.PlayerName = PlayerData.UserName;

                        string _carName = TokenName[int.Parse(_dataNEW.result[i].tokenId)];
                        int _carHealth = 100;
                        float _carTyreLaps = 0;
                        float _carOilLaps = 0;
                        float _carGasLaps = 0;

                        if (!string.IsNullOrEmpty(_dataNEW.result[i].mechanics))
                        {
                            JToken Jresponse = JObject.Parse(_dataNEW.result[i].mechanics);

                            //_carName = Jresponse.SelectToken("CarName") != null ? (string)Jresponse.SelectToken("CarName") : "";
                            _carHealth = Jresponse.SelectToken("CarHealth") != null ? (int)Jresponse.SelectToken("CarHealth") : 100;
                            _carTyreLaps = Jresponse.SelectToken("Tyre_Laps") != null ? (float)Jresponse.SelectToken("Tyre_Laps") : 0;
                            _carOilLaps = Jresponse.SelectToken("EngineOil_Laps") != null ? (float)Jresponse.SelectToken("EngineOil_Laps") : 0;
                            _carGasLaps = Jresponse.SelectToken("Gas_Laps") != null ? (float)Jresponse.SelectToken("Gas_Laps") : 0;
                        }

                        _newData.mechanicsData = new MechanicsData(_carName, _carHealth, _carTyreLaps, _carOilLaps, _carGasLaps);
                        NFTMehanics.Add(int.Parse(_dataNEW.result[i].tokenId), _newData);
                    }

                    Constants.GetMoralisData = true;
                }
                else
                {
                    Debug.LogError("Empty data received");
                    Constants.GetMoralisData = true;
                }
            }
            else
            {
                Invoke(nameof(GetNFTData), 0.5f);
            }
        }
    }
    #endregion

    #region Moralis
    async public void GetAssignStoreData()
    {
        CarDealer.Clear();
        string _data = await apiRequestHandler.Instance.ProcessAllStoreRequest();

        if(!string.IsNullOrEmpty(_data))
        {
            MoralisStoreResponse _storeData = JsonConvert.DeserializeObject<MoralisStoreResponse>(_data);
            for (int i = 0; i < _storeData.result.Count; i++)
            {
                for (int j = 0; j < NFTGameplayManager.Instance.DataNFTModel.Count; j++)
                {
                    if(_storeData.result[i].Id== NFTGameplayManager.Instance.DataNFTModel[j].MetaID)
                    {
                        JToken Dresponse = JObject.Parse(_storeData.result[i].mechanics);
                        //Debug.Log(Dresponse.SelectToken("Stats"));
                        StatSettings _stats = ScriptableObject.CreateInstance<StatSettings>();
                        _stats.CarStats = new BaseStats();
                        _stats.name = _storeData.result[i].name;

                        _stats.CarStats.Name = Dresponse.SelectToken("Stats").SelectToken("Name") != null ? (string)Dresponse.SelectToken("Stats").SelectToken("Name") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Name;
                        _stats.CarStats.Acceleration= Dresponse.SelectToken("Stats").SelectToken("Acceleration") != null ? (double)Dresponse.SelectToken("Stats").SelectToken("Acceleration") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Acceleration;
                        _stats.CarStats.TopSpeed = Dresponse.SelectToken("Stats").SelectToken("TopSpeed") != null ? (double)Dresponse.SelectToken("Stats").SelectToken("TopSpeed") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.TopSpeed;
                        _stats.CarStats.Cornering = Dresponse.SelectToken("Stats").SelectToken("Cornering") != null ? (double)Dresponse.SelectToken("Stats").SelectToken("Cornering") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Cornering;
                        _stats.CarStats.HP = Dresponse.SelectToken("Stats").SelectToken("HP") != null ? (double)Dresponse.SelectToken("Stats").SelectToken("HP") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.HP;
                        _stats.CarStats.Price = Dresponse.SelectToken("Stats").SelectToken("Price") != null ? (int)Dresponse.SelectToken("Stats").SelectToken("Price") : NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Price;
                        
                        int Tier= Dresponse.SelectToken("Stats").SelectToken("Tier") != null ? (int)Dresponse.SelectToken("Stats").SelectToken("Tier") : (int)NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Tier;
                        _stats.CarStats.Tier = (CarTier) Tier;

                        int Type = Dresponse.SelectToken("Stats").SelectToken("Type") != null ? (int)Dresponse.SelectToken("Stats").SelectToken("Type") : (int)NFTGameplayManager.Instance.DataNFTModel[j].settings.CarStats.Type;
                        _stats.CarStats.Type = (CarType)Type;

                        _stats.CarStats.Settings = MechanicsManager.Instance._consumableSettings;

                        //Debug.Log("Tier : " + Tier + " CAR : " + _stats.CarStats.Name);
                        SetDealerDic(Tier, _stats);
                        NFTGameplayManager.Instance.DataNFTModel[j].settings = _stats;
                        break;                    }
                }
            }

            logDic();
        }
    }

    public void SetDealerDic(int _key,StatSettings _settings)
    {    
        if (CarDealer.ContainsKey(_key))
        {
            List<StatSettings> _temp= CarDealer[_key];
            _temp.Add(_settings);
            CarDealer[_key] = _temp;
        }
        else
        {
            List<StatSettings> _temp = new List<StatSettings>();
            _temp.Add(_settings);
            CarDealer.Add(_key, _temp);
        }
    }

    public void logDic()
    {
        string _json = JsonConvert.SerializeObject(CarDealer);
        Debug.Log(_json);
    }
    #endregion
}
