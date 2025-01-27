using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class OctagonalLayoutGroup : LayoutGroup
{
    [SerializeField] private float spacing = 1f;
    [SerializeField] private Vector2 cellSize = new Vector2(1f, 1f);
    [SerializeField] private bool centerCells = true;
    [SerializeField] private float rowOffset = 0.5f;
    [SerializeField] private bool extendAlternateRows = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        UpdateLayout();
    }

    public override void CalculateLayoutInputVertical()
    {
        UpdateLayout();
    }

    public override void SetLayoutHorizontal()
    {
        UpdateLayout();
    }

    public override void SetLayoutVertical()
    {
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (transform.childCount == 0) return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // Calculate base grid dimensions
        float effectiveSpacing = spacing;
        int baseColumns = Mathf.Max(1, Mathf.FloorToInt(width / (cellSize.x * effectiveSpacing)));

        // Calculate total cells needed and rows
        int totalCells = transform.childCount;
        int estimatedRows = Mathf.Max(1, Mathf.CeilToInt(totalCells / (float)baseColumns));

        // Calculate actual cells per row (accounting for extended rows)
        int mainRowCells = baseColumns;
        int extendedRowCells = baseColumns + 1; // One extra cell for alternate rows to ensure coverage

        // Recalculate rows needed based on varying row lengths
        int totalRows = 0;
        int remainingCells = totalCells;
        while (remainingCells > 0)
        {
            int currentRowCells = (totalRows % 2 == 1 && extendAlternateRows) ? extendedRowCells : mainRowCells;
            remainingCells -= currentRowCells;
            totalRows++;
        }

        // Calculate starting positions for centering
        float startY = (centerCells) ? (totalRows - 1) * cellSize.y * effectiveSpacing * 0.5f : 0f;

        int cellIndex = 0;

        // Position cells row by row
        for (int row = 0; row < totalRows; row++)
        {
            bool isExtendedRow = row % 2 == 1 && extendAlternateRows;
            int currentRowCells = isExtendedRow ? extendedRowCells : mainRowCells;

            // Calculate row-specific start X position
            float rowWidth = currentRowCells * cellSize.x * effectiveSpacing;
            float startX = centerCells ? -rowWidth * 0.5f : 0f;

            // Add offset for alternating rows
            if (isExtendedRow)
            {
                startX -= cellSize.x * effectiveSpacing * rowOffset; // Apply the configurable row offset
            }

            // Position cells in the current row
            for (int col = 0; col < currentRowCells; col++)
            {
                if (cellIndex >= transform.childCount) break;

                RectTransform child = transform.GetChild(cellIndex) as RectTransform;
                if (child == null) continue;

                float x = startX + col * cellSize.x * effectiveSpacing;
                float y = startY - row * cellSize.y * effectiveSpacing;

                child.localPosition = new Vector2(x, y);
                child.sizeDelta = cellSize;

                cellIndex++;
            }
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateLayout();
    }

    protected override void OnTransformChildrenChanged()
    {
        base.OnTransformChildrenChanged();
        UpdateLayout();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        spacing = Mathf.Max(0.1f, spacing);
        cellSize = new Vector2(Mathf.Max(0.1f, cellSize.x), Mathf.Max(0.1f, cellSize.y));
        UpdateLayout();
    }
#endif
}