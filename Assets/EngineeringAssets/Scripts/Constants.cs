using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

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
    public static string UserName = "";
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
    public static bool IsTestNet = false;
    public static bool IsTest = true;
    public static bool IsSendConfirmation = false;
    public static bool IsResetPassword = false;
    public static string EmailSent = "";
    public static float SoundSliderValue = 1;
    public static float MusicSliderValue = 1;
    public static bool OnIce = false;
    public static bool IsMultiplayer = false;
    public static string StoredPID = "";

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
        UserName = "";
        //TournamentPassPrice = 500;//500
        BuyingPass = false;
        //DiscountPercentage = 50;
        //DiscountForCrace = 25000;//25000
       //TicketPrice = 0;
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
    }
}
