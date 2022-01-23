using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this function will be called whenver user points on this game object
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if this button is a game mode button
        if (gameObject.tag == "GameModeButton")
            AnimationsHandler.Instance.onGameModePointerEnter(gameObject);
        //if this button is not a game mode button
        else
            AnimationsHandler.Instance.scaleUpButton(gameObject);
    }

    //this function will be called when user perform pointerexit event
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.tag == "GameModeButton")
            AnimationsHandler.Instance.onGameModePointerExit(gameObject);
        else
            AnimationsHandler.Instance.scaleDownButton(gameObject);
    }
}
