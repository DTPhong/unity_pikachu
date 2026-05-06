using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [Header("UI Reference")]
    public ItemTile tilePrefab;
    public Transform boardPanel;
    public SpriteAtlas iconAtlas;
    private ItemTile firstSelectedTile;
    // Ma trận lưu trữ dữ liệu board. 0 là ô trống.
    public int[,] boardData; 
    
    [Header("Board Config")]
    public int logicRows = 4; // Số hàng thực tế có icon (vd: 4)
    public int logicCols = 5; // Số cột thực tế có icon (vd: 5)
    public int maxIconID = 10; // Tổng số loại icon (1 đến 10)

    private int totalRows; // Bao gồm cả padding
    private int totalCols;
    [Header("UI Reference")]
    public GridLayoutGroup gridLayout;

    void Start()
    {
        // Khởi tạo map 4x5 để test
        // InitializeBoard(logicRows, logicCols);
        
        // // In ra console để kiểm tra Data trước khi làm UI
        // PrintBoard();
        // RenderBoardUI();
        CreateNewGame(5, 5);
        Debug.Log("Start game");
    }
    public void StartGame5x5()
    {
        CreateNewGame(5, 5);
    }

    public void StartGame8x8()
    {
        CreateNewGame(8, 8);
    }
    public void CreateNewGame(int rows, int cols)
    {
        logicRows = rows;
        logicCols = cols;
        totalRows = logicRows + 2;
        totalCols = logicCols + 2;

        // 1. Cấu hình lại Grid Layout Group để hàng/cột hiển thị đúng
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = logicCols;

        // 2. Khởi tạo lại dữ liệu và vẽ UI
        GenerateBoard();
        PrintBoard();
        RenderBoardUI();
        
        // Reset trạng thái chọn
        firstSelectedTile = null;
    }
    // Cập nhật lại hàm GenerateBoard để xử lý trường hợp số ô lẻ (như 5x5)
    private void GenerateBoard()
    {
        boardData = new int[totalRows, totalCols];
        List<int> tempIcons = new List<int>();

        int totalPlayableCells = logicRows * logicCols;
        int pairCount = totalPlayableCells / 2; // Ví dụ: 5x5 = 25 ô -> 12 cặp (24 ô)

        // Tạo các cặp icon
        for (int i = 0; i < pairCount; i++)
        {
            int iconID = Random.Range(1, maxIconID + 1);
            tempIcons.Add(iconID);
            tempIcons.Add(iconID);
        }

        // Nếu tổng số ô là lẻ (như 5x5 = 25), ta thêm một số 0 (ô trống) vào danh sách
        if (totalPlayableCells % 2 != 0)
        {
            tempIcons.Add(0); // 0 đại diện cho ô trống ngay từ đầu
        }

        // Trộn danh sách
        for (int i = 0; i < tempIcons.Count; i++)
        {
            int temp = tempIcons[i];
            int randomIndex = Random.Range(i, tempIcons.Count);
            tempIcons[i] = tempIcons[randomIndex];
            tempIcons[randomIndex] = temp;
        }

        // Đổ vào mảng 2D
        int index = 0;
        for (int r = 1; r <= logicRows; r++)
        {
            for (int c = 1; c <= logicCols; c++)
            {
                boardData[r, c] = tempIcons[index];
                index++;
            }
        }
    }

    /// <summary>
    /// Hàm khởi tạo dữ liệu mảng 2D cho game
    /// </summary>
    public void InitializeBoard(int rows, int cols)
    {
        // 1. Tính toán kích thước ma trận có Padding (Viền ngoài bằng 0)
        totalRows = rows + 2;
        totalCols = cols + 2;
        boardData = new int[totalRows, totalCols];

        // 2. Tính số lượng cặp cần sinh ra
        int totalItems = rows * cols;
        if (totalItems % 2 != 0)
        {
            Debug.LogError("Số lượng ô phải là số chẵn để tạo cặp!");
            return;
        }
        int totalPairs = totalItems / 2;

        // 3. Đổ dữ liệu theo cặp vào List
        List<int> iconList = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            // Random ID từ 1 đến maxIconID (Random.Range trong int thì max bị loại trừ nên phải + 1)
            int randomID = Random.Range(1, maxIconID + 1); 
            iconList.Add(randomID);
            iconList.Add(randomID); // Thêm 2 lần để tạo thành 1 cặp
        }

        // 4. Trộn List bằng thuật toán Fisher-Yates (Chuẩn mực tối ưu)
        ShuffleList(iconList);

        // 5. Đổ List đã trộn vào Ma trận 2D (Chừa lại viền ngoài)
        int listIndex = 0;
        for (int r = 1; r <= rows; r++)
        {
            for (int c = 1; c <= cols; c++)
            {
                boardData[r, c] = iconList[listIndex];
                listIndex++;
            }
        }
    }

    /// <summary>
    /// Thuật toán Fisher-Yates xáo trộn mảng O(N)
    /// </summary>
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // Swap
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// In ma trận ra Console để Debug (Sử dụng StringBuilder để tránh rác bộ nhớ)
    /// </summary>
    private void PrintBoard()
    {
        if (boardData == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== PIKACHU BOARD DATA ===");

        for (int r = 0; r < totalRows; r++)
        {
            for (int c = 0; c < totalCols; c++)
            {
                // PadLeft(3) để các số căn lề cho thẳng cột đẹp mắt
                sb.Append(boardData[r, c].ToString().PadLeft(3)); 
            }
            sb.AppendLine(); // Xuống dòng khi hết 1 hàng
        }

        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// Đọc ma trận và khởi tạo UI tương ứng
    /// </summary>
    private void RenderBoardUI()
    {
        // Xóa các Tile cũ trước khi vẽ mới
        foreach (Transform child in boardPanel) { Destroy(child.gameObject); }

        for (int r = 1; r <= logicRows; r++)
        {
            for (int c = 1; c <= logicCols; c++)
            {
                int currentID = boardData[r, c];

                // Nếu là ô lẻ (ID = 0) thì không sinh Prefab, cứ để trống
                if (currentID == 0)
                {
                    // Tạo một Object rỗng để giữ chỗ trong Grid Layout giúp các ô sau không bị lệch
                    GameObject placeholder = new GameObject("EmptySpace");
                    placeholder.transform.SetParent(boardPanel);
                    continue; 
                }

                ItemTile newTile = Instantiate(tilePrefab, boardPanel);
                newTile.gameObject.name = $"Tile_{r}_{c}";
                Sprite tileSprite = iconAtlas.GetSprite($"icon_{currentID}");
                newTile.Setup(r, c, currentID, tileSprite, this);
            }
        }
    }
    /// <summary>
    /// Xử lý logic khi một ô được click
    /// </summary>
    public void OnTileClicked(ItemTile clickedTile)
    {
        // 1. Bỏ qua nếu click lại chính cái ô vừa chọn
        if (firstSelectedTile == clickedTile)
        {
            // Bỏ chọn ô đó
            firstSelectedTile.SetHighlight(false);
            firstSelectedTile = null;
            return;
        }

        // 2. Nếu chưa có ô nào được chọn (Đây là click đầu tiên)
        if (firstSelectedTile == null)
        {
            firstSelectedTile = clickedTile;
            firstSelectedTile.SetHighlight(true);
            Debug.Log($"Đã chọn ô đầu tiên: [{clickedTile.gridX}, {clickedTile.gridY}] - ID: {clickedTile.iconID}");
        }
        // 3. Nếu đã chọn ô đầu tiên rồi (Đây là click thứ hai)
        else
        {
            ItemTile secondSelectedTile = clickedTile;
            secondSelectedTile.SetHighlight(true);

            // Kiểm tra xem 2 ô có cùng ID không?
            if (firstSelectedTile.iconID == secondSelectedTile.iconID)
            {
                if (CheckPath(firstSelectedTile, secondSelectedTile))
                {
                    Debug.Log("<color=cyan>ĐƯỜNG NỐI HỢP LỆ! Xóa 2 ô này.</color>");
                    
                    // Cập nhật Data: Set 2 ô này về 0 (trống)
                    boardData[firstSelectedTile.gridX, firstSelectedTile.gridY] = 0;
                    boardData[secondSelectedTile.gridX, secondSelectedTile.gridY] = 0;

                    // Cập nhật UI: Tắt hình ảnh (Hoặc dùng Destroy)
                    // firstSelectedTile.gameObject.SetActive(false);
                    // secondSelectedTile.gameObject.SetActive(false);
                    firstSelectedTile.HideTile();
                    secondSelectedTile.HideTile();

                    // CHỈ CẦN XÓA THAM CHIẾU (Vì 2 ô đã biến mất, không cần đổi màu nữa)
                    firstSelectedTile = null; 
                }
                else
                {
                    Debug.Log("<color=orange>Giống nhau nhưng KHÔNG CÓ ĐƯỜNG NỐI.</color>");
                    // Không nối được thì mới cần reset màu
                    ResetSelection(secondSelectedTile);
                }
            }
            else
            {
                Debug.Log("<color=red>Hai ô KHÁC LOẠI! Chọn sai.</color>");
                // Chọn sai loại thì cũng reset màu
                ResetSelection(secondSelectedTile);
            }
        }
        PrintBoard();
    }

    private void ResetSelection(ItemTile secondTile)
    {
        firstSelectedTile.SetHighlight(false);
        secondTile.SetHighlight(false);
        firstSelectedTile = null;
    }
    /// <summary>
    /// Kiểm tra đường thẳng nằm NGANG (Cùng hàng r, đi từ cột c1 đến cột c2)
    /// </summary>
    private bool CheckLineX(int r, int c1, int c2)
    {
        // Đảm bảo c1 luôn nhỏ hơn c2 để vòng lặp chạy đúng từ trái sang phải
        int min = Mathf.Min(c1, c2);
        int max = Mathf.Max(c1, c2);

        for (int c = min + 1; c < max; c++)
        {
            if (boardData[r, c] != 0) // Có vật cản
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Kiểm tra đường thẳng nằm DỌC (Cùng cột c, đi từ hàng r1 đến hàng r2)
    /// </summary>
    private bool CheckLineY(int c, int r1, int r2)
    {
        // Đảm bảo r1 luôn nhỏ hơn r2 để vòng lặp chạy đúng từ trên xuống dưới
        int min = Mathf.Min(r1, r2);
        int max = Mathf.Max(r1, r2);

        for (int r = min + 1; r < max; r++)
        {
            if (boardData[r, c] != 0) // Có vật cản
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Thuật toán lõi kiểm tra đường nối giữa 2 điểm
    /// </summary>
    private bool CheckPath(ItemTile t1, ItemTile t2)
    {
        int r1 = t1.gridX;
        int c1 = t1.gridY;
        int r2 = t2.gridX;
        int c2 = t2.gridY;

        // 1. Kiểm tra đường thẳng (0 góc)
        if (r1 == r2 && CheckLineX(r1, c1, c2)) { Debug.Log("ĂN! Thẳng NGANG."); return true; }
        if (c1 == c2 && CheckLineY(c1, r1, r2)) { Debug.Log("ĂN! Thẳng DỌC."); return true; }

        // 2. Kiểm tra hình chữ L (1 góc)
        if (CheckRect(r1, c1, r2, c2)) { Debug.Log("ĂN! Chữ L."); return true; }

        // 3. Kiểm tra hình chữ U, Z (2 góc)
        if (CheckU_Z_Horizontal(r1, c1, r2, c2)) { Debug.Log("ĂN! Chữ U/Z ngang."); return true; }
        if (CheckU_Z_Vertical(r1, c1, r2, c2)) { Debug.Log("ĂN! Chữ U/Z dọc."); return true; }

        return false; // Nếu qua hết mà không nối được thì là ngõ cụt
    }
    /// <summary>
    /// Kiểm tra đường nối hình chữ L (1 góc khúc)
    /// </summary>
    private bool CheckRect(int r1, int c1, int r2, int c2)
    {
        // Kiểm tra góc giao thứ nhất: (r1, c2)
        if (boardData[r1, c2] == 0 && CheckLineX(r1, c1, c2) && CheckLineY(c2, r1, r2))
        {
            return true;
        }

        // Kiểm tra góc giao thứ hai: (r2, c1)
        if (boardData[r2, c1] == 0 && CheckLineY(c1, r1, r2) && CheckLineX(r2, c1, c2))
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// Kiểm tra hình U/Z bằng cách quét ngang (Tìm 2 đường ngang nối bởi 1 đường dọc)
    /// </summary>
    private bool CheckU_Z_Horizontal(int r1, int c1, int r2, int c2)
    {
        // Quét tất cả các cột từ 0 đến totalCols - 1 (bao gồm cả viền padding)
        for (int c = 0; c < totalCols; c++)
        {
            if (c == c1 || c == c2) continue; // Bỏ qua cột của A và B (đã check ở thẳng và L)

            // Hai điểm giao ở hàng r1 và r2 trên cột c phải trống
            if (boardData[r1, c] == 0 && boardData[r2, c] == 0)
            {
                // Kẻ tia ngang từ A -> (r1, c), tia dọc từ (r1, c) -> (r2, c), tia ngang từ (r2, c) -> B
                if (CheckLineX(r1, c1, c) && CheckLineY(c, r1, r2) && CheckLineX(r2, c, c2))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Kiểm tra hình U/Z bằng cách quét dọc (Tìm 2 đường dọc nối bởi 1 đường ngang)
    /// </summary>
    private bool CheckU_Z_Vertical(int r1, int c1, int r2, int c2)
    {
        // Quét tất cả các hàng từ 0 đến totalRows - 1 (bao gồm cả viền padding)
        for (int r = 0; r < totalRows; r++)
        {
            if (r == r1 || r == r2) continue; // Bỏ qua hàng của A và B

            if (boardData[r, c1] == 0 && boardData[r, c2] == 0)
            {
                // Kẻ tia dọc từ A -> (r, c1), tia ngang từ (r, c1) -> (r, c2), tia dọc từ (r, c2) -> B
                if (CheckLineY(c1, r1, r) && CheckLineX(r, c1, c2) && CheckLineY(c2, r2, r))
                {
                    return true;
                }
            }
        }
        return false;
    }
}

[System.Serializable]
public struct Point
{
    public int x; // Cột
    public int y; // Hàng
    public Point(int x, int y) { this.x = x; this.y = y; }
}