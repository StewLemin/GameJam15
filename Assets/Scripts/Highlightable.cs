using UnityEngine;

// Script placed on the child objects that have the MeshRenderer with the outline material
// already assigned as its second material slot (set up in the inspector).
// At Awake it captures that [base, outline] array, then CapsuleSwitcher calls 
// SetHighlighted to swap between showing just the base material and showing both,
// Depending on whether the player is looking at this capsule or not.
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