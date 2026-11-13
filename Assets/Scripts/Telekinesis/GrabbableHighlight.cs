using UnityEngine;

public class GrabbableHighlight : MonoBehaviour
{
    [Header("Highlight Material")]
    [SerializeField] private Material highlightMaterial;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private Material[][] runtimeMaterials;

    private static readonly int HighlightEnabled =
        Shader.PropertyToID("_HighlightEnabled");

    private bool isHighlighted;

    private void Awake()
    {
        // Find all renderers on this object and its children.
        renderers = GetComponentsInChildren<Renderer>();

        // Store the original materials so we can restore them.
        originalMaterials = new Material[renderers.Length][];
        runtimeMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;

            runtimeMaterials[i] =
                new Material[originalMaterials[i].Length];

            // Create an individual highlight material for each slot.
            for (int j = 0; j < originalMaterials[i].Length; j++)
            {
                runtimeMaterials[i][j] =
                    new Material(highlightMaterial);

                // Start with the highlight disabled.
                runtimeMaterials[i][j].SetFloat(
                    HighlightEnabled, 0f
                );
            }
        }
    }

        
public void SetHighlight(bool enabled)
{
    if (isHighlighted == enabled)
        return;

    isHighlighted = enabled;

    for (int i = 0; i < renderers.Length; i++)
    {
        if (enabled)
        {
            // Tell the Shader Graph to switch to cyan.
            foreach (Material mat in runtimeMaterials[i])
            {
                mat.SetFloat(HighlightEnabled, 1f);
            }

            // Apply the highlight material.
            renderers[i].materials = runtimeMaterials[i];
        }
        else
        {
            // Restore the object's original materials.
            renderers[i].sharedMaterials = originalMaterials[i];
        }
    }
}


    private void OnDestroy()
    {
        // Clean up the materials created at runtime.
        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            for (int j = 0; j < runtimeMaterials[i].Length; j++)
            {
                if (runtimeMaterials[i][j] != null)
                    Destroy(runtimeMaterials[i][j]);
            }
        }
    }
}
