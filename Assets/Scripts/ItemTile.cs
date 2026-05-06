using UnityEngine;
using UnityEngine.UI;

public class ItemTile : MonoBehaviour
{
    public int gridX, gridY;
    public int iconID;
    
    [SerializeField] private Image iconImage;
    [SerializeField] private Button tileButton;

    private BoardManager board;

    public void Setup(int x, int y, int id, Sprite sprite, BoardManager manager)
    {
        gridX = x;
        gridY = y;
        iconID = id;
        board = manager;

        if (iconImage != null)
        {
            iconImage.sprite = sprite;
        }

        tileButton.onClick.RemoveAllListeners();
        tileButton.onClick.AddListener(OnClickTile);
    }

    private void OnClickTile()
    {
        // Báo cho BoardManager biết ô này vừa bị click
        board.OnTileClicked(this);
    }

    // Hàm dùng để đổi màu khi được chọn
    public void SetHighlight(bool isSelected)
    {
        if (iconImage != null)
        {
            // Nếu được chọn thì đổi sang màu xám mờ, nếu không thì trả về màu trắng gốc
            iconImage.color = isSelected ? Color.gray : Color.white;
        }
    }
    /// <summary>
    /// Giữ nguyên GameObject để giữ chỗ trong Grid, chỉ tắt hình và khóa click
    /// </summary>
    public void HideTile()
    {
        if (iconImage != null)
        {
            iconImage.enabled = false; // Tắt hình ảnh (tàng hình)
        }
        
        if (tileButton != null)
        {
            tileButton.interactable = false; // Khóa nút bấm (không cho click)
        }
    }
}