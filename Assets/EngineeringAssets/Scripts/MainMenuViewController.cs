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
using Photon.Pun;

#region SuperClasses

[Serializable]
public class TokenCarSelectionUI
{
    public GameObject MainScreen;
    public TextMeshProUGUI TokenText;
    public Button NextButton;
    public Button PreviousButton;
}

[Serializable]
public class CraceApprovalUI
{
    public GameObject MainScreen;
    public Button ApproveButton;
    public Button CancelButton;
}

[Serializable]
public class ConnectionDetail
{
    public GameObject DetailScreen;
    public TextMeshProUGUI WinText;
    public TextMeshProUGUI WinnerNameText;
    public Image FlagImage;
}

[Serializable]
public class MultiplayerSelectionUI
{
    [Tooltip("Main screen for Multiplayer Selection.")]
    public GameObject MainScreen;
    [Tooltip("Text reference for coin price in dollars.")]
    public TextMeshProUGUI CracePriceText;
    [Tooltip("Button Reference for Cancel")]
    public Button CancelButton;
    [Tooltip("Button Reference for Play  Play for 5$")]
    public Button PlayButton_5;
    [Tooltip("Button Reference for Play Play for 10$")]
    public Button PlayButton_10;
    [Tooltip("Button Reference for Play Play for 50$")]
    public Button PlayButton_50;
    [Tooltip("Button Reference for Play for 100$")]
    public Button PlayButton_100;

    [Tooltip("Text reference for disclaimer for 5$")]
    public TextMeshProUGUI Disclaimer_5;
    [Tooltip("Text reference for disclaimer for 10$")]
    public TextMeshProUGUI Disclaimer_10;
    [Tooltip("Text reference for disclaimer for 50$")]
    public TextMeshProUGUI Disclaimer_50;
    [Tooltip("Text reference for disclaimer for 100$")]
    public TextMeshProUGUI Disclaimer_100;

    [Tooltip("Gameobject reference for multiplayer selection selection")]
    public GameObject MultiplayerSelection;
    [Tooltip("Button Reference for free play multiplayer")]
    public Button FreeMultiplayerButton;
    [Tooltip("Button Reference for waging multiplayer")]
    public Button WageMultiplayerButton;

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
    [Tooltip("Dropdown game object that will show list of pinged regions")]
    public GameObject RegionPingsDropdown;
    [Tooltip("Player Count text for connection screen.")]
    public TextMeshProUGUI PlayerCountText;
    [Tooltip("Detail reference for one player")]
    public ConnectionDetail Detail01;
    [Tooltip("Detail reference for other player")]
    public ConnectionDetail Detail02;
    [Tooltip("Text reference for Verses")]
    public TextMeshProUGUI VSText;
    [Tooltip("Button reference for deposit")]
    public Button DepositButton;
    [Tooltip("Text reference for deposit")]
    public TextMeshProUGUI DepositWaitText;
    [Tooltip("Button reference for withdraw")]
    public Button WithdrawButton;
    [Tooltip("GameObject reference for Timer")]
    public GameObject TimerObject;
    [Tooltip("Text reference for Timer for withdraw")]
    public TextMeshProUGUI TimerText;
    [HideInInspector]
    public int TimerTemp;
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
    public GameObject NextWeekScreen;
    public GameObject ActiveScreen;
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
    [SerializeField] private GameObject MessageUIContainer;
    [SerializeField] private TextMeshProUGUI ToastMsgText = null;
    [SerializeField] private TextMeshProUGUI UserNameText = null;
    [SerializeField] private Image FlagIcon = null;

    public GameObject TournamentMiniScreen;
    public TournamentUI UITournament;
    public RegisterUI UIRegister;
    public LoginUI UILogin;
    public SelectionUI UISelection;
    public FlagSelectionUI UIFlagSelection;
    public GarageUI UIGarage;
    public ForgetPasswordUI UIForgetPassword;
    private string[] CarNames = new string[5] { "Bonecrusher", "Merky", "CyberCar", "Coinrarri", "Malibu Express" };
    public GameObject ResendPopUp;
    public GameObject ResendPopUpContainer;
    public SettingsUI UISetting;
    public ConnectionUI UIConnection;
    public MultiplayerSelectionUI UIMultiplayerSelection;
    public CraceApprovalUI UICraceApproval;
    public TokenCarSelectionUI UITokenCarSelection;
    public GameObject MultiplayerPrefab;

    public GameObject SuccessIcon;
    public GameObject WarningIcon;

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
        _singlePlayerButton.onClick.AddListener(OnGoToCarSelectionPractice);
        _tournamentButton.onClick.AddListener(OnGoToCarSelectionTournament);
        //_backToModeSelectionButton.onClick.AddListener(OnGoBackToModeSelection);
        _goToMapSelectionButton.onClick.AddListener(OnGoToMapSelection);
        _backToCarSelectionButton.onClick.AddListener(BackToGoToCarSelection);
        _startRaceButton.onClick.AddListener(StartRace);
        _logoutButton.onClick.AddListener(SignOutUser);

        _currentSelectedCarIndex = 0;
        _SelectedTokenNameIndex = 0;
        _SelectedTokenIDIndex = 0;
        //UpdateSelectedCarVisual(_currentSelectedCarIndex);
        _versionText.text = APP_VERSION;

        _nextCarButton.onClick.AddListener(OnNextCar);
        _prevCarButton.onClick.AddListener(OnPrevCar);
        _nextMapButton.onClick.AddListener(OnNextMap);
        _prevMapButton.onClick.AddListener(OnPrevMap);

        UITokenCarSelection.NextButton.onClick.AddListener(OnNextToken);
        UITokenCarSelection.PreviousButton.onClick.AddListener(OnPrevToken);

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
        SubscribeButton_CraceUI();

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
        if (_state)
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
        if (WalletManager.Instance.CheckBalanceTournament(false, false, true, false))
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
                        //    Constants.PrintError("pass time is over, resting it");
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
                    Constants.PrintError("WM instance is null BuyPassCraceClicked_SelectionUI");
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
                        //   Constants.PrintError("pass time is over, resting it");
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
                    Constants.PrintError("WM instance is null PlayFromPass_SelectionUI");
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
                    if (WalletManager.Instance.CheckBalanceTournament(false, true, false, false))
                    {
                        ShowToast(2f, "Congrats!, You have received " + Constants.DiscountPercentage + "% discount.");
                        TicketPrice = (TicketPrice * DiscountPercentage) / 100;
                    }

                    if (WalletManager.Instance.CheckBalanceTournament(true, false, false, false))
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
                    Constants.PrintError("WM instance is null PlayOnce_SelectionUI");
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
        TournamentMiniScreen.SetActive(true);

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
                    if (Constants.isUsingFirebaseSDK)
                        FirebaseManager.Instance.LoginUser(FirebaseManager.Instance.Credentails.Email, FirebaseManager.Instance.Credentails.Password, FirebaseManager.Instance.Credentails.UserName);
                    else
                    {
                        apiRequestHandler.Instance.signInWithEmail(FirebaseManager.Instance.Credentails.Email,
                            FirebaseManager.Instance.Credentails.Password);
                    }
                }
                else
                {
                    Constants.WalletChanged = false;
                    ShowToast(3f, "Previous connected wallet was changed, auto login will not work.");
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
    public void ErrorMessage(string message = "Something went wrong, Please try again")
    {
        LoadingScreen.SetActive(false);
        ResetRegisterFields();
        ShowToast(3f, message);
    }
    public void SomethingWentWrong()
    {
        LoadingScreen.SetActive(false);
        ResetRegisterFields();
        ShowToast(3f, "credentails invalid, please try again.");
    }
    public void SomethingWentWrongMessage()
    {
        LoadingScreen.SetActive(false);
        ResetRegisterFields();
        ShowToast(3f, "Something went wrong, Please try again");
    }
    public void ShowToast(float _time, string _msg,bool showSuccessIcon=false)
    {
        MessageUI.SetActive(true);
        ToastMsgText.text = _msg;
        AnimationsHandler.Instance.runPopupAnimation(MessageUIContainer);
        StartCoroutine(DisableToast(_time));

        if (showSuccessIcon)
        {
            SuccessIcon.SetActive(true);
            WarningIcon.SetActive(false);
        }
        else
        {
            SuccessIcon.SetActive(false);
            WarningIcon.SetActive(true);
        }

    }

    

    public void ShowResendScreen(float _Sec)
    {
        ResendPopUp.SetActive(true);
        AnimationsHandler.Instance.runPopupAnimation(ResendPopUpContainer);
        Invoke("DisableResendScreen", _Sec);
    }

    public void DisableResendScreen()
    {
        ResendPopUp.SetActive(false); ;
    }
    public void OnGoBackToModeSelection()
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
        if (Constants.IsTest)
        {
            WalletConnected = true;
        }
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
        if (Constants.isUsingFirebaseSDK)
            FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckWalletDB(PlayerPrefs.GetString("Account")));
        else
        {
            apiRequestHandler.Instance.signUpWithEmail(email, pass, userName);
        }
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
        TournamentMiniScreen.SetActive(false);
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
        if (Constants.IsTest)
        {
            WalletConnected = true; //set temporary will remove later
        }
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
        if (IsMultiplayer)
        {
            if(TournamentManager.Instance)
            {
                if(TournamentManager.Instance.DataTournament.IsSingleMap)
                {
                    Constants.SelectedSingleLevel = TournamentManager.Instance.DataTournament.LevelIndex;
                    _levelsSettings.Add(_allLevelsSettings[Constants.SelectedSingleLevel - 1]);
                }else
                {
                    _levelsSettings.Add(_allLevelsSettings[0]);
                    _levelsSettings.Add(_allLevelsSettings[1]);
                    _levelsSettings.Add(_allLevelsSettings[2]);
                }
            }    
        }
        else
        {
            if (IsPractice)
            {
                _levelsSettings.Add(_allLevelsSettings[0]);
                _levelsSettings.Add(_allLevelsSettings[1]);
                _levelsSettings.Add(_allLevelsSettings[2]);
                //_levelsSettings.Add(_allLevelsSettings[3]);
            }
            else if (IsTournament)
            {
                _levelsSettings.Add(_allLevelsSettings[1]);
                //_levelsSettings.Add(_allLevelsSettings[2]);
            }
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
            WalletConnected = true;

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
            WalletConnected = true;

        if (WalletConnected)//WalletConnected
        {
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            IsTournament = false;
            IsPractice = true;
            // IsMultiplayer = false;
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
    public void OnGoToCarSelectionPractice()
    {
        Constants.IsMultiplayer = false;
        Constants.EarnMultiplayer = false;
        OnGoToCarSelection();
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
                    Constants.PrintError("WM instance is null OnGoToCarSelectionTournament");
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
        ToggleTokenScreen(true);
        int newIndex = _currentSelectedCarIndex + 1;
        newIndex %= _selecteableCars.Count;

        if (!Constants.EarnMultiplayer)
            ToggleTokenScreen(false);

        for (int i = 0; i < Constants.TokenNFT.Count; i++)
        {
            if (Constants.TokenNFT[i].Name.Contains(_selecteableCars[newIndex].carSettings.Name))
            {
                _SelectedTokenNameIndex = i;
                _SelectedTokenIDIndex = 0;
                break;
            }
        }

        UpdateSelectedCarVisual(newIndex);
        UpdateToken();
    }

    private void OnPrevCar()
    {
        ToggleTokenScreen(true);
        int newIndex = _currentSelectedCarIndex;
        newIndex--;
        if (newIndex < 0)
            newIndex = _selecteableCars.Count + newIndex;

        if (!Constants.EarnMultiplayer)
            ToggleTokenScreen(false);

        for (int i = 0; i < Constants.TokenNFT.Count; i++)
        {
            if (Constants.TokenNFT[i].Name.Contains(_selecteableCars[newIndex].carSettings.Name))
            {
                _SelectedTokenNameIndex = i;
                _SelectedTokenIDIndex = 0;
                break;
            }
        }

        UpdateSelectedCarVisual(newIndex);
        UpdateToken();
    }

    public void OnNextToken()
    {
        if (_SelectedTokenIDIndex < Constants.TokenNFT[_SelectedTokenNameIndex].ID.Count-1)
            _SelectedTokenIDIndex++;

        UpdateToken();
    }

    public void OnPrevToken()
    {
        if (_SelectedTokenIDIndex > 0)
            _SelectedTokenIDIndex--;

        UpdateToken();
    }

    public void UpdateToken()
    {
        try
        {
            UITokenCarSelection.TokenText.text = "#" + Constants.TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex];
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
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
            Constants.EarnMultiplayer = false;
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
            Constants.EarnMultiplayer = false;

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

    public void StartRace()
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
        SelectedCar = _selecteableCars[_currentSelectedCarIndex].carSettings;

        if (Constants.IsMultiplayer)
        {
            if (MultiplayerManager.Instance)
            {
                Constants.SelectedLevel = MainMenuViewController.Instance.getSelectedLevel() + 1;
                //MultiplayerManager.Instance.ConnectToPhotonServer();
                MainMenuViewController.Instance.SelectMultiplayer_ConnectionUI();
            } else
            {
                Constants.PrintError("MM Instance is null");
            }
        }
        else
        {
            LoadDesiredScene();
        }
    }

    public void LoadDesiredScene()
    {
        SceneManager.LoadScene(_levelsSettings[_currentlySelectedLevelIndex].SceneName, LoadSceneMode.Single);
    }


    public void TempStartRace()
    {
        //IsPractice = true;
        //SelectedCar = _selecteableCars[0].carSettings;
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
            WalletConnected = true;

        if (WalletConnected)//WalletConnected
        {

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

    public void ToggleTokenScreen(bool _state)
    {
        UITokenCarSelection.MainScreen.SetActive(_state);
    }
    public void CheckBoughtCars()
    {
        if (Constants.EarnMultiplayer)
        {
            for (int i = 0; i < _selecteableCars.Count; i++)
            {
                _selecteableCars[i].Deactivate();
            }

            ToggleTokenScreen(true);
            UpdateToken();
        }else
        {
            ToggleTokenScreen(false);
        }

        if (Constants.CheckAllNFT || Constants.NFTStored == 0)
        {
            //for (int i = 0; i < Constants.TokenNFT.Count; i++)
            //{
            //    Debug.Log(Constants.TokenNFT[i].Name);
            //    for (int j = 0; j < Constants.TokenNFT[i].ID.Count; j++)
            //    {
            //        Debug.Log(Constants.TokenNFT[i].ID[j]);
            //    }
            //}

            if (Constants.NFTStored == 0)
            {
                _selecteableCars.Clear();
                if (!Constants.EarnMultiplayer)
                {
                    _selecteableCars.Add(_allCars[0].CarDetail);

                    LoadingScreen.SetActive(false);
                    _currentSelectedCarIndex = 0;
                    UpdateSelectedCarVisual(_currentSelectedCarIndex);
                }
            }

            else if (Constants.StoredCarNames.Count != 0)
            {
                _selecteableCars.Clear();

                if (!Constants.EarnMultiplayer)
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
       // Debug.LogError(("Sending email verification"));
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
        //UIConnection.MultiplayerButton.onClick.AddListener(onMultiplayerBtnClick);
        UIConnection.MultiplayerButton.onClick.AddListener(EnableSelection_MultiplayerSelection);
        UIConnection.BackButton.onClick.AddListener(DisableScreen_ConnectionUI);
        UIConnection.DepositButton.onClick.AddListener(DepositAmount);
        UIConnection.WithdrawButton.onClick.AddListener(WithDrawDeposit);
    }
    public void ToggleDepositButton_ConnectionUI(bool _state)
    {
        UIConnection.DepositButton.interactable = _state;
    }

    public void ToggleWithdrawButton_ConnectionUI(bool _state)
    {
        UIConnection.WithdrawButton.interactable = _state;
    }

    public void ChangeTimerText_ConnectionUI(string _txt)
    {
        UIConnection.TimerText.text = _txt;
    }

    public void DisableWithDrawTimer_ConnectionUI()
    {
        StopCoroutine(WithDrawTimer_ConnectionUI());
        UIConnection.TimerObject.SetActive(false);
    }
    public void EnableWithDrawTimer_ConnectionUI()
    {
        if (!Constants.TimerRunning)
        {
            TimerRunning = true;
            UIConnection.TimerObject.SetActive(true);
            ToggleWithdrawButton_ConnectionUI(false);
            UIConnection.TimerTemp = Constants.WithdrawTime;
            ChangeTimerText_ConnectionUI(UIConnection.TimerTemp.ToString());

            if (Constants.DepositDone && !Constants.CanWithdraw)
                StartCoroutine(WithDrawTimer_ConnectionUI());
        }
    }

    IEnumerator WithDrawTimer_ConnectionUI()
    {
        while (!Constants.CanWithdraw && TimerRunning)
        {
            UIConnection.TimerTemp -= 1;
            ChangeTimerText_ConnectionUI(UIConnection.TimerTemp.ToString());
            yield return new WaitForSeconds(1f);

            if (UIConnection.TimerTemp <= 0)
            {
                UIConnection.TimerObject.SetActive(false);
                Constants.CanWithdraw = true;
                ToggleWithdrawButton_ConnectionUI(true);
            }
        }
    }

    public void ChangeDepositText_ConnectionUI(string _txt)
    {
        UIConnection.DepositWaitText.text = _txt;
    }

    public void ToggleButton_ConnectionUI(bool _state)
    {
        UIConnection.DepositButton.gameObject.SetActive(_state);
        UIConnection.WithdrawButton.gameObject.SetActive(_state);
    }

    public void UpdateDeposit_ConnectionUI(string _txt, bool _toggle)
    {

        if (!Constants.FreeMultiplayer)
        {
            ToggleButton_ConnectionUI(true);
            ChangeDepositText_ConnectionUI(_txt);
            ToggleDepositButton_ConnectionUI(_toggle);
        }
        else
        {
            ChangeDepositText_ConnectionUI("");
            ToggleButton_ConnectionUI(false);
        }
    }

    public void DepositAmount()
    {
        if (WalletManager.Instance)
            WalletManager.Instance.CallDeposit();
    }

    public void WithDrawDeposit()
    {
        if (WalletManager.Instance)
            WalletManager.Instance.CallWithdraw();
    }

    public void ToggleBackButton_ConnectionUI(bool state)
    {
        UIConnection.BackButton.gameObject.SetActive(state);
    }

    public void onMultiplayerBtnClick()
    {
        if(IsTest)
            WalletConnected = true;

        if (WalletConnected)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                ToggleScreen_MultiplayerSelection(false);
                Constants.IsMultiplayer = true;
                MainMenuViewController.Instance.OnGoToCarSelection();
            }
            else
            {
                LoadingScreen.SetActive(false);
                Constants.PrintError("WM instance is null onMultiplayerBtnClick");
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
        ToggleBackButton_ConnectionUI(true);

        UIConnection.Detail01.DetailScreen.SetActive(true);
        UpdateDetailData(true, Constants.UserName, Constants.TotalWins.ToString(), Constants.FlagSelectedIndex);

        UIConnection.Detail02.DetailScreen.SetActive(false);
        UIConnection.VSText.gameObject.SetActive(false);

        RegionPinged = false;
        ToggleScreen_ConnectionUI(true);
        AnimateConnectingDetail_ConnectionUI(UIConnection.Detail01.DetailScreen,true);

        //Debug.Log(_levelsSettings[_currentlySelectedLevelIndex].SceneName);

        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.ConnectToPhotonServer();
    }

    public void ToggleSecondDetail(bool _enable,string _name,string _wins,int _index)
    {
        if (_enable)
        {
            UIConnection.Detail02.DetailScreen.SetActive(true);
            UIConnection.VSText.gameObject.SetActive(true);
            UpdateDetailData(false, _name, _wins, _index);
            AnimateConnectingDetail_ConnectionUI(UIConnection.Detail02.DetailScreen, false);
        }
        else
        {
            UIConnection.Detail02.DetailScreen.SetActive(false);
            UIConnection.VSText.gameObject.SetActive(false);
        }
    }

    public void DisableScreen_ConnectionUI()
    {
        //Constants.IsMultiplayer = false;
        ChangeConnectionText_ConnectionUI("connecting...");
        ChangeRegionText_ConnectionUI("Selected Region : n/a");
        ToggleScreen_ConnectionUI(false);

        RegionPinged = false;

        Dropdown RegionList = UIConnection.RegionPingsDropdown.GetComponent<Dropdown>();
        RegionList.interactable = false;
        RegionList.options.Clear();
        RegionList.options.Add(new Dropdown.OptionData() { text = "Select Region" });
        RegionList.value = 0;
        Constants.RegionChanged = false;
        PhotonNetwork.SelectedRegion = "";
        RegionList.interactable = true;

        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.DisconnectPhoton();
    }

    public void UpdateDetailData(bool isPlayer1, string _name,string _wins,int index)
    {
        if (isPlayer1)
        {
            UIConnection.Detail01.WinnerNameText.text = _name;
            UIConnection.Detail01.WinText.text = "WINS : "+_wins;
            UIConnection.Detail01.FlagImage.sprite = FlagSkins.Instance.FlagSpriteWithIndex(index);
        }
        else
        {
            UIConnection.Detail02.WinnerNameText.text = _name;
            UIConnection.Detail02.WinText.text = "WINS : " + _wins;
            UIConnection.Detail02.FlagImage.sprite = FlagSkins.Instance.FlagSpriteWithIndex(index);
        }

    }

    public IEnumerator ShowPingedRegionList_ConnectionUI()
    {
        yield return new WaitUntil(() => PhotonNetwork.GotPingResult);
        UpdatePingList(PhotonNetwork.pingedRegions, PhotonNetwork.pingedRegionPings);
        PhotonNetwork.GotPingResult = false;
    }

    public void UpdatePingList(string[] regions, string[] pings)
    {
        if (!RegionPinged)
        {
            RegionPinged = true;
            //RegionPingsDropdown
            var dropdown = UIConnection.RegionPingsDropdown.GetComponent<Dropdown>();
            SetManualRegion _RegionRef = UIConnection.RegionPingsDropdown.GetComponent<SetManualRegion>();
            dropdown.options.Clear();
            List<string> _regions = new List<string>();
            if (pings.Length > 0)
            {
                int minimumPing = int.Parse(pings[0]);
                int currentPing;
                dropdown.value = 1;
                for (int i = 0; i < regions.Length; i++)
                {
                    dropdown.options.Add(new Dropdown.OptionData() { text = regions[i] + " " + pings[i] + "ms" });
                    _regions.Add(regions[i]);
                    currentPing = int.Parse(pings[i]);
                    if (currentPing < minimumPing)
                    {
                        minimumPing = currentPing;
                        dropdown.value = i + 1;
                    }
                }

                _RegionRef.SetRegionString(_regions);
            }
            else
            {
                //Debug.LogError("region list is empty");
            }
        }
    }

    //this function will be used to animate the connecting details screen
    //@param {_screen,_isMyScreen}, _screen: type(gameobject), contains reference to the screen to animate
    //_isMyScreen, _isMyScreen: type(bool), indicates if this is my player's screen that you are animating
    //@return {} no return
    private void AnimateConnectingDetail_ConnectionUI(GameObject _screen,Boolean _isMyScreen)
    {
        //save the initial position of this screen
        Vector3 initialPos = _screen.transform.position;
        //if this is my player's screen then move it to the left otherwise move it to the right
        if (_isMyScreen)
            _screen.transform.position = initialPos - new Vector3(300, 0, 0);
        else
            _screen.transform.position = initialPos + new Vector3(300, 0, 0);
        //start tweening the screen
        iTween.MoveTo(_screen, iTween.Hash("position", initialPos, "time", 1.5f, "easetype", iTween.EaseType.easeInOutSine));
    }
    #endregion

    #region Multiplayer Selection
    public void ToggleSelection_MultiplayerSelection(bool _state)
    {
        UIMultiplayerSelection.MultiplayerSelection.SetActive(_state);
    }
    public void SelectWage__MultiplayerSelection(int _amount)
    {
        Constants.SelectedWage = _amount;
        Constants.ChipraceScore = "50";

        //if (_amount == 5)
           // Constants.ChipraceScore = "10";
        //if (_amount == 10)
            //Constants.ChipraceScore = "25";
        //if (_amount == 50)
            //Constants.ChipraceScore = "100";
        //if (_amount == 100)
            //Constants.ChipraceScore = "250";

        Constants.ConvertDollarToCrace(Constants.SelectedWage);
        Constants.SelectedCrace = Constants.CalculatedCrace;

        onMultiplayerBtnClick();
    }
    public void SubscribeEvents_MultiplayerSelection()
    {
        UIMultiplayerSelection.CancelButton.onClick.AddListener(DisableScreen_MultiplayerSelection);
        UIMultiplayerSelection.FreeMultiplayerButton.onClick.AddListener(FreeMultiplayer_MultiplayerSelection);
        UIMultiplayerSelection.WageMultiplayerButton.onClick.AddListener(EnableWage_MultiplayerSelection);
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
        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_1);
        UIMultiplayerSelection.Disclaimer_5.text = "*price: " + Constants.MultiplayerPrice_1 + "$" + " (" + Constants.CalculatedCrace.ToString() + " $CRACE)";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_2);
        UIMultiplayerSelection.Disclaimer_10.text = "*price: " + Constants.MultiplayerPrice_2 + "$" + " (" + Constants.CalculatedCrace.ToString() + " $CRACE)";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_3);
        UIMultiplayerSelection.Disclaimer_50.text = "*price: " + Constants.MultiplayerPrice_3 + "$" + " (" + Constants.CalculatedCrace.ToString() + " $CRACE)";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_4);
        UIMultiplayerSelection.Disclaimer_100.text = "*price: " + Constants.MultiplayerPrice_4 + "$" + " (" + Constants.CalculatedCrace.ToString() + " $CRACE)";
    }

    public void EnableSelection_MultiplayerSelection()
    {
        ToggleSelection_MultiplayerSelection(true);
    }

    public void EnableWage_MultiplayerSelection()
    {
        Constants.FreeMultiplayer = false;
        Constants.EarnMultiplayer = true;
        LoadingScreen.SetActive(true);

        if (Constants.CheckAllNFT || Constants.NFTStored == 0)
        {
            if (Constants.NFTStored == 0)
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No NFT was purchased, please purchase one to play.");
            }
            else
            {
                ToggleSelection_MultiplayerSelection(false);
                Constants.GetCracePrice();
                ChangeCracePrice_MultiplayerSelection(Constants.CracePrice.ToString());
                ChangeDisclaimer_MultiplayerSelection();
                ToggleScreen_MultiplayerSelection(true);
                LoadingScreen.SetActive(false);
            }
        }
        else
        {
            Invoke("EnableWage_MultiplayerSelection", 1f);
        }
    }

    public void FreeMultiplayer_MultiplayerSelection()
    {
        Constants.FreeMultiplayer = true;
        Constants.EarnMultiplayer = false;
        ToggleSelection_MultiplayerSelection(false);
        onMultiplayerBtnClick();
    }

    public void DisableScreen_MultiplayerSelection()
    {
        ToggleScreen_MultiplayerSelection(false);
    }
    #endregion

    #region Crace Aproval UI
    public void ToogleScreen_CraceUI(bool _state)
    {
        UICraceApproval.MainScreen.SetActive(_state);
    }

    public void SubscribeButton_CraceUI()
    {
        UICraceApproval.ApproveButton.onClick.AddListener(ApproveCrace_CraceUI);
        UICraceApproval.CancelButton.onClick.AddListener(CancelApprove_CraceUI);

    }

    public void ApproveCrace_CraceUI()
    {
        if (WalletManager.Instance)
            WalletManager.Instance.CallApproveCrace();
    }

    public void CancelApprove_CraceUI()
    {
        ToogleScreen_CraceUI(false);
    }
    #endregion
}