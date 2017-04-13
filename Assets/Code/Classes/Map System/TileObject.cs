//Game executable hosted by JBT at: http://robins.tech/jbt/documents/assthree/GameExecutable.zip

using UnityEngine;

public class TileObject
{
    /// <summary>
    /// JBT added variable to help clean up editor object layout
    /// </summary>
    private static GameObject tileHolder;
    private static GameObject TILE_GRID_GAMEOBJECT;
    private const string TILE_GRID_PREFAB_PATH = "Prefabs/Map/Tile Grid/tileGridPrefab";
    public static Color TILE_DEFAULT_COLOUR = new Color(1, 1, 1);   //White
    private static Color TILE_HOVER_COLOUR = new Color(1, 1, 0);     //Yellow
    private static Color TILE_SELECT_COLOUR = new Color(0, 1, 0);    //Green
    private static float TILE_HIGHLIGHT_ALPHA = 0.25f;                //Value between 0-1, which determines the strength of tile highlights, added by JBT
    private static Vector3 tileGridPrefabSize;

    private Vector2 position;
    private Vector2 size;
    private int tileId;

    private GameObject tileGameObjectInScene;
    private GameObject tileCenter;              //Added by JBT to allow the entire tile area to be highlighted
    private GameObject tileRoboticon;           //Added by JBT to allow the display of installed roboticons on tiles
    private GameObject tileEventDisplay;        //Added by JBT to allow the display of any events happening on tiles

    public TileObject(int id, Vector2 position, Vector2 dimensions)
    {
        LoadTileGridGameobject();

        this.position = position;
        this.size = dimensions;
        this.tileId = id;
    }

    public Vector2 GetTilePosition()
    {
        return this.position;
    }

    public Vector3 GetTileWorldPosition()
    {
        return tileGameObjectInScene.transform.position;
    }

    // Edited by JBT to help reduce the amount of clutter in the editor objects window
    /// <summary>
    /// Instantiate the tile object at its stored position and size in the current scene.
    /// </summary>
    public void Instantiate(Vector3 mapCenterPosition)
    {
        if(TILE_GRID_GAMEOBJECT == null)
        {
            throw new System.NullReferenceException("Attempted to instantiate a tile without a reference to the tile grid gameobject prefab."+
                                                    "Check the file path for the tile grid gameobject prefab.");
        }

        if(tileHolder == null)
        {
            tileHolder = new GameObject();
            tileHolder.transform.position = Vector3.zero;
            tileHolder.name = "Tile Holder";
            MonoBehaviour.DontDestroyOnLoad(tileHolder);
        }

        Vector3 tilePositionInScene = new Vector3(position.x * size.x * (tileGridPrefabSize.x + 0.1f), 0, position.y * size.y * (tileGridPrefabSize.z + 0.1f));
        tilePositionInScene += mapCenterPosition;
        tileGameObjectInScene = (GameObject)GameObject.Instantiate(TILE_GRID_GAMEOBJECT, tilePositionInScene, Quaternion.identity);
        MonoBehaviour.DontDestroyOnLoad(tileGameObjectInScene);             //Instantiated in the main menu scene and carried over into the game scene.
                                                                            //This removes the need for callbacks as the LoadScene function is asynchronous.

        Vector3 objectScale = tileGameObjectInScene.transform.localScale;   //Cannot assign to individual components of scale.
        objectScale.x *= size.x;
        objectScale.z *= size.y;
        tileGameObjectInScene.transform.localScale = objectScale;
        tileGameObjectInScene.name = "Tile";
        tileGameObjectInScene.tag = TagManager.mapTile;
        tileGameObjectInScene.AddComponent<mapTileScript>().SetTileId(tileId);
        tileGameObjectInScene.transform.parent = tileHolder.transform;
        tileCenter = tileGameObjectInScene.transform.GetChild(0).gameObject;
        tileRoboticon = tileGameObjectInScene.transform.GetChild(1).gameObject;
        tileEventDisplay = tileGameObjectInScene.transform.GetChild(2).gameObject;
        tileCenter.SetActive(false);
        tileRoboticon.SetActive(false);
        tileEventDisplay.SetActive(false);
    }

    //JBT changed this method to support a highlightable center of the tile
    /// <summary>
    /// Changes the colour of a tile when selected
    /// </summary>
    public void OnTileSelected()
    {
        if (tileGameObjectInScene != null)
        {
            tileCenter.SetActive(true);
            tileCenter.GetComponent<MeshRenderer>().material.color = GetTransparentColor(TILE_SELECT_COLOUR, TILE_HIGHLIGHT_ALPHA);
            tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_SELECT_COLOUR;
        }
    }

    //JBT changed this method to support a highlightable center of the tile
    /// <summary>
    /// Changes the colour of a tile when hovered over
    /// </summary>
    public void OnTileHover()
    {
        if(tileGameObjectInScene != null)
        {
            tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_HOVER_COLOUR;
            tileCenter.SetActive(true);
            tileCenter.GetComponent<MeshRenderer>().material.color = GetTransparentColor(TILE_HOVER_COLOUR, TILE_HIGHLIGHT_ALPHA);
        }
    }

    //Changed by JBT to support a highlightable center of the tile
    /// <summary>
    /// Highlight the tile different colours, depending on who owns it
    /// </summary>
    public void OnTileNormal(Color tileColor, bool owned)
    {
        if (tileGameObjectInScene != null)
        {
            tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = tileColor;

            if (owned)
            {
                tileCenter.SetActive(true);
                tileCenter.GetComponent<MeshRenderer>().material.color = GetTransparentColor(tileColor, TILE_HIGHLIGHT_ALPHA);
            }
            else
            {
                tileCenter.SetActive(false);
            }

            /*

            switch (ownerType)
            {
                case TILE_OWNER_TYPE.CURRENT_PLAYER:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_OWNED;
                    tileCenter.SetActive(true);
                    tileCenter.GetComponent<MeshRenderer>().material.color = GetTransparentColor(TILE_DEFAULT_OWNED, TILE_HIGHLIGHT_ALPHA);
                    break;
                case TILE_OWNER_TYPE.ENEMY:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_ENEMY;
                    tileCenter.SetActive(true);
                    tileCenter.GetComponent<MeshRenderer>().material.color = GetTransparentColor(TILE_DEFAULT_ENEMY, TILE_HIGHLIGHT_ALPHA);
                    break;
                case TILE_OWNER_TYPE.UNOWNED:
                    tileGameObjectInScene.GetComponent<MeshRenderer>().material.color = TILE_DEFAULT_COLOUR;
                    tileCenter.SetActive(false);
                    break;
            }
            */
        }
    }

    //Method added by JBT
    /// <summary>
    /// Takes an opaque color and an alpha value and converts the colour into a transparent one, to be used with tile highlights
    /// </summary>
    /// <param name="c">The opaque color</param>
    /// <param name="a">The alpha value to set it to</param>
    /// <returns>The transparent color</returns>
    private Color GetTransparentColor(Color c, float a)
    {
        Color newColor = c;
        c.a = a;
        return c;
    }

    //Added by JBT to support the display of installed roboticons on tiles
    /// <summary>
    /// Shows the installed roboticon on a tile. This is called when a roboticon is installed on this tile instance
    /// </summary>
    public void ShowInstalledRoboticon()
    {
        tileRoboticon.SetActive(true);
    }

    //Added by JBT to support the display of installed roboticons on tiles
    /// <summary>
    /// Hides the installed roboticon on a tile. This is called when a roboticon is uninstalled from this tile instance
    /// </summary>
    public void HideInstalledRoboticon()
    {
        tileRoboticon.SetActive(false);
    }

    /// <summary>
    /// Load the tile grid gameobject from resources if it has not already been loaded.
    /// </summary>
    private void LoadTileGridGameobject()
    {
        if(TILE_GRID_GAMEOBJECT == null)
        {
            TILE_GRID_GAMEOBJECT = (GameObject)Resources.Load(TILE_GRID_PREFAB_PATH);
            tileGridPrefabSize = TILE_GRID_GAMEOBJECT.GetComponent<Renderer>().bounds.size;
        }
    }

    //Created by JBT
    /// <summary>
    /// Set the tiles event icon and display it
    /// </summary>
    /// <param name="icon">The path of the texture to display</param>
    public void SetEventIcon(string iconPath)
    {
        tileEventDisplay.SetActive(true);
        tileEventDisplay.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>(iconPath);
    }

    //Created by JBT
    /// <summary>
    /// Remove the random event display from a tile
    /// </summary>
    public void RemoveEventIcon()
    {
        tileEventDisplay.SetActive(false);
    }

    // created by jbt
    /// <summary>
    /// hide the event image 
    /// </summary>
    public void DisableEventImage()
    {
        tileEventDisplay.SetActive(false);
    }
}
