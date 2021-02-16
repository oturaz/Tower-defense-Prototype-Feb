using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorManager : MonoBehaviour
{
    public GameObject pathParent;

    public int nrOfWaves;

    public int selectedMapIndex;
    public int selectedAmount; //of enemies to be added
    public int selectedTypeIndex;
    public int selectedPathIndex;
    public int selectedWaveIndex;
    public int selectedEnemyListItemIndex;

    public Dropdown dropdownAmountSelector;
    public Dropdown dropdownPathSelector;
    public Dropdown dropdownTypeSelector;
    public Dropdown dropdownWaveSelector;
    public Dropdown dropdownSelectMap;

    public Button enemyListPanelAddToQueueButton;
    public Button enemyListPanelRemoveFromQueueButton;
    public Button saveLevelPanelDoneButton;
    public Button createLevelPanelNextButton;
    public Button editLevelPanelSaveButton;
    public Button selectLevelPanelEditLevelButton;
    public Button selectLevelPanelDeleteLevelButton;

    public InputField levelNameInputField;
    public InputField startingGoldInputField;
    public InputField startingHPInputField;

    public GameObject enemyListScrollContent;
    public GameObject enemyListScrollItemPrefab;
    public GameObject levelListScrollContent;
    public GameObject levelListScrollItemPrefab;

    public ObjectPlacer objectPlacer;

    public List<EnemyData> enemyQueue = new List<EnemyData>();

    public SaveLoadManager saveLoadManager;

    private void Awake()
    {
        nrOfWaves = 0;
        selectedAmount = 1;

        selectedPathIndex = 0;
        selectedTypeIndex = 0;
        selectedWaveIndex = 0;

        if(GameObject.Find("Paths") != null)
        {
            pathParent = GameObject.Find("Paths");
        }
        else
        {
            pathParent = new GameObject("Paths");
        }

        enemyListScrollItemPrefab = (GameObject)Resources.Load("Prefabs/UI Elements/EnemyListItem");
        levelListScrollItemPrefab = (GameObject)Resources.Load("Prefabs/UI Elements/LevelListItem");
    }

    void Start()
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

    /// <summary>
    /// Sets the amount of enemies in the cluster to be added to enemyQueue; Call this when the corresponding dropdown changes value
    /// </summary>
    public void SetSelectedAmount()
    {
        selectedAmount = int.Parse(dropdownAmountSelector.options[dropdownAmountSelector.value].text);
    }

    /// <summary>
    /// Sets the type of enemies in the cluster to be added to enemyQueue; Call this when the corresponding dropdown changes value
    /// </summary>
    public void SetSelectedTypeIndex(int index)
    {
        selectedTypeIndex = index;
        UpdateAddEnemyButtonAvailability();
    }

    /// <summary>
    /// Sets the path the enemies in the cluster that will be added to enemyQueue will follow; Call this when the corresponding dropdown changes value
    /// </summary>
    public void SetSelectedPathIndex(int index)
    {
        selectedPathIndex = index;
        UpdateAddEnemyButtonAvailability();
    }

    /// <summary>
    /// Sets the wave during which the enemies in the cluster that will be added to enemyQueue will spawn; Call this when the corresponding dropdown changes value
    /// </summary>
    public void SetSelectedWaveIndex(int index)
    {
        selectedWaveIndex = index;
        UpdateAddEnemyButtonAvailability();
    }

    /// <summary>
    /// Creates a new path and enters path editing mode; Updates the path dropdown
    /// </summary>
    public void CreateNewPath()
    {
        objectPlacer.EnablePathPlacementMode();

        GameObject go = new GameObject("Path");
        go.transform.parent = pathParent.transform;

        SetSelectedPathIndex(pathParent.transform.childCount);
        dropdownPathSelector.options.Insert(selectedPathIndex,new Dropdown.OptionData() { text = "Path " + (selectedPathIndex) });
        dropdownPathSelector.value = selectedPathIndex;
        dropdownPathSelector.RefreshShownValue();
    }

    /// <summary>
    /// Exits path editing mode; If the path does not have at least 2 nodes, it is not saved
    /// </summary>
    public void DoneCreatingPath()
    {
        objectPlacer.DisablePathPlacementMode();

        if(selectedPathIndex > 0)
        {
            if (pathParent.transform.GetChild(selectedPathIndex - 1).childCount < 2)
            {
                DeleteSelectedPath();
            }
        }
    }

    /// <summary>
    /// Deletes the selected path from the dropdown list, deletes all enemies that were going to follow that path, reduces the count of all path names everywhere by 1
    /// </summary>
    public void DeleteSelectedPath()
    {
        if(selectedPathIndex > 0)
        {
            GameObject go = pathParent.transform.GetChild(selectedPathIndex - 1).gameObject;
            go.transform.parent = null;
            Destroy(go);
            dropdownPathSelector.options.RemoveAt(selectedPathIndex);
            int _tempCount = 0;
            List<string> options = new List<string>();
            foreach (Dropdown.OptionData o in dropdownPathSelector.options)
            {
                if(_tempCount < selectedPathIndex)
                {
                    options.Add(o.text.ToString());
                }
                else
                {
                    options.Add("Path "+_tempCount); //update the names from the dropdown
                }
                _tempCount++;
            }
            dropdownPathSelector.ClearOptions();
            dropdownPathSelector.AddOptions(options);
            RemoveEnemiesByPathIndex(selectedPathIndex - 1);

            selectedPathIndex = 0;
            dropdownPathSelector.value = selectedPathIndex;
            dropdownPathSelector.RefreshShownValue();
        }
    }

    /// <summary>
    /// Creates a new wave in the dropdown and increases nrOfWaves by 1
    /// </summary>
    public void CreateNewWave()
    {
        nrOfWaves++;
        SetSelectedWaveIndex(nrOfWaves);
        dropdownWaveSelector.options.Insert(selectedWaveIndex, new Dropdown.OptionData() { text = "Wave " + (selectedWaveIndex) });
        dropdownWaveSelector.value = selectedWaveIndex;
        dropdownWaveSelector.RefreshShownValue();
    }

    /// <summary>
    /// Deletes the selected wave from the dropdown list, deletes all enemies that were going to spawn during that wave, decreases nrOfWaves by 1, reduces the count of all wave names everywhere by 1
    /// </summary>
    public void DeleteSelectedWave()
    {
        if (selectedWaveIndex > 0)
        {
            nrOfWaves--;
            dropdownWaveSelector.options.RemoveAt(selectedWaveIndex);
            int _tempCount = 0;
            List<string> options = new List<string>();
            foreach (Dropdown.OptionData o in dropdownWaveSelector.options)
            {
                if (_tempCount < selectedWaveIndex)
                {
                    options.Add(o.text.ToString());
                }
                else
                {
                    options.Add("Wave " + _tempCount); //update the names from the dropdown
                }
                _tempCount++;
            }
            dropdownWaveSelector.ClearOptions();
            dropdownWaveSelector.AddOptions(options);
            RemoveEnemiesByWaveIndex(selectedWaveIndex - 1);

            selectedWaveIndex = 0;
            dropdownWaveSelector.value = selectedWaveIndex;
            dropdownWaveSelector.RefreshShownValue();
        }
    }

    /// <summary>
    /// Call this whenever the parameters of adding enemies might have been changed; Makes the button that adds cluster of enemies to queue (un/)interactable if conditions are met
    /// </summary>
    public void UpdateAddEnemyButtonAvailability()
    {
        if(selectedTypeIndex > 0 && selectedPathIndex > 0 && selectedWaveIndex > 0)
        {
            enemyListPanelAddToQueueButton.interactable = true;
        }
        else
        {
            enemyListPanelAddToQueueButton.interactable = false;
        }
    }

    /// <summary>
    /// Add cluster of enemies in the enemyQueue and Enemy List
    /// </summary>
    public void AddEnemiesToQueue()
    {
        EnemyData enemy = new EnemyData(selectedWaveIndex - 1, selectedTypeIndex - 1, selectedPathIndex - 1, selectedAmount);
        enemyQueue.Add(enemy);

        AddItemToEnemyList(selectedAmount, selectedTypeIndex, selectedPathIndex, selectedWaveIndex);

        EnableDisableSaveLevelButton();
    }

    /// <summary>
    /// Removes the cluster of enemies from the queue at specified index and disables the remove enemy button until a new item is selected from the Enemy List; Call this when you press the remove button
    /// </summary>
    public void RemoveSelectedEnemiesFromQueue()
    {
        RemoveEnemiesByQueueIndex(selectedEnemyListItemIndex - 1);
        enemyListPanelRemoveFromQueueButton.interactable = false;
    }

    /// <summary>
    /// Remove the cluster of enemies in the queue and Enemy List at the specified index
    /// </summary>
    /// <param name="index"></param>
    private void RemoveEnemiesByQueueIndex(int index)
    {
        enemyQueue.RemoveAt(index);
        RemoveItemsFromEnemyList();
        PopulateEnemyList();

        EnableDisableSaveLevelButton();
    }

    /// <summary>
    /// Removes all enemies that were going to spawn during the wave passed in the arguments
    /// </summary>
    /// <param name="_waveIndex">the wave the enemies were going to follow</param>
    public void RemoveEnemiesByWaveIndex(int _waveIndex)
    {
        enemyQueue.RemoveAll(x => x.waveIndex == _waveIndex);

        for (int i = 0; i < enemyQueue.Count; i++)
        {
            if(enemyQueue[i].waveIndex > _waveIndex)
            {
                enemyQueue[i].waveIndex--;
            }
        }
        RemoveItemsFromEnemyList();
        PopulateEnemyList();

        EnableDisableSaveLevelButton();
    }

    /// <summary>
    /// Removes all enemies that were going to follow the path passed in the arguments
    /// </summary>
    /// <param name="_pathIndex">the path the enemies were going to follow</param>
    public void RemoveEnemiesByPathIndex(int _pathIndex)
    {
        enemyQueue.RemoveAll(x => x.pathIndex == _pathIndex);

        for (int i = 0; i < enemyQueue.Count; i++)
        {
            if (enemyQueue[i].pathIndex > _pathIndex)
            {
                enemyQueue[i].pathIndex--;
            }
        }
        RemoveItemsFromEnemyList();
        PopulateEnemyList();

        EnableDisableSaveLevelButton();
    }

    /// <summary>
    /// Instantiates a new interactable item in the Enemy List
    /// </summary>
    /// <param name="_amount">nr of selected enemies added to queue</param>
    /// <param name="_type">type of enemy added to queue</param>
    /// <param name="_path">the path the enemies will follow</param>
    /// <param name="_wave">during which wave the enemies will spawn</param>
    public void AddItemToEnemyList(int _amount, int _type, int _path, int _wave)
    {
        GameObject item = Instantiate(enemyListScrollItemPrefab);
        item.transform.SetParent(enemyListScrollContent.transform, false);
        item.transform.GetComponent<Button>().transform.GetChild(0).GetComponent<Text>().text = $"{_amount}x {dropdownTypeSelector.options[_type].text} on path {_path} during wave {_wave}";
        int temp = enemyListScrollContent.transform.childCount;
        item.transform.GetComponent<Button>().onClick.AddListener(() => OnEnemyItemPressed(temp));
    }

    /// <summary>
    /// Sets the index of the selected item in the Enemy List of the Level Editor; enables the remove selected item button; Listener for the procedurally generated items in the Enemy List of the Level Editor
    /// </summary>
    /// <param name="keyIndex">index of item in the Enemy List</param>
    private void OnEnemyItemPressed(int keyIndex)
    {
        enemyListPanelRemoveFromQueueButton.interactable = true;
        selectedEnemyListItemIndex = keyIndex;
    }

    /// <summary>
    /// Removes all the items from the Enemy List in the Level Editor(does not remove anything from the enemyQueue data)
    /// </summary>
    public void RemoveItemsFromEnemyList()
    {
        for (int i = enemyListScrollContent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject go = enemyListScrollContent.transform.GetChild(i).gameObject;
            go.transform.SetParent(null);
            Destroy(go);
        }
    }

    /// <summary>
    /// Populates the Enemy List from the Level Editor with the enemies in the queue; This method should be called after Removing the items from the Enemy List
    /// </summary>
    public void PopulateEnemyList()
    {
        foreach(EnemyData e in enemyQueue)
        {
            AddItemToEnemyList(e.quantity, e.enemyType + 1, e.pathIndex + 1, e.waveIndex + 1);
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
        selectLevelPanelEditLevelButton.interactable = true;
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

    /// <summary>
    /// Places in the scene the map corresponding to the index given from the map prefabs; Call this when the selectMapDropdown is changed
    /// </summary>
    /// <param name="index"></param>
    public void SelectMapToPlace(int index)
    {
        selectedMapIndex = index;
        objectPlacer.SetSelectedMapIndex(index);

        if (GameObject.FindObjectOfType<CameraManager>())
        {
            Destroy(GameObject.FindObjectOfType<CameraManager>());
        }
        
        if (index == 0)
        {
            createLevelPanelNextButton.interactable = false;
        }
        else
        {
            createLevelPanelNextButton.interactable = true;
            this.gameObject.AddComponent<CameraManager>();
        }
    }

    [ContextMenu("ResetEditor")]
    /// <summary>
    /// Resets all the data in the level editor
    /// </summary>
    public void ResetEditor()
    {
        dropdownSelectMap.value = 0;
        objectPlacer.SetSelectedMapIndex(0);
        for(int i = pathParent.transform.childCount; i >= 0; i--)
        {
            selectedPathIndex = i;
            DeleteSelectedPath();
        }
        for (int i = nrOfWaves; i >= 0; i--)
        {
            selectedWaveIndex = i;
            DeleteSelectedWave();
        }
        dropdownTypeSelector.value = 0;
        dropdownAmountSelector.value = 0;

        EmptyInputFields();
    }

    /// <summary>
    /// Removes negative values; This should be called when the startingGoldInputField changes
    /// </summary>
    /// <param name="txt"></param>
    public void InputFieldGoldValueChanged(string txt)
    {
        if (txt.Length > 0 && txt[0] == '-')
        {
            startingGoldInputField.text = txt.Remove(0, 1);
        }
    }

    /// <summary>
    /// Removes negative values; This should be called when the startingHPInputField changes
    /// </summary>
    /// <param name="txt"></param>
    public void InputFieldHPValueChanged(string txt)
    {
        if (txt.Length > 0 && txt[0] == '-')
        {
            startingHPInputField.text = txt.Remove(0, 1);
        }
    }

    /// <summary>
    /// Makes the button responsible for saving the data interactable if the inputfields from the last step of saving a level have been filled; Called on input field change
    /// </summary>
    public void EnableDoneButton()
    {
        if(levelNameInputField.text != "" && startingGoldInputField.text != "" && startingHPInputField.text != "")
        {
            saveLevelPanelDoneButton.interactable = true;
        }
        else
        {
            DisableDoneButton();
        }
    }

    /// <summary>
    /// Makes the button responsible for saving the data uninteractable
    /// </summary>
    public void DisableDoneButton()
    {
        saveLevelPanelDoneButton.interactable = false;
    }

    /// <summary>
    /// Empty the input fields from the last step of saving a level; this will make the button responsible for saving the data uninteractible
    /// </summary>
    public void EmptyInputFields()
    {
        levelNameInputField.text = "";
        startingGoldInputField.text = "";
        startingHPInputField.text = "";
        DisableDoneButton();
    }

    /// <summary>
    /// Makes the editLevel save buttin (un/)interactable when conditions are met; Call it whenever the conditions might have changed
    /// </summary>
    public void EnableDisableSaveLevelButton()
    {
        if(enemyQueue.Count > 0)
        {
            editLevelPanelSaveButton.interactable = true;
        }
        else
        {
            editLevelPanelSaveButton.interactable = false;
        }
    }

    public void SetSelectedLevelData()
    {
        saveLoadManager.selectedLevelData.levelName = levelNameInputField.text;
        saveLoadManager.selectedLevelData.enemyList.Clear();
        saveLoadManager.selectedLevelData.enemyList.AddRange(enemyQueue);

        List<List<Vector3>> paths = new List<List<Vector3>>();
        for(int i = 0; i < pathParent.transform.childCount; i++)
        {
            List<Vector3> path = new List<Vector3>();
            for(int j = 0; j < pathParent.transform.GetChild(i).childCount; j++)
            {
                path.Add(pathParent.transform.GetChild(i).GetChild(j).transform.position);
            }
            paths.Add(path);
        }
        saveLoadManager.selectedLevelData.paths.Clear();
        saveLoadManager.selectedLevelData.paths.AddRange(paths);

        saveLoadManager.selectedLevelData.startingGold = int.Parse(startingGoldInputField.text);
        saveLoadManager.selectedLevelData.startingHP = int.Parse(startingHPInputField.text);
        saveLoadManager.selectedLevelData.nrOfWaves = nrOfWaves;

        int tilesIARX = (int)(objectPlacer.grid.MapSizeX / objectPlacer.grid.TileSize);
        int tilesIARZ = (int)(objectPlacer.grid.MapSizeZ / objectPlacer.grid.TileSize);
        List<TileData> tiles = new List<TileData>();
        for (int i = 0; i < tilesIARX * tilesIARZ; i++)
        {
            TileData tile = new TileData();
            tile.Availability = objectPlacer.grid.GetTileAvailability(i);
            tile.Height = objectPlacer.grid.GetTileHeight(i);
            tiles.Add(tile);
        }
        saveLoadManager.selectedLevelData.tiles.Clear();
        saveLoadManager.selectedLevelData.tiles.AddRange(tiles);
        saveLoadManager.selectedLevelData.mapNumber = selectedMapIndex;
    }

    public void SaveEditedLevel()
    {
        SetSelectedLevelData();
        ResetEditor();
        saveLoadManager.SaveLevel();
    }

    private void InitSelectedLevelData()
    {
        levelNameInputField.text = saveLoadManager.selectedLevelData.levelName;

        for(int i = 0; i < saveLoadManager.selectedLevelData.enemyList.Count; i++)
        {
            selectedAmount = saveLoadManager.selectedLevelData.enemyList[i].quantity;
            selectedTypeIndex = saveLoadManager.selectedLevelData.enemyList[i].enemyType + 1;
            selectedPathIndex = saveLoadManager.selectedLevelData.enemyList[i].pathIndex + 1;
            selectedWaveIndex = saveLoadManager.selectedLevelData.enemyList[i].waveIndex + 1;
            AddEnemiesToQueue();
        }
        selectedAmount = 1;
        selectedTypeIndex = 0;
        selectedPathIndex = 0;
        selectedWaveIndex = 0;

        for(int i = 0; i < saveLoadManager.selectedLevelData.paths.Count; i++)
        {
            CreateNewPath();
            for(int j = 0; j < saveLoadManager.selectedLevelData.paths[i].Count; j++)
            {
                objectPlacer.PlacePathNodeOnTerrain(saveLoadManager.selectedLevelData.paths[i][j], objectPlacer.pathNodePrefab, pathParent, selectedPathIndex);
            }
        }
        DoneCreatingPath();
        dropdownPathSelector.value = 0;
        selectedPathIndex = 0;

        startingGoldInputField.text = saveLoadManager.selectedLevelData.startingGold.ToString();
        startingHPInputField.text = saveLoadManager.selectedLevelData.startingHP.ToString();
        
        for(int i = 0; i < saveLoadManager.selectedLevelData.nrOfWaves; i++)
        {
            CreateNewWave();
        }
        dropdownWaveSelector.value = 0;
        selectedWaveIndex = 0;

        SelectMapToPlace(saveLoadManager.selectedLevelData.mapNumber);

        for (int i = 0; i < saveLoadManager.selectedLevelData.tiles.Count; i++)
        {
            objectPlacer.grid.SetTileAvailability(i, saveLoadManager.selectedLevelData.tiles[i].Availability);
        }
    }

    public void LoadLevelToEdit()
    {
        saveLoadManager.LoadLevel();
        ResetEditor();
        InitSelectedLevelData();
    }
}
