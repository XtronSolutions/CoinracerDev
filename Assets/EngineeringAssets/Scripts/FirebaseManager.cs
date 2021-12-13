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

    public int AvatarID { get; set; }
    public EndDate TournamentEndDate { get; set; }

    public Timestamp ProfileCreated { get; set; }
}

public class AuthCredentials
{
    public string Email { get; set; }
    public string Password { get; set; }

    public string UserName { get; set; }

    public string WalletAddress { get; set; }
}


public class FirebaseManager : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void SetStorage(string key, string val);

    private int key=129;
    private string UID = "";
    public UserData PlayerData;
    public UserData[] PlayerDataArray;
    //private DependencyStatus dependencyStatus;
    //private FirebaseFirestore DatabaseInstance;
    public static FirebaseManager Instance;

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
    void Start()
    {
        Credentails = new AuthCredentials();

        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //AuthenticateFirebase();
        OnAuthChanged();
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
        Debug.Log(info);        
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
        Debug.Log(info);

        Debug.Log("email existed");

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.EmailAlreadyExisted();
    }

    public void OnEmailCheckError(string error)
    {
        if (error.Contains("Email Not Registered"))
        {
            Debug.Log("no email existed, creating new user");
            CreateNewUser(Constants.SavedEmail, Constants.SavedPass);
        }else
        {
            Debug.LogError(error);
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
            FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
            
            if(MainMenuViewController.Instance)
            {
                SendVerEmail();
                MainMenuViewController.Instance.ShowToast(4f, "Verification link sent to entered email address, please check inbox (or spam) and click on link to verify then login.");
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                Invoke("CallWithDelay", 3f);
            }
            //MainMenuViewController.Instance.OnLoginSuccess(true);
        }

        Debug.Log("new user created with below UID");
        Debug.Log(info);
    }

    public void CallWithDelay()
    {
        MainMenuViewController.Instance.DisableRegisterScreen();
    }

    public void OnCreateUserError(string error)
    {
        Debug.LogError(error);
    }

    public void LoginUser(string _email, string _pass,string _username)
    {
        Credentails.Email = _email;
        Credentails.Password = _pass;
        Credentails.UserName = _username;
        FirebaseAuth.SignInWithEmailAndPassword(_email, _pass, gameObject.name, "OnLoginUser", "OnLoginUserError");
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

            Debug.Log(info);
        }
        else
        {
            MainMenuViewController.Instance.ShowResendScreen(5f);
            //MainMenuViewController.Instance.ShowToast(4f, "Email verification pending, please check your inbox and click on verify link.");
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ResetRegisterFields();
        }
    }

    public void OnLoginUser(string info)
    { 
        CheckVerification();
        Debug.Log(info);
    }

    public void OnLoginUserError(string error)
    {
        Debug.LogError(error);
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
        Debug.Log(info);
    }

    public void OnSignOutError(string info)
    {
        Debug.LogError(info);
    }


    public void OnAuthChanged()
    {
        FirebaseAuth.OnAuthStateChanged(gameObject.name, "OnAuthChangedSuccess", "OnAuthChangedError");
    }

    public void OnAuthChangedSuccess(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
        UID = parsedUser.uid;
        //DisplayData($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
    }

    public void OnAuthChangedError(string info)
    {
        UID = "";
        Debug.LogError(info);
    }

    public void AddFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.SetDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnAddData", "OnAddDataError");
    }
    public void OnAddData(string info)
    {
        Debug.Log("Data successfully added");
        //Debug.Log(info);
    }

    public void OnAddDataError(string error)
    {
        Debug.LogError(error);
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
        Debug.Log("doc was fetched successfully");

        if (info == null || info=="null")
        {
            UserDataFetched = false;
            DataFetchError = true;
            DocFetched = false;
            ResultFetched = true;
            FetchUserData = true;
            Debug.Log("info is null for OnDocGet");
        }
        else
        {
            DataFetchError = false;
            PlayerData = JsonConvert.DeserializeObject<UserData>(info);
            UserDataFetched = true;
            DocFetched = true;
            ResultFetched = true;
            FetchUserData = true;
           // Debug.Log(info);
            //Debug.Log("info is not null");
        }
    }

    public void OnDocGetError(string error)
    {
        UserDataFetched = false;
        DataFetchError = true;
        DocFetched = false;
        ResultFetched = true;
        FetchUserData = true;
        Debug.Log(error);
    }

    public IEnumerator FetchUserDB(string _walletID, string _username)
    {
        UserDataFetched = false;
        FetchUserData = false;
        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => FetchUserData == true);
        if (UserDataFetched)
        {
            Debug.Log("user already exists!");
            Debug.Log(_walletID);
            Debug.Log(PlayerData.WalletAddress);
            Debug.Log(PlayerData.UserName);
            Debug.Log(PlayerData.TimeSeconds);
            Debug.Log(PlayerData.UID);
            Debug.Log(PlayerData.NumberOfTries);
            Debug.Log(PlayerData.PassBought);
            Debug.Log(PlayerData.Email);
            Debug.Log(PlayerData.TournamentEndDate);
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;

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
            Debug.Log("something went wrong with data fetching, trying again");
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

        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);

        if (DocFetched == true) //document existed
        {
            Debug.Log("user already exists!");
            Debug.Log(_walletID);
            Debug.Log(PlayerData.WalletAddress);
            Debug.Log(PlayerData.UserName);
            Debug.Log(PlayerData.TimeSeconds);
            Debug.Log(PlayerData.UID);
            Debug.Log(PlayerData.NumberOfTries);
            Debug.Log(PlayerData.PassBought);
            Debug.Log(PlayerData.Email);
            Debug.Log(PlayerData.TournamentEndDate);
            Constants.UserName = PlayerData.UserName;
            Constants.FlagSelectedIndex = PlayerData.AvatarID;
        }
        else
        {
            Debug.Log("user does not exists, creating new entry in database!");
            PlayerData = new UserData();
            PlayerData.WalletAddress = _walletID;
            PlayerData.UserName = Constants.SavedUserName;
            Constants.UserName = PlayerData.UserName;
            PlayerData.PassBought = false;
            PlayerData.TimeSeconds = 0;
            PlayerData.UID = UID;
            PlayerData.NumberOfTries = 0;
            PlayerData.NumberOfTriesPractice = 0;
            //PlayerData.ProfileCreated = TournamentManager.Instance.DataTournament.timestamp;
            PlayerData.Email = Constants.SavedEmail;
            PlayerData.TournamentEndDate = null;
            PlayerData.AvatarID = Constants.FlagSelectedIndex;
            AddFireStoreData(PlayerData);
            // Constants.LoggedIn = true;

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
        Debug.Log("Doc Updated");
        Debug.Log(info);
    }

    public void OnEmailSentError(string info)
    {
        Debug.LogError(info);
        Invoke("SendVerEmail", 1f);
    }

    public IEnumerator CheckWalletDB(string _walletID)
    {
        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);

        if (DocFetched == true) //document existed
        {
            Debug.Log("user already exists!");
            
            if(MainMenuViewController.Instance)
                MainMenuViewController.Instance.DBChecked(true);
        }
        else
        {
            Debug.Log("user does not exists, creating new entry in database!");

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.DBChecked(false);
        }
    }

    public void UpdatedFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.UpdateDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnDocUpdate", "OnDocUpdateError");
    }

    public void OnDocUpdate(string info)
    {
        if(Constants.PushingTries)
        {
            Constants.PushingTries = false;
            return;
        }

        if(RaceManager.Instance)
            RaceManager.Instance.RaceEnded();

        Debug.Log("Doc Updated");
        Debug.Log(info);
    }

    public void OnDocUpdateError(string error)
    {
        Debug.LogError(error);
    }

    public void QueryDB(string _field,string _type)
    {
        FirebaseFirestore.QueryDB(DocPath,_field, _type, gameObject.name, "OnQueryUpdate", "OnQueryUpdateError");
    }
    public void OnQueryUpdate(string info)
    {
        Debug.Log("leaderboard query completed");
        //Debug.Log(info);
        PlayerDataArray = JsonConvert.DeserializeObject<UserData[]>(info);
        //System.Array.Reverse(PlayerDataArray);
        LeaderboardManager.Instance.PopulateLeaderboardData(PlayerDataArray);
    }

    public void OnQueryUpdateError(string error)
    {
        Debug.LogError(error);
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
        FirebaseAuth.SendPasswordResetEmail(_email,gameObject.name, "OnPassEmailSent", "OnPassEmailSentError");
    }
    public void OnPassEmailSent(string info)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f,"Reset password link sent to entered email address, please click the link in inbox to reset password.");
    }

    public void OnPassEmailSentError(string info)
    {
        Debug.LogError(info);

        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending password reset link, please try again later.");
    }

    public void CallOnError()
    {
        SendPasswordResetEmail(Constants.EmailSent);
    }

    public void ResendVerificationEmail()
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        FirebaseAuth.SendEmailVerification(gameObject.name, "ResendEmailSent", "ResendEmailSentError");
    }

    public void ResendEmailSent(string info)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.DisableResendScreen();
        //MainMenuViewController.Instance.BackClicked_PasswordReset();
        MainMenuViewController.Instance.ShowToast(4f, "Confirmation link sent again, please click the link in inbox (or spam) to confirm.");
    }

    public void ResendEmailSentError(string info)
    {
        Debug.LogError(info);
        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
        MainMenuViewController.Instance.DisableResendScreen();
        MainMenuViewController.Instance.ShowToast(4f, "Something went wrong while sending confirmation link, please try again later.");
    }

}
