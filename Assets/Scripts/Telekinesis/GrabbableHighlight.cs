using UnityEngine;

public class GrabbableHighlight : MonoBehaviour
{
    [SerializeField] private Color highlightColor = Color.cyan;
    [SerializeField, Range(0f, 1f)]
    private float highlightStrength = 1f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private Color[][] originalColors;
    private int[][] colorPropertyIDs;

    private bool isHighlighted;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        originalColors = new Color[renderers.Length][];
        colorPropertyIDs = new int[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].sharedMaterials;

            originalColors[i] = new Color[materials.Length];
            colorPropertyIDs[i] = new int[materials.Length];

            for (int j = 0; j < materials.Length; j++)
            {
                Material mat = materials[j];

                int colorID = mat.HasProperty("_BaseColor")
                    ? Shader.PropertyToID("_BaseColor")
                    : Shader.PropertyToID("_Color");

                colorPropertyIDs[i][j] = colorID;

                if (mat.HasProperty(colorID))
                    originalColors[i][j] = mat.GetColor(colorID);
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
            for (int j = 0; j < originalColors[i].Length; j++)
            {
                renderers[i].GetPropertyBlock(propertyBlock, j);

                int colorID = colorPropertyIDs[i][j];

                Color targetColor = enabled
                    ? Color.Lerp(
                        originalColors[i][j],
                        highlightColor,
                        highlightStrength)
                    : originalColors[i][j];

                propertyBlock.SetColor(colorID, targetColor);
                renderers[i].SetPropertyBlock(propertyBlock, j);
            }
        }
    }

    private void OnDisable()
    {
        if (isHighlighted)
            SetHighlight(false);
    }
}
