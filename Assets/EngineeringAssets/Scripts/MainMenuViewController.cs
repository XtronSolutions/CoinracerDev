using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using static Constants;
using System.Runtime.InteropServices;

#region SuperClasses

[Serializable]
public class MultiplayerSelectionUI
{
    [Tooltip("Main screen for Multiplayer Selection.")]
    public GameObject MainScreen;
    [Tooltip("Text reference for coin price in dollars.")]
    public TextMeshProUGUI CracePriceText;
    [Tooltip("Text reference for disclaimer")]
    public TextMeshProUGUI Disclaimer;
    [Tooltip("Button Reference for Cancel")]
    public Button CancelButton;
    [Tooltip("Button Reference for Play")]
    public Button PlayButton;

}

[System.Serializable]
public class ConnectionUI
{
    [Tooltip("Main screen for connection.")]
    public GameObject MainScreen;
    [Tooltip("Back button for connection screen.")]
    public Button BackButton;
    [Tooltip("Multiplayer button for main menu screen.")]
    public Button MultiplayerButton;
    [Tooltip("Connection text for connection screen.")]
    public TextMeshProUGUI ConnectionText;
    [Tooltip("Region text for connection screen.")]
    public TextMeshProUGUI RegionText;
    [Tooltip("Player Count text for connection screen.")]
    public TextMeshProUGUI PlayerCountText;
}

[Serializable]
public class SettingsUI
{
    public GameObject MainScreen;
    public Button SettingButton;
    public Slider SoundSlider;
    public Slider MusicSlider;
    public Button BackButton;
}

[Serializable]
public class ForgetPasswordUI
{
    public GameObject MainScreen;
    public Button ForgetPassButton;
    public Button ResendConfirmationButton;
    public TMP_InputField EmailInput;
    public Button SendEmail;
    public Button SendResendEmail;
    public Button BackButton;
}

[Serializable]
public class GarageUI
{
    public GameObject MainScreen;
    public Button BackButton;
    public Button GarageButton;
    public GameObject ScrollContent;
    public int ContentHeight;
    public GameObject RowPrefab;
    public GameObject NFTPrefab;
}


[Serializable]
public class FlagSelectionUI
{
    public GameObject MainScreen;
    public Button SubmitButton;
}

[Serializable]
public class SelectionUI
{
    public GameObject MainScreen;
    public Button BuyPassButton;
    public TextMeshProUGUI BuyPassText;
    public Button PlayFromPassButton;
    public TextMeshProUGUI PlayFromPassText;
    public Button SingleTryButton;
    public TextMeshProUGUI SingleTryText;
    public Button CancelButton;
    public Button BuyPassCraceButton;
    public Button BuyPassCancelButton;
    public GameObject TournamentPassScreen;
}


[Serializable]
public class TournamentUI
{
    public GameObject MainScreen;
    public TextMeshProUGUI LowerHeaderText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI FotterText;
    public TextMeshProUGUI Fotter2Text;
    public TextMeshProUGUI DisclaimerText;
    public GameObject LoaderObj;
    public TextMeshProUGUI TournamentStartText;
}

[Serializable]
public class RegisterUI
{
    public GameObject MainScreen;
    public TMP_InputField UserName;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmPassword;
    public TMP_InputField WalletInput;
    public Button RegisterButton;
    public Button BackButton;
    public Button SelectFlag;
    public Image SelectedFlag;
}

[Serializable]
public class LoginUI
{
    public GameObject MainScreen;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public Button RegisterButton;
    public Button LoginButton;
}

[Serializable]
public class AllCarSelection
{
    public string CarName;
    public CarSelection CarDetail;
}
#endregion

public class MainMenuViewController : MonoBehaviour
{
    #region DataMembers
    [DllImport("__Internal")]
    private static extern string GetStorageClass(string key, string ObjectName, string callback);

    [DllImport("__Internal")]
    private static extern string GetStorage(string key, string ObjectName, string callback);

    public static CarSettings SelectedCar;
    public static MainMenuViewController Instance;
    public GameObject MenuScreen;
    [SerializeField] private GameObject GameModeSelectionObject = null;
    [SerializeField] private GameObject CarSelectionObject = null;
    [SerializeField] private GameObject CarSelection3dObject = null;
    [SerializeField] private GameObject MapSelection = null;
    [SerializeField] private Button _singlePlayerButton = null;
    [SerializeField] private Button _tournamentButton = null;
    [SerializeField] private Button _backToModeSelectionButton = null;
    [SerializeField] private Button _goToMapSelectionButton = null;
    [SerializeField] private Button _backToCarSelectionButton = null;
    [SerializeField] private Button _startRaceButton = null;
    [SerializeField] private Button _nextCarButton = null;
    [SerializeField] private Button _prevCarButton = null;
    [SerializeField] private Button _nextMapButton = null;
    [SerializeField] private Button _prevMapButton = null;
    [SerializeField] private Button _logoutButton = null;
    [SerializeField] private List<CarSelection> _selecteableCars = new List<CarSelection>();
    [SerializeField] private List<AllCarSelection> _allCars = new List<AllCarSelection>();
    [SerializeField] private TextMeshProUGUI _versionText = null;
    [SerializeField] private TextMeshProUGUI _selectedCarName = null;
    [SerializeField] private List<LevelSettings> _levelsSettings = new List<LevelSettings>();
    [SerializeField] private List<LevelSettings> _allLevelsSettings = new List<LevelSettings>();
    [SerializeField] private Image _selectedMapImage = null;
    [SerializeField] private TextMeshProUGUI _levelNameText = null;
    [SerializeField] public GameObject LoadingScreen = null;
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;

    [SerializeField] private GameObject MessageUI;
    [SerializeField] private TextMeshProUGUI ToastMsgText = null;
    [SerializeField] private TextMeshProUGUI UserNameText = null;
    [SerializeField] private Image FlagIcon = null;

    public TournamentUI UITournament;
    public RegisterUI UIRegister;
    public LoginUI UILogin;
    public SelectionUI UISelection;
    public FlagSelectionUI UIFlagSelection;
    public GarageUI UIGarage;
    public ForgetPasswordUI UIForgetPassword;
    private string[] CarNames = new string[5] { "Bonecrusher", "Merky", "CyberCar", "Coinrarri", "Malibu Express" };
    public GameObject ResendPopUp;
    public SettingsUI UISetting;
    public ConnectionUI UIConnection;
    public MultiplayerSelectionUI UIMultiplayerSelection;

    double RemainingTimeSecondPass;
    private int _currentSelectedCarIndex = 0;
    private int _currentlySelectedLevelIndex = 0;
    string email = "";
    string pass = "";
    string resetEmail = "";
    string walletAddress = "";
    string confirmPass = "";
    string userName = "";
    string tempInfo;
    private const string MatchEmailPatternOld =
       "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`{}|~\\w])*)(?<=[0-9a-z])@))(?([)([(\\d{1,3}.){3}\\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$";

    private const string MatchEmailPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    #endregion

    #region Start Functionality
    private void OnEnable()
    {
        Instance = this;
    }

    public void OnGetSound(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty)
        {

            string[] splitArray = info.Split('"');
            Constants.SoundSliderValue = float.Parse(splitArray[1]);
        }
    }

    public void OnGetMusic(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty)
        {
            string[] splitArray = info.Split('"');
            Constants.MusicSliderValue = float.Parse(splitArray[1]);
        }
    }


    void Start()
    {
        ResetRegisterFields();
        _audioSource.GetComponent<AudioSource>();
        _singlePlayerButton.onClick.AddListener(OnGoToCarSelection);
        _tournamentButton.onClick.AddListener(OnGoToCarSelectionTournament);
        _backToModeSelectionButton.onClick.AddListener(OnGoBackToModeSelection);
        _goToMapSelectionButton.onClick.AddListener(OnGoToMapSelection);
        _backToCarSelectionButton.onClick.AddListener(BackToGoToCarSelection);
        _startRaceButton.onClick.AddListener(StartRace);
        _logoutButton.onClick.AddListener(SignOutUser);

        _currentSelectedCarIndex = 0;
        //UpdateSelectedCarVisual(_currentSelectedCarIndex);
        _versionText.text = APP_VERSION;

        _nextCarButton.onClick.AddListener(OnNextCar);
        _prevCarButton.onClick.AddListener(OnPrevCar);
        _nextMapButton.onClick.AddListener(OnNextMap);
        _prevMapButton.onClick.AddListener(OnPrevMap);

        //UIGarage.BackButton.onClick.AddListener(BackButton_Garage);
        // UIGarage.GarageButton.onClick.AddListener(GarageButton_Garage);

        ButtonListenerRegister();
        ButtonListenerLogin();
        ButtonListeners_SelectionUI();
        ButtonListener_FlagSelectionUI();
        CheckForAutoLogin();
        SubscribeEvents_PasswordReset();
        SubscribeEvents_Settings();
        SubscribeEvents_ConnectionUI();
        SubscribeEvents_MultiplayerSelection();

#if UNITY_WEBGL && !UNITY_EDITOR
            GetStorage(Constants.SoundKey,this.gameObject.name,"OnGetSound");
            GetStorage(Constants.MusicKey,this.gameObject.name,"OnGetMusic");
#endif
    }

    public void CheckForAutoLogin()
    {
        if (FirebaseManager.Instance)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        GetStorageClass(Constants.CredKey,this.gameObject.name,"OnGetCred");
#endif
        }
    }

    #endregion

    #region SeletionUI (MainData/Selections)
    public void ToggleScreen_SelectionUI(bool _state)
    {
        if(_state)
            ChangeDisclaimerTexts_SelectionUI("*Price: " + Constants.TournamentPassPrice + " $crace. unlimited attempts in a single tournament.", "*if you have the pass, enter the tournament here.", "*price: " + Constants.TicketPrice + " $crace, if you hold " + Constants.DiscountForCrace + " $crace - " + Constants.DiscountPercentage + "% discount.");

        UISelection.MainScreen.SetActive(_state);
    }

    public void TogglePassScreen_SelectionUI(bool _state)
    {
        UISelection.TournamentPassScreen.SetActive(_state);
    }

    public void ButtonListeners_SelectionUI()
    {
        UISelection.BuyPassButton.onClick.AddListener(BuyPassClicked_SelectionUI);
        UISelection.PlayFromPassButton.onClick.AddListener(PlayFromPass_SelectionUI);
        UISelection.CancelButton.onClick.AddListener(CancelSelection_SelectionUI);
        UISelection.BuyPassCraceButton.onClick.AddListener(BuyPassCraceClicked_SelectionUI);
        UISelection.BuyPassCancelButton.onClick.AddListener(BackClickedPassScreen_SelectionUI);
        UISelection.SingleTryButton.onClick.AddListener(PlayOnce_SelectionUI);
    }
    public void ChangeDisclaimerTexts_SelectionUI(string txt1, string txt2, string txt3)
    {
        UISelection.BuyPassText.text = txt1;
        UISelection.PlayFromPassText.text = txt2;
        UISelection.SingleTryText.text = txt3;
    }

    public void CancelSelection_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
    }
    public void BuyPassClicked_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
        TogglePassScreen_SelectionUI(true);
    }

    public void BackClickedPassScreen_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
        TogglePassScreen_SelectionUI(false);
    }


    public void BuyPasswithCrace()
    {
        if (WalletManager.Instance.CheckBalanceTournament(false, false, true,false))
        {
            BuyingPass = true;
            WalletManager.Instance.TransferToken(TournamentPassPrice);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Insufficient $CRACE value, need " + Constants.TournamentPassPrice + " $CRACE");
        }
    }
    public void BuyPassCraceClicked_SelectionUI()
    {
        if (WalletConnected)
        {
            if (TournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    if (FirebaseManager.Instance.PlayerData.PassBought == false)
                    {
                        BuyPasswithCrace();
                    }
                    else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "You already own a tournament pass");

                        //var _data = TournamentManager.Instance.DataTournament;
                        //RemainingTimeSecondPass = FirebaseManager.Instance.PlayerData.TournamentEndDate.seconds - _data.timestamp.seconds;

                        //if (Mathf.Sign((float)RemainingTimeSecondPass) == -1)
                        //{
                        //    Debug.LogError("pass time is over, resting it");
                        //    LoadingScreen.SetActive(true);
                        //    FirebaseManager.Instance.PlayerData.PassBought = false;
                        //    FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
                        //    BuyPasswithCrace();
                        //}
                        //else
                        //{
                        //    LoadingScreen.SetActive(false);
                        //    ShowToast(3f, "You already own a tournament pass");
                        //}
                    }
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    Debug.LogError("WM instance is null BuyPassCraceClicked_SelectionUI");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No active tournament at the moment.");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void OnPassBuy(bool _state)
    {
        if (_state)
        {
            ShowToast(3f, "You have successfully bought tournament pass for this tournament week.");
            LoadingScreen.SetActive(false);
            BackClickedPassScreen_SelectionUI();

            FirebaseManager.Instance.PlayerData.PassBought = true;
            FirebaseManager.Instance.PlayerData.TournamentEndDate = TournamentManager.Instance.DataTournament.EndDate;
            FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Transaction was not successful, please try again.");
        }

    }

    public void PlayFromPass_SelectionUI()
    {
        if (WalletConnected)
        {
            if (TournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    if (FirebaseManager.Instance.PlayerData.PassBought)
                    {

                        BackClickedPassScreen_SelectionUI();
                        StartTournament(true);

                        //var _data = TournamentManager.Instance.DataTournament;
                        //RemainingTimeSecondPass = FirebaseManager.Instance.PlayerData.TournamentEndDate.seconds - _data.timestamp.seconds;

                        //if (Mathf.Sign((float)RemainingTimeSecondPass) == -1)
                        //{
                        //    Debug.LogError("pass time is over, resting it");
                        //    LoadingScreen.SetActive(false);
                        //    FirebaseManager.Instance.PlayerData.PassBought = false;
                        //    FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
                        //    ShowToast(3f, "you have not bought tournament pass or pass expired");
                        //}
                        //else
                        //{
                        //    BackClickedPassScreen_SelectionUI();
                        //    StartTournament(true);
                        //}
                    }
                    else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "you have not bought tournament pass or pass expired");
                    }

                }
                else
                {
                    LoadingScreen.SetActive(false);
                    Debug.LogError("WM instance is null PlayFromPass_SelectionUI");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No active tournament at the moment.");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void PlayOnce_SelectionUI()
    {
        if (Constants.IsTest)
        {
            MainMenuViewController.Instance.StartTournament(true);
            return;
        }

        if (WalletConnected)
        {
            if (TournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    TicketPrice = TournamentManager.Instance.DataTournament.TicketPrice;
                    if (WalletManager.Instance.CheckBalanceTournament(false, true, false,false))
                    {
                        ShowToast(2f, "Congrats!, You have received "+Constants.DiscountPercentage+"% discount.");
                        TicketPrice = (TicketPrice * DiscountPercentage) / 100;
                    }

                    if (WalletManager.Instance.CheckBalanceTournament(true, false, false,false))
                    {
                        WalletManager.Instance.TransferToken(TicketPrice);
                    }
                    else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "Insufficient $CRACE value.");
                    }
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    Debug.LogError("WM instance is null PlayOnce_SelectionUI");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No active tournament at the moment.");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    #endregion

    #region FlagSelection
    public void ToggleFlagSelection_FlagSelectionUI(bool _state)
    {
        UIFlagSelection.MainScreen.SetActive(_state);
    }

    public void ButtonListener_FlagSelectionUI()
    {
        UIFlagSelection.SubmitButton.onClick.AddListener(SubmitSelection_FlagSelectionUI);
    }

    public void EnableFlagSelection_FlagSelectionUI()
    {
        ToggleFlagSelection_FlagSelectionUI(true);

        if (FlagHandler.Instance)
            FlagHandler.Instance.EnableFlags();

    }

    public void SubmitSelection_FlagSelectionUI()
    {
        UIRegister.SelectedFlag.sprite = FlagSkins.Instance.FlagSpriteWithIndex(Constants.FlagSelectedIndex);
        FlagIcon.sprite = FlagSkins.Instance.FlagSpriteWithIndex(Constants.FlagSelectedIndex);
        ToggleFlagSelection_FlagSelectionUI(false);

        if (Constants.LoggedIn)
        {
            FirebaseManager.Instance.PlayerData.AvatarID = Constants.FlagSelectedIndex;
            FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
        }
    }
    #endregion

    #region MISC
    public void SignOutUser()
    {
        Constants.LoggedIn = false;
        ResetRegisterFields();

        if (FirebaseManager.Instance)
            FirebaseManager.Instance.LogoutUser();
    }


    public void OnGetCred(string info)
    {
        tempInfo = info;
        LoadingScreen.SetActive(false);
        if (info != null && info != "null")
        {
            if (Constants.WalletConnected)
            {
                LoginAfterConnect(info);
            }
            else
            {
                Invoke("InvokeCallCred", 1f);
            }
        }
    }

    public void InvokeCallCred()
    {
        OnGetCred(tempInfo);
    }

    public void LoginAfterConnect(string info)
    {

        if (info != "null" && info != "" && info != null)
        {
            if (!Constants.LoggedIn)
            {
                LoadingScreen.SetActive(true);
                FirebaseManager.Instance.Credentails = JsonConvert.DeserializeObject<AuthCredentials>(info);

                if (!Constants.WalletChanged)
                {
                    FirebaseManager.Instance.LoginUser(FirebaseManager.Instance.Credentails.Email, FirebaseManager.Instance.Credentails.Password, FirebaseManager.Instance.Credentails.UserName);
                }
                else
                {
                    Constants.WalletChanged = false;
                    ShowToast(3f, "Previous Conneted wallet was changed, auto login will not work.");
                    LoadingScreen.SetActive(false);

                    if (FirebaseManager.Instance)
                        FirebaseManager.Instance.ResetStorage();
                }
            }
        }
    }
    public void ChangeUserNameText(string _txt)
    {
        FlagIcon.sprite = FlagSkins.Instance.FlagSpriteWithIndex(Constants.FlagSelectedIndex);
        if (_txt != "")
        {
            UserNameText.text = "Hi, " + _txt;
        }
        else {
            UserNameText.text = "";
        }
    }

    public void DBChecked(bool _state)
    {
        if (_state)
        {
            Constants.RegisterSubmit = false;
            LoadingScreen.SetActive(false);
            ResetRegisterFields();
            ShowToast(3f, "Wallet already connected to an email address.");
        }
        else
        {
            LoadingScreen.SetActive(true);
            FirebaseManager.Instance.CheckEmailForAuth(Constants.SavedEmail, Constants.SavedPass, Constants.SavedUserName);
        }
    }

    public void EmailAlreadyExisted()
    {
        LoadingScreen.SetActive(false);
        ResetRegisterFields();
        ShowToast(3f, "User with entered email already registered.");
    }

    public void SomethingWentWrong()
    {
        LoadingScreen.SetActive(false);
        ResetRegisterFields();
        ShowToast(3f, "credentails invalid, please try again.");
    }

    public void ShowToast(float _time, string _msg)
    {
        MessageUI.SetActive(true);
        ToastMsgText.text = _msg;
        StartCoroutine(DisableToast(_time));
    }

    public void ShowResendScreen(float _Sec)
    {
        ResendPopUp.SetActive(true);
        Invoke("DisableResendScreen", _Sec);
    }

    public void DisableResendScreen()
    {
        ResendPopUp.SetActive(false); ;
    }
    private void OnGoBackToModeSelection()
    {
        GameModeSelectionObject.SetActive(true);
        CarSelectionObject.SetActive(false);
        CarSelection3dObject.SetActive(false);
        MapSelection.SetActive(false);
    }

    IEnumerator DisableToast(float _sec)
    {
        yield return new WaitForSeconds(_sec);
        MessageUI.SetActive(false);
    }

    public void PlayButtonDownAudioClip()
    {
        _audioSource.volume = Constants.SoundSliderValue;
        _audioSource.PlayOneShot(_buttonPressClip);
    }

    #endregion

    #region Register UI/Data
    public void ButtonListenerRegister()
    {
        UIRegister.BackButton.onClick.AddListener(DisableRegisterScreen);
        UIRegister.RegisterButton.onClick.AddListener(SubmitRegister);
        UIRegister.SelectFlag.onClick.AddListener(EnableFlagSelection_FlagSelectionUI);
    }

    public void ButtonListenerLogin()
    {
        UILogin.RegisterButton.onClick.AddListener(EnabledRegisterScreen);
        UILogin.LoginButton.onClick.AddListener(SubmitLogin);

    }

    public void DisableRegisterLogin()
    {
        UIRegister.MainScreen.SetActive(false);
        UILogin.MainScreen.SetActive(false);
    }

    public void EnableRegisterLogin()
    {
        UIRegister.MainScreen.SetActive(false);
        UILogin.MainScreen.SetActive(true);
    }

    public void ToggleMainMenuScreen(bool state)
    {
        MenuScreen.SetActive(state);
    }
    public void ToggleRegisterScreen(bool state)
    {
        UIRegister.MainScreen.SetActive(state);
        UILogin.MainScreen.SetActive(!state);
    }

    public void EnabledRegisterScreen()
    {
        if (WalletConnected)
        {
            ToggleRegisterScreen(true);
            SubmitSelection_FlagSelectionUI();
            ResetRegisterFields();
        } else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void DisableRegisterScreen()
    {
        ToggleRegisterScreen(false);
        ResetRegisterFields();
    }

    public void ResetRegisterFields()
    {
        UIRegister.EmailInput.text = "";
        UILogin.EmailInput.text = "";
        UILogin.PasswordInput.text = "";
        UIRegister.PasswordInput.text = "";
        UIRegister.ConfirmPassword.text = "";
        UIRegister.WalletInput.text = Constants.WalletAddress;
        UIRegister.UserName.text = "";
        email = "";
        pass = "";
        walletAddress = "";
        confirmPass = "";
        userName = "";

        SavedEmail = email;
        SavedPass = pass;
        SavedUserName = userName;
        SavedConfirmPass = confirmPass;
    }

    public void OnUsernameChanged_Register(string _val)
    {
        userName = _val;
    }
    public void OnEmailChanged_Register(string _val)
    {
        email = _val;
    }

    public void OnPassChanged_Register(string _val)
    {
        pass = _val;
    }
    public void OnWalletChanged_Register(string _val)
    {
        walletAddress = _val;
    }

    public void OnPassConfirmChanged_Register(string _val)
    {
        confirmPass = _val;
    }

    public void SubmitRegister()
    {
        if (userName == "")
        {
            ShowToast(3f, "please enter username.");
            return;
        }

        if (email == "")
        {
            ShowToast(3f, "Please enter email.");
            return;
        }

        if (Regex.IsMatch(email, MatchEmailPattern) == false)
        {
            ShowToast(3f, "Email format invalid.");
            return;
        }

        if (pass == "")
        {
            ShowToast(3f, "Please enter password.");
            return;
        }

        if (pass.Length < 6)
        {
            ShowToast(3f, "Password should be atleast 6 characters long.");
            return;
        }

        if (confirmPass == "")
        {
            ShowToast(3f, "Please enter password again to confirm.");
            return;
        }

        if (pass != confirmPass)
        {
            ShowToast(3f, "Passwords do not match.");
            return;
        }

        SavedEmail = email;
        SavedPass = pass;
        SavedUserName = userName;
        SavedConfirmPass = confirmPass;

        LoadingScreen.SetActive(true);
        Constants.RegisterSubmit = true;
        FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckWalletDB(PlayerPrefs.GetString("Account")));
        //FirebaseManager.Instance.CheckEmailForAuth(email, pass, userName);

    }
    #endregion

    #region Login UI/Data

    public void ToggleLoginScreen_Login(bool _state)
    {
        UILogin.MainScreen.SetActive(_state);
    }
    public void OnLoginSuccess(bool _newUser)
    {
        Constants.LoggedIn = true;
        //saving cred into browser local strorage
        if (FirebaseManager.Instance)
        {
            FirebaseManager.Instance.Credentails.WalletAddress = WalletManager.Instance.GetAccount();
            string _json = JsonConvert.SerializeObject(FirebaseManager.Instance.Credentails);
            FirebaseManager.Instance.SetLocalStorage(Constants.CredKey, _json);
        }

        ChangeUserNameText(Constants.UserName);
        LoadingScreen.SetActive(false);
        DisableRegisterLogin();
        MenuScreen.SetActive(true);

        if (!_newUser)
            FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.FetchUserDB(PlayerPrefs.GetString("Account"), ""));
        else
            FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
    }

    public void OnEmailChanged_Login(string _val)
    {
        email = _val;
    }

    public void OnPassChanged_Login(string _val)
    {
        pass = _val;
    }
    public void SubmitLogin()
    {
        if (!WalletConnected)
        {
            LoadingScreen.SetActive(false);
            ResetRegisterFields();
            ShowToast(3f, "Please connect your wallet first.");
            return;
        }

        if (email == "")
        {
            ShowToast(3f, "Please enter email.");
            return;
        }

        if (Regex.IsMatch(email, MatchEmailPattern) == false)
        {
            ShowToast(3f, "Email format invalid.");
            return;
        }

        if (pass == "")
        {
            ShowToast(3f, "Please enter password.");
            return;
        }

        SavedEmail = email;
        SavedPass = pass;
        SavedUserName = userName;

        LoadingScreen.SetActive(true);
        FirebaseManager.Instance.LoginUser(SavedEmail, SavedPass, SavedUserName);

    }

    #endregion

    #region MapSelection UI/Data
    private void OnNextMap()
    {
        int newIndex = _currentlySelectedLevelIndex + 1;
        newIndex %= _levelsSettings.Count;
        OnLevelSelected(newIndex);
    }

    private void OnPrevMap()
    {
        int newIndex = _currentlySelectedLevelIndex;
        newIndex--;
        if (newIndex < 0)
        {
            newIndex = _levelsSettings.Count + newIndex;
        }

        OnLevelSelected(newIndex);
    }

    private void OnLevelSelected(int i)
    {
        _currentlySelectedLevelIndex = i;
        _selectedMapImage.sprite = _levelsSettings[i].Icon;
        _levelNameText.text = _levelsSettings[i].LevelName;
    }
    private void OnGoToMapSelection()
    {
        _levelsSettings.Clear();

        if (IsPractice)
        {
            _levelsSettings.Add(_allLevelsSettings[0]);
            _levelsSettings.Add(_allLevelsSettings[1]);
            _levelsSettings.Add(_allLevelsSettings[2]);
        }
        else if (IsTournament)
        {
            //_levelsSettings.Add(_allLevelsSettings[1]);
            _levelsSettings.Add(_allLevelsSettings[2]);
        }

        OnLevelSelected(0);
        GameModeSelectionObject.SetActive(false);
        CarSelectionObject.SetActive(false);
        CarSelection3dObject.SetActive(false);
        MapSelection.SetActive(true);
    }
    public int getSelectedLevel()
    {
        return _currentlySelectedLevelIndex;
      
    }

    #endregion

    #region CarSelection UI/Data

    private void BackToGoToCarSelection()
    {
        if (Constants.IsTest)
        {
            WalletConnected = true;
        }

        if (WalletConnected)//WalletConnected
        {
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }
    private void OnGoToCarSelection()
    {
        if (Constants.IsTest)
        {
            WalletConnected = true;
        }

        if (WalletConnected)//WalletConnected
        {
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            IsTournament = false;
            IsPractice = true;
            //IsMultiplayer = false;
            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void OnGoToCarSelectionTournament()
    {
        if (Constants.IsTest)
        {
            ToggleScreen_SelectionUI(true);
            return;
        }

        if (WalletConnected)
        {
            if (TournamentActive == true)
            {
                if (WalletManager.Instance)
                {
                    ToggleScreen_SelectionUI(true);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    Debug.LogError("WM instance is null OnGoToCarSelectionTournament");
                }
            } else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No active tournament at the moment.");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    private void OnNextCar()
    {
        int newIndex = _currentSelectedCarIndex + 1;
        newIndex %= _selecteableCars.Count;
        UpdateSelectedCarVisual(newIndex);
    }

    private void OnPrevCar()
    {
        int newIndex = _currentSelectedCarIndex;
        newIndex--;
        if (newIndex < 0)
        {
            newIndex = _selecteableCars.Count + newIndex;
        }

        UpdateSelectedCarVisual(newIndex);
    }

    public void UpdateSelectedCarVisual(int newIndex)
    {
        for (int i = 0; i < _selecteableCars.Count; i++)
        {
            _selecteableCars[i].Deactivate();
        }
        //_selecteableCars[_currentSelectedCarIndex].Deactivate();
        _currentSelectedCarIndex = newIndex;
        _selecteableCars[_currentSelectedCarIndex].Activate();
        _selectedCarName.text = _selecteableCars[_currentSelectedCarIndex].carSettings.Name;
    }

    #endregion

    #region Tournament UI/Data
    public void StartTournament(bool _canstart = false)
    {
        if (Constants.IsTest)
        {
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            IsTournament = true;
            IsPractice = false;
            IsMultiplayer = false;
            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
            return;
        }

        if (_canstart)
        {
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            IsTournament = true;
            IsPractice = false;
            IsMultiplayer = false;

            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
        } else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Transaction was not successful, please try again.");
            //Invoke("StartWithDelay", 4f);
        }
    }

    private void StartRace()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (IsPractice)
        {
            PushingTries = true;
            FirebaseManager.Instance.PlayerData.NumberOfTriesPractice++;
            FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
        }
        else if (IsTournament)
        {
            PushingTries = true;
            FirebaseManager.Instance.PlayerData.NumberOfTries++;
            FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
        }
#endif
        if (Constants.IsMultiplayer)
        {
            if (MultiplayerManager.Instance)
            {
                MultiplayerManager.Instance.ConnectToPhotonServer();
                MainMenuViewController.Instance.SelectMultiplayer_ConnectionUI();
            } 
        }
        else
        {
            SelectedCar = _selecteableCars[_currentSelectedCarIndex].carSettings;
            SceneManager.LoadScene(_levelsSettings[_currentlySelectedLevelIndex].SceneName, LoadSceneMode.Single);
        }
    }


    public void TempStartRace()
    {
        IsPractice = true;
        SelectedCar = _selecteableCars[0].carSettings;
    }
    #endregion

    #region Garage UI/Data
    public void ToggleScreen_Garage(bool _state)
    {
        UIGarage.MainScreen.SetActive(_state);
    }

    public void BackButton_Garage()
    {
        ToggleScreen_Garage(false);
        NFTGameplayManager.Instance.RemoveSavedPrefabs();
    }

    public void GarageButton_Garage()
    {
        if (Constants.IsTest)
        {
            WalletConnected = true;
        }

        if (WalletConnected)//WalletConnected
        {
            if (Constants.IsTestNet)
            {
                ToggleScreen_Garage(true);
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No NFT was purchased, please purchase one.");
                return;
            }

            if (Constants.CheckAllNFT || Constants.NFTStored == 0)
            {

                ToggleScreen_Garage(true);

                if (Constants.NFTStored == 0)
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "No NFT was purchased, please purchase one.");
                }
                else
                {
                    NFTGameplayManager.Instance.InstantiateNFT();
                }

                //if(Constants.CheckAllNFT)
                //{
                //    WalletManager.Instance.CheckNFTBalance(true);
                //}
            }
            else
            {
                LoadingScreen.SetActive(true);
                ToggleScreen_Garage(true);
                Invoke("GarageButton_Garage", 1f);
            }
        } else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void CheckBoughtCars()
    {
        if(Constants.IsTestNet)
        {
            _selecteableCars.Clear();
            _selecteableCars.Add(_allCars[0].CarDetail);
            LoadingScreen.SetActive(false);
            _currentSelectedCarIndex = 0;
            UpdateSelectedCarVisual(_currentSelectedCarIndex);

            return;
        }

        if (Constants.CheckAllNFT || Constants.NFTStored == 0)
        {
            if (Constants.NFTStored == 0)
            {
                _selecteableCars.Clear();
                _selecteableCars.Add(_allCars[0].CarDetail);

                LoadingScreen.SetActive(false);
                _currentSelectedCarIndex = 0;
                UpdateSelectedCarVisual(_currentSelectedCarIndex);
            }

            else if (Constants.StoredCarNames.Count != 0)
            {
                //clearing list and adding BOLT car
                _selecteableCars.Clear();
                _selecteableCars.Add(_allCars[0].CarDetail);

                for (int i = 0; i < Constants.StoredCarNames.Count; i++)
                {
                    for (int j = 0; j < _allCars.Count; j++)
                    {
                        if (Constants.StoredCarNames[i].ToLower() == _allCars[j].CarName.ToLower())
                        {
                            _selecteableCars.Add(_allCars[j].CarDetail);
                            break;
                        }
                    }
                }

                LoadingScreen.SetActive(false);
                _currentSelectedCarIndex = 0;
                UpdateSelectedCarVisual(_currentSelectedCarIndex);
            } else
            {
                Invoke("CheckBoughtCars", 1f);
            }
        }
        else
        {
            Invoke("CheckBoughtCars", 1f);
        }
    }

    #endregion

    #region Password Forget Data/UI
    public void ToggleScreen_PasswordReset(bool _state)
    {
        UIForgetPassword.MainScreen.SetActive(_state);
    }

    public void SubscribeEvents_PasswordReset()
    {
        UIForgetPassword.ForgetPassButton.onClick.AddListener(EnableForgetScreen_PasswordReset);
        UIForgetPassword.ResendConfirmationButton.onClick.AddListener(EnableResendScreen_PasswordReset);
        UIForgetPassword.BackButton.onClick.AddListener(BackClicked_PasswordReset);
        UIForgetPassword.SendEmail.onClick.AddListener(SendEmailClicked_PasswordReset);
        UIForgetPassword.SendResendEmail.onClick.AddListener(SendEmailConfirmationClicked_PasswordReset);
    }

    public void EnableResendScreen_PasswordReset()
    {
        ResetData_PasswordReset();
        ToggleScreen_PasswordReset(true);
        ToggleLoginScreen_Login(false);

        Constants.IsSendConfirmation = true;
        Constants.IsResetPassword = false;

        UIForgetPassword.SendEmail.gameObject.SetActive(false);
        UIForgetPassword.SendResendEmail.gameObject.SetActive(true);
    }

    public void EnableForgetScreen_PasswordReset()
    {
        ResetData_PasswordReset();
        ToggleScreen_PasswordReset(true);
        ToggleLoginScreen_Login(false);

        Constants.IsSendConfirmation = false;
        Constants.IsResetPassword = true;

        UIForgetPassword.SendEmail.gameObject.SetActive(true);
        UIForgetPassword.SendResendEmail.gameObject.SetActive(false);
    }

    public void BackClicked_PasswordReset()
    {
        ToggleScreen_PasswordReset(false);
        ToggleLoginScreen_Login(true);
    }

    public void ResetData_PasswordReset()
    {
        UIForgetPassword.EmailInput.text = "";
        resetEmail = "";
    }

    public void OnEmailChanged_pass(string _val)
    {
        resetEmail = _val;
    }
    public void SendEmailClicked_PasswordReset()
    {
        LoadingScreen.SetActive(true);
        if (UIForgetPassword.EmailInput.text == "")
        {
            ShowToast(3f, "Please enter email.");
            LoadingScreen.SetActive(false);
            return;
        }

        if (Regex.IsMatch(UIForgetPassword.EmailInput.text, MatchEmailPattern) == false)
        {
            ShowToast(3f, "Email format invalid.");
            LoadingScreen.SetActive(false);
            return;
        }
        else
        {
            FirebaseManager.Instance.SendPasswordResetEmail(UIForgetPassword.EmailInput.text);
        }
    }

    public void SendEmailConfirmationClicked_PasswordReset()
    {
        LoadingScreen.SetActive(true);
        if (UIForgetPassword.EmailInput.text == "")
        {
            ShowToast(3f, "Please enter email.");
            LoadingScreen.SetActive(false);
            return;
        }

        if (Regex.IsMatch(UIForgetPassword.EmailInput.text, MatchEmailPattern) == false)
        {
            ShowToast(3f, "Email format invalid.");
            LoadingScreen.SetActive(false);
            return;
        }
        else
        {
            FirebaseManager.Instance.ResendVerificationEmail();
        }
    }
    #endregion

    #region Settings UI/data

    public void ToggleScreen_Settings(bool _state)
    {
        UISetting.MainScreen.SetActive(_state);
    }

    public void SubscribeEvents_Settings()
    {
        UISetting.BackButton.onClick.AddListener(DisableScreen_Settings);
        UISetting.SettingButton.onClick.AddListener(EnableScreen_Settings);
    }
    public void EnableScreen_Settings()
    {
        ToggleScreen_Settings(true);

        UISetting.SoundSlider.value = Constants.SoundSliderValue;
        UISetting.MusicSlider.value = Constants.MusicSliderValue;
    }

    public void DisableScreen_Settings()
    {
        ToggleScreen_Settings(false);

        Constants.SoundSliderValue = UISetting.SoundSlider.value;
        Constants.MusicSliderValue = UISetting.MusicSlider.value;

        FirebaseManager.Instance.SetLocalStorage(Constants.SoundKey, Constants.SoundSliderValue.ToString());
        FirebaseManager.Instance.SetLocalStorage(Constants.MusicKey, Constants.MusicSliderValue.ToString());
    }

    #endregion

    #region ConnectionUI

    public void SubscribeEvents_ConnectionUI()
    {
       // UIConnection.MultiplayerButton.onClick.AddListener(onMultiplayerBtnClick);
        UIConnection.MultiplayerButton.onClick.AddListener(EnableSelection_MultiplayerSelection);
        UIConnection.BackButton.onClick.AddListener(DisableScreen_ConnectionUI);
    }
    public void onMultiplayerBtnClick()
    {
        if (Constants.IsTest)
            WalletConnected = true;

        if (WalletConnected)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                if (WalletManager.Instance.CheckBalanceTournament(false, false, false, true))
                {
                    ToggleScreen_MultiplayerSelection(false);
                    Constants.IsMultiplayer = true;
                    MainMenuViewController.Instance.OnGoToCarSelection();
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "Insufficient $CRACE value, need " + Constants.CalculatedCrace + " $CRACE");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Debug.LogError("WM instance is null onMultiplayerBtnClick");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void ToggleScreen_ConnectionUI(bool _state)
    {
        UIConnection.MainScreen.SetActive(_state);
    }

    public void ChangeConnectionText_ConnectionUI(string _txt)
    {
        UIConnection.ConnectionText.text = _txt;
    }

    public void ChangeRegionText_ConnectionUI(string _txt)
    {
        UIConnection.RegionText.text = _txt;
    }

    public void ChangePlayerCountText_ConnectionUI(string _txt)
    {
        UIConnection.PlayerCountText.text = _txt;
    }

    public void SelectMultiplayer_ConnectionUI()
    {
        Constants.IsMultiplayer = true;
        ToggleScreen_ConnectionUI(true);
        
        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.ConnectToPhotonServer();
    }

    public void DisableScreen_ConnectionUI()
    {
        Constants.IsMultiplayer = false;
        ChangeConnectionText_ConnectionUI("connecting...");
        ChangeRegionText_ConnectionUI("Selected Region : n/a");
        ToggleScreen_ConnectionUI(false);

        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.DisconnectPhoton();
    }
    #endregion

    #region Multiplayer Selection
    public void SubscribeEvents_MultiplayerSelection()
    {
        UIMultiplayerSelection.CancelButton.onClick.AddListener(DisableScreen_MultiplayerSelection);
        UIMultiplayerSelection.PlayButton.onClick.AddListener(onMultiplayerBtnClick);
    }
    public void ToggleScreen_MultiplayerSelection(bool _state)
    {
        UIMultiplayerSelection.MainScreen.SetActive(_state);
    }

    public void ChangeCracePrice_MultiplayerSelection(string _price)
    {
        UIMultiplayerSelection.CracePriceText.text = "1 $CRACE : " + _price + "$";
    }

    public void ChangeDisclaimer_MultiplayerSelection()
    {
        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice);
        UIMultiplayerSelection.Disclaimer.text = "*price: " + Constants.MultiplayerPrice + "$" + " (" + Constants.CalculatedCrace.ToString() + " $CRACE)";
    }

    public void EnableSelection_MultiplayerSelection()
    {
        Constants.GetCracePrice();
        ChangeCracePrice_MultiplayerSelection(Constants.CracePrice.ToString());
        ChangeDisclaimer_MultiplayerSelection();
        ToggleScreen_MultiplayerSelection(true);
    }

    public void DisableScreen_MultiplayerSelection()
    {
        ToggleScreen_MultiplayerSelection(false);
    }
    #endregion
}