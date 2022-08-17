using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public enum FixType
{
    Tires =0,
    Gas=1,
    Oil=2,
    Damage=3
}
[Serializable]
public class ConfirmationDialogue
{
    public GameObject MainScreen;
    public TextMeshProUGUI Cost_Text;
    public Button PurchaseButton;
    public Button CancelButton;
}

[Serializable]
public class ButtonFunction
{
    public Button PayButton;
    public TextMeshProUGUI Disclaimer_Txt;
    public Slider Slider;
}

[Serializable]
public class StoreUI
{
    public GameObject MainScreen;
    public Button StoreButton;
    public TextMeshProUGUI CCashTxt;
    public ButtonFunction BuyVC;
    public ButtonFunction FixTires;
    public ButtonFunction FillOil;
    public ButtonFunction FillGas;
    public ButtonFunction FixDamage;
    public GameObject ConsumablesObject;
    public Button BackButton;
    public TextMeshProUGUI CarNameText;
    public ConfirmationDialogue DialogueConfirmation;
}
public class StoreHandler : MonoBehaviour
{
    public static StoreHandler Instance;
    public StoreUI UIStore;

    private FixType _fixType;

    private void OnEnable()
    {
        Instance = this;
        ButtonListeners_StoreUI();
    }

    #region StoreUI/Functionality

    private NFTMehanicsData tempNFTData;
    private int tempNFTID = 0;
    public void ToggleMainScreen_StoreUI(bool state)
    {
        UIStore.MainScreen.SetActive(state);
    }

    public void SetCCashText_StoreUI(string txt)
    {
        UIStore.CCashTxt.text = txt;
    }

    public void UpdateCarName_StoreUI(string _name)
    {
        UIStore.CarNameText.text = _name;
    }
    public void ButtonListeners_StoreUI()
    {
        UIStore.StoreButton.onClick.AddListener(EnableStore_StoreUI);
        UIStore.BackButton.onClick.AddListener(DisableStore_StoreUI);
        UIStore.FixTires.PayButton.onClick.AddListener(FixTyre_StoreUI);
        UIStore.BuyVC.PayButton.onClick.AddListener(BuyVC_StoreUI);
        UIStore.FillGas.PayButton.onClick.AddListener(FillGas_StoreUI);
        UIStore.FillOil.PayButton.onClick.AddListener(FillEngineOil_StoreUI);
        UIStore.FixDamage.PayButton.onClick.AddListener(RepairDamage_StoreUI);

        UIStore.DialogueConfirmation.PurchaseButton.onClick.AddListener(OnPurchaseClicked);
        UIStore.DialogueConfirmation.CancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void EnableStore_StoreUI()
    {
        ToggleMainScreen_StoreUI(true);
        UIStore.ConsumablesObject.SetActive(false);
        UIStore.BuyVC.PayButton.gameObject.SetActive(true);
        SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
        UpdateBUYVCText_StoreUI(Constants.CCashPurchaseAmount.ToString());
    }

    public void EnableConsumables_StoreUI(NFTMehanicsData _mainData, int NFTID)
    {
        tempNFTData = _mainData;
        tempNFTID = NFTID;
        Constants.StoredCarHealth = _mainData.mechanicsData.CarHealth;
        UpdateCarName_StoreUI(tempNFTData.mechanicsData.CarName);
        ToggleMainScreen_StoreUI(true);
        UIStore.ConsumablesObject.SetActive(true);
        UIStore.BuyVC.PayButton.gameObject.SetActive(false);
        SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
        UpdateConsumableText_StoreUI();
    }

    public void DisableStore_StoreUI()
    {
        ToggleMainScreen_StoreUI(false);
    }

    public void UpdateBUYVCText_StoreUI(string VC_Txt)
    {
        UIStore.BuyVC.Disclaimer_Txt.text = "*" + VC_Txt + " " + Constants.VirtualCurrency + " will be added in single transaction";
    }
    public void UpdateConsumableText_StoreUI()
    {
        MechanicsManager.Instance.UpdateConsumables(tempNFTData);
        UIStore.FixTires.Disclaimer_Txt.text = MechanicsManager.Instance.GetRemainingTyreLaps() + "/" + MechanicsManager.Instance._consumableSettings.Tyres.LapLimit + " Laps";
        UIStore.FillOil.Disclaimer_Txt.text = MechanicsManager.Instance.GetRemainingOilLaps() + "/" + MechanicsManager.Instance._consumableSettings.EngineOil.LapLimit + " Laps";
        UIStore.FillGas.Disclaimer_Txt.text = MechanicsManager.Instance.GetRemainingGasLaps() + "/" + MechanicsManager.Instance._consumableSettings.Gas.LapLimit + " Laps";
        UIStore.FixDamage.Disclaimer_Txt.text = Constants.StoredCarHealth + "/" + Constants.MaxCarHealth + " HP";

        UpdateSlider_StoreUI(MechanicsManager.Instance.GetRemainingTyreLaps(), MechanicsManager.Instance._consumableSettings.Tyres.LapLimit, UIStore.FixTires.Slider);
        UpdateSlider_StoreUI(MechanicsManager.Instance.GetRemainingOilLaps(), MechanicsManager.Instance._consumableSettings.EngineOil.LapLimit, UIStore.FillOil.Slider);
        UpdateSlider_StoreUI(MechanicsManager.Instance.GetRemainingGasLaps(), MechanicsManager.Instance._consumableSettings.Gas.LapLimit, UIStore.FillGas.Slider);
        UpdateSlider_StoreUI(Constants.StoredCarHealth, Constants.MaxCarHealth, UIStore.FixDamage.Slider);

        //UIStore.FixTyres_Txt.text = "*Remaining playable laps : " + MechanicsManager.Instance.GetRemainingTyreLaps() + " laps" + "\n" + "*Repair Cost : " + MechanicsManager.Instance._consumableSettings.Tyres.VC_Cost + " " + Constants.VirtualCurrency;
        //UIStore.FillOil_Txt.text = "*Remaining playable laps : " + MechanicsManager.Instance.GetRemainingOilLaps() + " laps" + "\n" + "*Repair Cost : " + MechanicsManager.Instance._consumableSettings.EngineOil.VC_Cost + " " + Constants.VirtualCurrency;
        //UIStore.FillGas_Txt.text = "*Remaining playable laps : " + MechanicsManager.Instance.GetRemainingGasLaps() + " laps" + "\n" + "*Repair Cost : " + MechanicsManager.Instance._consumableSettings.Gas.VC_Cost + " " + Constants.VirtualCurrency;
        //UIStore.RepairDamage_Text.text = "*Health : " + Constants.StoredCarHealth + " %" + "\n" + "*Repair Cost : " + MechanicsManager.Instance._consumableSettings.DamageRepair.VC_Cost + " " + Constants.VirtualCurrency;
    }

    public void UpdateSlider_StoreUI(float start, float end, Slider _slid)
    {
        float per = (start / end) * 100;
        _slid.value = per;
    }

    public void BuyVC_StoreUI()
    {
        FirebaseMoralisManager.Instance.PlayerData.VC_Amount += Constants.CCashPurchaseAmount;
        Constants.VirtualCurrencyAmount = FirebaseMoralisManager.Instance.PlayerData.VC_Amount;
        apiRequestHandler.Instance.updatePlayerData();
        MainMenuViewController.Instance.ShowToast(3f, Constants.VirtualCurrency + " was successfully purchased.", true);
        SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
        UpdateBUYVCText_StoreUI(Constants.CCashPurchaseAmount.ToString());
        MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount.ToString());
    }

    public void FixTyre_StoreUI()
    {
        if (Constants.VirtualCurrencyAmount >= MechanicsManager.Instance._consumableSettings.Tyres.VC_Cost)
        {
            _fixType = FixType.Tires;
            ConfirmPurchase(MechanicsManager.Instance._consumableSettings.Tyres.VC_Cost);
        }
        else
        {
            MainMenuViewController.Instance.ShowToast(3f, "You do not have enough " + Constants.VirtualCurrency + " , buy more.", false);
        }
    }

    public void FillEngineOil_StoreUI()
    {
        if (Constants.VirtualCurrencyAmount >= MechanicsManager.Instance._consumableSettings.EngineOil.VC_Cost)
        {
            _fixType = FixType.Oil;
            ConfirmPurchase(MechanicsManager.Instance._consumableSettings.EngineOil.VC_Cost);
        }
        else
        {
            MainMenuViewController.Instance.ShowToast(3f, "You do not have enough " + Constants.VirtualCurrency + " , buy more.", false);
        }
    }

    public void FillGas_StoreUI()
    {
        if (Constants.VirtualCurrencyAmount >= MechanicsManager.Instance._consumableSettings.Gas.VC_Cost)
        {
            _fixType = FixType.Gas;
            ConfirmPurchase(MechanicsManager.Instance._consumableSettings.Gas.VC_Cost);
        }
        else
        {
            MainMenuViewController.Instance.ShowToast(3f, "You do not have enough " + Constants.VirtualCurrency + " , buy more.", false);
        }
    }

    public void RepairDamage_StoreUI()
    {
        if (Constants.VirtualCurrencyAmount >= MechanicsManager.Instance._consumableSettings.DamageRepair.VC_Cost)
        {
            _fixType = FixType.Damage;
            ConfirmPurchase(MechanicsManager.Instance._consumableSettings.DamageRepair.VC_Cost);
        }
        else
        {
            MainMenuViewController.Instance.ShowToast(3f, "You do not have enough " + Constants.VirtualCurrency + " , buy more.", false);
        }
    }

    public void ConfirmPurchase(float _cost)
    {
        UIStore.DialogueConfirmation.MainScreen.SetActive(true);
        UIStore.DialogueConfirmation.Cost_Text.text = "Cost : " + _cost.ToString() + Constants.VirtualCurrency;
    }

    public void OnPurchaseClicked()
    {
        switch (_fixType)
        {
            case FixType.Tires:
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= MechanicsManager.Instance._consumableSettings.Tyres.VC_Cost;
                tempNFTData.mechanicsData.Tyre_Laps = 0;
                FirebaseMoralisManager.Instance.UpdateMechanics(tempNFTID, tempNFTData);
                Constants.VirtualCurrencyAmount = FirebaseMoralisManager.Instance.PlayerData.VC_Amount;
                apiRequestHandler.Instance.updatePlayerData();
                MainMenuViewController.Instance.ShowToast(2f, "Tires were successfully fixed.", true);
                SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
                MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount.ToString());
                UpdateConsumableText_StoreUI();
                UIStore.DialogueConfirmation.MainScreen.SetActive(false);
                break;
            case FixType.Oil:
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= MechanicsManager.Instance._consumableSettings.EngineOil.VC_Cost;
                tempNFTData.mechanicsData.EngineOil_Laps = 0;
                FirebaseMoralisManager.Instance.UpdateMechanics(tempNFTID, tempNFTData);

                Constants.VirtualCurrencyAmount = FirebaseMoralisManager.Instance.PlayerData.VC_Amount;
                apiRequestHandler.Instance.updatePlayerData();
                MainMenuViewController.Instance.ShowToast(3f, "Engine Oil was successfully filled.", true);

                SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
                MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount.ToString());
                UpdateConsumableText_StoreUI();
                UIStore.DialogueConfirmation.MainScreen.SetActive(false);
                break;
            case FixType.Gas:
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= MechanicsManager.Instance._consumableSettings.Gas.VC_Cost;
                tempNFTData.mechanicsData.Gas_Laps = 0;
                FirebaseMoralisManager.Instance.UpdateMechanics(tempNFTID, tempNFTData);

                Constants.VirtualCurrencyAmount = FirebaseMoralisManager.Instance.PlayerData.VC_Amount;
                apiRequestHandler.Instance.updatePlayerData();
                MainMenuViewController.Instance.ShowToast(3f, "Gas was successfully filled.", true);

                SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
                MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount.ToString());
                UpdateConsumableText_StoreUI();
                UIStore.DialogueConfirmation.MainScreen.SetActive(false);
                break;
            case FixType.Damage:
                FirebaseMoralisManager.Instance.PlayerData.VC_Amount -= MechanicsManager.Instance._consumableSettings.DamageRepair.VC_Cost;
                tempNFTData.mechanicsData.CarHealth = 100;
                Constants.StoredCarHealth = tempNFTData.mechanicsData.CarHealth;
                FirebaseMoralisManager.Instance.UpdateMechanics(tempNFTID, tempNFTData);
                Constants.VirtualCurrencyAmount = FirebaseMoralisManager.Instance.PlayerData.VC_Amount;
                apiRequestHandler.Instance.updatePlayerData();
                MainMenuViewController.Instance.ShowToast(3f, "Damage were repaired, car health is now full.", true);
                SetCCashText_StoreUI(Constants.VirtualCurrencyAmount.ToString());
                MainMenuViewController.Instance.UpdateVCText(Constants.VirtualCurrencyAmount.ToString());
                UpdateConsumableText_StoreUI();
                UIStore.DialogueConfirmation.MainScreen.SetActive(false);
                break;
        }
    }

    public void OnCancelClicked()
    {
        UIStore.DialogueConfirmation.MainScreen.SetActive(false);
    }
    #endregion
}
