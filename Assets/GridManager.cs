using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField]
    private int rows = 9;
    [SerializeField]
    private int cols = 4;
    [SerializeField]
    private float tileSize = 1;
    private const string GRID_LAYER_NAME = "Default";
    private float updateInterval = 1.0f / 2.0f;  // Update every half second
    // private float updateInterval = 1.0f / 32.0f;  // Update every 1/32 seconds
    private float lastUpdateTime = 0;
    PlaySpace[,] playGrid;
    public float count = 0;

    private Dictionary<PieceType, string> pieceTypeNames;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlayGrid();

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

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = cols - 1; col >= 0; col--)
                {
                    // Don't update static pieces
                    if (!playGrid[row, col].isFalling) continue;
                    hasMovingPieces = true;

                    dropPiece(row, col);
                }
            }
            
            if (!hasMovingPieces)
            {
                // Current implementation will always add 2 block up top
                // TODO: Randomly generate 3 or 4 at a set rate
                Debug.Log("TODO: Generated new units");
                System.Random random = new System.Random();
                // TODO: Smarter randomization
                PieceType randomPiece1 = (PieceType)random.Next(1, Enum.GetValues(typeof(PieceType)).Length);
                PieceType randomPiece2 = (PieceType)random.Next(1, Enum.GetValues(typeof(PieceType)).Length);
                // TODO: Randomize from the spaces not counting location 1
                int location1 = random.Next(0, cols);
                int location2 = random.Next(0, cols);

                playGrid[0, location1].updatePiece(randomPiece1);
                playGrid[0, location2].updatePiece(randomPiece2);
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

    // Update the game objects to render the correct pieces in each box
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
    // If they're at the bottom, stop it
    // If there's no tile below them, move them down
    // If they match the block below them, shatter them both
    // If they're a top shell, check for a matching bottom shell
    //   * If found, shatter everything in between
    //   * Else just shatter it
    // If they're over the top, Game over
    // If there is a block below them, place it and stop it from falling
    void dropPiece(int row, int col)
    {
        // TODO: Think of a smarter way to bundle these branches
        if (row == rows - 1)
        { // At the bottom row
            if (playGrid[row, col].piece == PieceType.TopShell)
            {
                playGrid[row, col].shatter();
            } else 
            { // Place the piece
                playGrid[row, col].place();
            }
        } else if (playGrid[row + 1, col].piece == PieceType.Empty) 
        { // There's an empty space beneath
            playGrid[row + 1, col].updatePiece(playGrid[row, col].piece);
            playGrid[row, col].shatter();
        } else 
        { // There is a block beneath
            if (playGrid[row, col].piece == playGrid[row + 1, col].piece)
            { // The pieces match, break em
                playGrid[row, col].shatter();
                playGrid[row + 1, col].shatter();
            } else if (playGrid[row, col].piece == PieceType.TopShell)
            { // Check for bottom shells, else shatter it
                for (int i = row + 1; i < rows; i++)
                {
                    if (playGrid[i, col].piece == PieceType.BottomShell)
                    { // Found a matching bottom shell, shatter everything between them
                        for (int j = i; j > row; j--)
                        {
                            playGrid[j, col].shatter();
                        }
                        break;
                    }
                }
                playGrid[row, col].shatter();
            } else if (row == 0)
            { // Game over scenario
                Debug.Log("TODO: GAME OVER!~!");
            } else
            { // Place the piece
                playGrid[row, col].place();
            }
        }
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

    public void updatePiece(PieceType newPiece)
    {
        if (newPiece != piece)
        {
            piece = newPiece;
            hasNewPiece = true;
            isFalling = true;
        }
    }

    public void shatter()
    {
        piece = PieceType.Empty;
        hasNewPiece = true;
        isFalling = false;
    }

    public void place()
    {
        isFalling = false;
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