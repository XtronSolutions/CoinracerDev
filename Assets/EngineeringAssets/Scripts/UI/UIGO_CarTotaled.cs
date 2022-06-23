using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIGO_CarTotaled : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI GO_DisclaimerText;
    [SerializeField] private Button MainMenuButton;

    private void OnEnable()
    {
        MainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void GoToMainMenu()
    {
        if (RaceManager.Instance)
            RaceManager.Instance.OpenMainMenu();
        else
            Debug.LogError("RM instance is null");
    }

    public void ChangeDisclaimerText(string _txt)
    {
        GO_DisclaimerText.text = _txt;
    }
}
