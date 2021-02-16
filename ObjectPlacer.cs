using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GridSystem grid;
    public int gridIndex;

    public List<GameObject> towerPrefabs = new List<GameObject>();
    public List<GameObject> mapsPrefabs = new List<GameObject>();
    public GameObject pathNodePrefab;
    public GameObject invisiblePathNodePrefab;
    public int selObjectIndex;

    private Material unplaceableObjMat;
    private Material placeableObjMat;

    public GameObject selectedObj;

    private Vector3 mousePos;
    private float distFromCamera; //selectedObj dist from camera when no valid tile

    private RaycastHit hitInfo;
    private Ray ray;
    private float rayRange;

    private bool canBePlaced;

    private GameObject map;
    private Material mapMaterial;
    private float maxRange;
    private float minRange;
    private float borderThickness;
    public float borderThicknessRatio = 0.6f;

    public int placementMode; //0 tower, 1 path, 2 props
    public InputManager input;

    private void Awake()
    {
        unplaceableObjMat = (Material)Resources.Load("Materials/MaterialUnplaceable");
        placeableObjMat = (Material)Resources.Load("Materials/MaterialPlaceable");

        distFromCamera = 5.0f; //when object has no valid placement location
        rayRange = 100f;
        canBePlaced = false;
        selObjectIndex = 0;
        LoadTowerPrefabs();
        pathNodePrefab = (GameObject)Resources.Load("Prefabs/PathNode");
        invisiblePathNodePrefab = (GameObject)Resources.Load("Prefabs/InvisiblePathNode");
        LoadMapsPrefabs();

        maxRange = 0f;
        minRange = 0f;
        borderThickness = 1f;

        placementMode = 0;
    }

    void Start()
    {
        input = GameObject.FindObjectOfType<InputManager>();
    }

    public void LoadTowerPrefabs()
    {
        towerPrefabs.Add(null);
        towerPrefabs.Add((GameObject)Resources.Load("Prefabs/Towers/Tower"));
        towerPrefabs.Add((GameObject)Resources.Load("Prefabs/Towers/Cannon"));
        towerPrefabs.Add((GameObject)Resources.Load("Prefabs/Towers/Machinegun"));
    }

    public void LoadMapsPrefabs()
    {
        mapsPrefabs.Add(null);
        mapsPrefabs.Add((GameObject)Resources.Load("Prefabs/Maps/Map1"));
        mapsPrefabs.Add((GameObject)Resources.Load("Prefabs/Maps/Map2"));
        mapsPrefabs.Add((GameObject)Resources.Load("Prefabs/Maps/Map3"));
    }

    public void PlaceMap(int index)
    {
        if(map != null)
        {
            Destroy(map);
        }

        map = GameObject.Instantiate(mapsPrefabs[index], Vector3.zero, Quaternion.identity);
        map.name = "Map";
        mapMaterial = map.transform.GetChild(0).GetComponent<Terrain>().materialTemplate;

        //currently only supports 1 grid in the scene, but I could make this a list of grids to easily support multiple grids if I needed it
        grid = FindObjectOfType<GridSystem>();
    }

    public void SetSelectedTowerIndex(int index)
    {
        placementMode = 0;
        if((selObjectIndex > 0 && selObjectIndex != index) || index == 0)
        {
            Destroy(selectedObj);
        }
        selObjectIndex = index;
        if(index > 0)
        {
            InitNewSelectedObject(towerPrefabs[index]);
        }
    }

    public void SetSelectedMapIndex(int index)
    {
        if(index != 0 && index < mapsPrefabs.Count)
        {
            PlaceMap(index);
        }
        else
        {
            Destroy(map);
        }
    }

    public void EnablePathPlacementMode()
    {
        placementMode = 1;
        Destroy(selectedObj);
        InitNewSelectedObject(pathNodePrefab);
    }

    public void DisablePathPlacementMode()
    {
        placementMode = 0;
        Destroy(selectedObj);
    }

    void Update()
    {
        if(selectedObj != null)
        {
            switch (placementMode)
            {
                case 0:
                    TowerPlacerUpdateLoop();
                    break;
                case 1:
                    PathPlacerUpdateLoop();
                    break;
                case 2:
                    //TO DO: add prop placer update loop
                    break;
            }
        }
        
    }

    private void TowerPlacerUpdateLoop()
    {
        ray = Camera.main.ScreenPointToRay(input.mousePos);

        if (Physics.Raycast(ray, out hitInfo, rayRange, LayerMask.GetMask("Terrain")))
        {
            gridIndex = grid.TilePosToTileIndex(hitInfo.point.x, hitInfo.point.z);
            if (grid.GetTileAvailability(gridIndex))
            {
                MoveSelectedObjectOnTile();
            }
            else
            {
                MoveSelectedObjectAtMousePos();
            }

            if (input.leftMouseButtonDown && grid.GetTileAvailability(gridIndex) == true)
            {
                GameManager sm = GameObject.FindObjectOfType<GameManager>();
                Tower t = selectedObj.GetComponent<Tower>();
                if (sm.IsEnoughGoldForItem(t.GoldWorth))
                {
                    PlaceObjectOnTile(towerPrefabs[selObjectIndex]);
                    sm.PayForItem(t.GoldWorth);
                }
            }
        }
        else
        {
            MoveSelectedObjectAtMousePos();
        }
    }

    private void PathPlacerUpdateLoop()
    {
        ray = Camera.main.ScreenPointToRay(input.mousePos);

        if (Physics.Raycast(ray, out hitInfo, rayRange, LayerMask.GetMask("Terrain")))
        {
            MoveSelectedObjectOnTerrain(hitInfo.point, 0.15f);

            if (input.leftMouseButtonDown)
            {
                EditorManager editor = GameObject.FindObjectOfType<EditorManager>();
                PlacePathNodeOnTerrain(hitInfo.point, pathNodePrefab, editor.pathParent, editor.selectedPathIndex, 0.15f);
            }
        }
        else
        {
            MoveSelectedObjectAtMousePos();
        }
    }

    /// <summary>
    /// Instantiates an object on the tile corresponding to the coordinates of point; the instance name in hierarchy is the index of the tile, so we can identify and manipulate it
    /// </summary>
    /// <param name="point">coordinates for which we want to calculate the middle of the corresponding tile</param>
    /// <param name="objectPrefab">Object prefab you want instatiated</param>
    private void PlaceObjectOnTile(GameObject objectPrefab)
    {
        Vector3 posOnGrid = grid.TileIndexToTilePos(gridIndex);
        posOnGrid.y = grid.GetTileHeight(gridIndex);
        GameObject.Instantiate(objectPrefab, posOnGrid, Quaternion.identity, grid.transform).name = gridIndex.ToString();
        grid.SwitchTileAvailability(gridIndex);
    }

    /// <summary>
    /// Instantiate an object at the exact point given in the arguments + an offset in height
    /// </summary>
    /// <param name="point"></param>
    /// <param name="objectPrefab"></param>
    public void PlacePathNodeOnTerrain(Vector3 point, GameObject objectPrefab, GameObject _pathParent, int _selectedPathIndex, float offset = 0f)
    {
        point.y += offset;
        if (_pathParent.transform.childCount > 0 && _selectedPathIndex > 0)
        {
            GameObject.Instantiate(objectPrefab, point, Quaternion.identity, _pathParent.transform.GetChild(_selectedPathIndex - 1).transform).name = (_pathParent.transform.GetChild(_selectedPathIndex - 1).transform.childCount - 1).ToString();
        }
    }

    /// <summary>
    /// Snap the selected object in the middle of the tile that corresponds to the point
    /// </summary>
    /// <param name="point">coordinates for which we want to calculate the middle of the corresponding tile</param>
    private void MoveSelectedObjectOnTile()
    {
        if (canBePlaced == false)
        {
            canBePlaced = true;
            ChangeObjectMaterial(selectedObj, canBePlaced);
        }

        Vector3 posOnGrid = grid.TileIndexToTilePos(gridIndex);
        posOnGrid.y = grid.GetTileHeight(gridIndex);

        selectedObj.transform.position = posOnGrid;

        SetRangeIndicator(posOnGrid, minRange, maxRange, borderThickness);
    }

    private void MoveSelectedObjectOnTerrain(Vector3 point, float offset = 0f)
    {
        if (canBePlaced == false)
        {
            canBePlaced = true;
            ChangeObjectMaterial(selectedObj, canBePlaced);
        }

        point.y += offset;
        selectedObj.transform.position = point;
    }

    /// <summary>
    /// The selected object will hover a certain distance from the camera at mouse position (use this when no tile is available at the raycast from the pointer)
    /// </summary>
    private void MoveSelectedObjectAtMousePos()
    {
        if (canBePlaced == true)
        {
            canBePlaced = false;
            ChangeObjectMaterial(selectedObj, canBePlaced);
        }

        selectedObj.transform.position = GetSelectedObjectPosAtMouse();

        SetRangeIndicator(new Vector3(0,0,0),0,0);
    }

    /// <summary>
    /// Makes the object a transparent red if it's not on a valid tile or blue if it is
    /// </summary>
    /// <param name="go">The gameobject whose material we want changed transparent red or blue</param>
    /// <param name="isPlaceable">if it's placeable, material is transparent blue, if not, it's red</param>
    private void ChangeObjectMaterial(GameObject go,bool isPlaceable)
    {
        Material tempMaterial;
        if(isPlaceable)
        {
            tempMaterial = placeableObjMat;
        }
        else
        {
            tempMaterial = unplaceableObjMat;
        }

        if (go.GetComponent<MeshRenderer>() != null)
        {
            go.GetComponent<MeshRenderer>().material = tempMaterial;
        }
        if (go.transform.childCount > 0)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                {
                    go.transform.GetChild(i).GetComponent<MeshRenderer>().material = tempMaterial;
                }
                if (go.transform.GetChild(i).childCount > 0)
                {
                    for (int j = 0; j < go.transform.GetChild(i).childCount; j++)
                    {
                        if (go.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>() != null)
                        {
                            go.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().material = tempMaterial;
                        }
                        if (go.transform.GetChild(i).GetChild(j).childCount > 0)
                        {
                            for (int k = 0; k < go.transform.GetChild(i).GetChild(j).childCount; k++)
                            {
                                if (go.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<MeshRenderer>() != null)
                                {
                                    go.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<MeshRenderer>().material = tempMaterial;
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Call this whenever a new object is selected; It will instantiate it then disable shadows and getting light cast upon it; it also changes the material to be transparent
    /// </summary>
    /// <param name="objectPrefab">Object prefab you want instatiated</param>
    private void InitNewSelectedObject(GameObject objectPrefab)
    {
        selectedObj = Instantiate(objectPrefab, GetSelectedObjectPosAtMouse(), Quaternion.identity, this.transform);
        ChangeObjectMaterial(selectedObj, canBePlaced);

        if (selectedObj.GetComponent<MeshRenderer>() != null)
        {
            selectedObj.GetComponent<MeshRenderer>().shadowCastingMode = 0;
            selectedObj.GetComponent<MeshRenderer>().receiveShadows = false;
        }
        if (selectedObj.transform.childCount > 0)
        {
            for (int i = 0; i < selectedObj.transform.childCount; i++)
            {
                if (selectedObj.transform.GetChild(i).GetComponent<MeshRenderer>() != null)
                {
                    selectedObj.transform.GetChild(i).GetComponent<MeshRenderer>().shadowCastingMode = 0;
                    selectedObj.transform.GetChild(i).GetComponent<MeshRenderer>().receiveShadows = false;
                }
                if (selectedObj.transform.GetChild(i).childCount > 0)
                {
                    for (int j = 0; j < selectedObj.transform.GetChild(i).childCount; j++)
                    {
                        if (selectedObj.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>() != null)
                        {
                            selectedObj.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().shadowCastingMode = 0;
                            selectedObj.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>().receiveShadows = false;
                        }
                        if (selectedObj.transform.GetChild(i).GetChild(j).childCount > 0)
                        {
                            for (int k = 0; k < selectedObj.transform.GetChild(i).GetChild(j).childCount; k++)
                            {
                                if (selectedObj.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<MeshRenderer>() != null)
                                {
                                    selectedObj.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<MeshRenderer>().shadowCastingMode = 0;
                                    selectedObj.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<MeshRenderer>().receiveShadows = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        //disable the script attached to towers
        if(selectedObj.GetComponent<Tower>() != null)
        {
            maxRange = selectedObj.GetComponent<Tower>().MaxRange;
            minRange = selectedObj.GetComponent<Tower>().MinRange;
            borderThickness = (maxRange - minRange) * borderThicknessRatio;
            selectedObj.GetComponent<Tower>().enabled = false;
        }
    }

    /// <summary>
    /// Return a vector3 representing the position of the mouse , but a certain distance from the camera
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSelectedObjectPosAtMouse()
    {
        mousePos = input.mousePos;
        mousePos.z = distFromCamera;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    /// <summary>
    /// Set the selected tower's range indicator on the terrain material via shader
    /// </summary>
    /// <param name="pos">center of the spherical range indicator</param>
    /// <param name="_minRange">tower min range</param>
    /// <param name="_maxRange">tower max range</param>
    /// <param name="_borderThickness">thickness of fade for indicator</param>
    public void SetRangeIndicator(Vector3 pos, float _minRange = 0, float _maxRange = 0, float _borderThickness = 1f)
    {
        float dif = _maxRange - _minRange;
        if(dif < _borderThickness || _borderThickness < 0)
        {
            mapMaterial.SetFloat("_CircleThickness", dif);
        }
        else
        {
            mapMaterial.SetFloat("_CircleThickness", _borderThickness);
        }
        mapMaterial.SetVector("_CirclePosition", pos);
        mapMaterial.SetFloat("_CircleMinRadius", _minRange);
        mapMaterial.SetFloat("_CircleMaxRadius", _maxRange);
        
    }
}
