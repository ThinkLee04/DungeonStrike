using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverlayWaterRiser : MonoBehaviour
{
    [Header("Water Object Settings")]
    public GameObject waterRectangleObject;
    public Color waterColor = new Color(0.2f, 0.5f, 1f, 0.5f);

    [Header("Level Geometry")]
    public Tilemap boundsReferenceTilemap;

    [Header("Timing")]
    public float timeBetweenRows = 20f;

    // --- PHẦN MỚI: CÀI ĐẶT THAY THẾ TILEMAP ---
    [Header("Tile Replacement Settings")]
    [Tooltip("Tilemap gốc sẽ bị thay đổi (ví dụ: Ground_Tilemap).")]
    public Tilemap sourceTilemapToReplace;

    [Tooltip("Tilemap chứa các tile phiên bản 'ngập nước'. Tilemap này có thể bị tắt (inactive).")]
    public Tilemap destinationTilemapWithNewTiles;

    // --- Biến nội bộ ---
    private SpriteRenderer waterSpriteRenderer;
    private int currentRow;
    private BoundsInt mapBounds;
    private Vector3 initialWaterPosition;
    private float mapWidthInUnits;
    private float tileHeightInUnits;

    void Start()
    {
        // --- Kiểm tra lỗi ---
        if (waterRectangleObject == null || boundsReferenceTilemap == null)
        {
            Debug.LogError("Chưa gán Water Rectangle Object hoặc Bounds Reference Tilemap! Vô hiệu hóa script.");
            this.enabled = false;
            return;
        }

        waterSpriteRenderer = waterRectangleObject.GetComponent<SpriteRenderer>();
        if (waterSpriteRenderer == null)
        {
            Debug.LogError("Đối tượng 'Water Rectangle Object' thiếu component SpriteRenderer!");
            this.enabled = false;
            return;
        }

        // --- Thiết lập ban đầu ---
        // Lấy ranh giới và kích thước của map
        boundsReferenceTilemap.CompressBounds();
        mapBounds = boundsReferenceTilemap.cellBounds;
        mapWidthInUnits = mapBounds.size.x * boundsReferenceTilemap.cellSize.x;
        tileHeightInUnits = boundsReferenceTilemap.cellSize.y;

        // Đặt màu và độ trong suốt cho nước
        waterSpriteRenderer.color = waterColor;
        // Đảm bảo SpriteRenderer được kích hoạt
        waterSpriteRenderer.enabled = true;

        // Tính toán vị trí bắt đầu
        float startX = boundsReferenceTilemap.CellToWorld(mapBounds.min).x + (mapWidthInUnits / 2);
        float startY = boundsReferenceTilemap.CellToWorld(mapBounds.min).y;
        initialWaterPosition = new Vector3(startX, startY, 0);

        // Đặt vị trí và kích thước ban đầu (chiều cao = 0)
        waterRectangleObject.transform.position = initialWaterPosition;
        waterRectangleObject.transform.localScale = new Vector3(mapWidthInUnits, 0, 1);

        // Bắt đầu từ hàng dưới cùng
        currentRow = mapBounds.yMin;
        ReplaceRowTiles(currentRow);

        // Bắt đầu Coroutine để dâng nước
        StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        // Vòng lặp chạy cho đến khi ngập hết map
        while (currentRow <= mapBounds.yMax)
        {
            // Đợi trong khoảng thời gian đã định
            yield return new WaitForSeconds(timeBetweenRows);

            // Cập nhật kích thước và vị trí của hình chữ nhật nước
            UpdateWaterRectangle();

            //Thay thế các tile của hàng hiện tại
            int nextRowToFlood = currentRow + 1;
            if (nextRowToFlood <= mapBounds.yMax)
            {
                ReplaceRowTiles(nextRowToFlood);
            }

            // Chuyển lên hàng tiếp theo để chuẩn bị cho lần sau
            currentRow++;
        }

        Debug.Log("Toàn bộ map đã bị ngập!");
    }

    private void UpdateWaterRectangle()
    {
        // Tính chiều cao mới của nước (số hàng đã ngập * chiều cao mỗi hàng)
        float currentHeightInUnits = (currentRow - mapBounds.yMin + 1) * tileHeightInUnits;

        // Cập nhật kích thước (scale) của hình chữ nhật
        waterRectangleObject.transform.localScale = new Vector3(mapWidthInUnits, currentHeightInUnits, 1);

        // Vì hình chữ nhật scale từ tâm, ta phải dịch chuyển nó lên trên một nửa chiều cao
        Vector3 newPosition = initialWaterPosition + new Vector3(0, currentHeightInUnits / 2, 0);
        waterRectangleObject.transform.position = newPosition;
    }
    private void ReplaceRowTiles(int y)
    {
        if (sourceTilemapToReplace == null || destinationTilemapWithNewTiles == null) return;
        for (int x = mapBounds.xMin; x < mapBounds.xMax; x++)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            TileBase newFloodedTile = destinationTilemapWithNewTiles.GetTile(tilePosition);
            sourceTilemapToReplace.SetTile(tilePosition, newFloodedTile);
        }
    }
    //===================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Nước đã chạm vào Player! GAME OVER.");
        }
    }
}