using UnityEngine;

public class ChessBoardManager : MonoBehaviour
{
    // Сюда мы перетащим наш префаб клетки
    public GameObject tilePrefab;

    // Размер доски 8х8
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private GameObject[,] tiles; // Массив для хранения клеток

    void Start()
    {
        GenerateBoard();
    }

    // Алгоритм генерации доски (для твоей блок-схемы)
    private void GenerateBoard()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                // Создаем клетку
                GameObject newTile = Instantiate(tilePrefab, transform);

                // Ставим её на позицию (x, y)
                newTile.transform.position = new Vector3(x, y, 0);

                // Даем понятное имя в иерархии
                newTile.name = $"Tile {x} {y}";

                // Красим клетки в шахматном порядке
                bool isWhite = (x + y) % 2 == 0;
                var renderer = newTile.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    // Белый цвет или черный (серый для наглядности)
                    renderer.color = isWhite ? Color.white : Color.gray;
                }

                tiles[x, y] = newTile;
            }
        }

        // Центруем камеру, чтобы было красиво (лайфхак)
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10);
    }
}