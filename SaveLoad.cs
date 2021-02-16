using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public static class SaveLoad
{
    /// <summary>
    /// Save the levelData in a save file named after the levelName; The gameData is also updated with the new levelName in the list of levels if it is a new level(name)
    /// </summary>
    /// <param name="_saveLevel"></param>
    public static void Save(LevelData _saveLevel)
    {
        CreateSaveDirectory();

        GameData gameData = ReadGameDataFile();

        if(gameData == null)
        {
            gameData = new GameData();
        }

        if(!CheckIfLevelNameExists(gameData, _saveLevel.levelName))
        {
            gameData.levelNames.Add(_saveLevel.levelName.ToString());

            WriteGameDataFile(gameData);
        }

        BinaryFormatter bf = new BinaryFormatter();

        SurrogateSelector ss = new SurrogateSelector();

        Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
        TileDataSerializationSurrogate tdss = new TileDataSerializationSurrogate();
        EnemyDataSerializationSurrogate edss = new EnemyDataSerializationSurrogate();

        ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);
        ss.AddSurrogate(typeof(TileData), new StreamingContext(StreamingContextStates.All), tdss);
        ss.AddSurrogate(typeof(EnemyData), new StreamingContext(StreamingContextStates.All), edss);

        bf.SurrogateSelector = ss;

        FileStream file = File.Create($"{Application.dataPath}/SavedLevels/{_saveLevel.levelName}.sav");
        bf.Serialize(file, _saveLevel);
        file.Close();
        Debug.Log("Saved Game: " + _saveLevel.levelName);

    }

    /// <summary>
    /// Load levelData from save file if the file with the specified level name exists
    /// </summary>
    /// <param name="_levelToLoad"></param>
    /// <returns></returns>
    public static LevelData Load(string _levelToLoad)
    {
        if (File.Exists($"{Application.dataPath}/SavedLevels/{_levelToLoad}.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();

            SurrogateSelector ss = new SurrogateSelector();

            Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
            TileDataSerializationSurrogate tdss = new TileDataSerializationSurrogate();
            EnemyDataSerializationSurrogate edss = new EnemyDataSerializationSurrogate();

            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);
            ss.AddSurrogate(typeof(TileData), new StreamingContext(StreamingContextStates.All), tdss);
            ss.AddSurrogate(typeof(EnemyData), new StreamingContext(StreamingContextStates.All), edss);

            bf.SurrogateSelector = ss;

            FileStream file = File.Open($"{Application.dataPath}/SavedLevels/{_levelToLoad}.sav", FileMode.Open);
            LevelData loadedLevel = (LevelData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Loaded Game: " + loadedLevel.levelName);
            return loadedLevel;
        }
        else
        {
            Debug.Log("Level doesn't exist!");
            return null;
        }

    }

    /// <summary>
    /// Delete level save file by name; Updates the list of level in the GameData file
    /// </summary>
    /// <param name="_levelName"></param>
    public static void Delete(string _levelName)
    {
        GameData gameData = ReadGameDataFile();

        if (gameData != null)
        {
            gameData.levelNames.Remove(_levelName);
            if (gameData.currentlyUnlockedLevel > gameData.levelNames.Count)
            {
                gameData.currentlyUnlockedLevel = gameData.levelNames.Count;
            }
            WriteGameDataFile(gameData);
        }

        if (File.Exists($"{Application.dataPath}/SavedLevels/{_levelName}.sav"))
        {
            File.Delete($"{Application.dataPath}/SavedLevels/{_levelName}.sav");
        }
    }

    /// <summary>
    /// Unlocks first X levels from the level list
    /// </summary>
    /// <param name="_levelIndex"></param>
    public static void UnlockLevel(int _levelIndex)
    {
        GameData gameData = ReadGameDataFile();

        if (gameData == null)
        {
            gameData = new GameData();
        }

        if (gameData.currentlyUnlockedLevel < _levelIndex)
        {
            gameData.currentlyUnlockedLevel = _levelIndex;
        }

        if (gameData.currentlyUnlockedLevel > gameData.levelNames.Count)
        {
            gameData.currentlyUnlockedLevel = gameData.levelNames.Count;
        }

        WriteGameDataFile(gameData);
    }

    /// <summary>
    /// Resets unlocked levels; only useful for testing purposes
    /// </summary>
    public static void ResetUnlockedLevel()
    {
        GameData gameData = ReadGameDataFile();

        if (gameData == null)
        {
            gameData = new GameData();
        }

        gameData.currentlyUnlockedLevel = 1;

        WriteGameDataFile(gameData);
    }

    /// <summary>
    /// Creates / updates a file that contains a list of all saved levels and stores the currently unlocked level
    /// </summary>
    /// <param name="_gameData"></param>
    public static void WriteGameDataFile(GameData _gameData)
    {
        CreateSaveDirectory();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.dataPath}/SavedLevels/GameData.dat");
        //_gameData.levelNames.Sort(); //sorts the levels by name
        bf.Serialize(file, _gameData);
        file.Close();
        Debug.Log("GameData file was written");
    }

    /// <summary>
    /// Read the file that contains the list of saved levels and the currenty unlocked level
    /// </summary>
    /// <returns></returns>
    public static GameData ReadGameDataFile()
    {
        if (File.Exists($"{Application.dataPath}/SavedLevels/GameData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.dataPath}/SavedLevels/GameData.dat", FileMode.Open);
            GameData loadedData = (GameData)bf.Deserialize(file);
            file.Close();
            Debug.Log("GameData file was read");
            return loadedData;
        }
        else
        {
            Debug.Log("GameData file couldn't be read because it doesn't exist!");
            return null;
        }
    }

    /// <summary>
    /// Checks if the specified levelName already exists in the list of saved levels
    /// </summary>
    /// <param name="_gameData"></param>
    /// <param name="_levelName"></param>
    /// <returns></returns>
    public static bool CheckIfLevelNameExists(GameData _gameData, string _levelName)
    {
        return _gameData.levelNames.Contains(_levelName.ToString());
    }

    /// <summary>
    /// Creates the save directory if it doesn't already exist to ensure that the save path is valid
    /// </summary>
    public static void CreateSaveDirectory()
    {
        if (!Directory.Exists($"{Application.dataPath}/SavedLevels"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/SavedLevels");
        }
    }
}