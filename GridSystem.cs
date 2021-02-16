using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private float tileSize;
    [SerializeField] private float mapSizeX;
    [SerializeField] private float mapSizeZ;
    private float rayRange;
    private float halfTileSize;         // Those were defined so I don't waste computing power, calculating them only once (need updated if we want to change the grid in real time)
    private float halfMapSizeX;         //
    private float halfMapSizeZ;         //
    int tilesIARX; //tiles in a row     //
    int tilesIARZ; //tiles in a row     //
    private float gridMinX;             //
    private float gridMaxX;             //
    private float gridMinZ;             //
    private float gridMaxZ;             //
    private Vector3 gridPos;

    private List<TileData> tiles = new List<TileData>();

    private void Awake()
    {
        rayRange = 100f;
        UpdateGridValues();
        InitTileData();
    }

    void Start()
    {

    }


    void Update()
    {
        if (this.transform.hasChanged)      // REMOVE this if you don't plan to change the position of the grid/map
        {                                   //
            UpdateGridValues();             //
        }                                   //
    }

    /// <summary>
    /// Get the index in the Tile List based on the position given as argument
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    /// <returns></returns>
    public int TilePosToTileIndex(float posX, float posZ)
    {
        posX = Mathf.Clamp(posX, gridMinX, gridMaxX);
        posZ = Mathf.Clamp(posZ, gridMinZ, gridMaxZ);
        return (int)((halfMapSizeX + posX - gridPos.x) / tileSize ) + (int)((halfMapSizeZ - posZ + gridPos.z) / tileSize ) * (int)(mapSizeX / tileSize);
    }

    /// <summary>
    /// Get center of tile pos based on the index number in the list of tiles
    /// </summary>
    /// <param name="index">index in the tiles List</param>
    /// <returns></returns>
    public Vector3 TileIndexToTilePos(int index)
    {
        return new Vector3((int)Mathf.Repeat(index, tilesIARX) * tileSize - halfMapSizeX + halfTileSize + gridPos.x, 0f, -(int)(index / tilesIARX) * tileSize + halfMapSizeZ - halfTileSize + gridPos.z);
    }

    /// <summary>
    /// Recalculate commonly used grid values when any of the grid parameters change (e.g. grid size, grid position, tile size, etc)
    /// </summary>
    public void UpdateGridValues()
    {
        gridPos = this.transform.position;

        halfTileSize = tileSize / 2f;
        halfMapSizeX = mapSizeX / 2f;
        halfMapSizeZ = mapSizeZ / 2f;

        tilesIARX = (int)(mapSizeX / tileSize);
        tilesIARZ = (int)(mapSizeZ / tileSize);

        gridMinX = -(tilesIARX * tileSize) / 2 + gridPos.x;
        gridMaxX = (tilesIARX * tileSize) / 2 - tileSize + gridPos.x;
        gridMinZ = -(tilesIARZ * tileSize) / 2 + tileSize + gridPos.z;
        gridMaxZ = (tilesIARZ * tileSize) / 2 + gridPos.z;
    }

    public bool GetTileAvailability(int gridIndex)
    {
        return tiles[gridIndex].Availability;
    }

    public void SwitchTileAvailability(int gridIndex)
    {
        tiles[gridIndex].Availability = !tiles[gridIndex].Availability;
    }

    public void SetTileAvailability(int gridIndex, bool _availability)
    {
        tiles[gridIndex].Availability = _availability;
    }

    public float GetTileHeight(int gridIndex)
    {
        return tiles[gridIndex].Height;
    }

    /// <summary>
    /// Creates a Raycast above the middle of the tile and return the height in world coords at which it hits the terrain ("Terrain" layer mask)
    /// </summary>
    /// <param name="gridIndex"></param>
    /// <returns></returns>
    public float CalculateTileHeight(int gridIndex)
    {
        Vector3 posOnGrid = TileIndexToTilePos(gridIndex);
        Ray ray = new Ray(new Vector3(posOnGrid.x, rayRange / 2, posOnGrid.z), Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, rayRange, LayerMask.GetMask("Terrain")))
        {
            return hitInfo.point.y;
        }
        else
        {
            return 0;
        }
    }

    public void InitTileData()
    {
        tiles.Clear();
        for (int i = 0; i < tilesIARX * tilesIARZ; i++)
        {
            TileData tempData = new TileData();
            tempData.Availability = true;
            tempData.Height = CalculateTileHeight(i);
            tiles.Add(tempData);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < tilesIARX * tilesIARZ; i++)
        {
            Gizmos.DrawCube(TileIndexToTilePos(i), new Vector3(tileSize * 0.8f, tileSize * 0.2f, tileSize * 0.8f));
        }
    }

    [ExecuteInEditMode]
    void OnValidate()
    {
        //update the grid if I change its parameters from the inspector; only useful while developing
        UpdateGridValues();

        //this is here just to make sure I don't make Unity crash while I play with the values in the inspector
        if (tileSize < 0)
        {
            tileSize = 0;
        }

        if (mapSizeX < 0)
        {
            mapSizeX = 0;
        }
        if (mapSizeZ < 0)
        {
            mapSizeZ = 0;
        }

        if (tileSize > mapSizeX)
        {
            tileSize = mapSizeX;
        }
        if (tileSize > mapSizeZ)
        {
            tileSize = mapSizeZ;
        }
    }

    public float TileSize
    {
        get
        {
            return tileSize;
        }
        set
        {
            tileSize = TileSize;
        }
    }
    public float MapSizeX
    {
        get
        {
            return mapSizeX;
        }
        set
        {
            mapSizeX = MapSizeX;
        }
    }
    public float MapSizeZ
    {
        get
        {
            return mapSizeZ;
        }
        set
        {
            mapSizeZ = MapSizeZ;
        }
    }

}