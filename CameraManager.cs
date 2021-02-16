using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera camera;
    public InputManager input;
    public GridSystem grid;
    public float rotX;
    public float zoom;

    public float speed = 10f;
    public float rotSpeed = 120f;
    public float zoomSpeed = 20f;

    public float defaultCameraAngle = 35f;
    public float defaultCameraHeight = 6.22f;
    public float defaultCameraZOffset = -3.58f;

    public float minCameraHeight = 2f;
    public float maxCameraHeight = 15f;
    public float minCameraAngle = 20f;
    public float maxCameraAngle = 90f;
    public float lerpSmoothness = 4f;

    private void Awake()
    {
        camera = Camera.main;
        input = GameObject.FindObjectOfType<InputManager>();
        GetGridSystemReference();

        camera.transform.rotation = Quaternion.Euler(defaultCameraAngle, 0f, 0.0f);
        camera.transform.position = new Vector3(camera.transform.position.x, defaultCameraHeight, (-grid.TileSize * grid.MapSizeZ / 2) + defaultCameraZOffset);
    }

    void Update()
    {
        camera.transform.Translate(input.movement * speed * Time.deltaTime, Space.World);
        
        float minZ = (-grid.TileSize * grid.MapSizeZ / 2) + defaultCameraZOffset;
        float maxZ = (grid.TileSize * grid.MapSizeZ / 2) + defaultCameraZOffset;
        camera.transform.position = new Vector3(
          Mathf.Clamp(camera.transform.position.x, -grid.TileSize * grid.MapSizeX / 2, grid.TileSize * grid.MapSizeX / 2),
          Mathf.Clamp(camera.transform.position.y, minCameraHeight, maxCameraHeight),
          Mathf.Clamp(camera.transform.position.z, minZ, maxZ));

        /*rotX = ExtensionMethods.Remap(camera.transform.position.z, minZ, maxZ, defaultCameraAngle, defaultCameraAngle + 45f);
        Quaternion localRotation = Quaternion.Euler(rotX, 0f, 0.0f);
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, localRotation, rotSpeed * Time.deltaTime);*/

        defaultCameraHeight += input.scrollWheel * zoomSpeed;
        defaultCameraHeight = Mathf.Clamp(defaultCameraHeight, minCameraHeight, maxCameraHeight);

        defaultCameraAngle += input.scrollWheel * rotSpeed;
        defaultCameraAngle = Mathf.Clamp(defaultCameraAngle, minCameraAngle, maxCameraAngle);

        zoom = defaultCameraHeight;
        Vector3 localPosition = new Vector3(camera.transform.position.x, zoom, camera.transform.position.z);
        camera.transform.position = Vector3.Lerp(camera.transform.position, localPosition, Time.deltaTime * lerpSmoothness);

        rotX = defaultCameraAngle;
        Quaternion localRotation = Quaternion.Euler(rotX, 0f, 0.0f);
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, localRotation, Time.deltaTime * lerpSmoothness);
    }

    public void GetGridSystemReference()
    {
        grid = GameObject.FindObjectOfType<GridSystem>();
    }
}
