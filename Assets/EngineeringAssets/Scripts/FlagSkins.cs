using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSkins : MonoBehaviour
{
    public static FlagSkins Instance;
    public Sprite[] FlagSprites;

    private void OnEnable()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Sprite FlagSpriteWithIndex(int Index)
    {
        return FlagSprites[Index];
    }
}
