using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationHandler : MonoBehaviour
{
    public Sprite[] AnimationSprites;
    private Image ImageRef;
    public float speed = 0.01f;

    private void OnEnable()
    {
        ImageRef = GetComponent<Image>();
        StartCoroutine(AnimateSprites());
    }

   public IEnumerator AnimateSprites()
   {
        for (int i = 0; i < AnimationSprites.Length; i++)
        {
            ImageRef.sprite = AnimationSprites[i];
            yield return new WaitForSeconds(speed);
        }

        StartCoroutine(AnimateSprites());
    }
}
