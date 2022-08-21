using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public enum DealerScreenState
{
    MAINSTORE=1,
    ATM=2,
    BuyCar=3,
    CARTIER=4,
    CARTYPE=5
}

[Serializable]
public class DealerStatsUI
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Percentage;
    public Slider slider;
}

[Serializable]
public class DealerCar
{
    public GameObject MainScreen;
    public GameObject DealerScreen;
    public GameObject ConsumablesScreen;
    public GameObject BottomContainer_ATM;
    public GameObject BottomContainer_CarTier;
    public GameObject BottomContainer_CarType;
    public GameObject TopContainer_CarType;
    public GameObject CarSelectionContainer;
    public GameObject CarSelection3DContainer;

    public GameObject MainBG;
    public GameObject SecondaryBG;

    public Button ATMButton;
    public Button BuyCarButton;

    public Button TopTierButton;
    public Button MediumTierButton;
    public Button LowTierButton;

    public Button AllCarsButton;
    public Button DestructionDerbyButton;
    public Button RacetrackButton;
    public Button RallyButton;

    public DealerStatsUI AccelerationUI;
    public DealerStatsUI TopSpeedUI;
    public DealerStatsUI CorneringUI;
    public DealerStatsUI HPUI;

    public TextMeshProUGUI CarNameText;
    public Button NextButton;
    public Button PrevButton;
    public Button BackButton;

    public Transform MiddleCar;
    public Transform LeftCar;
    public Transform RightCar;

    public Animator MiddleCarAnimator;
    public GameObject MiddleCarLoader;
    public GameObject LeftCarLoader;
    public GameObject RightCarLoader;
    public CarSelection CarBolt;
    public TextMeshProUGUI CarName_Text;
}
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
    public DealerCar CarDealer;
    private DealerScreenState _state;
    private CarTier _tierSelected;
    private CarType _typeSelected;

    private FixType _fixType;
    private void OnEnable()
    {
        Instance = this;
        ButtonListeners_StoreUI();
        DealerButtonListeners();
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
        //UIStore.BackButton.onClick.AddListener(DisableStore_StoreUI);
        UIStore.FixTires.PayButton.onClick.AddListener(FixTyre_StoreUI);
        //UIStore.BuyVC.PayButton.onClick.AddListener(BuyVC_StoreUI);
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
        //UIStore.BuyVC.PayButton.gameObject.SetActive(true);

        ToggleDealer(true, false);
        ToggleBG_Dealer(true, false);
        ToggleBottomContainer_ATM(true);
        ToggleBottomContainer_CarTier(false);
        ToggleBottomContainer_CarType(false);
        _state = DealerScreenState.MAINSTORE;

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
        //UIStore.BuyVC.PayButton.gameObject.SetActive(false);
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

    #region Dealer
    public void ToggleBG_Dealer(bool stat1, bool stat2)
    {
        CarDealer.MainBG.SetActive(stat1);
        CarDealer.SecondaryBG.SetActive(stat2);
    }
    public void ToggleBottomContainer_ATM(bool state)
    {
        CarDealer.BottomContainer_ATM.SetActive(state);
    }
    public void ToggleBottomContainer_CarTier(bool state)
    {
        CarDealer.BottomContainer_CarTier.SetActive(state);
    }
    public void ToggleBottomContainer_CarType(bool state)
    {
        CarDealer.BottomContainer_CarType.SetActive(state);
    }
    public void ToggleTopContainer_CarType(bool state)
    {
        CarDealer.TopContainer_CarType.SetActive(state);
    }
    public void TogglerCarSelection(bool state)
    {
        CarDealer.CarSelectionContainer.SetActive(state);
    }
    public void TogglerCarSelection3D(bool state)
    {
        CarDealer.CarSelection3DContainer.SetActive(state);
    }
    public void ToggleDealer(bool stat1, bool stat2)
    {
        CarDealer.DealerScreen.SetActive(stat1);
        CarDealer.ConsumablesScreen.SetActive(stat2);
    }
    public void DealerButtonListeners()
    {
        //CarDealer.ATMButton.onClick.AddListener(EnableStore_StoreUI);
        CarDealer.BuyCarButton.onClick.AddListener(OnBuyCarClicked_Dealer);
        CarDealer.BackButton.onClick.AddListener(OnBackButtonClicked_Dealer);
        CarDealer.TopTierButton.onClick.AddListener(OnTopTierClicked_Dealer);
        CarDealer.MediumTierButton.onClick.AddListener(OnMedTierClicked_Dealer);
        CarDealer.LowTierButton.onClick.AddListener(OnLowTierClicked_Dealer);

        CarDealer.NextButton.onClick.AddListener(OnNextCarClicked_Store);
        CarDealer.PrevButton.onClick.AddListener(OnPrevCarClicked_Store);

        CarDealer.AllCarsButton.onClick.AddListener(OnAllCarButtonClicked);
        CarDealer.DestructionDerbyButton.onClick.AddListener(OnDestructionDerbyButtonClicked);
        CarDealer.RacetrackButton.onClick.AddListener(OnRaceTrackButtonClicked);
        CarDealer.RallyButton.onClick.AddListener(OnRallyButtonClicked);
    }
    public void OnBackButtonClicked_Dealer()
    {
        MainMenuViewController.Instance.PlayButtonDownAudioClip();
        switch (_state)
        {
            case DealerScreenState.MAINSTORE:
                ToggleDealer(false, false);
                ToggleBG_Dealer(true, false);
                ToggleBottomContainer_ATM(false);
                ToggleBottomContainer_CarTier(false);
                ToggleBottomContainer_CarType(false);

                DisableStore_StoreUI();
                break;
            case DealerScreenState.ATM:

                break;
            case DealerScreenState.BuyCar:
                ToggleBottomContainer_ATM(true);
                ToggleBottomContainer_CarTier(false);
                _state = DealerScreenState.MAINSTORE;
                break;
            case DealerScreenState.CARTIER:
                ToggleBG_Dealer(true, false);
                ToggleCarBuySelection(false);
                _state = DealerScreenState.BuyCar;
                MainMenuViewController.Instance.ResetSelectedCarStore();
                break;
            case DealerScreenState.CARTYPE:
                break;
            default:
                break;
        }
    }
    public void OnBuyCarClicked_Dealer()
    {
        MainMenuViewController.Instance.PlayButtonDownAudioClip();
        _state = DealerScreenState.BuyCar;

        ToggleBottomContainer_ATM(false);
        ToggleBottomContainer_CarTier(true);
    }
    public void OnTopTierClicked_Dealer()
    {
        MainMenuViewController.Instance.PlayButtonDownAudioClip();
        _state = DealerScreenState.CARTIER;
        _tierSelected = CarTier.Top;
        ShowCarDeals();
    }
    public void OnMedTierClicked_Dealer()
    {
        MainMenuViewController.Instance.PlayButtonDownAudioClip();
        _state = DealerScreenState.CARTIER;
        _tierSelected = CarTier.Medium;
        ShowCarDeals();
    }
    public void OnLowTierClicked_Dealer()
    {
        MainMenuViewController.Instance.PlayButtonDownAudioClip();
        _state = DealerScreenState.CARTIER;
        _tierSelected = CarTier.Low;
        ShowCarDeals();
    }
    public void ToggleCarBuySelection(bool stat1)
    {
        ToggleBottomContainer_CarTier(!stat1);
        ToggleBottomContainer_CarType(stat1);
        ToggleTopContainer_CarType(stat1);
        TogglerCarSelection(stat1);
        TogglerCarSelection3D(stat1);
    }
    public void ShowCarDeals()
    {
        ToggleBG_Dealer(false, true);
        _typeSelected = CarType.AllCar;
        ToggleCarBuySelection(true);
        ToggleLoadersStore(false, false, false);
        MainMenuViewController.Instance.ResetSelectedCarStore();
        InstantiateStoreCars();
    }
    public void ChangeCarName(string txt, TextMeshProUGUI TextRef)
    {
        TextRef.text = txt;
    }
    public void ChangeCarID(string txt, TextMeshProUGUI TextRef)
    {
        TextRef.text = txt;
    }
    public void ToggleLoadersStore(bool state1 = true, bool state2 = true, bool state3 = true)
    {
        CarDealer.MiddleCarLoader.SetActive(state1);
        CarDealer.LeftCarLoader.SetActive(state2);
        CarDealer.RightCarLoader.SetActive(state3);
    }
    public void InstantiateStoreCars()
    {
        foreach (var doc in FirebaseMoralisManager.Instance.CarDealer)
        {
            if (doc.Key == (int)_tierSelected)
            {
                for (int i = 0; i < doc.Value.Count; i++)
                {
                    if (doc.Value[i].CarStats.Type == _typeSelected || _typeSelected==CarType.AllCar)
                    {
                        
                        for (int l = 0; l < NFTGameplayManager.Instance.DataNFTModel.Count; l++)
                        {
                            if (doc.Value[i].CarStats.ID == NFTGameplayManager.Instance.DataNFTModel[l].MetaID)
                            {
                                MainMenuViewController.Instance.AssignStoreGarageData(NFTGameplayManager.Instance.DataNFTModel[l].CarSelection.gameObject, doc.Value[i].CarStats.ID, doc.Value[i].CarStats.Name, doc.Value[i], CarDealer.CarSelection3DContainer.transform, false, true);
                                break;
                            }
                        }
                    }
                }
            }
        }

            MainMenuViewController.Instance.LoadingScreen.SetActive(false);
            MainMenuViewController.Instance.AssignStoreGarageCars(CarDealer.MiddleCar, CarDealer.LeftCar, CarDealer.RightCar, CarDealer.CarSelection3DContainer.transform, CarDealer.CarName_Text, null, false,true);
    }
    public void OnNextCarClicked_Store()
    {
        MainMenuViewController.Instance.OnNextButtonClicked();
        MainMenuViewController.Instance.AssignStoreGarageCars(CarDealer.MiddleCar, CarDealer.LeftCar, CarDealer.RightCar, CarDealer.CarSelection3DContainer.transform, CarDealer.CarName_Text, null, false,true);
    }
    public void OnPrevCarClicked_Store()
    {
        MainMenuViewController.Instance.OnPrevButtonClicked();
        MainMenuViewController.Instance.AssignStoreGarageCars(CarDealer.MiddleCar, CarDealer.LeftCar, CarDealer.RightCar, CarDealer.CarSelection3DContainer.transform, CarDealer.CarName_Text, null, false,true);
    }
    public void UpdateCarStats(StatSettings _settings)
    {
        double Acc = _settings.CarStats.Acceleration;
        double Speed = _settings.CarStats.TopSpeed;
        double Cornering = _settings.CarStats.Cornering;
        double HP = _settings.CarStats.HP;

        UpdateStatsUI(CarDealer.AccelerationUI.Percentage, Acc, CarDealer.AccelerationUI.slider);
        UpdateStatsUI(CarDealer.TopSpeedUI.Percentage, Speed, CarDealer.TopSpeedUI.slider);
        UpdateStatsUI(CarDealer.CorneringUI.Percentage, Cornering, CarDealer.CorneringUI.slider);
        UpdateStatsUI(CarDealer.HPUI.Percentage, HP, CarDealer.HPUI.slider);
    }
    public void UpdateStatsUI(TextMeshProUGUI PerText,double Value,Slider slider)
    {
        PerText.text = Value.ToString()+"%";
        slider.maxValue = 100;

        if (Value > 100)
            slider.maxValue = (float)Value;

        OnSliderValueChanged(slider, Value);
    }
    public void OnSliderValueChanged(Slider slider,double value)
    {
        slider.value = (float)value;
    }
    public void OnAllCarButtonClicked()
    {
        if (_typeSelected != CarType.AllCar)
        {
            _typeSelected = CarType.AllCar;
            MainMenuViewController.Instance.ResetSelectedCarStore();
            InstantiateStoreCars();
        }
    }
    public void OnDestructionDerbyButtonClicked()
    {
        if (_typeSelected != CarType.DestructionDerby)
        {
            _typeSelected = CarType.DestructionDerby;
            MainMenuViewController.Instance.ResetSelectedCarStore();
            InstantiateStoreCars();
        }
    }
    public void OnRaceTrackButtonClicked()
    {
        if (_typeSelected != CarType.RaceTrack)
        {
            _typeSelected = CarType.RaceTrack;
            MainMenuViewController.Instance.ResetSelectedCarStore();
            InstantiateStoreCars();
        }
    }
    public void OnRallyButtonClicked()
    {
        if (_typeSelected != CarType.Rally)
        {
            _typeSelected = CarType.Rally;
            MainMenuViewController.Instance.ResetSelectedCarStore();
            InstantiateStoreCars();
        }
    }
    #endregion
}
