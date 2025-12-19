using OpenTK.Mathematics;

namespace LibUC2;

public static class UC2
{
    public const float VertexSpacing = 6.7283464566f;
    public const float TerrainCorrectionFactor = 1.1153f; // found through trial and error
    public static readonly Vector3 OddEvenCompensationOffset = new Vector3(-VertexSpacing * 0.5f, 0, -VertexSpacing * 0.5f);
    public const int VertsPerDirection = 64;
    public const int VertsPerSide = VertsPerDirection * 2;
    public static Schema.World LoadWorld(string path)
    {
        var text = File.ReadAllText(path);
        var world = new Schema.World();
        world.Parse(text);
        return world;
    }
}

// IDK what to call uc2's format, so I'll call it *UCON* because ultimate coaster object notation