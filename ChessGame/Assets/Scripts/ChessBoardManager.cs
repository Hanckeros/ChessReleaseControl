using UnityEngine;

public class ChessBoardManager : MonoBehaviour
{
    [Header("Настройки доски")]
    public GameObject tilePrefab; // Клетка

    [Header("Белые фигуры")]
    public GameObject wPawn;
    public GameObject wRook, wKnight, wBishop, wQueen, wKing;

    [Header("Черные фигуры")]
    public GameObject bPawn;
    public GameObject bRook, bKnight, bBishop, bQueen, bKing;

    // Внутренняя логика
    private GameObject[,] tiles = new GameObject[8, 8];
    private ChessPiece[,] pieces = new ChessPiece[8, 8];
    private ChessPiece selectedPiece = null;
    private bool isWhiteTurn = true; // Чей ход

    // Класс для хранения данных о фигуре
    private class ChessPiece
    {
        public GameObject obj;
        public string type; // "Pawn", "King" и т.д.
        public bool isWhite;
        public bool hasMoved = false; // Для первого хода пешки
    }

    void Start()
    {
        GenerateBoard();
        SpawnAllPieces();
    }

    void Update()
    {
        // Обработка клика мыши
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Превращаем координаты мыши в координаты клетки (0..7)
            int x = Mathf.RoundToInt(mousePos.x);
            int y = Mathf.RoundToInt(mousePos.y);

            // Если кликнули в пределах доски
            if (x >= 0 && x < 8 && y >= 0 && y < 8)
            {
                HandleClick(x, y);
            }
        }
    }

    void HandleClick(int x, int y)
    {
        // 1. Если фигура уже выбрана - пробуем походить
        if (selectedPiece != null)
        {
            // Если кликнули в свою же другую фигуру - меняем выбор
            if (pieces[x, y] != null && pieces[x, y].isWhite == isWhiteTurn)
            {
                selectedPiece = pieces[x, y];
                Debug.Log($"Перевыбрали: {selectedPiece.type}");
                return;
            }

            // Проверяем правила хода
            if (IsValidMove(selectedPiece, x, y))
            {
                MovePiece(selectedPiece, x, y);
                selectedPiece = null; // Снимаем выделение
                return;
            }

            // Если ход невозможен - сброс
            selectedPiece = null;
        }
        else
        {
            // 2. Выбор фигуры (можно выбрать только свой цвет)
            if (pieces[x, y] != null && pieces[x, y].isWhite == isWhiteTurn)
            {
                selectedPiece = pieces[x, y];
                Debug.Log($"Выбрали: {selectedPiece.type} на {x},{y}");
            }
        }
    }

    void MovePiece(ChessPiece p, int x, int y)
    {
        // Ищем где фигура стояла раньше
        int oldX = -1, oldY = -1;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] == p) { oldX = i; oldY = j; }

        // Если в целевой клетке враг - уничтожаем (Рубим)
        if (pieces[x, y] != null)
        {
            Destroy(pieces[x, y].obj);
            if (pieces[x, y].type == "King") Debug.Log("GAME OVER!");
        }

        // Перемещаем данные в массиве
        pieces[oldX, oldY] = null;
        pieces[x, y] = p;

        // Перемещаем визуальный объект
        p.obj.transform.position = new Vector3(x, y, -1);
        p.hasMoved = true;

        // Передаем ход
        isWhiteTurn = !isWhiteTurn;
    }

    // --- ПРАВИЛА ШАХМАТ ---
    bool IsValidMove(ChessPiece p, int x, int y)
    {
        // Нельзя рубить своих
        if (pieces[x, y] != null && pieces[x, y].isWhite == p.isWhite) return false;

        // Находим текущие координаты
        int oldX = -1, oldY = -1;
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] == p) { oldX = i; oldY = j; }

        int dx = Mathf.Abs(x - oldX);
        int dy = Mathf.Abs(y - oldY);

        switch (p.type)
        {
            case "Pawn": // ПЕШКА
                int direction = p.isWhite ? 1 : -1; // Белые вверх (+1), Черные вниз (-1)
                // Обычный ход на 1 клетку
                if (dx == 0 && y == oldY + direction && pieces[x, y] == null) return true;
                // Первый ход на 2 клетки
                if (dx == 0 && y == oldY + 2 * direction && !p.hasMoved && pieces[x, y] == null && pieces[x, oldY + direction] == null) return true;
                // Атака по диагонали
                if (dx == 1 && y == oldY + direction && pieces[x, y] != null && pieces[x, y].isWhite != p.isWhite) return true;
                return false;

            case "Rook": // ЛАДЬЯ (Прямо)
                return (dx == 0 || dy == 0) && IsPathClear(oldX, oldY, x, y);

            case "Bishop": // СЛОН (Диагональ)
                return (dx == dy) && IsPathClear(oldX, oldY, x, y);

            case "Queen": // ФЕРЗЬ (Прямо + Диагональ)
                return (dx == 0 || dy == 0 || dx == dy) && IsPathClear(oldX, oldY, x, y);

            case "King": // КОРОЛЬ (1 клетка)
                return dx <= 1 && dy <= 1;

            case "Knight": // КОНЬ (Буквой Г)
                return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }
        return false;
    }

    // Проверка, что путь свободен (чтобы фигуры не перепрыгивали друг друга)
    bool IsPathClear(int x1, int y1, int x2, int y2)
    {
        int dx = System.Math.Sign(x2 - x1);
        int dy = System.Math.Sign(y2 - y1);
        int x = x1 + dx;
        int y = y1 + dy;

        while (x != x2 || y != y2)
        {
            if (pieces[x, y] != null) return false;
            x += dx;
            y += dy;
        }
        return true;
    }

    // --- СПАВН (РАССТАНОВКА) ---
    void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(x, y, 0);
                tile.name = $"Tile {x} {y}";

                // Красим клетки
                bool isWhite = (x + y) % 2 == 0;
                var rend = tile.GetComponent<SpriteRenderer>();
                rend.color = isWhite ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.4f, 0.4f, 0.4f); // Приятные цвета
            }
        }
        Camera.main.transform.position = new Vector3(3.5f, 3.5f, -10);
    }

    void SpawnAllPieces()
    {
        // Белые (0 ряд - фигуры, 1 ряд - пешки)
        SpawnPiece(wRook, "Rook", true, 0, 0); SpawnPiece(wKnight, "Knight", true, 1, 0);
        SpawnPiece(wBishop, "Bishop", true, 2, 0); SpawnPiece(wQueen, "Queen", true, 3, 0);
        SpawnPiece(wKing, "King", true, 4, 0); SpawnPiece(wBishop, "Bishop", true, 5, 0);
        SpawnPiece(wKnight, "Knight", true, 6, 0); SpawnPiece(wRook, "Rook", true, 7, 0);

        for (int i = 0; i < 8; i++) SpawnPiece(wPawn, "Pawn", true, i, 1);

        // Черные (7 ряд - фигуры, 6 ряд - пешки)
        SpawnPiece(bRook, "Rook", false, 0, 7); SpawnPiece(bKnight, "Knight", false, 1, 7);
        SpawnPiece(bBishop, "Bishop", false, 2, 7); SpawnPiece(bQueen, "Queen", false, 3, 7);
        SpawnPiece(bKing, "King", false, 4, 7); SpawnPiece(bBishop, "Bishop", false, 5, 7);
        SpawnPiece(bKnight, "Knight", false, 6, 7); SpawnPiece(bRook, "Rook", false, 7, 7);

        for (int i = 0; i < 8; i++) SpawnPiece(bPawn, "Pawn", false, i, 6);
    }

    void SpawnPiece(GameObject prefab, string type, bool isWhite, int x, int y)
    {
        if (prefab == null) return; // Защита от дурака
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.position = new Vector3(x, y, -1); // -1 чтобы быть поверх доски
        ChessPiece p = new ChessPiece { obj = obj, type = type, isWhite = isWhite };
        pieces[x, y] = p;
    }
}