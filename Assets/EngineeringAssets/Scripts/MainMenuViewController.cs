using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using static Constants;
using System.Runtime.InteropServices;
using Photon.Pun;

#region SuperClasses

[Serializable]
public class MemberUI
{
    public GameObject MainScreen;
    public Button SubmitButton;
}

[Serializable]
public class SecondTournamentUI
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
public class SecondTourSelectionUI
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
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetStorageClass(string key, string ObjectName, string callback);

    [DllImport("__Internal")]
    private static extern string GetStorage(string key, string ObjectName, string callback);
#endif

    public static CarSettings SelectedCar;
    public static MainMenuViewController Instance;
    public GameObject MenuScreen;
    [SerializeField] private GameObject GameModeSelectionObject = null;
    [SerializeField] private GameObject CarSelectionObject = null;
    [SerializeField] private GameObject CarSelection3dObject = null;
    [SerializeField] private GameObject MapSelection = null;
    [SerializeField] private Button _singlePlayerButton = null;
    [SerializeField] private Button _tournamentButton = null;
    [SerializeField] private Button _secondTournamentButton = null;
    [SerializeField] private Button _backToModeSelectionButton = null;
    [SerializeField] private Button _goToMapSelectionButton = null;
    [SerializeField] private Button _backToCarSelectionButton = null;
    [SerializeField] private Button _startRaceButton = null;
    [SerializeField] private Button _nextCarButton = null;
    [SerializeField] private Button _prevCarButton = null;
    [SerializeField] private Button _nextMapButton = null;
    [SerializeField] private Button _prevMapButton = null;
    [SerializeField] private Button _logoutButton = null;
    [SerializeField] private Button _destructionDerbyButton = null;
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

    [SerializeField] private TextMeshProUGUI CCashText = null;

    public GameObject TournamentMiniScreen;
    public TournamentUI UITournament;
    public SecondTournamentUI UISecondTournament;
    public RegisterUI UIRegister;
    public LoginUI UILogin;
    public SelectionUI UISelection;
    public SecondTourSelectionUI UISecondTourSelection;
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
    public MemberUI UIMember;
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
    string MemberPass = "";
    private const string MatchEmailPatternOld =
       "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`{}|~\\w])*)(?<=[0-9a-z])@))(?([)([(\\d{1,3}.){3}\\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$";

    private const string MatchEmailPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    
    private List<CarSelection> SelectedCars = new List<CarSelection>();
    int carIndex = 0;
    int nextCarIndex = 0;
    int PrevCarIndex = 0;
    GameObject _generatedPrefab;

    #endregion

    #region StartFunctionality
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
        _secondTournamentButton.onClick.AddListener(OnGoToCarSelectionSecondTourTournament);


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

        UIMember.SubmitButton.onClick.AddListener(OnSubmitClick_MemberUI);

        //UIGarage.BackButton.onClick.AddListener(BackButton_Garage);
        // UIGarage.GarageButton.onClick.AddListener(GarageButton_Garage);

        ButtonListenerRegister();
        ButtonListenerLogin();
        ButtonListeners_SelectionUI();
        SecondTourButtonListeners_SelectionUI();
        ButtonListener_FlagSelectionUI();
        CheckForAutoLogin();
        SubscribeEvents_PasswordReset();
        SubscribeEvents_Settings();
        SubscribeEvents_ConnectionUI();
        SubscribeEvents_MultiplayerSelection();
        SubscribeButton_CraceUI();
        AddButtonListeners_DD();

        if (Constants.EarlyBuild)
            UIMember.MainScreen.SetActive(true);
        else
            UIMember.MainScreen.SetActive(false);

#if UNITY_WEBGL && !UNITY_EDITOR
            GetStorage(Constants.SoundKey,this.gameObject.name,"OnGetSound");
            GetStorage(Constants.MusicKey,this.gameObject.name,"OnGetMusic");
#endif
    }

    public void CheckForAutoLogin()
    {
        if (FirebaseMoralisManager.Instance)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        GetStorageClass(Constants.CredKey,this.gameObject.name,"OnGetCred");
#endif
        }
    }

    #endregion

    #region TournamentSelectionUI (MainData/Selections)
    public void ToggleScreen_SelectionUI(bool _state)
    {
        if (_state)
            ChangeDisclaimerTexts_SelectionUI("*Price: " + Constants.TournamentPassPrice + " $"+Constants.GetCurrencyName()+". unlimited attempts in a single tournament.", "*if you have the pass, enter the tournament here.", "*price: " + TournamentManager.Instance.DataTournament.TicketPrice + " $" + Constants.GetCurrencyName() +", if you hold " + Constants.DiscountForCrace + " $"+Constants.TokenName+" - " + Constants.DiscountPercentage + "% discount.");

        UISelection.MainScreen.SetActive(_state);
    }

    public void SecondTourToggleScreen_SelectionUI(bool _state)
    {
        if (_state)
            SecondTourChangeDisclaimerTexts_SelectionUI("*Price: " + Constants.SecondTournamentPassPrice + " $" + Constants.GetCurrencyName() +". unlimited attempts in a single tournament.", "*if you have the pass, enter the tournament here.", "*price: " + TournamentManager.Instance.DataTournament.GTicketPrice + " $" + Constants.GetCurrencyName() +", if you hold " + Constants.DiscountForCrace + " $"+Constants.TokenName+" - " + Constants.DiscountPercentage + "% discount.");

        UISecondTourSelection.MainScreen.SetActive(_state);
    }

    public void TogglePassScreen_SelectionUI(bool _state)
    {
        UISelection.TournamentPassScreen.SetActive(_state);
    }

    public void SecondTourTogglePassScreen_SelectionUI(bool _state)
    {
        UISecondTourSelection.TournamentPassScreen.SetActive(_state);
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

    public void SecondTourButtonListeners_SelectionUI()
    {
        UISecondTourSelection.BuyPassButton.onClick.AddListener(SecondTourBuyPassClicked_SelectionUI);
        UISecondTourSelection.PlayFromPassButton.onClick.AddListener(SecondTourPlayFromPass_SelectionUI);
        UISecondTourSelection.CancelButton.onClick.AddListener(SecondTourCancelSelection_SelectionUI);
        UISecondTourSelection.BuyPassCraceButton.onClick.AddListener(SecondTourBuyPassCraceClicked_SelectionUI);
        UISecondTourSelection.BuyPassCancelButton.onClick.AddListener(SecondTourBackClickedPassScreen_SelectionUI);
        UISecondTourSelection.SingleTryButton.onClick.AddListener(SecondTourPlayOnce_SelectionUI);
    }

    public void ChangeDisclaimerTexts_SelectionUI(string txt1, string txt2, string txt3)
    {
        UISelection.BuyPassText.text = txt1;
        UISelection.PlayFromPassText.text = txt2;
        UISelection.SingleTryText.text = txt3;
    }

    public void SecondTourChangeDisclaimerTexts_SelectionUI(string txt1, string txt2, string txt3)
    {
        UISecondTourSelection.BuyPassText.text = txt1;
        UISecondTourSelection.PlayFromPassText.text = txt2;
        UISecondTourSelection.SingleTryText.text = txt3;
    }

    public void CancelSelection_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
    }

    public void SecondTourCancelSelection_SelectionUI()
    {
        SecondTourToggleScreen_SelectionUI(false);
    }

    public void BuyPassClicked_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
        TogglePassScreen_SelectionUI(true);
    }

    public void SecondTourBuyPassClicked_SelectionUI()
    {
        SecondTourToggleScreen_SelectionUI(false);
        SecondTourTogglePassScreen_SelectionUI(true);
    }

    public void BackClickedPassScreen_SelectionUI()
    {
        ToggleScreen_SelectionUI(false);
        TogglePassScreen_SelectionUI(false);
    }

    public void SecondTourBackClickedPassScreen_SelectionUI()
    {
        SecondTourToggleScreen_SelectionUI(false);
        SecondTourTogglePassScreen_SelectionUI(false);
    }

    public void BuyPasswithCrace()
    {
        if (Constants.ConvertToCCash)
        {
            if (FirebaseMoralisManager.Instance.PlayerData.VC_Amount >= Constants.TournamentPassPrice)
            {
                Constants.GATransferAmount = Constants.TournamentPassPrice;
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= Constants.TournamentPassPrice;

                if (AnalyticsManager.Instance)
                    AnalyticsManager.Instance.TournamentPassEvent(Constants.GATransferAmount);

                Constants.PrintLog("pass bought was success");
                UpdateVCText(FirebaseMoralisManager.Instance.PlayerData.VC_Amount);
                OnPassBuy(true);
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value, need " + Constants.TournamentPassPrice + " $" + Constants.GetCurrencyName());
            }
        }
        else
        {
            if (WalletManager.Instance.CheckBalanceTournament(false, false, true, false, false, false))
            {
                SecondTourBuyingPass = false;
                BuyingPass = true;
                WalletManager.Instance.TransferToken(TournamentPassPrice, false);
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value, need " + Constants.TournamentPassPrice + " $" + Constants.GetCurrencyName());
            }
        }
    }

    public void SecondTourBuyPasswithCrace()
    {
        if (Constants.ConvertToCCash)
        {
            if (FirebaseMoralisManager.Instance.PlayerData.VC_Amount >= Constants.SecondTournamentPassPrice)
            {
                Constants.GATransferAmount = Constants.SecondTournamentPassPrice;
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= Constants.SecondTournamentPassPrice;

                if (AnalyticsManager.Instance)
                    AnalyticsManager.Instance.TournamentPassEvent(Constants.GATransferAmount);

                Constants.PrintLog("pass bought was success");
                UpdateVCText(FirebaseMoralisManager.Instance.PlayerData.VC_Amount);
                SecondTourOnPassBuy(true);
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value, need " + Constants.SecondTournamentPassPrice + " $" + Constants.GetCurrencyName());
            }
        }
        else
        {
            if (WalletManager.Instance.CheckBalanceTournament(false, false, false, false, true, false))
            {
                BuyingPass = false;
                SecondTourBuyingPass = true;
                WalletManager.Instance.TransferToken(SecondTournamentPassPrice, false);
            }
            else
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value, need " + Constants.SecondTournamentPassPrice + " $" + Constants.GetCurrencyName());
            }
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
                    if (FirebaseMoralisManager.Instance.PlayerData.PassBought == false)
                    {
                        BuyPasswithCrace();
                    }
                    else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "You already own a tournament pass");
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

    public void SecondTourBuyPassCraceClicked_SelectionUI()
    {
        if (WalletConnected)
        {
            if (SecondTournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    if (FirebaseMoralisManager.Instance.PlayerData.GPassBought == false)
                    {
                        SecondTourBuyPasswithCrace();
                    }
                    else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "You already own a tournament pass");
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
            FirebaseMoralisManager.Instance.PlayerData.PassBought = true;
            FirebaseMoralisManager.Instance.PlayerData.TournamentEndDate = TournamentManager.Instance.DataTournament.EndDate;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    public void SecondTourOnPassBuy(bool _state)
    {
        if (_state)
        {
            ShowToast(3f, "You have successfully bought tournament pass for this tournament week.");
            LoadingScreen.SetActive(false);
            SecondTourBackClickedPassScreen_SelectionUI();

            FirebaseMoralisManager.Instance.PlayerData.GPassBought = true;
            FirebaseMoralisManager.Instance.PlayerData.GTournamentEndDate = TournamentManager.Instance.DataTournament.GEndDate;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
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
                    if (FirebaseMoralisManager.Instance.PlayerData.PassBought)
                    {
                        BackClickedPassScreen_SelectionUI();
                        StartTournament(true);
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

    public void SecondTourPlayFromPass_SelectionUI()
    {
        if (WalletConnected)
        {
            if (SecondTournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    if (FirebaseMoralisManager.Instance.PlayerData.GPassBought)
                    {

                        SecondTourBackClickedPassScreen_SelectionUI();
                        SecondTourStartTournament(true);
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
            WalletConnected = true;
            //StartTournament(true);
            //return;
        }

        if (WalletConnected)
        {
            if (TournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    TicketPrice = TournamentManager.Instance.DataTournament.TicketPrice;

                    if (WalletManager.Instance.CheckBalanceTournament(false, true, false, false, false, false))
                    {
                        ShowToast(2f, "Congrats!, You have received " + Constants.DiscountPercentage + "% discount.");
                        TicketPrice = (TicketPrice * DiscountPercentage) / 100;
                    }

                    if (Constants.ConvertToCCash)
                    {
                        if (FirebaseMoralisManager.Instance.PlayerData.VC_Amount >= TicketPrice)
                        {
                            Constants.GATransferAmount = TicketPrice;
                            FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= TicketPrice;
                            apiRequestHandler.Instance.updatePlayerData();

                            if (AnalyticsManager.Instance)
                                AnalyticsManager.Instance.TournamentTicketEvent(Constants.GATransferAmount);

                            Constants.PrintLog("transaction was success for tournament");
                            UpdateVCText(FirebaseMoralisManager.Instance.PlayerData.VC_Amount);
                            StartTournament(true);
                            
                        }
                        else
                        {
                            LoadingScreen.SetActive(false);
                            ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value.");
                        }
                    }
                    else
                    {
                        if (WalletManager.Instance.CheckBalanceTournament(true, false, false, false, false, false))
                        {
                            WalletManager.Instance.TransferToken(TicketPrice, false);
                        }
                        else
                        {
                            LoadingScreen.SetActive(false);
                            ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value.");
                        }
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

    public void SecondTourPlayOnce_SelectionUI()
    {
        if (Constants.IsTest)
        {
            WalletConnected = true;
            //SecondTourStartTournament(true);
            //return;
        }

        if (WalletConnected)
        {
            if (SecondTournamentActive == true)
            {
                LoadingScreen.SetActive(true);
                if (WalletManager.Instance)
                {
                    SecondTourTicketPrice = TournamentManager.Instance.DataTournament.GTicketPrice;
                    if (WalletManager.Instance.CheckBalanceTournament(false, true, false, false, false, false))
                    {
                        ShowToast(2f, "Congrats!, You have received " + Constants.DiscountPercentage + "% discount.");
                        SecondTourTicketPrice = (SecondTourTicketPrice * DiscountPercentage) / 100;
                    }

                    if (Constants.ConvertToCCash)
                    {
                        if (FirebaseMoralisManager.Instance.PlayerData.VC_Amount >= SecondTourTicketPrice)
                        {
                            Constants.GATransferAmount = SecondTourTicketPrice;
                            FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= SecondTourTicketPrice;
                            apiRequestHandler.Instance.updatePlayerData();

                            if (AnalyticsManager.Instance)
                                AnalyticsManager.Instance.TournamentTicketEvent(Constants.GATransferAmount);

                            Constants.PrintLog("transaction was success for tournament");
                            UpdateVCText(FirebaseMoralisManager.Instance.PlayerData.VC_Amount);
                            SecondTourStartTournament(true);

                        }
                        else
                        {
                            LoadingScreen.SetActive(false);
                            ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value.");
                        }
                    }
                    else
                    {
                        if (WalletManager.Instance.CheckBalanceTournament(false, false, false, false, false, true))
                        {
                            WalletManager.Instance.TransferToken(SecondTourTicketPrice, true);
                        }
                        else
                        {
                            LoadingScreen.SetActive(false);
                            ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value.");
                        }
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
            FirebaseMoralisManager.Instance.PlayerData.AvatarID = Constants.FlagSelectedIndex;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
        }
    }
    #endregion

    #region MISC

    public void UpdateVCText(double txt)
    {
        CCashText.text = txt.ToString("F1");
    }
    public void SignOutUser()
    {
        Constants.LoggedIn = false;
        ResetRegisterFields();
        TournamentMiniScreen.SetActive(true);


        Constants.StoredCarNames.Clear();
        Constants.ResetData();
        WalletManager.Instance.ResetData();

        if (FirebaseMoralisManager.Instance)
            FirebaseMoralisManager.Instance.LogoutUser();
    }

    public void OnGetCred(string info)
    {
        tempInfo = info;
        LoadingScreen.SetActive(false);
        if (info != null && info != "null")
        {
            if (Constants.WalletConnected)
            {
                CancelInvoke("InvokeCallCred");
                LoginAfterConnect(info);
            }
            else
            {
                Invoke("InvokeCallCred", 0.1f);
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
                FirebaseMoralisManager.Instance.Credentails = JsonConvert.DeserializeObject<AuthCredentials>(info);

                if (!Constants.WalletChanged)
                {
                    if (Constants.isUsingFirebaseSDK)
                        FirebaseMoralisManager.Instance.LoginUser(FirebaseMoralisManager.Instance.Credentails.Email, FirebaseMoralisManager.Instance.Credentails.Password, FirebaseMoralisManager.Instance.Credentails.UserName);
                    else
                    {
                        apiRequestHandler.Instance.signInWithEmail(FirebaseMoralisManager.Instance.Credentails.Email,
                            FirebaseMoralisManager.Instance.Credentails.Password);
                    }
                }
                else
                {
                    Constants.WalletChanged = false;
                    ShowToast(3f, "Previous connected wallet was changed, auto login will not work, please login again.");
                    LoadingScreen.SetActive(false);

                    if (FirebaseMoralisManager.Instance)
                        FirebaseMoralisManager.Instance.ResetStorage();
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
            FirebaseMoralisManager.Instance.CheckEmailForAuth(Constants.SavedEmail, Constants.SavedPass, Constants.SavedUserName);
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

    public void ShowToast(float _time, string _msg, bool showSuccessIcon = false)
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
            FirebaseMoralisManager.Instance.StartCoroutine(FirebaseMoralisManager.Instance.CheckWalletDB(PlayerPrefs.GetString("Account")));
        else
        {
            apiRequestHandler.Instance.signUpWithEmail(email, pass, userName);
        }
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
        if (FirebaseMoralisManager.Instance)
        {
            FirebaseMoralisManager.Instance.Credentails.WalletAddress = WalletManager.Instance.GetAccount();
            string _json = JsonConvert.SerializeObject(FirebaseMoralisManager.Instance.Credentails);
            FirebaseMoralisManager.Instance.SetLocalStorage(Constants.CredKey, _json);
        }

        FirebaseMoralisManager.Instance.GetNFTData();
        ChangeUserNameText(Constants.UserName);
        UpdateVCText(Constants.VirtualCurrencyAmount);
        LoadingScreen.SetActive(false);
        DisableRegisterLogin();
        TournamentMiniScreen.SetActive(false);
        MenuScreen.SetActive(true);

        if (!_newUser)
            FirebaseMoralisManager.Instance.StartCoroutine(FirebaseMoralisManager.Instance.FetchUserDB(PlayerPrefs.GetString("Account"), ""));
        else
            FirebaseMoralisManager.Instance.StartCoroutine(FirebaseMoralisManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));
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
        FirebaseMoralisManager.Instance.LoginUser(SavedEmail, SavedPass, SavedUserName);

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

        if (AnalyticsManager.Instance)
            AnalyticsManager.Instance.StoredProgression.MapUsed = _levelsSettings[i].LevelName;
    }

    public void AddLevels(bool isSelective = false, int Index = 0)
    {
        for (int q = 0; q < _allLevelsSettings.Count-1; q++)
        {
            if (isSelective)
                _levelsSettings.Add(_allLevelsSettings[Index]);
            else
                _levelsSettings.Add(_allLevelsSettings[q]);
        }
    }

    public bool CheckMechanics()
    {
        Constants.PrintLog("Selected Car : " + TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex]);
        SelectedCarToken = TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex];
        MechanicsManager.Instance.UpdateMechanicsData(SelectedCarToken);

        if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Health)
        { ShowToast(4f, "Your car health is zero, go to garage to repair your car.", false); return false; }
        else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Tyres)
        { ShowToast(4f, "Your Tyres has been worn out, go to garage to repair them.", false); return false; }
        else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Oil)
        { ShowToast(4f, "Your Engine oil is empty, go to garage to fill it.", false); return false; }
        else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Gas)
        { ShowToast(4f, "Your Gas is empty, go to garage to fill it.", false); return false; }

        return true;
    }
    private void OnGoToMapSelection()
    {
        if (Constants.GameMechanics)
        {
            if (!CheckMechanics())
                return;
        }

        #region mapLogic
        _levelsSettings.Clear();
        if(IsDestructionDerby)
        {
            if (AnalyticsManager.Instance)
                AnalyticsManager.Instance.StoredProgression.Mode = "Destruction Derby";

            _levelsSettings.Add(_allLevelsSettings[5]);
        }
        else if (IsMultiplayer)
        {
            if (TournamentManager.Instance)
            {
                if (TournamentManager.Instance.DataTournament.IsSingleMap)
                {
                    Constants.SelectedSingleLevel = TournamentManager.Instance.DataTournament.LevelIndex;
                    _levelsSettings.Add(_allLevelsSettings[Constants.SelectedSingleLevel - 1]);
                }
                else
                {
                    AddLevels();
                }
            }
        }
        else
        {
            if (IsPractice)
            {
                if (AnalyticsManager.Instance)
                    AnalyticsManager.Instance.StoredProgression.Mode = "Practice";

                AddLevels();
            }
            else if (IsTournament)
            {
                if (AnalyticsManager.Instance)
                    AnalyticsManager.Instance.StoredProgression.Mode = "Coinracer Tournament";

                AddLevels(true, 1);
            }
            else if (IsSecondTournament)
            {
                if (AnalyticsManager.Instance)
                    AnalyticsManager.Instance.StoredProgression.Mode = "Second Tournament";

                AddLevels(true, 3);
            }

        }

        OnLevelSelected(0);
        GameModeSelectionObject.SetActive(false);
        CarSelectionObject.SetActive(false);
        CarSelection3dObject.SetActive(false);
        MapSelection.SetActive(true);

        #endregion
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

        if (WalletConnected)
        {
            LoadingScreen.SetActive(true);
            IsTournament = false;
            IsSecondTournament = false;
            IsPractice = true;
            IsDestructionDerby = false;
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
                    if (Constants.FreeTournament)
                        StartTournament(true);
                    else
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
    public void OnGoToCarSelectionSecondTourTournament()
    {
        if (Constants.IsTest)
        {
            SecondTourToggleScreen_SelectionUI(true);
            return;
        }

        if (WalletConnected)
        {
            if (SecondTournamentActive == true)
            {
                if (WalletManager.Instance)
                {
                    SecondTourToggleScreen_SelectionUI(true);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    Constants.PrintError("WM instance is null OnGoToCarSelectionTournament");
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
    public void CheckTokenIndex(int newIndex)
    {
        for (int i = 0; i < Constants.TokenNFT.Count; i++)
        {
            if (Constants.TokenNFT[i].Name.ToLower() == _selecteableCars[newIndex].carSettings.Name.ToLower())
            {
                _SelectedTokenNameIndex = i;
                _SelectedTokenIDIndex = 0;
                break;
            }
        }
    }
    private void OnNextCar()
    {
        ToggleTokenScreen(true);
        int newIndex = _currentSelectedCarIndex + 1;
        newIndex %= _selecteableCars.Count;

        //if (!Constants.EarnMultiplayer)
        //ToggleTokenScreen(false);

        CheckTokenIndex(newIndex);
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

        // if (!Constants.EarnMultiplayer)
        //ToggleTokenScreen(false);

        CheckTokenIndex(newIndex);
        UpdateSelectedCarVisual(newIndex);
        UpdateToken();
    }
    public void OnNextToken()
    {
        if (_SelectedTokenIDIndex < Constants.TokenNFT[_SelectedTokenNameIndex].ID.Count - 1)
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
        //if (DebugAllCars)
            //return;
        try
        {
            UITokenCarSelection.TokenText.text = "#" + Constants.TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex];

            if (Constants.EarnMultiplayer)
            {
                if (AnalyticsManager.Instance)
                {
                    if (AnalyticsManager.Instance.StoredProgression.fields.ContainsKey("NFTID"))
                        AnalyticsManager.Instance.StoredProgression.fields["NFTID"] = "#" + Constants.TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex];
                    else
                        AnalyticsManager.Instance.StoredProgression.fields.Add("NFTID", "#" + Constants.TokenNFT[_SelectedTokenNameIndex].ID[_SelectedTokenIDIndex]);
                }
            }
            else
            {
                if (AnalyticsManager.Instance)
                {
                    if (AnalyticsManager.Instance.StoredProgression.fields.ContainsKey("NFTID"))
                        AnalyticsManager.Instance.StoredProgression.fields["NFTID"] = "#---";
                    else
                        AnalyticsManager.Instance.StoredProgression.fields.Add("NFTID", "#----");
                }
            }
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

        _currentSelectedCarIndex = newIndex;
        _selecteableCars[_currentSelectedCarIndex].Activate();
        _selectedCarName.text = _selecteableCars[_currentSelectedCarIndex].carSettings.Name;

        if (AnalyticsManager.Instance)
            AnalyticsManager.Instance.StoredProgression.CarName = _selecteableCars[_currentSelectedCarIndex].carSettings.Name;
    }

    #endregion

    #region Tournament Data
    public void StartTournament(bool _canstart = false)
    {
        if (Constants.IsTest)
        {
            LoadingScreen.SetActive(true);
            IsTournament = true;
            IsSecondTournament = false;
            CheckBoughtCars();
            IsPractice = false;
            IsDestructionDerby = false;
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
            IsTournament = true;
            IsSecondTournament = false;
            CheckBoughtCars();
            IsPractice = false;
            IsDestructionDerby = false;
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
        }
    }

    public void SecondTourStartTournament(bool _canstart = false)
    {
        if (Constants.IsTest)
        {
            LoadingScreen.SetActive(true);
            IsTournament = false;
            IsSecondTournament = true;
            CheckBoughtCars();
            IsPractice = false;
            IsDestructionDerby = false;
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
            IsTournament = false;
            IsSecondTournament = true;
            LoadingScreen.SetActive(true);
            CheckBoughtCars();
            IsPractice = false;
            IsDestructionDerby = false;
            IsMultiplayer = false;
            Constants.EarnMultiplayer = false;

            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    public void StartRace()
    {
        if(Constants.GameMechanics)
        {
            //if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Health)
            //{ ShowToast(4f, "Your car health is zero, go to store to repair your car.", false); return; }
            //else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Tyres)
            //{ ShowToast(4f, "Your Tyres has been worn out, go to store to repair them.", false); return; }
            //else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Oil)
            //{ ShowToast(4f, "Your Engine oil is empty, go to store to fill it.", false); return; }
            //else if (MechanicsManager.Instance.CheckConsumables() == ConsumableType.Gas)
            //{ ShowToast(4f, "Your Gas is empty, go to store to fill it.", false); return; }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (IsPractice || IsDestructionDerby)
        {
            PushingTries = true;
            FirebaseMoralisManager.Instance.PlayerData.NumberOfTriesPractice++;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
        }
        else if (IsTournament)
        {
            PushingTries = true;
            FirebaseMoralisManager.Instance.PlayerData.NumberOfTries++;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
        } 
        else if (IsSecondTournament)
        {
            PushingTries = true;
            FirebaseMoralisManager.Instance.PlayerData.GNumberOfTries++;
            FirebaseMoralisManager.Instance.UpdatedFireStoreData(FirebaseMoralisManager.Instance.PlayerData);
        }
#endif
        SelectedCar = _selecteableCars[_currentSelectedCarIndex].carSettings;

        if (Constants.IsMultiplayer)
        {
            if (MultiplayerManager.Instance)
            {
                Constants.SelectedLevel = MainMenuViewController.Instance.getSelectedLevel() + 1;
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

        if (_state)
            StoreHandler.Instance.SetCCashText_Garage(Constants.VirtualCurrencyAmount);
    }

    public void BackButton_Garage()
    {
        ToggleScreen_Garage(false);
        //NFTGameplayManager.Instance.RemoveSavedPrefabs();
        GarageHandler.Instance.DeleteData();
    }

    public void GarageButton_Garage()
    {
        if (Constants.IsTest)
            WalletConnected = true;


        if (Constants.DebugAllCars)
        {
            ToggleScreen_Garage(true);
            NFTGameplayManager.Instance.ProcessNFT();

            return;
        }

        if (WalletConnected)
        {
            if (Constants.CheckAllNFT)
            {
                LoadingScreen.SetActive(true);
                ToggleScreen_Garage(true);

                int storedNFTS = 0;
                for (int i = 0; i < Constants.NFTStored.Length; i++)
                    storedNFTS += Constants.NFTStored[i];


                NFTGameplayManager.Instance.ProcessNFT();
                //if (storedNFTS == 0)
                //{
                //    LoadingScreen.SetActive(false);
                //    ShowToast(3f, "No NFT was purchased, please purchase one.");
                //}
                //else
                //{
                //    NFTGameplayManager.Instance.ProcessNFT();
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

    public void DeactivateCars()
    {
        for (int i = 0; i < _selecteableCars.Count; i++)
        {
            _selecteableCars[i].Deactivate();
        }
    }
    public void CheckBoughtCars()
    {

        for (int i = 0; i < _selecteableCars.Count; i++)
        {
            _selecteableCars[i].Deactivate();
        }

        ToggleTokenScreen(true);
        //UpdateToken();

        //if (Constants.EarnMultiplayer)
        //{
        //    for (int i = 0; i < _selecteableCars.Count; i++)
        //    {
        //        _selecteableCars[i].Deactivate();
        //    }

        //    ToggleTokenScreen(true);
        //    UpdateToken();
        //} else
        //{

        //    ToggleTokenScreen(false);
        //}

        //if (Constants.DebugAllCars)
        //{
        //    for (int j = 0; j < _allCars.Count; j++)
        //    {
        //        _selecteableCars.Add(_allCars[j].CarDetail);
        //    }

        //    LoadingScreen.SetActive(false);
        //    _currentSelectedCarIndex = 0;
        //    _SelectedTokenNameIndex = 0;
        //    _SelectedTokenIDIndex = 0;
        //    UpdateSelectedCarVisual(_currentSelectedCarIndex);
        //    UpdateToken();
        //    return;
        //}

        if (Constants.CheckAllNFT && (Constants.GetMoralisData || Constants.DebugAllCars))
        {
            int storedNFTS = 0;

            for (int i = 0; i < Constants.NFTStored.Length; i++)
                storedNFTS += Constants.NFTStored[i];

            if (storedNFTS == 0)
            {
                DeactivateCars();
                _selecteableCars.Clear();
                if (!Constants.EarnMultiplayer)
                {
                    if (IsSecondTournament)
                        _selecteableCars.Add(_allCars[27].CarDetail);
                    else
                        _selecteableCars.Add(_allCars[0].CarDetail);

                    LoadingScreen.SetActive(false);
                    _currentSelectedCarIndex = 0;
                    UpdateSelectedCarVisual(_currentSelectedCarIndex);
                }
            }
            else if (Constants.StoredCarNames.Count != 0 || Constants.StoredCarNamesMoralis.Count!=0)
            {
                DeactivateCars();
                _selecteableCars.Clear();

                //if (!Constants.EarnMultiplayer)
                //{
                    if (IsSecondTournament)
                        _selecteableCars.Add(_allCars[27].CarDetail);
                    else
                        _selecteableCars.Add(_allCars[0].CarDetail);
                //}

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

                for (int i = 0; i < Constants.StoredCarNamesMoralis.Count; i++)
                {
                    for (int j = 0; j < _allCars.Count; j++)
                    {
                        if (Constants.StoredCarNamesMoralis[i].ToLower() == _allCars[j].CarName.ToLower())
                        {
                            _selecteableCars.Add(_allCars[j].CarDetail);
                            break;
                        }
                    }
                }

                LoadingScreen.SetActive(false);
                _currentSelectedCarIndex = 0;
                _SelectedTokenNameIndex = 0;
                _SelectedTokenIDIndex = 0;
                UpdateSelectedCarVisual(_currentSelectedCarIndex);
                UpdateToken();
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
            FirebaseMoralisManager.Instance.SendPasswordResetEmail(UIForgetPassword.EmailInput.text);
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
            FirebaseMoralisManager.Instance.ResendVerificationEmail();
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

        FirebaseMoralisManager.Instance.SetLocalStorage(Constants.SoundKey, Constants.SoundSliderValue.ToString());
        FirebaseMoralisManager.Instance.SetLocalStorage(Constants.MusicKey, Constants.MusicSliderValue.ToString());
    }

    #endregion

    #region MultiplayerConnectionUI/Data
    public void SubscribeEvents_ConnectionUI()
    {
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
        if (Constants.ConvertToCCash)
        {
            if (Constants.IsMultiplayer)
            {
                if (PhotonNetwork.IsConnected)
                {
                    if(FirebaseMoralisManager.Instance.PlayerData.VC_Amount>=Constants.SelectedCurrencyAmount)
                    {
                        Constants.GATransferAmount = Constants.SelectedCurrencyAmount;
                        FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= Constants.SelectedCurrencyAmount;
                        apiRequestHandler.Instance.updatePlayerData();

                        if (AnalyticsManager.Instance)
                            AnalyticsManager.Instance.MultiplayerEvent(Constants.GATransferAmount);

                        if (PhotonNetwork.IsMasterClient && !Constants.OtherPlayerDeposit)
                            WalletManager.Instance.OnRaceCreateCalled(true);
                        else
                            WalletManager.Instance.OnDepositCalled(true);
                    }else
                    {
                        LoadingScreen.SetActive(false);
                        ShowToast(3f, "Insufficient $" + Constants.GetCurrencyName() + " value.");
                    }
                }else
                {
                    Constants.PrintError("Photon is not connected");
                }
            }
        }
        else
        {
            if (WalletManager.Instance)
                WalletManager.Instance.CallDeposit();
        }
    }

    public void WithDrawDeposit()
    {
        if (Constants.ConvertToCCash)
        {
            if (Constants.IsMultiplayer && Constants.CanWithdraw)
            {
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount += Constants.SelectedCurrencyAmount;
                apiRequestHandler.Instance.updatePlayerData();
                WalletManager.Instance.OnDepositBackCalled(true);
            }
        }
        else
        {
            if (WalletManager.Instance)
                WalletManager.Instance.CallWithdraw();
        }
    }

    public void ToggleBackButton_ConnectionUI(bool state)
    {
        UIConnection.BackButton.gameObject.SetActive(state);
    }

    public void onMultiplayerBtnClick()
    {
        if (IsTest)
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
        AnimateConnectingDetail_ConnectionUI(UIConnection.Detail01.DetailScreen, true);
        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.ConnectToPhotonServer();
    }

    public void ToggleSecondDetail(bool _enable, string _name, string _wins, int _index)
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
        ChangeConnectionText_ConnectionUI("connecting...");
        ChangeRegionText_ConnectionUI("Selected Region : n/a");
        ToggleScreen_ConnectionUI(false);

        RegionPinged = false;

        //Dropdown RegionList = UIConnection.RegionPingsDropdown.GetComponent<Dropdown>();
        //RegionList.interactable = false;
        //RegionList.options.Clear();
        //RegionList.options.Add(new Dropdown.OptionData() { text = "Select Region" });
        //RegionList.value = 0;
        //Constants.RegionChanged = false;
        //PhotonNetwork.SelectedRegion = "";
        //RegionList.interactable = true;

        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.DisconnectPhoton();
    }

    public void UpdateDetailData(bool isPlayer1, string _name, string _wins, int index)
    {
        if (isPlayer1)
        {
            UIConnection.Detail01.WinnerNameText.text = _name;
            UIConnection.Detail01.WinText.text = "WINS : " + _wins;
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
        yield return null;
       // yield return new WaitUntil(() => PhotonNetwork.GotPingResult);
       // UpdatePingList(PhotonNetwork.pingedRegions, PhotonNetwork.pingedRegionPings);
       // PhotonNetwork.GotPingResult = false;
    }

    public void UpdatePingList(string[] regions, string[] pings)
    {
        if (!RegionPinged)
        {
            RegionPinged = true;
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
                //Debug.Log("region list is empty");
            }
        }
    }

    //this function will be used to animate the connecting details screen
    //@param {_screen,_isMyScreen}, _screen: type(gameobject), contains reference to the screen to animate
    //_isMyScreen, _isMyScreen: type(bool), indicates if this is my player's screen that you are animating
    //@return {} no return
    private void AnimateConnectingDetail_ConnectionUI(GameObject _screen, Boolean _isMyScreen)
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

        if (IsDebugBuild)
            Constants.ChipraceScore = "550";

        Constants.ConvertDollarToCrace(Constants.SelectedWage);
        Constants.SelectedCurrencyAmount = Constants.CalculatedCurrencyAmount;

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
        UIMultiplayerSelection.CracePriceText.text = "1 $"+Constants.GetCurrencyName()+" : " + _price + "$";
    }

    public void ChangeDisclaimer_MultiplayerSelection()
    {
        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_1);
        UIMultiplayerSelection.Disclaimer_5.text = "*price: " + Constants.MultiplayerPrice_1 + "$" + " (" + Constants.CalculatedCurrencyAmount.ToString() + " $"+Constants.GetCurrencyName()+")";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_2);
        UIMultiplayerSelection.Disclaimer_10.text = "*price: " + Constants.MultiplayerPrice_2 + "$" + " (" + Constants.CalculatedCurrencyAmount.ToString() + " $"+ Constants.GetCurrencyName()+")";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_3);
        UIMultiplayerSelection.Disclaimer_50.text = "*price: " + Constants.MultiplayerPrice_3 + "$" + " (" + Constants.CalculatedCurrencyAmount.ToString() + " $"+ Constants.GetCurrencyName()+")";

        Constants.ConvertDollarToCrace(Constants.MultiplayerPrice_4);
        UIMultiplayerSelection.Disclaimer_100.text = "*price: " + Constants.MultiplayerPrice_4 + "$" + " (" + Constants.CalculatedCurrencyAmount.ToString() + " $"+ Constants.GetCurrencyName()+")";
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
        if (Constants.CheckAllNFT)
        {
            int storedNFTS = 0;
            for (int i = 0; i < Constants.NFTStored.Length; i++)
                storedNFTS += Constants.NFTStored[i];
            if (storedNFTS == 0)
            {
                LoadingScreen.SetActive(false);
                ShowToast(3f, "No NFT was purchased, please purchase one to play.");
            }
            else
            {
                ToggleSelection_MultiplayerSelection(false);
                Constants.GetCracePrice();
                ChangeCracePrice_MultiplayerSelection(Constants.CurrencyPrice.ToString());
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

    #region MemberCheck
    public void OnPassChanged_MemberUI(string _val)
    {
        MemberPass = _val;
    }
    public void OnSubmitClick_MemberUI()
    {
        if (MemberPass.ToLower() == Constants.VIPPassword.ToLower())
            UIMember.MainScreen.SetActive(false);
        else
            ShowToast(3f, "Entered password is wrong!",false);
    }
    #endregion

    #region StoreGarage
    public void AddSelected(CarSelection _car)
    {
        SelectedCars.Add(_car);
    }

    public List<CarSelection> GetSelectedCar()
    {
        return SelectedCars;
    }

    public CarSelection GetSelectedCarByIndex()
    {
        return SelectedCars[carIndex];
    }

    public void ResetSelectedCarStore()
    {
        for (int i = 0; i < SelectedCars.Count; i++)
            Destroy(SelectedCars[i].gameObject);

        carIndex = 0;
        nextCarIndex = 0;
        PrevCarIndex = 0;
        SelectedCars.Clear();
    }

    public void AssignStoreGarageData(GameObject _car, int _tokenID, string _carname, StatSettings _settings,Transform _parent,bool _setMechanics, bool _setStats)
    {
        _generatedPrefab = Instantiate(_car, Vector3.zero, Quaternion.identity) as GameObject;
        _generatedPrefab.transform.SetParent(_parent);
        _generatedPrefab.transform.localScale = new Vector3(1, 1, 1);
        var NFTDataHandlerRef = _generatedPrefab.AddComponent(typeof(NFTDataHandler)) as NFTDataHandler;
        NFTDataHandlerRef.SetTokenID(_tokenID);
        NFTDataHandlerRef.SetCarName(_carname);

        if(_setMechanics)
            NFTDataHandlerRef.SetMechanics();

        if(_setStats)
        NFTDataHandlerRef.SetStatsSettings(_settings);

        AddSelected(_generatedPrefab.GetComponent<CarSelection>());
    }

    public void AssignStoreGarageCars(Transform _middleParent, Transform _leftParent, Transform _rightParent,Transform _mainParent, TextMeshProUGUI _name, TextMeshProUGUI _id, bool _assignID, bool _showStats, bool _showConsumables)
    {
        for (int i = 0; i < SelectedCars.Count; i++)
        {
            if (carIndex == i)
            {
                nextCarIndex = carIndex + 1;
                PrevCarIndex = carIndex - 1;

                AssignMiddleCar(SelectedCars[i].gameObject, _middleParent, _assignID, _showStats,_showConsumables, _name, _id);

                if (PrevCarIndex >= 0)
                    AssignLeftCar(SelectedCars[i - 1].gameObject, _leftParent);

                if (nextCarIndex < SelectedCars.Count)
                    AssignRightCar(SelectedCars[i + 1].gameObject, _rightParent);
            }
            else if (nextCarIndex != i && PrevCarIndex != i)
            {
                PlaceCarBack(SelectedCars[i].gameObject, _mainParent);
            }
        }
    }

    public void PlaceCarBack(GameObject _car,Transform _parent)
    {
        _car.transform.SetParent(_parent);
        _car.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void AssignMiddleCar(GameObject _car, Transform _parent, bool _assignID,bool _showStats,bool _showConsumables,TextMeshProUGUI _name, TextMeshProUGUI _id)
    {
        _car.transform.SetParent(_parent);
        ResetCarTransform(_car);
        _car.transform.GetChild(0).gameObject.SetActive(true);

        var _ref = _car.GetComponent<NFTDataHandler>();
        _name.text = _ref.CarName;

        if (_assignID)
            _id.text = _ref.tokenID.ToString();

        if(_showStats)
            StoreHandler.Instance.UpdateCarStats(SelectedCars[carIndex].GetComponent<NFTDataHandler>()._settings);

        if (_showConsumables)
            StoreHandler.Instance.UpdateMainConsumablesStats(SelectedCars[carIndex].GetComponent<NFTDataHandler>().Mechanics, SelectedCars[carIndex].GetComponent<NFTDataHandler>()._settings);
    }

    public void AssignLeftCar(GameObject _car, Transform _parent)
    {
        _car.transform.SetParent(_parent);
        ResetCarTransform(_car);
        _car.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void AssignRightCar(GameObject _tempcar, Transform _parent)
    {
        Constants.PrintLog("Right : "+ _tempcar.gameObject.name);
        _tempcar.transform.SetParent(_parent);
        ResetCarTransform(_tempcar);
        _tempcar.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ResetCarTransform(GameObject _car)
    {
        _car.transform.localPosition = new Vector3(0, 0, 0);
        _car.transform.localEulerAngles = new Vector3(0, 0, 0);
        _car.transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnNextButtonClicked()
    {
        if (carIndex + 1 < SelectedCars.Count)
        {
            carIndex++;
            nextCarIndex = carIndex + 1;
            PrevCarIndex = carIndex - 1;
        }
    }

    public void OnPrevButtonClicked()
    {
        if (carIndex - 1 >= 0)
        {
            carIndex--;
            nextCarIndex = carIndex + 1;
            PrevCarIndex = carIndex - 1;
        }
    }

    public void OnRepairButtonCLicked()
    {
        SelectedCars[carIndex].GetComponent<NFTDataHandler>().AccessConsumables();
    }
    #endregion

    #region Destruction Derby

    public void AddButtonListeners_DD()
    {
        _destructionDerbyButton.onClick.AddListener(StartDerby_DD);
    }

    public void StartDerby_DD()
    {
        if (Constants.IsTest)
            WalletConnected = true;


        if (WalletConnected)
        {
            IsTournament = false;
            IsSecondTournament = false;
            IsPractice = false;
            IsDestructionDerby = true;
            IsMultiplayer = true;
            Constants.EarnMultiplayer = false;
            Constants.FreeMultiplayer = true;

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
    #endregion
}