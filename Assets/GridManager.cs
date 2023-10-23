using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 8;
    [SerializeField]
    private int cols = 4;
    [SerializeField]
    private float tileSize = 1;
    private const string GRID_LAYER_NAME = "Default";
    private float updateInterval = 1.0f / 32.0f;  // Update every 1/32 seconds
    private float lastUpdateTime = 0;
    PlaySpace[,] playGrid;
    public float count = 0;

    private Dictionary<PieceType, string> pieceTypeNames;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlayGrid();
        Invoke("GenerateStuff", 1);

        pieceTypeNames = new Dictionary<PieceType, string>
        {
            { PieceType.Empty, "Empty" },
            { PieceType.Blooper, "Blooper" },
            { PieceType.Piranha, "Piranha" },
            { PieceType.Boo, "Boo" },
            { PieceType.Goomba, "Goomba" },
            { PieceType.BottomShell, "BottomShell" },
            { PieceType.TopShell, "TopShell" },
        };
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;

        // This check will ensure the game runs at 32 frames per second
        if (currentTime - lastUpdateTime >= updateInterval)
        {
            // If nothing is falling, we'll generate a new set of pieces instead
            bool hasMovingPieces = false;
            lastUpdateTime = currentTime;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Don't update static pieces
                    if (!playGrid[row, col].isFalling) continue;
                    hasMovingPieces = true;

                    dropPiece(row, col);
                }
            }
            
            if (!hasMovingPieces)
            {
                Debug.Log("TODO: Generated new units");
            }
        }
    }

    void GeneratePlayGrid()
    {
        playGrid = new PlaySpace[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float posX = col * tileSize;
                float posY = row * -tileSize;
                playGrid[row, col] = new PlaySpace(PieceType.Empty, new Vector2(posX, posY));
            }
        }

        float gridWidth = cols * tileSize;
        float gridHeight = rows * tileSize;
    }

    void GenerateStuff() 
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                playGrid[i, j].piece = PieceType.Goomba;
                playGrid[i, j].hasNewPiece = true;
            }
        }
    }

    void OnGUI()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (playGrid[row, col].hasNewPiece) {
                    Destroy(playGrid[row, col].gameObject);
                    GameObject prefab = (GameObject)Instantiate(Resources.Load(pieceTypeNames[playGrid[row, col].piece]));
                    prefab.transform.position = new Vector3(getXCoord(col), getYCoord(row));
                    playGrid[row, col].gameObject = prefab;
                    playGrid[row, col].hasNewPiece = false;
                }
            }
        }
    }

    // Controls the logic about moving a piece down and checking for matches
    void dropPiece(int x, int y)
    {
        // If there's no tile below them, move them down
        
        // If they match the block below them, shatter them both
        // If they're a top shell, check for a matching bottom shell
        //   * If found, shatter everything in between
        //   * Else just shatter it
        // If they're over the top, Game over
        // If there is a block below them, place it and stop it from falling
    }

    float getYCoord(float y)
    {
        return (y * -tileSize) + 3.5f;
    }

    float getXCoord(float x)
    {
        return (x * tileSize) + 0.5f;
    }
}

internal class PlaySpace
{ 
    public PieceType piece;
    public Vector2 position;
    public GameObject gameObject;
    public bool hasNewPiece;
    public bool isFalling;

    public PlaySpace(PieceType initPiece, Vector2 initPosition)
    {
        piece = initPiece;
        position = initPosition;
        gameObject = new GameObject();
        if (piece != PieceType.Empty)
        {
            hasNewPiece = true;
            isFalling = true;  
        } 
    }
}

public enum PieceType {
    Empty,
    Blooper,
    Piranha,
    Boo,
    Goomba,
    BottomShell,
    TopShell,
}