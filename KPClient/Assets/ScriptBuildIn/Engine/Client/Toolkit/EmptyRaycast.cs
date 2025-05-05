using UnityEngine.UI;

public sealed class EmptyRaycast : MaskableGraphic
{
    private EmptyRaycast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
