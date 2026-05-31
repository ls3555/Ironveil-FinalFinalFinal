using UnityEngine;
using UnityEngine.Rendering;

public class FloorSortingSync : MonoBehaviour
{
    [Header("Match these to your Layer names in Project Settings")]
    public string layer1SortingLayer = "Layer 1";
    public string layer2SortingLayer = "Layer 2";
    public string layer3SortingLayer = "Layer 3";

    private SortingGroup _sortingGroup;
    private int _lastLayer = -1;

    void Start()
    {
        _sortingGroup = GetComponent<SortingGroup>();
        SyncSortingLayer();
    }

    void Update()
    {
        // Only update when the layer actually changes, not every frame
        if (gameObject.layer != _lastLayer)
        {
            SyncSortingLayer();
            _lastLayer = gameObject.layer;
        }
    }

    void SyncSortingLayer()
    {
        if (_sortingGroup == null) return;

        string layerName = LayerMask.LayerToName(gameObject.layer);

        if (layerName.Contains("1"))
            _sortingGroup.sortingLayerName = layer1SortingLayer;
        else if (layerName.Contains("2"))
            _sortingGroup.sortingLayerName = layer2SortingLayer;
        else if (layerName.Contains("3"))
            _sortingGroup.sortingLayerName = layer3SortingLayer;

        Debug.Log($"[FloorSortingSync] Layer: {layerName} → SortingLayer: {_sortingGroup.sortingLayerName}");
    }
}