using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System;


public static class Constants
{
    public static string VirtualCurrency = "CCASH";
    public static string TokenName = "CRACER";
    public static string APP_VERSION = "Alpha 2.0";
    public static string WalletAccoutKey = "Account";
    public static string NameCurrency = "USD";
    public static string SoundKey = "Sound";
    public static string MusicKey = "Music";
    public static string CredKey = "Credentails";
    public static string NFTKey = "StalkedNFT";
    public static string MAIN_MENU_SCENE_NAME = "MainMenu";
    public static bool IsPractice = false;
    public static bool IsTournament = false;
    public static bool TournamentActive = false;

    public static bool EnableSecondTournament = false;
    public static bool IsSecondTournament = false;
    public static bool SecondTournamentActive = false;
    public static int SecondTournamentPassPrice = 200;
    public static bool SecondTourBuyingPass = false;
    public static int SecondTourDiscountPercentage = 50;
    public static int SecondTourDiscountForCrace = 25000;
    public static int SecondTourTicketPrice = 0;
    public static double SecondTourGameSeconds = 0;
    public static bool SecondTourPushingTime = false;
    public static bool SecondTourPushingTries = false;
    public static bool SecondTourTransfer = false;

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
    public static string ResendTokenID = "";
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
    public static int[] NFTBought = new int[3]
    {
        -2,
        -2,
        -2
    };
    public static int[] NFTStored = new int[3]
    {
        -1,
        -1,
        -1
    };
    public static bool[] NFTChanged = new bool[3]
    {
        false,
        false,
        false
    };
    public static bool[] nftDataFetched = new bool[3]
    {
        false,
        false,
        false
    };
    public static List<string> gifLinks = new List<string>()
    {
        "https://ipfs.io/ipfs/QmZ1X3REmPM7tyZHts1PBso3QLrHamfe5VvF5TqFhdehvK",
        "https://ipfs.io/ipfs/QmTV1naEXYnwpxwqtzrmKrr5RrDpKVBmHVmpEQgBQBE7e6",
        "https://ipfs.io/ipfs/QmSwXe4Znek9mhJ8YeyaVmVzpuQ8FdZR9818jAvc86kfFV",
        "https://ipfs.io/ipfs/QmWbpDUcKCbfnZAnqknATWKG3JaXWdAKUnGa1dVKnfvNK5",
        "https://ipfs.io/ipfs/QmcAoECUaMsk5GQMdUauFgW13yECnLh1EywkgvYV4XVniA",
    };
    public static List<string> gifLinksAlternative = new List<string>()
    {
        "https://ipfs.io/ipfs/QmP9XJX5Ler2pa7sWyRdoLRXtNgk8ffC4Hjt9aYnSDk2U2",
        "https://ipfs.io/ipfs/QmQBZksfbruM4QXoiVbsao9tSqk7TLZoPFvzqTw3t94ZaE",
        "https://ipfs.io/ipfs/QmUZnADARvRJU2hVoXYFBuezSoxhrmJGPsntRmyafhRi8V",
        "https://ipfs.io/ipfs/Qma7oWboiRMQvrF5CebWL9dGKeqcF1m1A5KGLLAfnZQfZE",
        "https://ipfs.io/ipfs/Qmb9dF1AjPd2fHjWatGWvGugZTowA7J6c79S3ZKqViaEgF"
    };
    public static bool CheckAllNFT = false;
    public static List<string> StoredCarNames = new List<string>();
    public static bool PushingTries = false;
    public static bool PushingWins = false;
    public static bool WalletChanged = false;

    public static bool IsTestNet = true;
    public static bool IsTest = true;
    public static bool IsStagging = true;
    public static bool DebugAllCars = false;// bool to enable all cars
    public static bool IsDebugBuild = false;//apply pvp score 500+/enable force end race on levels// and force bid of 1 crace for pvp
    public static bool UseHashMec = true;//bool to store if contracts use hash mechanism or signature for verification
    public static bool GameMechanics = true;
    public static bool EarlyBuild = false;
    public static string VIPPassword = "hailcracer";
    public static bool ConvertToCCash = true;
    public static int CCashToDollar = 1;

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
    public static double CurrencyPrice = 0.0888;
    public static int CalculatedCurrencyAmount = 0;
    public static string PlayerDataKey = "playerData";
    public static string RoomDataKey = "roomData";
    public static bool isMultiplayerGameEnded = false;

    public static string MAP_PROP_KEY = "map";
    public static string WAGE_PROP_KEY = "wage";
    public static string MODE_PROP_KEY = "mode";
    public static int SelectedLevel = 0;
    public static int SelectedWage = 0;
    public static int TotalWins = 0;
    public static int SelectedCurrencyAmount = 0;
    public static int SelectedMaxPlayer = 2;
    public static int TotalPlayingTime = 600;
    public static bool DepositDone = false;
    public static bool CanWithdraw = false;
    public static int WithdrawTime = 300; //time in seconds//300
    public static bool TimerRunning = false;
    public static bool ClaimedReward = false;
    public static readonly string HashKey = "us-H*?fBg9]6Kse";
    public static readonly string TestnetHashKey = "testingkey";

    public static bool FreeMultiplayer = false;
    public static bool EarnMultiplayer = false;
    public static bool OtherPlayerDeposit = false;
    public static string PIDString = "";
    public static bool EventRaised = false;
    public static string EventData = "";
    public static string SelectedRegion = "";
    public static bool RegionChanged = false;
    public static bool RegionPinged = false;
    
    public static bool isUsingFirebaseSDK = false;
    public static int LeaderboardCount = 200;
    public static int GLeaderboardCount = 200;
    public static int PoolCounter = 5;
    public static List<TotalNFTData> NFTTotalData=new List<TotalNFTData>();
    public static List<NFTTokens> TokenNFT = new List<NFTTokens>();
    public static int _SelectedTokenNameIndex = 0;
    public static int _SelectedTokenIDIndex = 0;
    public static string OpponentTokenID = "0";
    public static string ChipraceScore = "50";
    public static bool ChipraceDataChecked = false;
    public static int NFTTokenApproval = -1;
    public static int ChipraceWithdrawFees = 10;
    public static bool ChipraceInteraction = false;
    public static bool ForceUpdateChiprace = false;
    public static bool UseChipraceLocalDB = false;
    public static int SelectedSingleLevel = 0;
    public static string storedName;
    public static string storedToken;
    public static bool FreeTournament = false; //if tournament entrance fees from server is zero , this means tournament is free to enter

    public static double GATransferAmount = 0;
    public static int MaxCarHealth = 100;
    public static int StoredCarHealth = 100;
    public static double VirtualCurrencyAmount = 0;
    public static int CCashPurchaseAmount = 100;
    public static bool CarTotaled = false;
    public static int SelectedCarToken = 0;
    public static bool GetMoralisData = false;

    async public static void GetCracePrice()
    {
        if (ConvertToCCash)
        {
            CurrencyPrice = CCashToDollar;
        }
        else
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(CoinBaseURL);
            await webRequest.SendWebRequest();
            string _json = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);

            try
            {
                var _data = JObject.Parse(_json);
                CurrencyPrice = double.Parse(_data["data"]["prices"]["latest"].ToString());
                CurrencyPrice = System.Math.Round(CurrencyPrice, 4);
            }
            catch (System.Exception)
            {
                //Debug.LogError("something went wrong while fetching crace price from coinbase.");
                CurrencyPrice = 0.0888;
            }
        }
    }

    public static double ConvertDollarToCrace(double _amount)
    {
        double _calulcatedAmount = 0;
        if (CurrencyPrice != 0)
            _calulcatedAmount = System.Math.Round(_amount / CurrencyPrice, 4);

        CalculatedCurrencyAmount = (int)_calulcatedAmount;
        return _calulcatedAmount;
    }

    public static double ConvertCraceToDollar(double _amount)
    {
        double _calulcatedAmount = 0;
        if (CurrencyPrice != 0)
            _calulcatedAmount = System.Math.Round(_amount * CurrencyPrice, 4);

        return _calulcatedAmount;
    }

    public static void PrintLog(string Txt)
    {
        Debug.Log(Txt);
    }

    public static void PrintError(string Txt)
    {
        Debug.LogError(Txt);
    }

    public static void PrintExp(Exception Txt,UnityEngine.Object ins)
    {
        Debug.LogException(Txt,ins);
    }

    public static void ResetData()
    {
        WalletChanged=false;
        APP_VERSION = "Alpha 2.0";
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
        for(int i = 0; i < NFTBought.Length; i++)
        {
            NFTBought[i] = -2;
            NFTStored[i] = -1;
            NFTChanged[i] = false;
            nftDataFetched[i] = false;
        }
        CheckAllNFT = false;
        StoredCarNames.Clear();

    }

    public static string GetCurrencyName()
    {
        if (ConvertToCCash)
            return VirtualCurrency;
        else
            return TokenName;
    }
}
