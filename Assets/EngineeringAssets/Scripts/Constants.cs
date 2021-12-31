using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

public static class Constants
{
    public static string APP_VERSION = "Alpha 1.7";
    public static string WalletAccoutKey = "Account";
    public static string SoundKey = "Sound";
    public static string MusicKey = "Music";
    public static string CredKey = "Credentails";
    public static string MAIN_MENU_SCENE_NAME = "MainMenu";
    public static bool IsPractice = false;
    public static bool IsTournament = false;
    public static bool TournamentActive = false;
    public static bool WalletConnected = false;//false
    public static bool MoveCar = false;
    public static bool PushingTime = false;
    public static double GameSeconds = 0;
    public static string WalletAddress = "";
    public static string UserName = "XYZ";
    public static int TournamentPassPrice =200;//500
    public static bool BuyingPass = false;
    public static int DiscountPercentage = 50;
    public static int DiscountForCrace = 25000;//25000
    public static int TicketPrice = 0;
    public static int FlagSelectedIndex = 0;
    public static bool LoggedIn = false;
    public static bool RegisterSubmit = false;
    public static string SavedEmail = "";
    public static string SavedPass = "";
    public static string SavedConfirmPass = "";
    public static string SavedUserName = "";
    public static BigInteger NFTAmount = BigInteger.Parse("370000000000000000000");//200000000000000000000
    public static BigInteger BnBValue = BigInteger.Parse("120000000000000000");
    public static BigInteger GasLimit = BigInteger.Parse("526054");
    public static int GasPrice = 15;
    public static BigInteger StoredNFTAmount;
    public static BigInteger StoredBNBAmount;
    public static int SelectedIndex = 0;
    public static bool ApproveCrace = false;
    public static bool HaveNFTData = false;
    public static int NFTBought = -2;
    public static int NFTStored = -1;
    public static bool CheckAllNFT = false;
    public static bool NFTChanged = false;
    public static List<string> StoredCarNames = new List<string>();
    public static bool PushingTries = false;
    public static bool WalletChanged = false;
    public static bool IsTestNet = true;
    public static bool IsTest = false;
    public static bool IsSendConfirmation = false;
    public static bool IsResetPassword = false;
    public static string EmailSent = "";
    public static float SoundSliderValue = 1;
    public static float MusicSliderValue = 1;
    public static bool OnIce = false;
    public static bool IsMultiplayer = false;
    public static string StoredPID = "";

    public static int MultiplayerPrice_1 = 5;
    public static int MultiplayerPrice_2 = 10;
    public static int MultiplayerPrice_3 = 50;
    public static int MultiplayerPrice_4 = 100;
    public static string CoinBaseURL = "https://www.coinbase.com/api/v2/assets/prices/coinracer?base=USD";
    public static double CracePrice = 0.0888;
    public static int CalculatedCrace = 0;
    public static string PlayerDataKey = "playerData";
    public static string RoomDataKey = "roomData";
    public static bool isMultiplayerGameEnded = false;

    public static string MAP_PROP_KEY = "map";
    public static string WAGE_PROP_KEY = "wage";
    public static int SelectedLevel = 0;
    public static int SelectedWage = 0;
    public static int TotalWins = 0;
    public static int SelectedCrace = 0;
    public static int SelectedMaxPlayer = 2;
    public static int TotalPlayingTime = 600;
    public static bool DepositDone = false;
    public static bool CanWithdraw = false;
    public static int WithdrawTime = 60; //time in seconds//300
    public static bool DisableCSP = false;
    public static bool TimerRunning = false;

    async public static void GetCracePrice()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(CoinBaseURL);
        await webRequest.SendWebRequest();
        string _json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);

        try
        {
            var _data = JObject.Parse(_json);
            CracePrice = double.Parse(_data["data"]["prices"]["latest"].ToString());
            CracePrice = System.Math.Round(CracePrice, 4);
        }
        catch (System.Exception)
        {
            Debug.LogError("something went wrong while fetching crace price from coinbase.");
            CracePrice = 0.0888;
        }
    }

    public static double ConvertDollarToCrace(double _amount)
    {
        double _calulcatedAmount = 0;
        if (CracePrice != 0)
            _calulcatedAmount = System.Math.Round(_amount / CracePrice, 4);

        CalculatedCrace = (int)_calulcatedAmount;
        return _calulcatedAmount;
    }

    public static double ConvertCraceToDollar(double _amount)
    {
        double _calulcatedAmount = 0;
        if (CracePrice != 0)
            _calulcatedAmount = System.Math.Round(_amount * CracePrice, 4);

        return _calulcatedAmount;
    }

    public static void ResetData()
    {
        WalletChanged=false;
        APP_VERSION = "Alpha 1.7";
        WalletAccoutKey = "Account";
        CredKey = "Credentails";
        MAIN_MENU_SCENE_NAME = "MainMenu";
        IsPractice = false;
        IsTournament = false;
        TournamentActive = false;
        WalletConnected = false;
        MoveCar = false;
        PushingTime = false;
        GameSeconds = 0;
        WalletAddress = "";
        UserName = "XYZ";
        BuyingPass = false;
        FlagSelectedIndex = 0;
        LoggedIn = false;
        RegisterSubmit = false;
        SavedEmail = "";
        SavedPass = "";
        SavedConfirmPass = "";
        SavedUserName = "";
        NFTAmount = BigInteger.Parse("370000000000000000000");//200000000000000000000
        BnBValue = BigInteger.Parse("120000000000000000");
        GasLimit = BigInteger.Parse("526054");
        GasPrice = 15;
        StoredNFTAmount=0;
        StoredBNBAmount=0;
        SelectedIndex = 0;
        ApproveCrace = false;
        HaveNFTData = false;
        NFTBought = -2;
        NFTStored = -1;
        CheckAllNFT = false;
        NFTChanged = false;
        StoredCarNames.Clear();
        //TournamentPassPrice = 500;//500
        //DiscountPercentage = 50;
        //DiscountForCrace = 25000;//25000
        //TicketPrice = 0;
    }
}
