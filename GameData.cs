using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<string> levelNames;
    public int currentlyUnlockedLevel;

    public GameData(int _currentlyUnlockedLevel = 1)
    {
        currentlyUnlockedLevel = _currentlyUnlockedLevel;
        levelNames = new List<string>();
    }
}