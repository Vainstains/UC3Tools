using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Mathematics;
using VProto;

namespace LibUC3.Schema;

public class TerrainSettingsMsg : Message
{
    [ProtoField(1)]
    public QuantizedFloatArrayMsg Heightmap;

    [ProtoField(2)]
    private int m_verticesPerDirection;
    public int VerticesPerDirection
    {
        get => m_verticesPerDirection;
        private set => m_verticesPerDirection = value;
    }

    [ProtoField(3)]
    public float VertexSpacing;

    [ProtoField(6)]
    public float WaterYLevel;

    [ProtoField(7)]
    public ColorMsg WaterColor;

    [ProtoField(8)]
    public SkyboxSettingsMsg SkyboxSettings;

    // Private storage for JPEG bytes
    [ProtoField(5)]
    private byte[] m_tintmapBytes;

    // Public RGB888 bytes, automatically serialized/deserialized
    public byte[] TintmapRGB { get; private set; }

    /// <summary>
    /// Resets the terrain with a given number of vertices per direction.
    /// Automatically allocates a heightmap of the correct size.
    /// </summary>
    public void ResetTerrain(int vertsPerDirection, Color4 terrainColor)
    {
        VerticesPerDirection = vertsPerDirection;
        int totalPoints = (vertsPerDirection * 2 + 1) * (vertsPerDirection * 2 + 1);
        Heightmap.Clear(totalPoints);
        TintmapRGB = new byte[totalPoints * 3];
        var R = (byte)(Math.Clamp(terrainColor.R, 0, 1) * 255.0f);
        var G = (byte)(Math.Clamp(terrainColor.G, 0, 1) * 255.0f);
        var B = (byte)(Math.Clamp(terrainColor.B, 0, 1) * 255.0f);

        for (int i = 0; i < totalPoints; i++)
        {
            TintmapRGB[i * 3] = R;
            TintmapRGB[i * 3 + 1] = G;
            TintmapRGB[i * 3 + 2] = B;
        }
    }

    public void SetHeightAtCoord(int r, int c, float height)
    {
        var stride = (m_verticesPerDirection * 2 + 1);
        Heightmap[c + r * stride] = height;
    }
    
    public float GetHeightAtCoord(int r, int c)
    {
        var stride = (m_verticesPerDirection * 2 + 1);
        return Heightmap[c + r * stride];
    }

    public override void OnPreSerialize()
    {
        // Encode TintmapRGB to JPEG bytes before serialization
        if (TintmapRGB != null && TintmapRGB.Length > 0)
        {
            int size = (int)Math.Sqrt(TintmapRGB.Length / 3);
            using var bmp = new Bitmap(size, size, PixelFormat.Format24bppRgb);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int index = (y * size + x) * 3;
                    Color color = Color.FromArgb(
                        TintmapRGB[index],
                        TintmapRGB[index + 1],
                        TintmapRGB[index + 2]);
                    bmp.SetPixel(x, y, color);
                }
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            m_tintmapBytes = ms.ToArray();
        }
    }

    public override void OnPostDeserialize()
    {
        // Decode JPEG bytes into TintmapRGB
        if (m_tintmapBytes != null && m_tintmapBytes.Length > 0)
        {
            using var ms = new MemoryStream(m_tintmapBytes);
            using var bmp = new Bitmap(ms);
            int width = bmp.Width;
            int height = bmp.Height;

            TintmapRGB = new byte[width * height * 3];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int index = (y * width + x) * 3;
                    TintmapRGB[index] = c.R;
                    TintmapRGB[index + 1] = c.G;
                    TintmapRGB[index + 2] = c.B;
                }
            }
        }
    }
}