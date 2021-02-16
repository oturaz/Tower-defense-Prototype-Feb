using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public LevelData selectedLevelData = new LevelData();
    public GameData gameData;
    public int levelToBeUnlocked;

    [ContextMenu("Save")]
    public void SaveLevel()
    {
        LevelData newSaveGame = new LevelData();
        newSaveGame.mapNumber    = selectedLevelData.mapNumber;
        newSaveGame.enemyList    = selectedLevelData.enemyList;
        newSaveGame.paths        = selectedLevelData.paths;
        newSaveGame.nrOfWaves    = selectedLevelData.nrOfWaves;
        newSaveGame.tiles        = selectedLevelData.tiles;
        newSaveGame.startingGold = selectedLevelData.startingGold;
        newSaveGame.startingHP   = selectedLevelData.startingHP;
        newSaveGame.levelName    = selectedLevelData.levelName;

        SaveLoad.Save(newSaveGame);
        ReadGameData();
    }

    [ContextMenu("Load")]
    public void LoadLevel()
    {
        LevelData loadedGame = SaveLoad.Load(selectedLevelData.levelName);
        if (loadedGame != null)
        {
            selectedLevelData.mapNumber    = loadedGame.mapNumber;
            selectedLevelData.enemyList    = loadedGame.enemyList;
            selectedLevelData.paths        = loadedGame.paths;
            selectedLevelData.nrOfWaves    = loadedGame.nrOfWaves;
            selectedLevelData.tiles        = loadedGame.tiles;
            selectedLevelData.startingGold = loadedGame.startingGold;
            selectedLevelData.startingHP   = loadedGame.startingHP;
            selectedLevelData.levelName    = loadedGame.levelName;
        }
    }

    [ContextMenu("Delete")]
    public void DeleteLevel()
    {
        SaveLoad.Delete(selectedLevelData.levelName);
        ReadGameData();
    }

    [ContextMenu("Unlock Level")]
    public void UnlockLevel()
    {
        SaveLoad.UnlockLevel(levelToBeUnlocked);
        ReadGameData();
    }

    [ContextMenu("Read Game Data")]
    public void ReadGameData()
    {
        gameData = SaveLoad.ReadGameDataFile();
    }
}
