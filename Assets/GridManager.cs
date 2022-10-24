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
    PlaySpace[,] playGrid;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlayGrid();
        Invoke("GenerateStuff", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    GameObject prefab;
                    switch (playGrid[row, col].piece)
                    {
                        case PieceType.Blooper:
                            prefab = (GameObject)Instantiate(Resources.Load("Blooper"));
                            break;
                        case PieceType.Goomba:
                            prefab = (GameObject)Instantiate(Resources.Load("Goomba"));
                            break;
                        case PieceType.Boo:
                            prefab = (GameObject)Instantiate(Resources.Load("Boo"));
                            break;
                        case PieceType.Pirahna:
                            prefab = (GameObject)Instantiate(Resources.Load("Pirahna"));
                            break;
                        case PieceType.TopShell:
                            prefab = (GameObject)Instantiate(Resources.Load("TopShell"));
                            break;
                        case PieceType.BottomShell:
                            prefab = (GameObject)Instantiate(Resources.Load("BottomShell"));
                            break;
                        default:
                            prefab = new GameObject();
                            break;
                    }

                    prefab.transform.position = new Vector3(getX(col), getY(row));
                    playGrid[row, col].gameObject = prefab;
                    playGrid[row, col].hasNewPiece = false;
                }
            }
        }
    }

    float getY(float y)
    {
        return (y * -tileSize) + 3.5f;
    }

    float getX(float x)
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

    public PlaySpace(PieceType initPiece, Vector2 initPosition)
    {
        piece = initPiece;
        position = initPosition;
        gameObject = new GameObject();
        hasNewPiece = false;
    }
}

public enum PieceType {
    Empty,
    Blooper,
    Pirahna,
    Boo,
    Goomba,
    BottomShell,
    TopShell,
}