using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using System.Numerics;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

#region SuperClasses
[Serializable]
public class UIData
{
    [Tooltip("UI Reference for Connect Button as gameObject")]
    public GameObject ConnectBtn;
    [Tooltip("UI Reference for Connected Button as gameObject")]
    public GameObject ConnectedBtn;
    [Tooltip("UI Reference for Coin text as texMeshGUI")]
    public TextMeshProUGUI CoinText;
}
#endregion

public class WalletManager : MonoBehaviour
{
    #region DataMembers
    [Tooltip("Instance of the serialized class")]
    public UIData MainUI;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetStorage(string key, string val);

    [DllImport("__Internal")]
    private static extern string GetStorage(string key, string ObjectName, string callback);

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);
#endif

    public static WalletManager Instance; //static instanc of the class WalletManager
    private string account; //string to store connected wallet address
    private BigInteger mainbalanceOf;//Biginteger to store balance of token (Crace) inside wallet
    private BigInteger decimals;//Biginteger to store decimal settings for the token in wallet
    private string symbol;//string to store symbol of token in wallet
    private BigInteger totalSupply;//Biginteger to store totalSupply of tokens
    private string nameContract;//string to store name of the contract (BEP20)

    private BigInteger decimalValue; //stores Decimal calculation with power of 10
    private BigInteger actualBalance;//stores actual balance after dividing with 'DecimalValue'
    private BigInteger mainBalance = 1000000000000000000; //10^18

    private string chain = "binance";//name of the chain
    private string network = "mainnet";//name of the network //mainnet//testnet

    //address of the BEP20 (Crace) contract
    //mainnet : 0xFBb4F2f342c6DaaB63Ab85b0226716C4D1e26F36
    //testnet : 0x08Da683F43fCAe68119602d838979F056CD3f3aD
    private string contract = "0xFBb4F2f342c6DaaB63Ab85b0226716C4D1e26F36";

    //address of the BEP721 contract
    //mainnet : 0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08
    //testnet: 0x312b151a0e87785649ed835d946c2b0de5745c30
    private string contractNFT = "0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08";
    private string CSPContract = "0x57988AE6CC7F6fEd1B13A0C88bbBE7216ceC6DA9";

    private string toAccount = "0xe1E4160F4AcDf756AA0d2B02D786a42527560E82"; //wallet address to send BEP20 (crace) amount for transactions

    private string amount = "";
    private readonly string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";
    private readonly string abiNFTContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"}],\"name\":\"updateMinBNB\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"craceValue\",\"type\":\"uint256\"},{\"internalType\":\"string[5]\",\"name\":\"data\",\"type\":\"string[5]\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
    private readonly string abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pNumber\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"pNumber\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pNumber\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"pNumber\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    private BigInteger mainbalanceOfNFT;//Biginteger to store balance of token inside wallet
    private string tokenId = "19877";
    private string ownerNFT = "";

    private int tempNFTCounter = 0;
    [HideInInspector]
    public List<int> NFTTokens = new List<int>();
    [HideInInspector]
    public List<string> metaDataURL = new List<string>();
    string StoredWallet = "null";
    int NFTCounter = 0;
    string StoredHash = "";
    string StoredMethodName = "";
    #endregion

    #region Start Functionality
    /// <summary>
    /// called by unity engine whenever class object is enabled (called before start)
    /// </summary>
    private void OnEnable()
    {
        Instance = this;
        Constants.WalletConnected = false;

        if (Constants.IsTestNet)
        {
            chain = "binance";
            network = "testnet";
            contract = "0x08Da683F43fCAe68119602d838979F056CD3f3aD";
            contractNFT = "0x312b151a0e87785649ed835d946c2b0de5745c30";
            CSPContract = "0x57988AE6CC7F6fEd1B13A0C88bbBE7216ceC6DA9";
        }

        if (Constants.IsTest && !Constants.IsTestNet)
        {
            //TesT
            SetAcount("0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10");//0x54815A2afe0393F167B2ED59D6DF5babD40Be6Db//0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10
            InvokeRepeating("CheckNFTBalance", 0.1f, 10f);
        }
    }

    /// <summary>
    /// called by unity engine whenever class object is created
    /// </summary>
    private void Start()
    {
        //get wallet address stored inside local storage (call only inisde browser not in editor)
#if UNITY_WEBGL && !UNITY_EDITOR
            GetStorage(Constants.WalletAccoutKey,this.gameObject.name,"OnGetAcount");
#endif
    }

    /// <summary>
    /// Callback received when "GetStorage" function is called
    /// </summary>
    /// <param name="info"></param>
    public void OnGetAcount(string info)
    {
        if (info != "null" && info != "" && info != null && info != string.Empty) //if received data is not empty (we had stored some wallet address before)
        {
            StoredWallet = info;
            ConnectWallet();
        } else
        {
            StoredWallet = "null";
        }
    }

    public string GetAccount()
    {
        return amount;
    }
    public void SetAcount(string _acc)
    {
        account = _acc;
    }
    #endregion

    #region Wallet Functionality

    /// <summary>
    /// Called to connect wallet using web3 libraries (called from connect wallet button and "OnGetAcount" function (if wallet address was stored in local storage))
    /// </summary>
    public void ConnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            Web3Connect();
            OnConnected();
#else
        Debug.Log("Cannot call inside editor, has support of webgl build only");
#endif
    }

    /// <summary>
    /// Aysnc call when user is connected to a wallet
    /// </summary>
    async private void OnConnected()
    {
        CancelInvoke();
        account = ConnectAccount(); //get connected wallet address and store it inside a variable

        while (account == "")//check every 1 seconds if retured wallet address was empty (wallet connection in process)
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        PlayerPrefs.SetString(Constants.WalletAccoutKey, account); //save connected wallet address in a playerpref
        Constants.WalletAddress = account;//store wallet address in a singleton static class as well (for single session)
        string _newAcount = '"' + account.ToLower() + '"';

        if (StoredWallet != "null" && StoredWallet.ToLower() != _newAcount)
        {
            Debug.Log("Wallet address was changed.");
            Constants.WalletChanged = true;
        } else
        {
            Constants.WalletChanged = false;
        }

        //store connected wallet address in local storage by a key
#if UNITY_WEBGL && !UNITY_EDITOR
            SetStorage("Account", PlayerPrefs.GetString(Constants.WalletAccoutKey));
#endif
        Constants.WalletConnected = true;
        FirebaseManager.Instance.DocFetched = false;
        FirebaseManager.Instance.ResultFetched = true;
        SetConnectAccount(""); // reset login message
        MainUI.ConnectBtn.SetActive(false); //disable connect button
        MainUI.ConnectedBtn.SetActive(true);// enable connected button
        PrintWalletAddress(); // print wallet address on connected button
        BEPBalanceOf();//calculte and display BEP20 (crace) balance on screen

        if (!Constants.IsTestNet)
            InvokeRepeating("CheckNFTBalance", 0.1f, 10f);//check number of NFT purchased after every 10 seconds of interval
    }

    /// <summary>
    /// Called to print connected wallet address on Connected button in short form (with **** in between of wallet addresses)
    /// </summary>
    public void PrintWalletAddress()
    {
        char[] charArr = account.ToCharArray();//convert connected wallet address to character array
        string FirstPart = "";
        string MidPart = "*****************";
        string EndPart = "";

        for (int i = 0; i < 4; i++)
            FirstPart += charArr[i];

        for (int j = charArr.Length - 4; j < charArr.Length; j++)
            EndPart += charArr[j];

        MainUI.ConnectedBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Connected" + "\n" + FirstPart + MidPart + EndPart;
    }

    /// <summary>
    /// Call to get balance of specific BEp20/ERC20 contract of connected wallet
    /// </summary>
    async public void BEPBalanceOf()
    {
        mainbalanceOf = await ERC20.BalanceOf(chain, network, contract, account);
        DisplayBalance();
    }

    /// <summary>
    /// Display (after calculating) token balance on screen after connecting to wallet
    /// </summary>
    public void DisplayBalance()
    {
        decimalValue = (BigInteger)Math.Pow(10, (double)decimals);
        actualBalance = mainbalanceOf / decimalValue;
        MainUI.CoinText.text = actualBalance.ToString();
    }

    /// <summary>
    /// Call to get name of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPName()
    {
        nameContract = await ERC20.Name(chain, network, contract);
    }

    /// <summary>
    /// Call to get symbol of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPSymbol()
    {
        symbol = await ERC20.Symbol(chain, network, contract);
    }

    /// <summary>
    /// Call to get decimal of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPDecimals()
    {
        decimals = await ERC20.Decimals(chain, network, contract);
    }

    /// <summary>
    /// Call to get total supply of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPTotalSupply()
    {
        totalSupply = await ERC20.TotalSupply(chain, network, contract);
    }

    /// <summary>
    /// called to update connected wallet values
    /// </summary>
    public void UpdateWallet()
    {
        BEPName();
        BEPSymbol();
        BEPDecimals();
        BEPTotalSupply();

        if (Constants.WalletConnected)
            BEPBalanceOf();
    }

    /// <summary>
    /// Transfer BEP20 token (transfer certain amount of crace for tournament)
    /// </summary>
    async public void TransferToken(int _amount)
    {
        BigInteger _mainAmount = _amount * mainBalance;
        amount = _mainAmount.ToString();
        string method = "transfer"; //function name to call on contract
        string[] obj = { toAccount, amount }; //create string array for data to be send to contract function as parameters
        string args = JsonConvert.SerializeObject(obj);//convert c# array to json
        string value = "0"; //value of coin for transaction (for example BNB if binance is used)
        string gasLimit = "210000";
        string gasPrice = "10000000000";

        try
        {
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);

            if (response.Contains("Returned error: internal error"))
            {
                Debug.LogError("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }

            BEPBalanceOf(); //display/update remaining balance after successful transaction

            if (response != "")
            {
                if (Constants.BuyingPass) //if buying pass call was initiated for transaction
                {
                    StoredHash = response;
                    StoredMethodName = "BuyingPass";

                } else
                {
                    StoredHash = response;
                    StoredMethodName = "transfer";
                }

                CheckTransaction();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            if (Constants.BuyingPass)
            {
                Constants.BuyingPass = false;
                MainMenuViewController.Instance.OnPassBuy(false);
            }
            else
            {
                MainMenuViewController.Instance.StartTournament(false);
            }
        }
    }

    async public void CheckTransaction()
    {
        string txStatus = await EVM.TxStatus(chain, network, StoredHash);

        if (txStatus == "success")
        {
            switch (StoredMethodName)
            {
                case "BuyingPass":
                    Constants.BuyingPass = false;
                    Debug.Log("pass bought was success");
                    MainMenuViewController.Instance.OnPassBuy(true);
                    break;
                case "transfer":
                    Debug.Log("transaction was success for tournament");
                    MainMenuViewController.Instance.StartTournament(true);
                    break;
                case "createRace":
                    Debug.Log("createRace was success");
                    OnRaceCreateCalled(true);
                    break;
                case "deposit":
                    Debug.Log("deposit was success");
                    OnDepositCalled(true);
                    break;
                case "endRace":
                    Debug.Log("endrace was success");
                    OnEndRaceCalled(true);
                    break;
            }
        }
        else if (txStatus == "fail")
        {
            switch (StoredMethodName)
            {
                case "BuyingPass":
                    Constants.BuyingPass = false;
                    Debug.Log("pass bought was failed TX");
                    MainMenuViewController.Instance.OnPassBuy(false);
                    break;
                case "transfer":
                    Debug.Log("transaction was failed for tournament TX");
                    MainMenuViewController.Instance.StartTournament(false);
                    break;
                case "createRace":
                    Debug.Log("createRace was failed TX");
                    OnRaceCreateCalled(false);
                    break;
                case "deposit":
                    Debug.Log("deposit was failed TX");
                    OnDepositCalled(false);
                    break;
                case "endRace":
                    Debug.Log("endrace was failed TX");
                    OnEndRaceCalled(false);
                    break;
            }
        }
        else if (txStatus == "pending")
        {
            Invoke("CheckTransaction", 2f);
        }
    }

    /// <summary>
    /// Called to check amount of  BEP20 (crace) before entering tournament
    /// </summary>
    /// <returns></returns>
    public bool CheckBalanceTournament(bool _checkBalance, bool _checkDiscountBalance, bool _checkPassBalance, bool _checkMultiplayerAmount)
    {
        bool _havebalance = false;
        int _amountToCheck = 0;
        if (TournamentManager.Instance)
        {
            if (_checkBalance)
                _amountToCheck = Constants.TicketPrice;
            else if (_checkDiscountBalance)
                _amountToCheck = Constants.DiscountForCrace;
            else if (_checkPassBalance)
                _amountToCheck = Constants.TournamentPassPrice;
            else if (_checkMultiplayerAmount)
                _amountToCheck = Constants.CalculatedCrace;

            if (actualBalance >= _amountToCheck || Constants.IsTest)
                _havebalance = true;
        }
        else
        {
            Debug.LogError("TM is null for CheckBalanceTournament");
            Invoke("CheckBalanceTournament", 1f); //call CheckBalanceTournament function again if instance of TournamentManager is not created yet
        }

        return _havebalance;
    }

    /// <summary>
    /// Called to check amount of  BEP20 (crace) before entering tournament on discount timeslot
    /// </summary>
    /// <returns></returns>
    public bool CheckBalanceDiscountTournament()
    {
        bool _havebalance = false;
        if (TournamentManager.Instance)
        {
            if (actualBalance >= Constants.DiscountForCrace)
                _havebalance = true;
        }
        else
        {
            Debug.LogError("TM is null for CheckBalanceTournament");
        }

        return _havebalance;
    }
    #endregion

    #region NFT Functionality
    /// <summary>
    /// Call to get balance of specific BEP721/ERC721 nft contract
    /// </summary>
    async public void BEP721BalanceOf()
    {
        mainbalanceOfNFT = await ERC721.BalanceOf(chain, network, contractNFT, account);
    }

    /// <summary>
    /// Call to get name of owner of specific BEP721/ERC721 nft contract
    /// </summary>
    async public void BEP721OwnerOf()
    {
        ownerNFT = await ERC721.OwnerOf(chain, network, contractNFT, tokenId);
    }

    async public void CheckNFTBalance()
    {
        string methodNFT = "balanceOf";// smart contract method to call
        string[] obj = { account };
        string argsNFT = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, contractNFT, abiNFTContract, methodNFT, argsNFT);

        PrintOnConsoleEditor(response);

        if (response.Contains("Returned error: internal error"))
        {
            Debug.Log("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        Constants.NFTBought = int.Parse(response);

        if (Constants.NFTBought == 0)
        {
            Constants.NFTStored = 0;
            return;
        }

        if (Constants.NFTStored != Constants.NFTBought)
        {
            if (Constants.NFTChanged)
            {
                Debug.Log("NFT was changed during gameplay");
                NFTTokens.Clear();
                metaDataURL.Clear();
                Constants.ResetData();

                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.ShowToast(3f, "NFT data was changed, game will automatically restart.");
                    Invoke("RestartGame", 3.1f);
                }

                if (GamePlayUIHandler.Instance)
                {
                    GamePlayUIHandler.Instance.ShowToast(3f, "NFT data was changed, game will automatically restart.");
                    Invoke("RestartGame", 3.1f);
                }
            }

            Constants.NFTChanged = true;
            Constants.NFTStored = Constants.NFTBought;
            NFTTokens.Clear();
            metaDataURL.Clear();
            Constants.StoredCarNames.Clear();
            CheckTokenOwnerByIndex();
        } else
        {
            //Debug.Log("nothing new purchased or sold");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    async public void CheckTokenOwnerByIndex()
    {
        Constants.CheckAllNFT = false;
        string methodNFT = "tokenOfOwnerByIndex";// smart contract method to call
        string[] obj = { account, tempNFTCounter.ToString() };
        string argsNFT = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, contractNFT, abiNFTContract, methodNFT, argsNFT);
        PrintOnConsoleEditor(response);

        if (response.Contains("Returned error: internal error"))
        {
            Debug.Log("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        NFTTokens.Add(int.Parse(response));
        if (tempNFTCounter < Constants.NFTBought - 1)
        {
            tempNFTCounter++;
            CheckTokenOwnerByIndex();
        }
        else
        {
            tempNFTCounter = 0;
            GetNFTIPFS();
        }
    }

    async public void GetNFTIPFS()
    {
        string methodNFT = "tokenURI";// smart contract method to call
        string[] obj = { NFTTokens[tempNFTCounter].ToString() };
        string argsNFT = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, contractNFT, abiNFTContract, methodNFT, argsNFT);
        PrintOnConsoleEditor(response);

        if (response.Contains("Returned error: internal error"))
        {
            Debug.Log("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        metaDataURL.Add(response);

        if (tempNFTCounter < NFTTokens.Count - 1)
        {
            tempNFTCounter++;
            GetNFTIPFS();
        } else
        {
            Constants.StoredCarNames.Clear();
            for (int i = 0; i < metaDataURL.Count; i++)
            {
                StartCoroutine(GetJSONDataToStore(metaDataURL[i]));
            }

            NFTCounter = 0;
            WaitForAllData();
        }
    }


    public void WaitForAllData()
    {
        if (NFTCounter == metaDataURL.Count)
        {
            Constants.CheckAllNFT = true;
        } else
        {
            Invoke("WaitForAllData", 1f);
        }
    }
    public IEnumerator GetJSONDataToStore(string _URL)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_URL))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:

                    IPFSdata dataIPFS = JsonConvert.DeserializeObject<IPFSdata>(webRequest.downloadHandler.text);

                    if (!Constants.StoredCarNames.Contains(dataIPFS.name))
                        Constants.StoredCarNames.Add(dataIPFS.name);

                    NFTCounter++;

                    break;
            }
        }
    }
    #endregion

    #region CSP Contract

    public void CallCreateRace()
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        CreateRace(2, 5);
    }

    public void CallDeposit()
    {
        Deposit(Constants.StoredPID);
    }

    public void CallEndRace()
    {
        EndRace(Constants.StoredPID);
    }

    public void OnRaceCreateCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            if (Web3GL.eventResponse != "")
            {
                var details = JObject.Parse(Web3GL.eventResponse);
                //Debug.Log("Pid: "+ details["returnValues"]["pid"]);
                //Debug.Log("pNumber: " + details["returnValues"]["pNumber"]);
                //Debug.Log("price: " + details["returnValues"]["price"]);

                Constants.StoredPID = details["returnValues"]["pid"].ToString();
                Deposit(Constants.StoredPID);

                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Race created.");
            }
            else
            {
                Debug.LogError("Something went wrong for raceCreate");
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong, please try again.");
            }
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    public void OnDepositCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Deposit was done successfully.");
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    public void OnEndRaceCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Race ended, received reward.");
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    async public void CreateRace(int _nFTNumber, double _price)
    {
        string methodCSP = "createRace";
        string[] obj = { _nFTNumber.ToString(), _price.ToString() };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string value = "0";
        string gasLimit = "210000";
        string gasPrice = "10000000000";

        try
        {
            string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, true);

            if (response.Contains("Returned error: internal error"))
            {
                Debug.Log("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }

            if (response != "")
            {
                Debug.LogError(response);
                StoredHash = response;
                StoredMethodName = "createRace";
                CheckTransaction();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            OnRaceCreateCalled(false);
        }
    }

    async public void Deposit(string _pid)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        string methodCSP = "deposit";
        string[] obj = { _pid };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string value = "0";
        string gasLimit = "210000";
        string gasPrice = "10000000000";

        try
        {
            string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, true);

            if (response.Contains("Returned error: internal error"))
            {
                Debug.Log("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }

            if (response != "")
            {
                StoredHash = response;
                StoredMethodName = "deposit";
                CheckTransaction();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            OnDepositCalled(false);
        }
    }

    async public void EndRace(string _pid)
    {
        MainMenuViewController.Instance.LoadingScreen.SetActive(true);
        string methodCSP = "endRace";
        string[] obj = { _pid, Constants.WalletAddress };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string value = "0";
        string gasLimit = "210000";
        string gasPrice = "10000000000";

        try
        {
            string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, true);

            if (response.Contains("Returned error: internal error"))
            {
                Debug.Log("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }

            if (response != "")
            {
                StoredHash = response;
                StoredMethodName = "endRace";
                CheckTransaction();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            OnDepositCalled(false);
        }
    }
    #endregion

    public void PrintOnConsoleEditor(string _con)
    {
#if UNITY_EDITOR
        Debug.Log(_con);
#endif

    }
}
