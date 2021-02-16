using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int hp = 10;
    private int gold = 100;

    public Text goldText;
    public Text hpText;

    public ObjectPlacer objectPlacer;

    public SaveLoadManager saveLoadManager;
    public EnemyManager enemyManager;

    public GameObject levelListScrollItemPrefab;
    public GameObject levelListScrollContent;
    public Button levelSelectPanelPlayButton;

    public void Awake()
    {
        UpdateDisplayedGold();
        UpdateDisplayedHP();

        levelListScrollItemPrefab = (GameObject)Resources.Load("Prefabs/UI Elements/LevelListItem");
    }

    private void Start()
    {
        if (GameObject.FindObjectOfType<ObjectPlacer>() != null)
        {
            objectPlacer = GameObject.FindObjectOfType<ObjectPlacer>();
        }

        if (GameObject.FindObjectOfType<SaveLoadManager>() != null)
        {
            saveLoadManager = GameObject.FindObjectOfType<SaveLoadManager>();
        }
        PopulateLevelList();
    }

    public void LoseHp(int h = 1)
    {
        hp -= h;
        if(hp <= 0)
        {
            //TO DO: lose game
        }
        UpdateDisplayedHP();
    }

    public void SetHp(int h = 1)
    {
        hp = h;
        if (hp <= 0)
        {
            //TO DO: lose game
        }
        UpdateDisplayedHP();
    }

    public void GainGold(int g = 1)
    {
        gold += g;
        UpdateDisplayedGold();
    }

    public void SetGold(int g = 1)
    {
        gold = g;
        UpdateDisplayedGold();
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsEnoughGoldForItem(int cost)
    {
        if(gold - cost >=0)
        {
            return true;
        }
        else
        { 
            return false;
        }
    }

    public void PayForItem(int cost)
    {
        gold -= cost;
        UpdateDisplayedGold();
    }

    public void UpdateDisplayedGold()
    {
        goldText.text = "Gold: " + gold.ToString();
    }
    public void UpdateDisplayedHP()
    {
        hpText.text = "HP: " + hp.ToString();
    }

    public void SelectTowerToPurchase(int index)
    {
        objectPlacer.SetSelectedTowerIndex(index);
    }

    private void InitSelectedLevelData()
    {
        if (GameObject.FindObjectOfType<EnemyManager>() != null)
        {
            enemyManager = this.gameObject.AddComponent<EnemyManager>();
        }
        
        saveLoadManager.selectedLevelData.levelName = "Level 1";
        saveLoadManager.LoadLevel();

        enemyManager.enemyWaves.Clear();
        for (int i = 0; i < saveLoadManager.selectedLevelData.enemyList.Count; i++)
        {
            EnemyData enemies = new EnemyData();
            enemies.quantity = saveLoadManager.selectedLevelData.enemyList[i].quantity;
            enemies.enemyType = saveLoadManager.selectedLevelData.enemyList[i].enemyType;
            enemies.pathIndex = saveLoadManager.selectedLevelData.enemyList[i].pathIndex;
            enemies.waveIndex = saveLoadManager.selectedLevelData.enemyList[i].waveIndex;
            enemyManager.enemyWaves.Add(enemies);
        }

        if(GameObject.Find("Paths") != null)
        {
            Destroy(GameObject.Find("Paths"));
        }
        GameObject paths = new GameObject("Paths");

        for (int i = 0; i < saveLoadManager.selectedLevelData.paths.Count; i++)
        {
            GameObject path = new GameObject("Path");
            path.transform.parent = paths.transform;
            for (int j = 0; j < saveLoadManager.selectedLevelData.paths[i].Count; j++)
            {
                objectPlacer.PlacePathNodeOnTerrain(saveLoadManager.selectedLevelData.paths[i][j], objectPlacer.invisiblePathNodePrefab, paths, i + 1); //TO DO: dont spawn a new object every time
            }
        }

         
        SetGold(saveLoadManager.selectedLevelData.startingGold);
        SetHp(saveLoadManager.selectedLevelData.startingHP);

        enemyManager.currentWaveIndex = 0;
        enemyManager.nrOfWaves = saveLoadManager.selectedLevelData.nrOfWaves;

        SelectMapToPlace(saveLoadManager.selectedLevelData.mapNumber);

        for (int i = 0; i < saveLoadManager.selectedLevelData.tiles.Count; i++)
        {
            objectPlacer.grid.SetTileAvailability(i, saveLoadManager.selectedLevelData.tiles[i].Availability);
        }
    }

    public void LoadLevelToPlay()
    {
        saveLoadManager.LoadLevel();
        //ResetLevel();
        InitSelectedLevelData();
    }

    public void SelectMapToPlace(int index)
    {
        objectPlacer.SetSelectedMapIndex(index);

        if (GameObject.FindObjectOfType<CameraManager>())
        {
            Destroy(GameObject.FindObjectOfType<CameraManager>());
        }

        if (index == 0)
        {
        }
        else
        {
            this.gameObject.AddComponent<CameraManager>();
        }
    }

    /// <summary>
    /// Instantiates a new interactable item in the Level List
    /// </summary>
    /// <param name="_amount">nr of selected enemies added to queue</param>
    /// <param name="_type">type of enemy added to queue</param>
    /// <param name="_path">the path the enemies will follow</param>
    /// <param name="_wave">during which wave the enemies will spawn</param>
    public void AddItemToLevelList(string _name)
    {
        GameObject item = Instantiate(levelListScrollItemPrefab);
        item.transform.SetParent(levelListScrollContent.transform, false);
        item.transform.GetComponent<Button>().transform.GetChild(0).GetComponent<Text>().text = $"{_name}";
        item.transform.GetComponent<Button>().onClick.AddListener(() => OnLevelItemPressed(_name));
    }

    /// <summary>
    /// Sets the name of the selected level in the saveloadmanager; enables the edit selected level button; Listener for the procedurally generated items in the Level List of the Level Editor
    /// </summary>
    /// <param name="keyIndex">index of item in the Enemy List</param>
    private void OnLevelItemPressed(string _name)
    {
        levelSelectPanelPlayButton.interactable = true;
        saveLoadManager.selectedLevelData.levelName = _name;
    }

    /// <summary>
    /// Populates the Level List from the Level Editor with the levels; This method should be called when the list changes
    /// </summary>
    public void PopulateLevelList()
    {
        saveLoadManager.ReadGameData();
        foreach (string levelName in saveLoadManager.gameData.levelNames)
        {
            AddItemToLevelList(levelName);
        }
    }

    public void NextWaveButton()
    {
        if (enemyManager.currentWaveIndex + 1 < enemyManager.nrOfWaves)
        {
            enemyManager.currentWaveIndex += 1;
        }
    }
}
