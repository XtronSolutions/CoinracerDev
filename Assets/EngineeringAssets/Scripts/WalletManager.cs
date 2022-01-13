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
using Photon.Pun;

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
    //testnet02:0xe9852a19b7E15993d99a51099eb3f8DAC4f51997
    private string contract = "0xFBb4F2f342c6DaaB63Ab85b0226716C4D1e26F36";

    //address of the BEP721 contract
    //mainnet : 0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08
    //testnet: 0x312b151a0e87785649ed835d946c2b0de5745c30
    private string contractNFT = "0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08";

    //mainnet : 0x57988AE6CC7F6fEd1B13A0C88bbBE7216ceC6DA9
    //testnet : 0x112EFCA673Bba55a11e4685CCa9998a96b065565
    //testnet2: 0x5D60CADfd0a205d3D40E98662d1ec860b898E98a
    //testnet3: 0x21F1D67cE612208f7F08ee9487Ec9e62A375190B
    //testnet4: 0xf44f4e56C21bdE66b888296Ef70DAC432c13a562
    //testnet5: 0x0C08C54AE734FA0Bd84870a5115A7Cf802d40A3a
    private string CSPContract = "0x0C08C54AE734FA0Bd84870a5115A7Cf802d40A3a";

    private string TestContract = "0xc91618907d17aC466f3F80bbeBE9a70a86F64083";
    private readonly string abiTest = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"checkHash\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";

    private string toAccount = "0xe1E4160F4AcDf756AA0d2B02D786a42527560E82"; //wallet address to send BEP20 (crace) amount for transactions

    private string amount = "";
    private readonly string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";
    private readonly string abiNFTContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"}],\"name\":\"updateMinBNB\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"craceValue\",\"type\":\"uint256\"},{\"internalType\":\"string[5]\",\"name\":\"data\",\"type\":\"string[5]\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
    // private readonly string abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"raceIds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyEndRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"canStartRace\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";
    // working//private readonly string abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"raceIds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyEndRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
    // private readonly string abiCSPContract= "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_privatekey\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"raceIds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newKey\",\"type\":\"string\"}],\"name\":\"updatePrivateKey\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPrivateKey\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";
    private readonly string abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_privatekey\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"raceIds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newKey\",\"type\":\"string\"}],\"name\":\"updatePrivateKey\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPrivateKey\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";

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

    public bool IsGamePlay = false;
    bool isConnected = false;
    #endregion

    #region Start Functionality
    /// <summary>
    /// called by unity engine whenever class object is enabled (called before start)
    /// </summary>
    private void OnEnable()
    {
        Instance = this;

        if(!IsGamePlay)
            Constants.WalletConnected = false;

        if (Constants.IsTestNet)
        {
            chain = "binance";
            network = "testnet";
            contract = "0xe9852a19b7E15993d99a51099eb3f8DAC4f51997";
            contractNFT = "0x312b151a0e87785649ed835d946c2b0de5745c30";
            CSPContract = "0xA9fD4D8c68cd9b506CB791F9E4526af088b383c9";

            //SetAcount("0xAE79Dc61917d0de544db72C75de727421AcD7566");//0x54815A2afe0393F167B2ED59D6DF5babD40Be6Db//0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10
            //Constants.WalletAddress = "0xAE79Dc61917d0de544db72C75de727421AcD7566";
            //CheckCraceApproval();

        }

        if (Constants.IsTest && !Constants.IsTestNet)
        {
            //TesT
            SetAcount("0x54815A2afe0393F167B2ED59D6DF5babD40Be6Db");//0x54815A2afe0393F167B2ED59D6DF5babD40Be6Db//0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10
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
            if(!IsGamePlay)
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

        SetConnectAccount(""); // reset login message
        BEPBalanceOf();//calculte and display BEP20 (crace) balance on screen


        //store connected wallet address in local storage by a key
#if UNITY_WEBGL && !UNITY_EDITOR
            SetStorage("Account", PlayerPrefs.GetString(Constants.WalletAccoutKey));
#endif
        Constants.WalletConnected = true;
        FirebaseManager.Instance.DocFetched = false;
        FirebaseManager.Instance.ResultFetched = true;

        if (!IsGamePlay)
        {
            CheckCraceApproval();
            MainUI.ConnectBtn.SetActive(false); //disable connect button
            MainUI.ConnectedBtn.SetActive(true);// enable connected button
            PrintWalletAddress(); // print wallet address on connected button

            if (!Constants.IsTestNet)
                InvokeRepeating("CheckNFTBalance", 0.1f, 10f);//check number of NFT purchased after every 10 seconds of interval
        }
        else
        {
            isConnected = true;
            EndRace(Constants.StoredPID);
        }

        GetHashEncoded();
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

        if(!IsGamePlay)
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
                case "emergencyWithdraw":
                    OnDepositBackCalled(true);
                    break;
                case "approve":
                    OnApproveCalled(true);
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
                case "emergencyWithdraw":
                    OnDepositBackCalled(false);
                    break;
                case "approve":
                    OnApproveCalled(false);
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

    public void CallDeposit()
    {
        if (Constants.IsMultiplayer)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (Constants.ApproveCrace)
                {
                    if (PhotonNetwork.IsMasterClient)
                        CreateRace(Constants.StoredPID, Constants.SelectedCrace, Constants.SelectedMaxPlayer);
                    else
                        Deposit(Constants.StoredPID, Constants.SelectedCrace);
                }
                else
                {
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                        MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                    }
                }
            }
        }
    }

    public void CallWithdraw()
    {
        if (Constants.IsMultiplayer && Constants.CanWithdraw)
        {
            WithdrawDeposit(Constants.StoredPID);
        }
    }

    public void CallApproveCrace()
    {
        ApproveCrace();
    }

    public void CallRaceWinner()
    {
        if (isConnected)
        {
            EndRace(Constants.StoredPID);
        }
        else
        {
            ConnectWallet();
        }
    }

    public void OnRaceCreateCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(1f, "Transaction was successful.");

            if (MultiplayerManager.Instance)
            {
                MultiplayerManager.Instance.UpdateTransactionData(false, true, "waiting for other player to deposit...", false, true, false);
                RPCCalls.Instance.PHView.RPC("DepositCompleted", RpcTarget.Others);
            }


            //if (Web3GL.eventResponse != "")
            //{
            //    var details = JObject.Parse(Web3GL.eventResponse);
            //    //Debug.Log("Pid: "+ details["returnValues"]["pid"]);
            //    //Debug.Log("pNumber: " + details["returnValues"]["pNumber"]);
            //    //Debug.Log("price: " + details["returnValues"]["price"]);

            //    Constants.StoredPID = details["returnValues"]["pid"].ToString();
            //    //Deposit(Constants.StoredPID);

            //    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            //    MainMenuViewController.Instance.ShowToast(3f, "Race created.");
            //}
            //else
            //{
            //    Debug.LogError("Something went wrong for raceCreate");
            //    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            //    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong, please try again.");
            //}
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
            MainMenuViewController.Instance.ShowToast(1f, "Transaction was successful.");

            if (MultiplayerManager.Instance)
            {
                MultiplayerManager.Instance.UpdateTransactionData(false, true, "waiting for other player to deposit...", false, true, false);
                RPCCalls.Instance.PHView.RPC("DepositCompleted", RpcTarget.Others);
            }

            if(Constants.OtherPlayerDeposit)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (MultiplayerManager.Instance)
                        MultiplayerManager.Instance.LoadSceneDelay(1f,true);
                }
            }
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
            Constants.ClaimedReward = true;
            Debug.Log("Race ended, received reward.");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(2f, "Race ended, received reward.");
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(2f, "Reward received successfully.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);
        }
        else
        {
            Debug.Log("Transaction was not successful");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(3f, "Transaction was not successful, please try again or contact support.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);
        }
    }

    public void OnDepositBackCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(1f, "funds returned.");
            MultiplayerManager.Instance.UpdateTransactionData(false, false, "", false, false, true);
            MainMenuViewController.Instance.DisableScreen_ConnectionUI();
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    public void OnApproveCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.ToogleScreen_CraceUI(false);
            Constants.ApproveCrace = true;
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(2f, "Allocated allowance");
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }

    async public void CreateRace(string _pid, double _price,int _maxPlayers)
    {
        if (Constants.IsTest)
        {
            OnRaceCreateCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            BigInteger _totalPrice = (int)_price * (BigInteger)Math.Pow(10, 18);
            string methodCSP = "createRace";
            string[] obj = { _pid, _totalPrice.ToString(), _maxPlayers.ToString() };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "2100000";
            string gasPrice = "10000000000";

            try
            {
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, false);

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
    }

    async public void Deposit(string _pid,double _price)
    {
        if (Constants.IsTest)
        {
            OnDepositCalled(true);
        }
        else
        {
            BigInteger _totalPrice = (int)_price * (BigInteger)Math.Pow(10, 18);
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            string methodCSP = "deposit";
            string[] obj = { _pid.ToString(), _totalPrice.ToString() };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "2100000";
            string gasPrice = "10000000000";

            try
            {
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, false);

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
    }

    async public void EndRace(string _pid)
    {
        if (Constants.IsTest)
        {
            OnEndRaceCalled(true);
        }
        else
        {
            if(MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(true);

            string _hash = await Web3GL.GetEncodedHash(Constants.HashKey, Constants.WalletAddress);
            string methodCSP = "endRace";
            string[] obj = { _pid, Constants.WalletAddress, _hash };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "2100000";
            string gasPrice = "10000000000";

            try
            {
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, false);

                if (response.Contains("Returned error: internal error"))
                {
                    Debug.Log("Returned error: internal error");
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                        MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                        return;
                    }

                    if (RaceManager.Instance)
                    {
                        RaceManager.Instance.ToggleLoadingScreen(false);
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
                OnEndRaceCalled(false);
            }
        }
    }

    async public void WithdrawDeposit(string _pid)
    {
        if (Constants.IsTest)
        {
            OnDepositBackCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            string methodCSP = "emergencyWithdraw";
            string[] obj = { _pid };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "2100000";
            string gasPrice = "10000000000";

            try
            {
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, false);

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
                    StoredMethodName = "emergencyWithdraw";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                OnDepositBackCalled(false);
            }
        }
    }

    async public void ApproveCrace()
    {
        if (Constants.IsTest)
        {
            OnApproveCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            string methodCrace = "approve";
            string[] obj = { CSPContract,"20000000000000000000000" };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "67224";
            string gasPrice = "10000000000";

            try
            {
                string response = await Web3GL.SendContract(methodCrace, abi, contract, argsCSP, value, gasLimit, gasPrice, false);

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
                    StoredMethodName = "approve";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                OnApproveCalled(false);
            }
        }
    }

    async public void CheckCraceApproval()
    {
        string methodCrace = "allowance";// smart contract method to call
        string[] obj = { Constants.WalletAddress, CSPContract };
        string argsCSP = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, contract, abi, methodCrace, argsCSP);

        if (response.Contains("Returned error: internal error"))
        {
            Debug.Log("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        //Debug.Log(response);

        if(response!="")
        {
            BigInteger _val = BigInteger.Parse(response);
            if (_val > 0)
            {
                Constants.ApproveCrace = true;
            }
            else
            {
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                }
            }
        }

    }

    async public void GetHashEncoded()
    {
        if (Constants.IsTest)
        {
            //OnApproveCalled(true);
        }
        else
        {
            //MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            string privatekey = "testkey";
            string address = Constants.WalletAddress;

            Debug.Log("private key: " + privatekey);
            Debug.Log("wallet address : " + address);
            try
            {
                string response = await Web3GL.GetEncodedHash(privatekey, address);
                Debug.Log("encoded hash :"+ response);
                CheckHashMatched(response);
                //if (response != "")
                //{
                //  Debug.Log(response);
                //}
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                //OnApproveCalled(false);
            }
        }
    }

    async public void CheckHashMatched(string hash)
    {
        string methodNFT = "checkHash";// smart contract method to call
        string[] obj = { Constants.WalletAddress, hash };
        string argsNFT = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, TestContract, abiTest, methodNFT, argsNFT);


        Debug.Log("Check hash response : " + response);        
    }

    #endregion

    public void PrintOnConsoleEditor(string _con)
    {
#if UNITY_EDITOR
        Debug.Log(_con);
#endif

    }
}
