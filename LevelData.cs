using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int mapNumber;
    public List<EnemyData> enemyList = new List<EnemyData>();
    public List<List<Vector3>> paths = new List<List<Vector3>>();
    public int nrOfWaves;
    public List<TileData> tiles = new List<TileData>();
    public int startingGold;
    public int startingHP;
    public string levelName;

    public LevelData()
    {

    }

    public LevelData(string _levelName, int _startingHP, int _startingGold, int _nrOfWaves, List<EnemyData> _enemyList, List<List<Vector3>> _paths, List<TileData> _tiles, int _mapNumber)
    {
        levelName = _levelName;
        enemyList = _enemyList;
        paths = _paths;
        startingGold = _startingGold;
        startingHP = _startingHP;
        nrOfWaves = _nrOfWaves;
        tiles = _tiles;
        mapNumber = _mapNumber;
    }
}