﻿// Game Executable hosted at: http://www-users.york.ac.uk/~jwa509/alpha01BugFree.exe

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class mapManagerScript : MonoBehaviour
{
    private Map map;
    private const int LEFT_MOUSE_BUTTON = 0;
    
    private Tile lastTileHovered;
    private Tile currentTileSelected;
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (map != null)
        {
            CheckMouseHit();
        }
	}

    public void SetMap(Map map)
    {
        this.map = map;
    }

    private void CheckMouseHit()
    {
        /* jamaican bobsleigh team changes to this method
         * fixed an issue where the last hovered tile would stay highlighted even when the mouse is not hovering over any tile
         * fixed an issue where when hovering the selected tile the previously hovered but unselected tile would still have the hover highlight
         */
        Camera mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !eventSystem.IsPointerOverGameObject())   //Ray hit something and cursor is not over GUI object
        {
            if (hit.collider.tag == "mapTile")
            {
                Tile hitTile = map.GetTile(hit.collider.GetComponent<mapTileScript>().GetTileId());

                // if the mouse is not on the currently hovered tile (is either on a new tile or the selected tile)
                if (lastTileHovered != hitTile)
                {
                    if (lastTileHovered != null)
                    {
                        lastTileHovered.TileNormal();
                        lastTileHovered = null;
                    }
                    if (hitTile != currentTileSelected)
                    { 
                        hitTile.TileHovered();
                        lastTileHovered = hitTile;
                    }
                }
                
                if (hitTile != currentTileSelected)
                {
                    if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
                    {
                        if (currentTileSelected != null)
                        {
                            currentTileSelected.TileNormal();   //Reset the previously selected tile before overwriting the variable
                        }

                        // remove the hovering highlight as the selected highlight is more important
                        lastTileHovered.TileNormal();
                        lastTileHovered = null;

                        currentTileSelected = hitTile;
                        hitTile.TileSelected();
                    }
                }
            }
        }
        else
        {
            // mouse is not hovering on a tile
            if (lastTileHovered != null)
            {
                // reset the previously hovered tile unless its the selected tile
                if (lastTileHovered != currentTileSelected)
                {
                    lastTileHovered.TileNormal();
                }
            }
            lastTileHovered = null;
        }
    }
}
