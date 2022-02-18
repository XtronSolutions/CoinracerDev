using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class ImageAnimation : MonoBehaviour
{
    #region PUBLIC_VARS
    public Sprite[] sprites;
    public float speed = 50f;
    #endregion

    #region PRIVATE_VARS
    private float time = 0;
    private int index = 0;
    private Image image;
    #endregion

    #region UNITY_CALLBACKS
    private void Start()
    {
        if (GetComponent<Image>() != null)
        {
            image = GetComponent<Image>();
            image.sprite = sprites[index];
        }
    }

    private void Update()
    {
        time += Time.unscaledDeltaTime * speed;
        if (time >= 1)
        {
            index++;
            if (index == sprites.Length)
            {
                index = 0;
            }
            image.sprite = sprites[index];
            time = 0;
        }
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    #endregion

    #region PRIVATE_FUNCTIONS
    #endregion

    #region CO-ROUTINES
    #endregion

    #region EVENT_HANDLERS
    #endregion

    #region UI_CALLBACKS
    #endregion
}
