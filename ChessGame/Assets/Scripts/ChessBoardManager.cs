using UnityEngine;

public class ChessBoardManager : MonoBehaviour
{
    [Header("Art Assets")]
    public GameObject tilePrefab;       // Клетка
    public GameObject whitePiecePrefab; // Белая фигура (от дизайнера)
    public GameObject blackPiecePrefab; // Черная фигура (от дизайнера)

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;

    void Start()
    {
        GenerateBoard();
        SpawnAllPieces();
    }

    private void GenerateBoard()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.transform.position = new Vector3(x, y, 0);
                newTile.name = $"Tile {x} {y}";

                bool isWhite = (x + y) % 2 == 0;
                var renderer = newTile.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.color = isWhite ? Color.white : Color.gray;

                tiles[x, y] = newTile;
            }
        }
        // Центровка камеры
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10);
    }

    private void SpawnAllPieces()
    {
        // Проходим по всей доске
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                // Логика расстановки:
                // 0 и 1 ряды - Белые
                // 6 и 7 ряды - Черные

                GameObject prefabToSpawn = null;

                if (y == 0 || y == 1)
                    prefabToSpawn = whitePiecePrefab;
                else if (y == 6 || y == 7)
                    prefabToSpawn = blackPiecePrefab;

                // Если нужно что-то создать
                if (prefabToSpawn != null)
                {
                    GameObject piece = Instantiate(prefabToSpawn, transform);
                    piece.transform.position = new Vector3(x, y, -1); // -1 чтобы было поверх доски
                    piece.name = $"Piece {x} {y}";
                }
            }
        }
    }
}