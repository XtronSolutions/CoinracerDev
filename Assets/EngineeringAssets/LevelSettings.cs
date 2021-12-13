using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/Create level settings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public string LevelName;
    public string SceneName;
    public Sprite Icon;
}
