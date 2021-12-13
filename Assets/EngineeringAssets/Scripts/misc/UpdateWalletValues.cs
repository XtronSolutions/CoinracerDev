using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWalletValues : MonoBehaviour
{
    private void OnEnable()
    {
        UpdateData();
    }

    public void UpdateData()
    {
        if (WalletManager.Instance)
        {
            //Debug.Log("here");
            WalletManager.Instance.UpdateWallet();
        }
        else
        {
            Debug.LogError("WalletManager instance is null");
            Invoke("UpdateData", 1f);
        }
    }

}
