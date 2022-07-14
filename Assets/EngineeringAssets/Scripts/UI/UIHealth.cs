using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private Image BackgroundImage;
    [SerializeField] private Image FillBar;
    [SerializeField] private TextMeshProUGUI HealthText;

    public static UIHealth Instance;

    private Color Red = Color.red;
    private Color Green = Color.green;

    public bool IsMiniBar=false;

    private void OnEnable()
    {
        if (!IsMiniBar)
        {
            Instance = this;
            UpdateHealth(Constants.StoredCarHealth);
        }
    }

    private void OnDisable()
    {
        Instance = null;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetBG_Health(Sprite _sp)
    {
        BackgroundImage.sprite = _sp;
    }

    public void SetFill_Health(Sprite _sp)
    {
        FillBar.sprite = _sp;
    }

    public void SetText_Health(string _txt)
    {
        HealthText.text = _txt;
    }

    public void UpdateHealth(float _health)
    {
        HealthText.text = _health.ToString();
        FillBar.fillAmount = _health / Constants.MaxCarHealth;

        if (_health > 20)
            FillBar.color = Green;
        else
            FillBar.color = Red;
    }
}
