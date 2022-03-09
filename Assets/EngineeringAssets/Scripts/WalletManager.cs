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
using System.Threading.Tasks;

#region SuperClasses

[Serializable]
public class EndRacePayload
{
    public string _pid;
    public string _winner;
    public string _score;
    public string[] _tokenIds = new string[2];
    public string _hash;
}

[Serializable]
public class PIDResponse
{
    public string pids;
}

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
    //private string contractNFT = "0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08";

    //address of the BEP721 contract 2.0
    //mainnet : 0x1B967351e96Bc52E7f4c28EB97406bfa7eB8c8b2
    //testnet : 0x8070c987d80B1363710BF53998C9078ebD75A05B
    //private string contractNFT2 = "0x8070c987d80B1363710BF53998C9078ebD75A05B";

    private string[] NFTContracts = new string[2]
    {
        "0x3a7951ff955d4e0b6cbbe54de8593606e5e0fa08",
        "0x1B967351e96Bc52E7f4c28EB97406bfa7eB8c8b2"
    };

    private string[] NFTContractsAbi = new string[2]
    {
        "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"}],\"name\":\"updateMinBNB\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"craceValue\",\"type\":\"uint256\"},{\"internalType\":\"string[5]\",\"name\":\"data\",\"type\":\"string[5]\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]",
        "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"contract IERC20\",\"name\":\"_busd\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"_mintAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxMintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"metadata\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"minBUSD\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"minCRACE\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"nftAmount\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_maxSupply\",\"type\":\"uint256\"}],\"name\":\"updateMaxSupply\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"idx\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"updateMetadata\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBUSD\",\"type\":\"uint256\"}],\"name\":\"updateMinBUSD\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
    };

    //mainnet : 0xb574f1566B629399d320a72227b26964377d12eB
    //testnet : 0x129CE73e896689FFeEbD1C1220B5D7fBB2f66340
    //testnet2: 0xBA3276DD939793d2012186C3eD259009d1074D62
    private string CSPContract = "0x676764226E2ea90e2b8C0b3816a61f4D2F9349D5";
    private string abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_chipRaceAddress\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_privatekey\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"emergencyWithdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_score\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_tokenURI\",\"type\":\"string\"}],\"name\":\"getCarTypeOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getEmergencyWithdrawFunds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPrivateKey\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"isExistsRoom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isUpgradable\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newKey\",\"type\":\"string\"}],\"name\":\"updatePrivateKey\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    //private string TestContract = "0xc91618907d17aC466f3F80bbeBE9a70a86F64083";
    //private readonly string abiTest = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"checkHash\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";

    private string toAccount = "0xe1E4160F4AcDf756AA0d2B02D786a42527560E82"; //wallet address to send BEP20 (crace) amount for transactions

    //0x316B51F88021C95247710469aE41cAc23bB2F3e4
    //0x3Fb54a2e64242cF9cba881d6BC24761Dc65b7baA
    //0xE11E38fB9F9f4227f8F1B31143A34771D5BD2717
    private string ChipraceContract = "0x5886e5aCfEeeE4E20B6416a174Cad95C6fA25b93";
    private string abiChipraceContract = "[{\"inputs\":[{\"internalType\":\"contract ERC721Enumerable\",\"name\":\"g_firstAddress\",\"type\":\"address\"},{\"internalType\":\"contract ERC721Enumerable\",\"name\":\"_secondAddress\",\"type\":\"address\"},{\"internalType\":\"contract IERC20\",\"name\":\"_tokenAddress\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_oldChip\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_passedMins\",\"type\":\"uint256\"}],\"name\":\"ClaimRewardsFromChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_feeAmount\",\"type\":\"uint256\"}],\"name\":\"EmergencyExitChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"}],\"name\":\"EnterChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"UpdateScore\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_feeAmount\",\"type\":\"uint256\"}],\"name\":\"UpgradeNFT\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"claimRewards\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"depositeFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"depositePoolFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"}],\"name\":\"enterChipRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"feePrice\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_amountForUpgrade\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"g_caller\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"name\":\"g_carType\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"g_eventDoubling\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"g_maxLockTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_minableAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_poolRewardsAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_targetScores\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_tokenInfo\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"timeStamp\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"runningCounter\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"},{\"internalType\":\"bool\",\"name\":\"isRestored\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_tokenURI\",\"type\":\"string\"}],\"name\":\"getCarTypeOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"remainningTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"canUpgrade\",\"type\":\"bool\"},{\"internalType\":\"bool\",\"name\":\"isEnter\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getLevelOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getOwnerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getRemainingTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isRunningChipRace\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isUpgradable\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC721Received\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_caller\",\"type\":\"address\"}],\"name\":\"setCaller\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_uri\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"}],\"name\":\"setCarType\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_doubling\",\"type\":\"uint256\"}],\"name\":\"setEventDoubling\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_caller\",\"type\":\"address\"}],\"name\":\"setExpUpdater\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"setFeePrice\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_time\",\"type\":\"uint256\"}],\"name\":\"setMaxLockTime\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"setMinableAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_targetScore\",\"type\":\"uint256\"}],\"name\":\"setTargetScoreOf\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"setUpgradeAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_exp\",\"type\":\"uint256\"}],\"name\":\"updateExp\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_score\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"updateScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"upgradeNFT\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawAllFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFeeAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"withdrawNFT\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"withdrawPoolFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    private string amount = "";
    private readonly string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";
    //private readonly string abiNFTContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"}],\"name\":\"updateMinBNB\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"craceValue\",\"type\":\"uint256\"},{\"internalType\":\"string[5]\",\"name\":\"data\",\"type\":\"string[5]\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    private BigInteger mainbalanceOfNFT;//Biginteger to store balance of token inside wallet
    private string tokenId = "19877";
    private string ownerNFT = "";

    private int tempNFTCounter = 0;
    [HideInInspector] public List<List<int>> NFTTokens = new List<List<int>>()
    {
        new List<int>(),
        new List<int>()
    };
    [HideInInspector] public List<List<string>> metaDataURL = new List<List<string>>() 
    { 
        new List<string>(),
        new List<string>()
    };
    string StoredWallet = "null";
    int NFTCounter = 0;
    string StoredHash = "";
    string StoredMethodName = "";

    public bool IsGamePlay = false;
    bool isConnected = false;
    private bool storedCarsCleared = false;

    #endregion
    #region Start Functionality
    /// <summary>
    /// called by unity engine whenever class object is enabled (called before start)
    /// </summary>
    private void OnEnable()
    {
        Instance = this;

        if (!IsGamePlay)
            Constants.WalletConnected = false;

        if (Constants.IsTestNet)
        {
            chain = "binance";
            network = "testnet";
            contract = "0xe9852a19b7E15993d99a51099eb3f8DAC4f51997";
            //contractNFT = "0xD20086Ff85bc773f54d16Abce2e5bA0dD616B395";
            NFTContracts = new string[2]
            {
                "0xD20086Ff85bc773f54d16Abce2e5bA0dD616B395",
                "0x8070c987d80B1363710BF53998C9078ebD75A05B"
            };
            NFTContractsAbi = new string[2]
            {
                "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBNB\",\"type\":\"uint256\"}],\"name\":\"updateMinBNB\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"craceValue\",\"type\":\"uint256\"},{\"internalType\":\"string[5]\",\"name\":\"data\",\"type\":\"string[5]\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]",
                "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_crace\",\"type\":\"address\"},{\"internalType\":\"contract IERC20\",\"name\":\"_busd\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"_data\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"_mintAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"maxMintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"metadata\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"minBUSD\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"minCRACE\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"nftAmount\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"mintByOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"mintSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"pos\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_maxSupply\",\"type\":\"uint256\"}],\"name\":\"updateMaxSupply\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"idx\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"data\",\"type\":\"string\"}],\"name\":\"updateMetadata\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minBUSD\",\"type\":\"uint256\"}],\"name\":\"updateMinBUSD\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_minCRACE\",\"type\":\"uint256\"}],\"name\":\"updateMinCRACE\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"recipient\",\"type\":\"address\"}],\"name\":\"withdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
            };
            //CSPContract = "0xeF200cAa118ebA1CF4C2A6ef4Ccb2aaCdb094a82";
            CSPContract = "0xE08Db32a665c518101d907a2a2245012040A1dB9";
            ChipraceContract = "0xF5A7D73d52a1994ff3581C5fC3f8f2A69DD37925";
            abiChipraceContract = "[{\"inputs\":[{\"internalType\":\"contract ERC721Enumerable\",\"name\":\"g_firstAddress\",\"type\":\"address\"},{\"internalType\":\"contract ERC721Enumerable\",\"name\":\"_secondAddress\",\"type\":\"address\"},{\"internalType\":\"contract IERC20\",\"name\":\"_tokenAddress\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"ClaimRewardsFromChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_feeAmount\",\"type\":\"uint256\"}],\"name\":\"EmergencyExitChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"}],\"name\":\"EnterChipRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"UpdateScore\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_feeAmount\",\"type\":\"uint256\"}],\"name\":\"UpgradeNFT\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"feePrice\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_amountForUpgrade\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"g_caller\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"name\":\"g_carType\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"g_maxLockTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_minableAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_poolRewardsAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_targetScores\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"g_tokenInfo\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"timeStamp\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"runningCounter\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getLevelOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isRunningChipRace\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isUpgradable\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_tokenURI\",\"type\":\"string\"}],\"name\":\"getCarTypeOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getRemainingTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getOwnerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"remainningTime\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"canUpgrade\",\"type\":\"bool\"},{\"internalType\":\"bool\",\"name\":\"isEnter\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"}],\"name\":\"enterChipRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"claimRewards\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"upgradeNFT\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_score\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"updateScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_caller\",\"type\":\"address\"}],\"name\":\"setCaller\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_targetScore\",\"type\":\"uint256\"}],\"name\":\"setTargetScoreOf\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_uri\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"}],\"name\":\"setCarType\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"setMinableAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_type\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_level\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"setUpgradeAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"setFeePrice\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_time\",\"type\":\"uint256\"}],\"name\":\"setMaxLockTime\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFeeAmount\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawAllFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"depositeFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"depositePoolFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_poolId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"withdrawPoolFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC721Received\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";
            abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_chipRaceAddress\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_privatekey\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"isExistsRoom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"emergencyWithdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getEmergencyWithdrawFunds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newKey\",\"type\":\"string\"}],\"name\":\"updatePrivateKey\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPrivateKey\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_score\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isUpgradable\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_tokenURI\",\"type\":\"string\"}],\"name\":\"getCarTypeOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";
            //abiCSPContract = "[{\"inputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"_token\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_chipRaceAddress\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_privatekey\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"CreateRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"string\",\"name\":\"errMsg\",\"type\":\"string\"}],\"name\":\"Deposit\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"}],\"name\":\"EmergencyEndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"pid\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"winner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"endTime\",\"type\":\"uint256\"}],\"name\":\"EndRace\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"feeAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"isExistsRoom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"players\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"races\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"userInfo\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"getFeeWallet\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_feeWallet\",\"type\":\"address\"}],\"name\":\"updateFeeWallet\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"withdrawFee\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"emergencyWithdrawFunds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getEmergencyWithdrawFunds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_newKey\",\"type\":\"string\"}],\"name\":\"updatePrivateKey\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPrivateKey\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_maxPlayers\",\"type\":\"uint256\"}],\"name\":\"createRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_price\",\"type\":\"uint256\"}],\"name\":\"deposit\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"emergencyWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_score\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"_tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"testUpdateScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"}],\"name\":\"getRaceInfo\",\"outputs\":[{\"components\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"price\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"maxPlayers\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"inLobby\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isRunning\",\"type\":\"bool\"}],\"internalType\":\"struct CoinracerSmartPool.PrizePool\",\"name\":\"\",\"type\":\"tuple\"},{\"internalType\":\"address[]\",\"name\":\"\",\"type\":\"address[]\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"isUpgradable\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_tokenURI\",\"type\":\"string\"}],\"name\":\"getCarTypeOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";
        }

        if (Constants.IsTest)//&& !Constants.IsTestNet
        {
            //Humza Khalid
            Constants.WalletAddress = "0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10";
            SetAcount("0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10");//0x54815A2afe0393F167B2ED59D6DF5babD40Be6Db//0x5ae0d51FA54C70d731a4d5940Aef216F3fCbEd10
            
            //Muhammad Ijlal
            //Constants.WalletAddress = "0x88F1696C24115b23D80088eA6cbEf2Ee4ef4495c";
            //SetAcount("0x88F1696C24115b23D80088eA6cbEf2Ee4ef4495c");
            //InvokeRepeating("CheckNFTBalance", 0.1f, 10f);
            //getNftsData();
            InvokeRepeating("getNftsData", 0.1f, 10f);
        }
        //CheckHash("12345");
        //g_tokenInfo("12");
        //getAllDataFromFunc("12");
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
            Constants.PrintLog("Wallet address was changed.");
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
        Constants.ForceUpdateChiprace = false;

        if (!IsGamePlay)
        {
            bool _isapproved = await CheckCraceApproval(0);
            if (!_isapproved)
            {
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                }
            }

            MainUI.ConnectBtn.SetActive(false); //disable connect button
            MainUI.ConnectedBtn.SetActive(true);// enable connected button
            PrintWalletAddress(); // print wallet address on connected button
            InvokeRepeating("getNftsData", 0.1f, 10f);
            //InvokeRepeating("CheckNFTBalance", 0.1f, 10f);//check number of NFT purchased after every 10 seconds of interval
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
        string MidPart = "********";
        string EndPart = "";

        for (int i = 0; i < 4; i++)
            FirstPart += charArr[i];

        for (int j = charArr.Length - 4; j < charArr.Length; j++)
            EndPart += charArr[j];

        MainUI.ConnectedBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = FirstPart + MidPart + EndPart;
    }

    /// <summary>
    /// Call to get balance of specific BEp20/ERC20 contract of connected wallet
    /// </summary>
    async public void BEPBalanceOf()
    {
        mainbalanceOf = await ERC20.BalanceOf(chain, network, contract, account);

        if (!IsGamePlay)
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
        string gasLimit = "";//210000
        string gasPrice = "";//10000000000

        try
        {
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);

            if (response.Contains("Returned error: internal error"))
            {
                Constants.PrintError("Returned error: internal error");
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
            Constants.PrintExp(e, this);
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
                    Constants.PrintLog("pass bought was success");
                    MainMenuViewController.Instance.OnPassBuy(true);
                    break;
                case "transfer":
                    Constants.PrintLog("transaction was success for tournament");
                    MainMenuViewController.Instance.StartTournament(true);
                    break;
                case "createRace":
                    Constants.PrintLog("createRace was success");
                    OnRaceCreateCalled(true);
                    break;
                case "deposit":
                    Constants.PrintLog("deposit was success");
                    OnDepositCalled(true);
                    break;
                case "endRace":
                    Constants.PrintLog("endrace was success");
                    OnEndRaceCalled(true);
                    break;
                case "emergencyWithdraw":
                    OnDepositBackCalled(true);
                    break;
                case "approve":
                    OnApproveCalled(true);
                    break;
                case "enterChipRace":
                    OnChipraceEnterCalled(true);
                    break;
                case "claimRewards":
                    OnClaimRewardCalled(true);
                    break;
                case "emergencyExitChipRace":
                    OnEmergencyExitChipRaceCalled(true);
                    break;
                case "upgradeNFT":
                    OnUpgradeNFTCalled(true);
                    break;
                case "approveNFT":
                    OnApproveNFTCalled(true);
                    break;
                case "approveChiprace":
                    OnApproveCraceChipraceCalled(true);
                    break;
            }
        }
        else if (txStatus == "fail")
        {
            switch (StoredMethodName)
            {
                case "BuyingPass":
                    Constants.BuyingPass = false;
                    Constants.PrintLog("pass bought was failed TX");
                    MainMenuViewController.Instance.OnPassBuy(false);
                    break;
                case "transfer":
                    Constants.PrintLog("transaction was failed for tournament TX");
                    MainMenuViewController.Instance.StartTournament(false);
                    break;
                case "createRace":
                    Constants.PrintLog("createRace was failed TX");
                    OnRaceCreateCalled(false);
                    break;
                case "deposit":
                    Constants.PrintLog("deposit was failed TX");
                    OnDepositCalled(false);
                    break;
                case "endRace":
                    Constants.PrintLog("endrace was failed TX");
                    OnEndRaceCalled(false);
                    break;
                case "emergencyWithdraw":
                    OnDepositBackCalled(false);
                    break;
                case "approve":
                    OnApproveCalled(false);
                    break;
                case "enterChipRace":
                    OnChipraceEnterCalled(false);
                    break;
                case "claimRewards":
                    OnClaimRewardCalled(false);
                    break;
                case "emergencyExitChipRace":
                    OnEmergencyExitChipRaceCalled(false);
                    break;
                case "upgradeNFT":
                    OnUpgradeNFTCalled(false);
                    break;
                case "approveNFT":
                    OnApproveNFTCalled(false);
                    break;
                case "approveChiprace":
                    OnApproveCraceChipraceCalled(false);
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
            Constants.PrintError("TM is null for CheckBalanceTournament");
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
            Constants.PrintError("TM is null for CheckBalanceTournament");
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
        //mainbalanceOfNFT = await ERC721.BalanceOf(chain, network, contractNFT, account);
        //Debug.Log("mainbalanceOfNFT: " + mainbalanceOfNFT);
    }

    /// <summary>
    /// Call to get name of owner of specific BEP721/ERC721 nft contract
    /// </summary>
    async public void BEP721OwnerOf()
    {
        //ownerNFT = await ERC721.OwnerOf(chain, network, contractNFT, tokenId);
    }

    async public void getNftsData()
    {
        for (int i = 0; i < NFTContracts.Length; i++)
            checkNFTsBalance(i);
    }
    
    //this function will get the total number of tokens owned by this user and is also responsible for calling the function that will get their respective
    // ids
    //@param {integer}, index of the contract that this function is traversing
    //@return {} no return
    async public void checkNFTsBalance(int _index)
    {
        string method = "balanceOf";
        string[] obj = { account };
        string args = JsonConvert.SerializeObject(obj);
        string res = await EVM.Call(chain, network, NFTContracts[_index], NFTContractsAbi[_index], method, args);

        if(res.Contains("Returned error: internal error"))
        {
            Debug.Log("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }
        int totalNfts = int.Parse(res);
        Constants.NFTBought[_index] = totalNfts;

        if (Constants.ChipraceInteraction)
            return;

        //if user does not own any NFT
        if (Constants.NFTBought[_index] == 0 && !Constants.NFTChanged[_index])
        {
            if(Constants.NFTStored[_index] != Constants.NFTBought[_index])
            {
                WaitForAllDataNoNFT();
                Constants.NFTBought[_index] = 0;
                
            }
            Constants.NFTStored[_index] = Constants.NFTBought[_index];
            Constants.NFTChanged[_index] = true;
            markFetchCompleted(_index);
            return;
        }

        if(Constants.NFTBought[_index] != Constants.NFTStored[_index])
        {
            Debug.Log("Constants.NFTBought[" + _index + "]" + ": " + Constants.NFTBought[_index]);
            Debug.Log("Constants.NFTStored[" + _index + "]" + ": " + Constants.NFTStored[_index]);
            //will be false for the first time
            if (Constants.NFTChanged[_index])
            {
                Constants.StoredCarNames.Clear();
                Constants.ResetData();

                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.ShowToast(3f, "NFT data was changed, game will automatically restart.");
                    Invoke("RestartGame", 3.1f);
                }
                Constants.NFTChanged[_index] = false;
                storedCarsCleared = false;
            }
            if(!storedCarsCleared)
            {
                Constants.StoredCarNames.Clear();
                storedCarsCleared = true;
            }
                
            Constants.NFTChanged[_index] = true;
            Constants.NFTStored[_index] = Constants.NFTBought[_index];
            
            //Debug.Log("totalNFTS: " + totalNfts + " index: " + _index);
            getTokenIds(totalNfts, _index);
        }
        else
        {
            //Debug.Log("No new NFT bought or changed");
        }
    }

    //this function will get the ids of all nfts owned by this user and is also responsible for calling the function
    //that will get the ipfs link of each nft
    //@param {integer, integer}, total NFTs owned by this user and the index of the contract that this function is traversing
    //@return {} no return
    async public void getTokenIds(int totalNfts, int _index)
    {
        List<int> tokens = new List<int>();
        int token;
        for(int i = 0; i < totalNfts; i++)
        {
            string methodNFT = "tokenOfOwnerByIndex";// smart contract method to call
            string[] obj = { account, i.ToString() };
            string argsNFT = JsonConvert.SerializeObject(obj);
            string response = await EVM.Call(chain, network, NFTContracts[_index], NFTContractsAbi[_index], methodNFT, argsNFT);
            

            if (response.Contains("Returned error: internal error"))
            {
                Constants.PrintLog("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }
            //Debug.Log("token: " + int.Parse(response) + " _index: " + _index);
            token = int.Parse(response);
            tokens.Add(token);
            NFTTokens[_index].Add(token);
        }

        getNFTsIPFS(tokens, _index);
    }

    async public void getNFTsIPFS(List<int> tokens, int _index) 
    {
        List<string> links = new List<string>();
        foreach(int token in tokens)
        {
            string methodNFT = "tokenURI";// smart contract method to call
            string[] obj = { token.ToString() };
            string argsNFT = JsonConvert.SerializeObject(obj);
            string response = await EVM.Call(chain, network, NFTContracts[_index], NFTContractsAbi[_index], methodNFT, argsNFT);
            PrintOnConsoleEditor(response);

            if (response.Contains("Returned error: internal error"))
            {
                Constants.PrintLog("Returned error: internal error");
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                    return;
                }
            }
            //Debug.Log("IPFS link: " + response + " _index: " + _index);
            if (!Constants.gifLinks.Contains(response))
            {
                metaDataURL[_index].Add(response);
                links.Add(response);
            }
            else
            {
                string link = Constants.gifLinksAlternative[Constants.gifLinks.IndexOf(response)];
                metaDataURL[_index].Add(link);
                links.Add(link);
            }
        }
        //Debug.Log("Clearing stored car names");
        //Constants.StoredCarNames.Clear();
        Constants.NFTTotalData.Clear();
        StartCoroutine(getNftsMetaData(links, tokens, _index, 0));
        WaitForAllData();
    }

    public IEnumerator getNftsMetaData(List<string> Ipfs,List<int> _tokens, int _contractIndex, int _entryIndex)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(Ipfs[_entryIndex]))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Constants.PrintError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Constants.PrintError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    IPFSdata dataIPFS = JsonConvert.DeserializeObject<IPFSdata>(webRequest.downloadHandler.text);
                    //Debug.Log("Token Name: " + dataIPFS.name + " _index: " + _contractIndex);

                    StoreNameWithToken(dataIPFS.name, _tokens[_entryIndex]);
                    StoreChipraceData(dataIPFS.name, _tokens[_entryIndex]);

                    if (!Constants.StoredCarNames.Contains(dataIPFS.name))
                    {
                        Constants.StoredCarNames.Add(dataIPFS.name);
                        Debug.Log(dataIPFS.name + " added in storedcarnames");
                    }
                        

                    if (_entryIndex < Ipfs.Count - 1)
                        StartCoroutine(getNftsMetaData(Ipfs, _tokens, _contractIndex, _entryIndex + 1));
                    else
                        markFetchCompleted(_contractIndex);
                    break;
            }
        }

    }

    private void markFetchCompleted(int _index)
    {
        //Debug.Log("Fetch completed of: " + _index);
        Constants.nftDataFetched[_index] = true;
        if (checkAllNftData())
            Constants.CheckAllNFT = true;
    }

    //this function will be used to check if the nft data of all NFT contracts has been fetched
    //@param {integer}, index of the contract that just fetched all of its data
    //@return {} no return
    public bool checkAllNftData()
    {
        for(int i=0;i< Constants.nftDataFetched.Length; i++)
        {
            if (!Constants.nftDataFetched[i])
                return false;
        }
        return true;
    }

    private void resetFetchedData()
    {
        Constants.CheckAllNFT = false;
        for (int i = 0; i < Constants.nftDataFetched.Length; i++)
        {
            Constants.nftDataFetched[i] = false;
        }
    }

    async public void ForceUpdateNFT()
    {
        resetFetchedData();
        for (int i = 0; i < NFTContracts.Length; i++)
            forceUpdateNFTByContract(i);

    }

    async public void forceUpdateNFTByContract(int _index)
    {
        //Debug.Log("In forceUpdateNFTByContract index " + _index);
        string methodNFT = "balanceOf";// smart contract method to call
        string[] obj = { account };
        string argsNFT = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, NFTContracts[_index], NFTContractsAbi[_index], methodNFT, argsNFT);

        PrintOnConsoleEditor(response);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        Constants.NFTBought[_index] = int.Parse(response);

        if (Constants.NFTBought[_index] == 0)
        {
            Constants.NFTChanged[_index] = true;
            Constants.NFTStored[_index] = Constants.NFTBought[_index];
            Constants.ChipraceInteraction = false;
            NFTTokens[_index].Clear();
            metaDataURL[_index].Clear();
            Constants.StoredCarNames.Clear();
            tempNFTCounter = 0;
            ClearChipraceData();
            Constants.TokenNFT.Clear();
            ChipraceHandler.Instance.nftStalked = new StalkedNFT();
            Constants.ForceUpdateChiprace = true;

            WaitForAllDataNoNFT();

            Constants.ChipraceInteraction = false;
            return;
        }

        Constants.NFTChanged[_index] = true;
        Constants.NFTStored[_index] = Constants.NFTBought[_index];
        Constants.ChipraceInteraction = false;
        NFTTokens[_index].Clear();
        metaDataURL[_index].Clear();
        Constants.StoredCarNames.Clear();
        tempNFTCounter = 0;
        ClearChipraceData();
        Constants.TokenNFT.Clear();
        ChipraceHandler.Instance.nftStalked = new StalkedNFT();
        Constants.ForceUpdateChiprace = true;

        if (Constants.ForceUpdateChiprace)
            ChipraceHandler.Instance.GetNFTData();

        getTokenIds(Constants.NFTBought[_index], _index);
    }

    public void RestartGame()
    {
        Constants.ForceUpdateChiprace = false;
        for (int i = 0; i < Constants.NFTBought.Length; i++)
        {
            Constants.NFTBought[i] = -2;
            Constants.NFTStored[i] = -1;
            Constants.NFTChanged[i] = false;
        }
        for(int i = 0; i < NFTTokens.Count; i++)
        {
            NFTTokens[i].Clear();
            metaDataURL[i].Clear();
        }
        resetFetchedData();
        Constants.StoredCarNames.Clear();
        ClearChipraceData();
        Constants.TokenNFT.Clear();
        ChipraceHandler.Instance.nftStalked = new StalkedNFT();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void UpdateOnlyStoredData()
    {
        string info = FirebaseManager.Instance.GetStalkedNFT();
        if (info != "" && string.IsNullOrEmpty(info))
        {
            ChipraceHandler.Instance.nftStalked = JsonConvert.DeserializeObject<StalkedNFT>(info);
            if (ChipraceHandler.Instance)
            {
                for (int i = 0; i < ChipraceHandler.Instance.nftStalked.NFTList.Count; i++)
                {
                    StoreChipraceData(ChipraceHandler.Instance.nftStalked.NFTNameList[i], ChipraceHandler.Instance.nftStalked.NFTList[i]);
                }
            }
        }

        Constants.CheckAllNFT = true;
        Constants.ChipraceDataChecked = false;

        if (Constants.ForceUpdateChiprace)
        {
            Constants.ForceUpdateChiprace = false;
            if (ChipraceHandler.Instance)
                ChipraceHandler.Instance.ForceUpdate();
        }
    }
    public void WaitForAllData()
    {
        //if (NFTCounter == metaDataURL.Count && Constants.LoggedIn)
        if(Constants.CheckAllNFT && Constants.LoggedIn)
        {
            CancelInvoke("WaitForAllData");
            try
            {
                string info = FirebaseManager.Instance.GetStalkedNFT();
                Debug.Log(info);
                if (info != "" && !string.IsNullOrEmpty(info))
                {
                    ChipraceHandler.Instance.nftStalked = JsonConvert.DeserializeObject<StalkedNFT>(info);
                    if (ChipraceHandler.Instance)
                    {
                        for (int i = 0; i < ChipraceHandler.Instance.nftStalked.NFTList.Count; i++)
                        {
                            StoreChipraceData(ChipraceHandler.Instance.nftStalked.NFTNameList[i], ChipraceHandler.Instance.nftStalked.NFTList[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
          
            Constants.CheckAllNFT = true;
            Constants.ChipraceDataChecked = false;

            if(Constants.ForceUpdateChiprace)
            {
                Constants.ForceUpdateChiprace = false;
                if (ChipraceHandler.Instance)
                    ChipraceHandler.Instance.ForceUpdate();
            }
            //ChipraceHandler.Instance.UpdateChipraceData();
        } else
        {
            Invoke("WaitForAllData", 1f);
        }
    }

    public void WaitForAllDataNoNFT()
    {
        if (Constants.LoggedIn)
        {
            string info = FirebaseManager.Instance.GetStalkedNFT();
            if (info != "" && !string.IsNullOrEmpty(info))
            {
                ChipraceHandler.Instance.nftStalked = JsonConvert.DeserializeObject<StalkedNFT>(info);
                if (ChipraceHandler.Instance)
                {
                    for (int i = 0; i < ChipraceHandler.Instance.nftStalked.NFTList.Count; i++)
                    {
                        StoreChipraceData(ChipraceHandler.Instance.nftStalked.NFTNameList[i], ChipraceHandler.Instance.nftStalked.NFTList[i]);
                    }
                }
            }

            Constants.CheckAllNFT = true;
            Constants.ChipraceDataChecked = false;

            if (Constants.ForceUpdateChiprace)
            {
                Constants.ForceUpdateChiprace = false;
                if (ChipraceHandler.Instance)
                    ChipraceHandler.Instance.ForceUpdate();
            }
            //ChipraceHandler.Instance.UpdateChipraceData();
        }
        else
        {
            Invoke("WaitForAllDataNoNFT", 1f);
        }
    }

    public void StoreNameWithToken(string _name, int _token)
    {
        bool _found = false;
        int _index = 0;

        if (Constants.TokenNFT.Count != 0)
        {
            for (int i = 0; i < Constants.TokenNFT.Count; i++)
            {
                if (Constants.TokenNFT[i].Name.Contains(_name))
                {
                    _index = i;
                    _found = true;
                    break;
                }
            }
        }

        if (_found)
        {
            Constants.TokenNFT[_index].ID.Add(_token);
        }
        else
        {
            NFTTokens _data = new NFTTokens();
            _data.Name = _name;
            _data.ID.Add(_token);
            Constants.TokenNFT.Add(_data);
        }
    }

    public void StoreChipraceData(string _name, int _token)
    {
        if (ChipraceHandler.Instance)
        {
            for (int i = 0; i < ChipraceHandler.Instance.PoolNFT.Length; i++)
            {
                for (int j = 0; j < ChipraceHandler.Instance.PoolNFT[i].Name.Length; j++)
                {
                    if (_name == ChipraceHandler.Instance.PoolNFT[i].Name[j])
                    {
                        bool _found = false;
                        foreach (var item in ChipraceHandler.Instance.PoolNFT[i].NFTTotalData)
                        {
                            if (_token == item.ID)
                                _found = true;
                        }

                        if (!_found)
                        {
                            TotalNFTData _data = new TotalNFTData();
                            _data.Name = _name;
                            _data.ID = _token;
                            _data.Skin = ChipraceHandler.Instance.PoolNFT[i].NFTSkins[j];
                            ChipraceHandler.Instance.PoolNFT[i].NFTTotalData.Add(_data);
                        }
                    }
                }
            }
        }
    }

    public void ClearChipraceData()
    {
        if (ChipraceHandler.Instance)
        {
            for (int i = 0; i < ChipraceHandler.Instance.PoolNFT.Length; i++)
            {
                ChipraceHandler.Instance.PoolNFT[i].NFTTotalData.Clear();
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
                if (PhotonNetwork.IsMasterClient && !Constants.OtherPlayerDeposit)
                   CreateRace(Constants.StoredPID, Constants.SelectedCrace, Constants.SelectedMaxPlayer);
                else
                   Deposit(Constants.StoredPID, Constants.SelectedCrace);
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
            //    //Constants.PrintLog("Pid: "+ details["returnValues"]["pid"]);
            //    //Constants.PrintLog("pNumber: " + details["returnValues"]["pNumber"]);
            //    //Constants.PrintLog("price: " + details["returnValues"]["price"]);

            //    Constants.StoredPID = details["returnValues"]["pid"].ToString();
            //    //Deposit(Constants.StoredPID);

            //    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            //    MainMenuViewController.Instance.ShowToast(3f, "Race created.");
            //}
            //else
            //{
            //    Constants.PrintError("Something went wrong for raceCreate");
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

            if (Constants.OtherPlayerDeposit)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (MultiplayerManager.Instance)
                        MultiplayerManager.Instance.LoadSceneDelay(0.5f, true);
                }
            }
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please find match again.");
            Invoke("CallDisconnect", 3f);
        }
    }

    public void CallDisconnect()
    {
        if (MultiplayerManager.Instance)
            MultiplayerManager.Instance.DisconnectDelay();
    }

    public void OnEndRaceCalled(bool _state)
    {
        if (_state)
        {
            Constants.ClaimedReward = true;
            Constants.PrintLog("Race ended, received reward.");
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
            Constants.PrintLog("Transaction was not successful");
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

    public BigInteger GetPrice(double _price)
    {
        BigInteger _totalPrice = (int)_price * (BigInteger)Math.Pow(10, 18);
        return _totalPrice;
    }

    async public void CreateRace(string _pid, double _price, int _maxPlayers)
    {
        if (Constants.IsTest)
        {
            OnRaceCreateCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            BigInteger _totalPrice = GetPrice(_price);

            bool _isapproved = await CheckCraceApproval(_totalPrice);
            if(!_isapproved)
            {
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                }

                return;
            }
           
            string methodCSP = "createRace";
            string[] obj = { _pid, _totalPrice.ToString(), _maxPlayers.ToString() };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                    StoredMethodName = "createRace";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnRaceCreateCalled(false);
            }
        }
    }

    async public void Deposit(string _pid, double _price)
    {
        if (Constants.IsTest)
        {
            OnDepositCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            BigInteger _totalPrice = GetPrice(_price);
            bool _isapproved = await CheckCraceApproval(_totalPrice);
            if (!_isapproved)
            {
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                }

                return;
            }

            string methodCSP = "deposit";
            string[] obj = { _pid.ToString(), _totalPrice.ToString() };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "310000";//310000//2100000
            string gasPrice = "10000000000";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                Constants.PrintExp(e, this);
                OnDepositCalled(false);
            }
        }
    }

    async public void CheckHash(string _pid)
    {
        string _hash = await Web3GL.GetEncodedHash(_pid, Constants.WalletAddress, Constants.HashKey);

        string methodCrace = "checkHash";// smart contract method to call
        string[] obj = { _pid, Constants.WalletAddress, _hash };
        string argsCSP = JsonConvert.SerializeObject(obj);

        string testAbi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_pid\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_winner\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"_hash\",\"type\":\"bytes32\"}],\"name\":\"checkHash\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\",\"constant\":true}]";
        string response = await EVM.Call(chain, network, "0x3249E9b661F0787E78B4c82784e2A92e3CFBee07", testAbi, methodCrace, argsCSP);
       // Debug.Log(response);

    }

    public void CallEndRaceForce()
    {
        EndRace("53887");
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

            string _hash = await Web3GL.GetEncodedHash(_pid, Constants.WalletAddress, Constants.HashKey);
            string methodCSP = "endRace";

            EndRacePayload _data = new EndRacePayload();
            _data._pid = _pid;
            _data._winner = Constants.WalletAddress;
            _data._score = Constants.ChipraceScore;
            _data._hash = _hash;

            string[] Tokens;
            if (Constants.OpponentTokenID != "0")
                Tokens = new string[2] { Constants.OpponentTokenID, Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString() };
            else
                Tokens = new string[1] { Constants.TokenNFT[Constants._SelectedTokenNameIndex].ID[Constants._SelectedTokenIDIndex].ToString() };

            for (int i = 0; i < _data._tokenIds.Length; i++)
                _data._tokenIds[i] = Tokens[i];

            string argsCSP = JsonConvert.SerializeObject(_data);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                Constants.PrintExp(e, this);
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
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCSP, abiCSPContract, CSPContract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                Constants.PrintExp(e, this);
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
            string[] obj = { CSPContract, "50000000000000000000000" };//20000000000000000000000
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "";//67224//46327
            string gasPrice = "";//5000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCrace, abi, contract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                Constants.PrintExp(e, this);
                OnApproveCalled(false);
            }
        }
    }

    //csp check crace approval
    public async Task<bool> CheckCraceApproval(BigInteger _amount)
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
                return false;
            }
        }

        if(response!="")
        {
            BigInteger _val = BigInteger.Parse(response);
            if (_val > _amount)
            {
                Constants.ApproveCrace = true;
                return true;
            }
            else
            {
                if (MainMenuViewController.Instance)
                {
                    MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                    MainMenuViewController.Instance.ToogleScreen_CraceUI(true);
                }

                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void OnApproveCraceChipraceCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Crace is approved for chiprace.",true);
            ChipraceHandler.Instance.CraceChipraceApprovalScreen.SetActive(false);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }


    async public void ApproveCraceChiprace()
    {
        if (Constants.IsTest)
        {
            OnApproveCraceChipraceCalled(true);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);
            string methodCrace = "approve";
            string[] obj = { ChipraceContract, "50000000000000000000000" };//20000000000000000000000
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "";//67224//46327
            string gasPrice = "";//5000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodCrace, abi, contract, argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                    StoredMethodName = "approveChiprace";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnApproveCraceChipraceCalled(false);
            }
        }
    }
    public async Task<bool> CheckCraceApprovalChiprace(double _amount)
    {
        BigInteger _totalPrice = (int)_amount * (BigInteger)Math.Pow(10, 18);

        string methodCrace = "allowance";// smart contract method to call
        string[] obj = { Constants.WalletAddress, ChipraceContract };
        string argsCSP = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, contract, abi, methodCrace, argsCSP);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return false;
            }
        }

        if (response != "")
        {
            BigInteger _val = BigInteger.Parse(response);
            if (_val > _totalPrice)
                return true;
            else
                return false;
        }else
        {
            return false;
        }
    }
    public void OnApproveNFTCalled(bool _state)
    {
        if (_state)
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "NFT is approved, you can now enter chip race.",true);
            ChipraceHandler.Instance.NFTApprovalScreen.SetActive(false);
        }
        else
        {
            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
        }
    }
    async public void ApproveNFT()
    {
        if (Constants.IsTest)
        {
            OnApproveNFTCalled(true);
        }
        else
        {
            if(MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            string methodCrace = "approve";
            string[] obj = { ChipraceContract, Constants.NFTTokenApproval.ToString() };
            string argsCSP = JsonConvert.SerializeObject(obj);
            string value = "0";
            string gasLimit = "";//67224//46327
            string gasPrice = "";//5000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response;

                if(Constants.NFTTokenApproval>5000)
                    response = await Web3GL.SendContract(methodCrace, NFTContractsAbi[1], NFTContracts[1], argsCSP, value, gasLimit, gasPrice, _raiseEvent);
                else
                    response = await Web3GL.SendContract(methodCrace, NFTContractsAbi[0], NFTContracts[0], argsCSP, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                    StoredMethodName = "approveNFT";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnApproveNFTCalled(false);
            }
        }
    }
    public async Task<bool> CheckNFTApproval(string _token)
    {
        string methodNFT = "getApproved";// smart contract method to call
        string[] obj = { _token };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string response;

        if (int.Parse(_token) > 5000)
            response = await EVM.Call(chain, network, NFTContracts[1], NFTContractsAbi[1], methodNFT, argsCSP);
        else
            response = await EVM.Call(chain, network, NFTContracts[0], NFTContractsAbi[0], methodNFT, argsCSP);


        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return false;
            }
        }

        if (response != "")
        {
           // Debug.Log(response);
            if (response.Contains(ChipraceContract))
                return true;
            else
                return false;
                
        }else
        {
            return false;
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
            string privatekey = Constants.HashKey;
            string address = Constants.WalletAddress;

            try
            {
                //string response = await Web3GL.GetEncodedHash(privatekey, address);
                //CheckHashMatched(response);
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
            }
        }
    }

    async public void CheckHashMatched(string hash)
    {
        string methodNFT = "checkHash";// smart contract method to call
        string[] obj = { Constants.WalletAddress, hash };
        string argsNFT = JsonConvert.SerializeObject(obj);
        //string response = await EVM.Call(chain, network, TestContract, abiTest, methodNFT, argsNFT);
    }

    async public void getRaceIds()
    {
        string methodCSP = "getRaceIds";
        string[] obj = { };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, CSPContract, abiCSPContract, methodCSP, argsCSP);
        Constants.PIDString = response;
    }

    async public void isExistRoom(string _pid)
    {
        string methodCSP = "isExistsRoom";
        string[] obj = { _pid };
        string argsCSP = JsonConvert.SerializeObject(obj);
        string response = await EVM.Call(chain, network, CSPContract, abiCSPContract, methodCSP, argsCSP);
        Constants.PIDString = response;
    }

    #endregion

    #region Chiprace Contract

    public void DelayLoading()
    {
        if (RaceManager.Instance)
            RaceManager.Instance.ToggleLoadingScreen(true);

        if (MainMenuViewController.Instance)
            MainMenuViewController.Instance.LoadingScreen.SetActive(true);

        ForceUpdateNFT();
    }

    public void OnChipraceEnterCalled(bool _state)
    {
        if (_state)
        {
            int _tok = int.Parse(Constants.storedToken);
            if (ChipraceHandler.Instance)
                ChipraceHandler.Instance.SetChipraceData(_tok, Constants.storedName);

            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(2f, "NFT staked successfully.",true);
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(2f, "NFT staked successfully.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Invoke("DelayLoading", 1f);
          
           
        }
        else
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(3f, "Transaction was not successful, please try again or contact support.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Constants.ChipraceInteraction = false;
        }
    }
    async public void enterChipRace(string _name, string _nFTToken,string _poolId)
    {
        Constants.ChipraceInteraction = true;
        Constants.storedName = _name;
        Constants.storedToken = _nFTToken;
        if (Constants.IsTest)
        {
            OnChipraceEnterCalled(true);
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(true);

            string methodChiprace = "enterChipRace";
            string[] _data = { _nFTToken , _poolId };
            string argsChiprace = JsonConvert.SerializeObject(_data);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodChiprace, abiChipraceContract, ChipraceContract, argsChiprace, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                        MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                        Constants.ChipraceInteraction = false;
                        return;
                    }

                    if (RaceManager.Instance)
                    {
                        RaceManager.Instance.ToggleLoadingScreen(false);
                        Constants.ChipraceInteraction = false;
                        return;
                    }
                }

                if (response != "")
                {
                    StoredHash = response;
                    StoredMethodName = "enterChipRace";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnChipraceEnterCalled(false);
            }
        }
    }

    public void OnClaimRewardCalled(bool _state)
    {
        if (_state)
        {
            int _tok = int.Parse(Constants.storedToken);
            if (ChipraceHandler.Instance)
                ChipraceHandler.Instance.RemoveAndSetChipraceData(_tok, Constants.storedName);

            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(2f, "Reward successfully claimed.",true);
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(2f, "Reward successfully claimed.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Invoke("DelayLoading", 1f);
        }
        else
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(3f, "Transaction was not successful, please try again or contact support.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Constants.ChipraceInteraction = false;
        }
    }
    async public void claimRewards(string _name, string _tokenID)
    {
        Constants.ChipraceInteraction = true;
        Constants.storedName = _name;
        Constants.storedToken = _tokenID;
        if (Constants.IsTest)
        {
            OnClaimRewardCalled(true);
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(true);

            string methodChiprace = "claimRewards";
            string[] _data = { _tokenID };
            string argsChiprace = JsonConvert.SerializeObject(_data);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodChiprace, abiChipraceContract, ChipraceContract, argsChiprace, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                        MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                        Constants.ChipraceInteraction = false;
                        return;
                    }

                    if (RaceManager.Instance)
                    {
                        RaceManager.Instance.ToggleLoadingScreen(false);
                        Constants.ChipraceInteraction = false;
                        return;
                    }
                }

                if (response != "")
                {
                    StoredHash = response;
                    StoredMethodName = "claimRewards";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnClaimRewardCalled(false);
            }
        }
    }

    public void OnEmergencyExitChipRaceCalled(bool _state)
    {
        if (_state)
        {
            int _tok = int.Parse(Constants.storedToken);
            if (ChipraceHandler.Instance)
                ChipraceHandler.Instance.RemoveAndSetChipraceData(_tok, Constants.storedName);

            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(2f, "NFT withdrawn successfully.",true);
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(2f, "Emergency exit chiprace called.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Invoke("DelayLoading", 1f);
        }
        else
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(3f, "Transaction was not successful, please try again.");
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(3f, "Transaction was not successful, please try again or contact support.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            Constants.ChipraceInteraction = false;
        }
    }
    async public void emergencyExitChipRace(string _name,string _tokenID,double _amount)
    {
        Constants.ChipraceInteraction = true;
        Constants.storedName = _name;
        Constants.storedToken = _tokenID;
        if (Constants.IsTest)
        {
            OnEmergencyExitChipRaceCalled(true);
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(true);

            BigInteger _totalFee = (BigInteger)(_amount * 1000000000000000000);
            string methodChiprace = "emergencyExitChipRace";
            string[] _data = { _tokenID, _totalFee.ToString() };
            string argsChiprace = JsonConvert.SerializeObject(_data);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodChiprace, abiChipraceContract, ChipraceContract, argsChiprace, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
                    if (MainMenuViewController.Instance)
                    {
                        MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                        MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                        Constants.ChipraceInteraction = false;
                        return;
                    }

                    if (RaceManager.Instance)
                    {
                        RaceManager.Instance.ToggleLoadingScreen(false);
                        Constants.ChipraceInteraction = false;
                        return;
                    }
                }

                if (response != "")
                {
                    StoredHash = response;
                    StoredMethodName = "emergencyExitChipRace";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnEmergencyExitChipRaceCalled(false);
            }
        }
    }

    public void OnUpgradeNFTCalled(bool _state)
    {
        if (_state)
        {
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.LoadingScreen.SetActive(false);
                MainMenuViewController.Instance.ShowToast(2f, "NFT upgraded to new level successfully.",true);
            }

            if (GamePlayUIHandler.Instance)
                GamePlayUIHandler.Instance.ShowToast(2f, "NFT upgraded to new level.");

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(false);

            if (ChipraceHandler.Instance)
                ChipraceHandler.Instance.ForceUpdate();
        }
        else
        {
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
    async public void upgradeNFT(string _tokenID, double _amount)
    {
        if (Constants.IsTest)
        {
            OnUpgradeNFTCalled(true);
        }
        else
        {
            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.LoadingScreen.SetActive(true);

            if (RaceManager.Instance)
                RaceManager.Instance.ToggleLoadingScreen(true);

            BigInteger _totalPriceUpgrade = (BigInteger)(_amount * 1000000000000000000);
            string methodChiprace = "upgradeNFT";
            string[] _data = { _tokenID, _totalPriceUpgrade.ToString() };
            string argsChiprace = JsonConvert.SerializeObject(_data);
            string value = "0";
            string gasLimit = "";//310000//2100000
            string gasPrice = "";//10000000000
            bool _raiseEvent = false;

            try
            {
                Constants.EventRaised = _raiseEvent;
                string response = await Web3GL.SendContract(methodChiprace, abiChipraceContract, ChipraceContract, argsChiprace, value, gasLimit, gasPrice, _raiseEvent);

                if (response.Contains("Returned error: internal error"))
                {
                    Constants.PrintLog("Returned error: internal error");
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
                    StoredMethodName = "upgradeNFT";
                    CheckTransaction();
                }
            }
            catch (Exception e)
            {
                Constants.PrintExp(e, this);
                OnUpgradeNFTCalled(false);
            }
        }
    }

    async public void getCarType(string _tokenURI)
    {
        string methodChiprace = "getCarTypeOf";
        string[] obj = { _tokenURI };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

       // if (response != "")
           // Debug.Log("Pool id for car type is : " + response);
    }
    public async Task<string> getRemainingTime(string _tokenId)
    {
        string methodChiprace = "getRemainingTime";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return "";
            }
        }

        if (response != "")
        {
            //Debug.Log("Remaining time for token id #" + _tokenId + " is " + response);
            return response;
        }
        else
        {
            return "";
        }
    }
    public async Task<bool> isUpgradable(string _tokenId)
    {
        string methodChiprace = "isUpgradable";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return false;
            }
        }

        if (response != "")
        {
            //
            //Debug.Log("IsUpgradable token id #" + _tokenId + " is " + response);
            if (response.ToLower() == "true")
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }
    public async Task<bool> isRunningChipRace(string _tokenId)
    {
        string methodChiprace = "isRunningChipRace";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return false;
            }
        }

        if (response != "")
        {
            //Debug.Log("Running chiprace for token id #" + _tokenId + " is " + response);
            if (response.ToLower() == "true")
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }
    public async Task<int> getLevelOf(string _tokenId)
    {
        string methodChiprace = "getLevelOf";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return -1;
            }
        }

        if (response != "")
        {
            //Debug.Log("Level for token id #" + _tokenId + " is " + response);
            return int.Parse(response);
        }
        else
        {
            return -1;
        }
    }
    public async Task<int> getScore(string _tokenId)
    {
        string methodChiprace = "g_tokenInfo";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return -1;
            }
        }

        if (response != "")
        {
           // Debug.Log("TokenInfo for token id #" + _tokenId + " is " + response);
            var _data = JObject.Parse(response);
            int _score = int.Parse(_data["score"].ToString());
            return _score;
        }
        else
        {
            return -1;
        }
    }

    public async Task<TotalNFTData> getAllDataFromFunc(string _tokenId)
    {
        string methodChiprace = "getInfo";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return null;
            }
        }

        if (response != "")
        {
            //Debug.Log("TokenInfo for token id #" + _tokenId + " is " + response);
            //{ "0":"0","1":"1645135926","2":"0","3":"0","4":false,"5":false,"level":"0","remainningTime":"1645135926","score":"0","rewards":"0","canUpgrade":false,"isEnter":false}
            TotalNFTData _tokenData = new TotalNFTData();
            var _data = JObject.Parse(response);
            _tokenData.ID = int.Parse(_tokenId);
            _tokenData.TargetScore = int.Parse(_data["score"].ToString());
            _tokenData.Level = int.Parse(_data["level"].ToString());
            _tokenData.IsRunningChipRace = bool.Parse(_data["isEnter"].ToString());

            BigInteger _rewards = BigInteger.Parse(_data["rewards"].ToString());

            BigInteger _decimalValue = (BigInteger)Math.Pow(10, 18);
            BigInteger _actualBalance = _rewards / _decimalValue;

            _tokenData.Rewards =(int) _actualBalance;
            _tokenData.RemainingTime = _data["remainningTime"].ToString();
            _tokenData.IsUpgradable = bool.Parse(_data["canUpgrade"].ToString());
            return _tokenData;
        }
        else
        {
            return null;
        }
    }

    public async Task<TotalNFTData> getAllData(string _tokenId)
    {
        string methodChiprace = "g_tokenInfo";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return null;
            }
        }

        if (response != "")
        {
            //Debug.Log("TokenInfo for token id #" + _tokenId + " is " + response);

            TotalNFTData _tokenData = new TotalNFTData();
            var _data = JObject.Parse(response);
            _tokenData.ID = int.Parse(_tokenId);
            _tokenData.TargetScore=int.Parse(_data["score"].ToString());
            _tokenData.Level = int.Parse(_data["level"].ToString());  
            _tokenData.IsRunningChipRace = bool.Parse(_data["isRunning"].ToString());  
            _tokenData.Rewards= int.Parse(_data["rewards"].ToString());
            _tokenData.runningCounter = int.Parse(_data["runningCounter"].ToString());
            _tokenData.RemainingTime = await getRemainingTime(_tokenId);
            _tokenData.IsUpgradable = await isUpgradable(_tokenId);
            return _tokenData;
        }
        else
        {
            return null;
        }
    }
    async public void g_tokenInfo(string _tokenId)
    {
        string methodChiprace = "g_tokenInfo";
        string[] obj = { _tokenId };
        string argsChiprace = JsonConvert.SerializeObject(obj);

        string response = await EVM.Call(chain, network, ChipraceContract, abiChipraceContract, methodChiprace, argsChiprace);

        if (response.Contains("Returned error: internal error"))
        {
            Constants.PrintLog("Returned error: internal error");
            if (MainMenuViewController.Instance)
            {
                MainMenuViewController.Instance.ShowToast(3f, "Something went wrong please refresh page and try again.");
                return;
            }
        }

        if (response != "")
        {
           // Debug.Log("TokenInfo for token id #" + _tokenId + " is " + response);
            //var _data = JObject.Parse(response);
            //double _score = double.Parse(_data["score"].ToString());
            //Debug.Log(_score);
        }
    }

    public bool CheckChipracebalance(double _amount)
    {
        bool _havebalance = false;
        int _tempAmount = (int)_amount;
        _tempAmount += 1;
        if (actualBalance >= _tempAmount)
            _havebalance = true;

        return _havebalance;
    }

    public bool CheckChipracebalanceWhole(int _amount)
    {
        bool _havebalance = false;
        if (actualBalance >= _amount)
            _havebalance = true;

        return _havebalance;
    }

    #endregion
    public void PrintOnConsoleEditor(string _con)
    {
#if UNITY_EDITOR
        //Debug.Log(_con);
#endif

    }
}
