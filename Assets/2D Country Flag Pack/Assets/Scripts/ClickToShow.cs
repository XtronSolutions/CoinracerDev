using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickToShow : MonoBehaviour
{
    public TextMeshProUGUI countryText;     // TextMeshPro variable in the inspector
    public GameObject countryTextpanel;     // Gameobject variable in the inspector

    private void Start()
    {
        countryTextpanel.SetActive(false);     // Sets gameobject ”CountryTexPanel” inactive
    }
    public void ChangeText()
    {
        StartCoroutine(Delay());    // Starts coroutine ”Delay”
    }
    private IEnumerator Delay()
    {
        countryTextpanel.SetActive(true);      // Sets gameobject ”CountryTextPanel” active
        countryText.text = gameObject.transform.name;   // Name of the ”CountryText” equals flagobject name
        yield return new WaitForSeconds(1f);    // Waits 1 second (”CountryTexPanel” stays active 1 second)
        countryTextpanel.SetActive(false);     // Sets gameobject ”CountryTexPanel” inactive
    }
}
