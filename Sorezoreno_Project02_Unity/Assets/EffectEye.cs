using UnityEngine;

[RequireComponent(typeof(Camera)),ExecuteInEditMode,ImageEffectAllowedInSceneView]

public class EffectEye : MonoBehaviour
{
    [SerializeField] private Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
