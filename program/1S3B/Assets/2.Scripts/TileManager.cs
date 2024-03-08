using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileManager: MonoBehaviour
{
    public Transform playerObj;
    private Vector3 playerTransform;
    private Vector3Int playerTransformInt;

    private Vector2 mousePosition;

    [SerializeField] private Tilemap backgroundTile;

    [SerializeField] private Tilemap interactableMap;

    [SerializeField] private TileBase hiddenInteractableTile;

    private Camera mainCamera;

    private void Awake()
    {
        playerTransform = playerObj.position;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        playerTransform = playerObj.position;

        playerTransformInt = new Vector3Int((int)playerTransform.x, (int)playerTransform.y);
    }

    public void OnInteraction(InputValue value)
    {
        Debug.Log("눌림");

        //playerTransform = playerObj.position;
        //
        //Vector3Int vector3Int = new Vector3Int((int)playerTransform.x, (int)playerTransform.y);

        //Vector3Int vector3Int = new Vector3Int((int)mousePosition.x, (int)mousePosition.y,0);

        if (interactableMap.GetTile(playerTransformInt) == hiddenInteractableTile)
        {
            Debug.Log("맞다야");
            backgroundTile.SetTile(playerTransformInt, hiddenInteractableTile);
        }
    }

    //public void OnMouse(InputValue value)
    //{
    //    Vector2 mousPos = value.Get<Vector2>();
    //
    //    mousePosition = mainCamera.ScreenToWorldPoint(mousPos);
    //
    //    Debug.Log(mousePosition);
    //}


}
