using UnityEngine;

/// <summary>
/// Put this on the child object that has the MeshRenderer with the outline material
/// already assigned as its second material slot (as set up in the inspector).
/// At Awake it captures that [base, outline] array, then SetHighlighted swaps between
/// showing just the base material and showing both.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class Highlightable : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material[] materialsWithOutline;
    private Material[] materialsWithoutOutline;
    private bool isHighlighted;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Whatever you set up in the inspector — expected to be [base, outline].
        materialsWithOutline = meshRenderer.sharedMaterials;

        materialsWithoutOutline = new Material[materialsWithOutline.Length - 1];
        for (int i = 0; i < materialsWithoutOutline.Length; i++)
        {
            materialsWithoutOutline[i] = materialsWithOutline[i];
        }

        meshRenderer.materials = materialsWithoutOutline;
        isHighlighted = false;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (isHighlighted == highlighted) return;
        isHighlighted = highlighted;
        meshRenderer.materials = highlighted ? materialsWithOutline : materialsWithoutOutline;
    }
}