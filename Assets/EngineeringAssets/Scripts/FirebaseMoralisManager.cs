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
public class Stats
{
    public string Name { get; set; }
    public double Acceleration { get; set; }
    public double TopSpeed { get; set; }
    public double Cornering { get; set; }
    public double HP { get; set; }
    public int Price { get; set; }
    public int Tier { get; set; }
    public int Type { get; set; }
}

[System.Serializable]
public class MechanicsData
{
    public string CarName;
    public int CarHealth;
    public float Tyre_Laps;
    public float EngineOil_Laps;
    public float Gas_Laps;
    public Stats Stats { get; set; }

    public MechanicsData(string _carName,int carHealth,float tyre_Laps,float engineOil_Laps,float gas_Laps, Stats _stats)
    {
        this.CarName = _carName;
        this.CarHealth = carHealth;
        this.Tyre_Laps = tyre_Laps;
        this.EngineOil_Laps = engineOil_Laps;
        this.Gas_Laps = gas_Laps;
        this.Stats = _stats;
    }
}

[System.Serializable]
public class NFTMehanicsData
{
    public string PlayerName;
    public string OwnerWalletAddress;
    public string MetaData;
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

    string _carName = "";
    string _carNameStats = "";
    int _carHealth = 0;
    float _carTyreLaps = 0;
    float _carOilLaps = 0;
    float _carGasLaps = 0;
    double _acceleration = 0;
    double _topSpeed = 0;
    double _cornering = 0;
    double _hp = 0;
    int _price = 0;
    int _tier = 0;
    int _type = 0;
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
        } else
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
        Constants.PrintLog("UserBo response : " + _response);
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

        //Constants.PrintLog(PlayerData.Mechanics.VC_Amount);
        //Constants.PrintLog(PlayerData.Mechanics.CarHealth);
        //Constants.PrintLog(PlayerData.Mechanics.Tyre_Laps);
        //Constants.PrintLog(PlayerData.Mechanics.EngineOil_Laps);
        //Constants.PrintLog(PlayerData.Mechanics.Gas_Laps);

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
    public void SetLocalStorage(string key, string data)
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
        Constants.PrintLog(info);        
    }
    public void OnAuthError(string error)
    {
        Constants.PrintError(error);
    }
    public void CheckEmailForAuth(string _email, string _pass, string _username)
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
        } else
        {
            Constants.PrintError("Email check error : " + error);
        }
    }
    public void CreateNewUser(string _email, string _pass)
    {
        FirebaseAuth.CreateUserWithEmailAndPassword(_email, _pass, gameObject.name, "OnCreateUser", "OnCreateUserError");
    }
    public void OnCreateUser(string info)
    {
        if (MainMenuViewController.Instance)
        {
            UID = info;
            StartCoroutine(CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
            SendVerEmail();
            MainMenuViewController.Instance.ShowToast(4f, "Verification link sent to entered email address, please check inbox (or spam) and click on link to verify then login.");
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            Invoke("CallWithDelay", 3f);
        } else
        {
            Constants.PrintError("MMVC is null for OnCreateUser");
        }
    }
    public void CallWithDelay()
    {
        MainMenuViewController.Instance.DisableRegisterScreen();
    }
    public void OnCreateUserError(string error)
    {
        Constants.PrintError("Create user error : " + error);
    }
    public void LoginUser(string _email, string _pass, string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;

        //Login with Firebase SDK and API
        if (Constants.isUsingFirebaseSDK)
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
        Constants.PrintError("Login User error: " + error);
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

        if (Constants.isUsingFirebaseSDK)
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
        Constants.PrintLog(info);
    }
    public void OnSignOutError(string info)
    {
        Constants.PrintError("Logout User error : " + info);
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
        Constants.PrintError("Auth change error : " + info);
    }
    public void AddFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.SetDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnAddData", "OnAddDataError");
    }
    public void OnAddData(string info)
    {
        Constants.PrintLog("Data successfully added on firestore");
    }
    public void OnAddDataError(string error)
    {
        Constants.PrintError("firestore data add error: " + error);
    }
    public void GetFireStoreData(string _collectionID, string _docID)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;
        FirebaseFirestore.GetDocument(_collectionID, _docID, gameObject.name, "OnDocGet", "OnDocGetError");
    }
    public void OnDocGet(string info)
    {
        if (info == null || info == "null")
        {
            UserDataFetched = false;
            DataFetchError = true;
            DocFetched = false;
            ResultFetched = true;
            FetchUserData = true;
            Constants.PrintError("doc was fetched but is null");
        }
        else
        {
            Constants.PrintLog("doc was fetched successfully from firestore");
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
        Constants.PrintLog("Doc fetching error : " + error);
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
            Constants.PrintError("something went wrong with user data fetching, trying again");
            StartCoroutine(FetchUserDB(PlayerPrefs.GetString("Account"), ""));
            yield return null;
        }
    }
    public IEnumerator CheckCreateUserDB(string _walletID, string _username)
    {
        DataFetchError = false;
        DocFetched = false;
        ResultFetched = false;

        if (TournamentManager.Instance)
            TournamentManager.Instance.GetTournamentDataDB();

        if (Constants.isUsingFirebaseSDK)
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

            if (Constants.isUsingFirebaseSDK)
                AddFireStoreData(PlayerData);
        }

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.ChangeUserNameText(Constants.UserName);

        if (Constants.PushingTime)
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
        Constants.PrintError("Sending Verfication email error: " + info);
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
            Constants.PrintError("MMVC is null for CheckWalletDB");
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
        if (Constants.PushingTries)
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
        Constants.PrintError("Doc update error : " + error);
    }
    public void QueryDB(string _field, string _type, bool IsSecondTour)
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
    public void OnQueryUpdate(string info, bool IsSecondTour)
    {
        PlayerDataArray = JsonConvert.DeserializeObject<UserData[]>(info);
        LeaderboardManager.Instance.PopulateLeaderboardData(PlayerDataArray, IsSecondTour);
    }
    public void OnQueryUpdateError(string error)
    {
        Constants.PrintError("Leaderboard query error : " + error);
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
        if (Constants.isUsingFirebaseSDK)
            FirebaseAuth.SendPasswordResetEmail(_email, gameObject.name, "OnPassEmailSent", "OnPassEmailSentError");
        else
        {
            apiRequestHandler.Instance.onForgetPassword(_email);
        }
    }
    public void OnPassEmailSent(string info)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f, "Reset password link sent to entered email address, please click the link in inbox to reset password.");
    }
    public void OnPassEmailSentError(string info)
    {
        Constants.PrintError("Password resent sending error : " + info);
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
        if (Constants.isUsingFirebaseSDK)
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
        Constants.PrintError("Resend verification email error : " + info);
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

    public void SetMechanics(int key, NFTMehanicsData _data,bool _canUpdate=false)
    {
        if (!NFTMehanics.ContainsKey(key))
        {
            NFTMehanics.Add(key, _data);
        } else
        {
            Constants.PrintLog("key already existed");

            if (_canUpdate)
            {
                NFTMehanics[key] = _data;
                Constants.PrintLog("updating key data");
            }
        }
    }
    public void UpdateMechanics(int key, NFTMehanicsData _data, bool _pushData = true)
    {
        if (key == 0)
            return;

        if (NFTMehanics.ContainsKey(key))
        {
            NFTMehanics[key] = _data;

            if (_pushData || Constants.DebugAllCars)
                SaveNFTData(key, _data);
        }
        else
        {
            Constants.PrintLog("NO data for NDT was updated as key dpes not exist");
        }
    }
    async public void SaveNFTData(int key = 0, NFTMehanicsData _data = null)
    {
        if (Constants.DebugAllCars)
        {
            string _json = JsonConvert.SerializeObject(NFTMehanics);
            StoreNFTLocally(_json);
        }
        else
        {
            string _json = JsonConvert.SerializeObject(_data.mechanicsData);
            bool isDone = await apiRequestHandler.Instance.ProcessNFTUpdateDataRequest(key.ToString(), _data.mechanicsData.CarName, _json,_data.MetaData);

            if (isDone)
                Constants.PrintLog("updated successfully");
            else
                Constants.PrintLog("updating failed");
        }
    }
    async public void GetNFTData()
    {
        Constants.GetMoralisData = false;
        NFTMehanics.Clear();
        NFTMehanicsData _data = new NFTMehanicsData();
        _data.OwnerWalletAddress = "testone";
        _data.PlayerName = "rider";

        Stats _settings = new Stats();
        _settings.Name = "Bolt";
        _settings.Acceleration = 100;
        _settings.TopSpeed = 100;
        _settings.Cornering = 100;
        _settings.HP = 100;
        _settings.Price = 0;
        _settings.Tier = 0;
        _settings.Type = 1;

        _data.mechanicsData = new MechanicsData("Bolt", 100, 0, 0, 0, _settings);
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

                    Stats _Statsettings = new Stats();
                    _Statsettings.Name = NFTGameplayManager.Instance.DataNFTModel[i].name;
                    _Statsettings.Acceleration = NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.Acceleration;
                    _Statsettings.TopSpeed = NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.TopSpeed;
                    _Statsettings.Cornering = NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.Cornering;
                    _Statsettings.HP = NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.HP;
                    _Statsettings.Price = NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.Price;
                    _Statsettings.Tier =(int)NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.Tier;
                    _Statsettings.Type = (int)NFTGameplayManager.Instance.DataNFTModel[i].settings.CarStats.Type;

                    _newData.mechanicsData = new MechanicsData(NFTGameplayManager.Instance.DataNFTModel[i].name, 100, 0, 0, 0, _Statsettings);
                    NFTMehanics.Add(i + 1, _newData);
                }

                SaveNFTData();
                Constants.PrintLog("NFT DATA STORED");

                foreach (var item in NFTMehanics)
                {
                    Constants.PrintLog(item.Key.ToString());
                    Constants.PrintLog(item.Value.mechanicsData.CarName);
                }
            }
            else
            {
                Constants.PrintLog("NFT DATA Retrieved");
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

                Constants.StoredCarNamesMoralis.Clear();
                string _NFTresponse = await apiRequestHandler.Instance.ProcessAllMyNFTRequest(Constants.WalletAddress);
                StoreDataforOffchain(_NFTresponse);

                string _response = await apiRequestHandler.Instance.ProcessNFTDataArrayRequest(TokenPayload);
                UpdateCarPurchasedData(_response);
            }
            else
            {
                Invoke(nameof(GetNFTData), 0.5f);
            }
        }
    }

    public void StoreDataforOffchain(string _NFTresponse)
    {
        if (!string.IsNullOrEmpty(_NFTresponse))
        {
            MoralisNFTArrayResponse _dataNEW = new MoralisNFTArrayResponse();
            _dataNEW = JsonConvert.DeserializeObject<MoralisNFTArrayResponse>(_NFTresponse);


            for (int i = 0; i < _dataNEW.result.Count; i++)
            {
                if (!string.IsNullOrEmpty(_dataNEW.result[i].name))
                {
                    for (int k = 0; k < NFTGameplayManager.Instance.DataNFTModel.Count; k++)
                    {
                        if (_dataNEW.result[i].name.ToLower() == NFTGameplayManager.Instance.DataNFTModel[k].name.ToLower())
                        {
                            if (Constants.StoredCarNames.Contains(_dataNEW.result[i].name))
                            {
                                Constants.PrintLog("sorting data offchain, car "+ _dataNEW.result[i].tokenId +" already added from wallet, skipping....");
                            }
                            else
                            {
                                Constants.PrintLog("adding from offchain : "+ _dataNEW.result[i].name+" "+ _dataNEW.result[i].tokenId);
                                AddMoralisCarInfo(_dataNEW.result[i].name, _dataNEW.result[i].tokenId);
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddMoralisCarInfo(string carName,string carID)
    {
        Constants.StoredCarNamesMoralis.Add(carName);
        WalletManager.Instance.StoreNameWithToken(carName, int.Parse(carID));
        TokenPayload.Add(carID);
        TokenName.Add(int.Parse(carID), carName);
    }

    public void UpdateCarPurchasedData(string _response)
    {
        if (_response != "" && !string.IsNullOrEmpty(_response))
        {
            MoralisNFTArrayResponse _dataNEW = new MoralisNFTArrayResponse();
            _dataNEW = JsonConvert.DeserializeObject<MoralisNFTArrayResponse>(_response);

            for (int i = 0; i < _dataNEW.result.Count; i++)
            {
                NFTMehanicsData _newData = new NFTMehanicsData();
                _newData.OwnerWalletAddress = _dataNEW.result[i].ownerWallet;
                _newData.PlayerName = PlayerData.UserName;

                 _carName = _dataNEW.result[i].name;
                 _carNameStats = "";
                 _carHealth = Constants.MaxCarHealth;
                 _carTyreLaps = 0;
                 _carOilLaps = 0;
                 _carGasLaps = 0;
                 _acceleration = 0;
                 _topSpeed = 0;
                 _cornering = 0;
                 _hp = 0;
                 _price = 0;
                 _tier = 0;
                 _type = 0;

                if (!string.IsNullOrEmpty(_dataNEW.result[i].mechanics))
                {
                    JToken Jresponse = JObject.Parse(_dataNEW.result[i].mechanics);

                    _carHealth = Jresponse.SelectToken("CarHealth") != null ? (int)Jresponse.SelectToken("CarHealth") : Constants.MaxCarHealth;
                    _carTyreLaps = Jresponse.SelectToken("Tyre_Laps") != null ? (float)Jresponse.SelectToken("Tyre_Laps") : 0;
                    _carOilLaps = Jresponse.SelectToken("EngineOil_Laps") != null ? (float)Jresponse.SelectToken("EngineOil_Laps") : 0;
                    _carGasLaps = Jresponse.SelectToken("Gas_Laps") != null ? (float)Jresponse.SelectToken("Gas_Laps") : 0;
                    _carNameStats = Jresponse.SelectToken("Stats").SelectToken("Name") != null ? (string)Jresponse.SelectToken("Stats").SelectToken("Name") : _carName;
                    _acceleration = Jresponse.SelectToken("Stats").SelectToken("Acceleration") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("Acceleration") : 100;
                    _topSpeed = Jresponse.SelectToken("Stats").SelectToken("TopSpeed") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("TopSpeed") : 100;
                    _cornering = Jresponse.SelectToken("Stats").SelectToken("Cornering") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("Cornering") : 100;
                    _hp = Jresponse.SelectToken("Stats").SelectToken("HP") != null ? (double)Jresponse.SelectToken("Stats").SelectToken("HP") : Constants.MaxCarHealth;
                    _price = Jresponse.SelectToken("Stats").SelectToken("Price") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Price") : 250;
                    _tier = Jresponse.SelectToken("Stats").SelectToken("Tier") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Tier") : 2;
                    _type = Jresponse.SelectToken("Stats").SelectToken("Type") != null ? (int)Jresponse.SelectToken("Stats").SelectToken("Type") : 0;
                }

                _newData.MetaData = _dataNEW.result[i].metadata;

                Stats _MoralisStatsettings = new Stats();
                _MoralisStatsettings.Name = _carNameStats;
                _MoralisStatsettings.Acceleration = _acceleration;
                _MoralisStatsettings.TopSpeed = _topSpeed;
                _MoralisStatsettings.Cornering = _cornering;
                _MoralisStatsettings.HP = _hp;
                _MoralisStatsettings.Price = _price;
                _MoralisStatsettings.Tier = _tier;
                _MoralisStatsettings.Type = _type;

                _newData.mechanicsData = new MechanicsData(_carName, _carHealth, _carTyreLaps, _carOilLaps, _carGasLaps, _MoralisStatsettings);
                NFTMehanics.Add(int.Parse(_dataNEW.result[i].tokenId), _newData);
            }

            Constants.GetMoralisData = true;

            Constants.PrintLog("finished UpdateCarPurchasedData");
        }
        else
        {
            Constants.PrintError("Empty data received");
            Constants.GetMoralisData = true;
        }
    }
    #endregion

    #region Moralis
    async public void GetAssignStoreData()
    {
        if(!Constants.GetSecKey)
        {
            Invoke(nameof(GetAssignStoreData), 0.5f);
            return;
        }

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
                        StatSettings _stats = ScriptableObject.CreateInstance<StatSettings>();
                        _stats.CarStats = new BaseStats();
                        _stats.name = _storeData.result[i].name;
                        _stats.CarStats.ID = _storeData.result[i].Id;
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

                        SetDealerDic(Tier, _stats);
                        NFTGameplayManager.Instance.DataNFTModel[j].settings = _stats;
                        break;                    }
                }
            }

            logDic();
        }
    }

    async public void BuyCar(string _metaID, string _owneraddress)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        string _data = await apiRequestHandler.Instance.ProcessPurchaseCarServerRequest(_metaID, _owneraddress);

        if (!string.IsNullOrEmpty(_data))
        {
            JToken token = JObject.Parse(_data);

            string msg = token.SelectToken("result").SelectToken("message") != null ? (string)token.SelectToken("result").SelectToken("message") : "";

            if (msg.Contains("Successfully Purchased"))
            {
                Constants.VirtualCurrencyAmount = (float)token.SelectToken("result").SelectToken("VC_amount");
                string _CarName = (string)token.SelectToken("result").SelectToken("name");
                int _tokenId = (int)token.SelectToken("result").SelectToken("tokenId");

                StoreHandler.Instance.SetCCashText_StoreUI(Constants.VirtualCurrencyAmount); 
                StoreHandler.Instance.SetCCashText_Garage(Constants.VirtualCurrencyAmount);
                MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount);

                AddMoralisCarInfo(_CarName, _tokenId.ToString());

                List<string> _tokens = new List<string>();
                _tokens.Add(_tokenId.ToString());
                string _response = await apiRequestHandler.Instance.ProcessNFTDataArrayRequest(_tokens);
                UpdateCarPurchasedData(_response);

                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(4f, "Car was successfully purchased, you can view it in garage", true);
            }
            else
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong, please try again later.", false);
            }
        }
        else
            Constants.PrintError("Empty data received after purchase");
    }

    public void SetNewBoughtCarMechanics(string _car,int _tokenid,string _mechanics, JToken _response)
    {
        NFTMehanicsData _newData = new NFTMehanicsData();
        _newData.OwnerWalletAddress = Constants.WalletAddress;

         _carName = _car;
         _carHealth = Constants.MaxCarHealth;
         _carTyreLaps = 0;
         _carOilLaps = 0;
         _carGasLaps = 0;
         _carNameStats = "";
         _acceleration = 0;
         _topSpeed = 0;
         _cornering = 0;
         _hp = 0;
         _price = 0;
         _tier = 0;
         _type = 0;

        if (!string.IsNullOrEmpty(_mechanics))
        {
            _carHealth = _response.SelectToken("result").SelectToken("mechanics").SelectToken("CarHealth") != null ? (int)_response.SelectToken("result").SelectToken("mechanics").SelectToken("CarHealth") : Constants.MaxCarHealth;
            _carTyreLaps = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Tyre_Laps") != null ? (float)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Tyre_Laps") : 0;
            _carOilLaps = _response.SelectToken("result").SelectToken("mechanics").SelectToken("EngineOil_Laps") != null ? (float)_response.SelectToken("result").SelectToken("mechanics").SelectToken("EngineOil_Laps") : 0;
            _carGasLaps = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Gas_Laps") != null ? (float)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Gas_Laps") : 0;

            _carNameStats = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Name") != null ? (string)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Name") : _carName;
            _acceleration = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Acceleration") != null ? (double)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Acceleration") : 100;
            _topSpeed = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("TopSpeed") != null ? (double)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("TopSpeed") : 100;
            _cornering = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Cornering") != null ? (double)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Cornering") : 100;
            _hp = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("HP") != null ? (double)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("HP") : Constants.MaxCarHealth;
            _price = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Price") != null ? (int)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Price") : 250;
            _tier = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Tier") != null ? (int)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Tier") : 2;
            _type = _response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Type") != null ? (int)_response.SelectToken("result").SelectToken("mechanics").SelectToken("Stats").SelectToken("Type") : 0;

        }

        Stats _MoralisStatsettings = new Stats();
        _MoralisStatsettings.Name = _carNameStats;
        _MoralisStatsettings.Acceleration = _acceleration;
        _MoralisStatsettings.TopSpeed = _topSpeed;
        _MoralisStatsettings.Cornering = _cornering;
        _MoralisStatsettings.HP = _hp;
        _MoralisStatsettings.Price = _price;
        _MoralisStatsettings.Tier = _tier;
        _MoralisStatsettings.Type = _type;

        _newData.MetaData = "";
        _newData.mechanicsData = new MechanicsData(_carName, _carHealth, _carTyreLaps, _carOilLaps, _carGasLaps, _MoralisStatsettings);
        SetMechanics(_tokenid, _newData);
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

    public Dictionary<int,List<StatSettings>> GetDealerDic()
    {
        return CarDealer;
    }

    public void logDic()
    {
        string _json = JsonConvert.SerializeObject(CarDealer);
        Constants.PrintLog(_json);
    }

    async public void SetupUpGame_DD(string _roomID, string _playerID, string _address, string _token)
    {
        string _data = await apiRequestHandler.Instance.ProcessGameSetupRequest_DD(_roomID, _playerID, _address,_token);
        if (!string.IsNullOrEmpty(_data))
        {
            Debug.Log("Game Setup for DD completed");
        }
    }
    #endregion
}
