using OpenTK.Mathematics;

namespace LibUC2.Schema;

internal static class Helpers
{
    public static float[] ParseFloats(this string input, int expectedCount)
    {
        if (string.IsNullOrEmpty(input))
            return new float[expectedCount];
            
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new float[expectedCount];
        
        for (int i = 0; i < expectedCount; i++)
        {
            if (i < parts.Length && float.TryParse(parts[i], out float value))
            {
                result[i] = value;
            }
            else
            {
                result[i] = 0.0f; // Default value
            }
        }
        
        return result;
    }
}

public class World : IParsable
{
    public string Version;
    public float[] TerrainHeight;
    public CoasterData Coaster;

    public Vector3 CameraStartPosition;
    public Vector3 CameraStartDirection;
    public Vector3 CameraStartUp;
    
    public string Name;
    public float SunAngle0;
    public float TimeOfDay;
    public int Fog;

    public bool EnableWater;
    public Color4 WaterColor;
    public float WaterHeight;
    
    public List<SceneryItem> SceneryItems;

    public float GetHeightAtCoord(int r, int c)
    {
        return TerrainHeight[c + r * UC2.VertsPerSide];
    }
    
    public float GetHeightAtFloatCoord(float x, float z)
    {
        const float a = 427.25f;
        float i_f = (x + a) / (2 * a) * 127.0f;
        float j_f = (z + a) / (2 * a) * 127.0f;

        i_f = Math.Clamp(i_f, 0.0f, 127.0f);
        j_f = Math.Clamp(j_f, 0.0f, 127.0f);

        int i0 = Math.Max((int)Math.Floor(i_f), 0);
        int j0 = Math.Max((int)Math.Floor(j_f), 0);
        int i1 = Math.Min(i0 + 1, 127);
        int j1 = Math.Min(j0 + 1, 127);

        float tx = i_f - i0;
        float tz = j_f - j0;

        float h00 = TerrainHeight[j0 * 128 + i0];
        float h10 = TerrainHeight[j0 * 128 + i1];
        float h01 = TerrainHeight[j1 * 128 + i0];
        float h11 = TerrainHeight[j1 * 128 + i1];

        float hx0 = lerp(h00, h10, tx);
        float hx1 = lerp(h01, h11, tx);

        return lerp(hx0, hx1, tz);
    }
    
    private static float lerp(float a, float b, float t) => a + (b - a) * t;
    
    public void Parse(string text)
    {
        var node = UCONNode.ParseBaseNode(text);
        node.TryGetString("version", out Version);
        node.TryGetFloatTuple("terrain", out var terrain);
        
        // unfortunately color is useless because of the EVIL libgdx float packing uc2 uses
        // grrrrr

        var terrainLen = terrain.Length - 2;
        
        // the last 2 terrain elements are water enable and water height??? wtf?
        EnableWater = terrain[terrain.Length - 2] > 0.5f;
        WaterHeight = terrain[terrain.Length - 1];
        
        var heightmapLen = terrainLen / 2;
        TerrainHeight = new float[heightmapLen];

        for (var i = 0; i < heightmapLen; i++)
        {
            TerrainHeight[i] = terrain[i * 2];
        }
        
        node.TryGetParseable("coaster", out Coaster);
        
        // Parse scenery arrays
        node.TryGetArray("scenery", out var scenery);
        node.TryGetArray("scenery2", out var scenery2);
        node.TryGetArray("scenery3", out var scenery3);
        node.TryGetArray("scenerySc", out var scenerySc);
        node.TryGetArray("sceneryER", out var sceneryER);
        node.TryGetArray("sceneryCR", out var sceneryCR);
        node.TryGetArray("sceneryCG", out var sceneryCG);
        node.TryGetArray("sceneryCB", out var sceneryCB);
        node.TryGetArray("sceneryCA", out var sceneryCA);

        // Parse and combine scenery items
        SceneryItems = ParseSceneryItems(
            scenery, scenery2, scenery3, scenerySc, sceneryER, 
            sceneryCR, sceneryCG, sceneryCB, sceneryCA
        );

        float[] tuple = [];
        node.TryGetFloatTuple("camera_start_position", out tuple);
        CameraStartPosition = new Vector3(tuple[0], tuple[1], tuple[2]);
        node.TryGetFloatTuple("camera_start_direction", out tuple);
        CameraStartDirection = new Vector3(tuple[0], tuple[1], tuple[2]);
        node.TryGetFloatTuple("camera_start_up", out tuple);
        CameraStartUp = new Vector3(tuple[0], tuple[1], tuple[2]);
        
        node.TryGetString("name", out Name);
        node.TryGetFloat("sun_angle0", out SunAngle0);
        node.TryGetFloat("time_of_day", out TimeOfDay);
        node.TryGetInt("fog", out Fog);
        
        node.TryGetFloatTuple("water_color", out tuple);
        WaterColor = new Color4(tuple[0], tuple[1], tuple[2], 1.0f);
    }
    
    private List<SceneryItem> ParseSceneryItems(
        string[] scenery, string[] scenery2, string[] scenery3,
        string[] scenerySc, string[] sceneryER,
        string[] sceneryCR, string[] sceneryCG,
        string[] sceneryCB, string[] sceneryCA)
    {
        var items = new List<SceneryItem>();
        
        // The number of scenery items is determined by the 'scenery' array
        int itemCount = scenery?.Length ?? 0;
        
        for (int i = 0; i < itemCount; i++)
        {
            var item = new SceneryItem();
            var pos = new Vector3();
            
            if (i < scenery.Length && !string.IsNullOrEmpty(scenery[i]))
            {
                var values = scenery[i].ParseFloats( 3);
                pos.X = values[0];
                item.Type = (SceneryType)(int)values[1];
                pos.Z = values[2];
            }
            
            if (scenery2 != null && i < scenery2.Length && !string.IsNullOrEmpty(scenery2[i]))
            {
                var values = scenery2[i].ParseFloats(3);
                pos.Y = values[0];
                // values[1] and values[2] are unused/ignored
            }
            
            if (scenery3 != null && i < scenery3.Length && !string.IsNullOrEmpty(scenery3[i]))
            {
                var values = scenery3[i].ParseFloats(3);
                item.Texture = (TextureType)(int)values[0];
                item.TextureScale = values[1];
                // values[2] is unused/ignored
            }
            
            if (scenerySc != null && i < scenerySc.Length && !string.IsNullOrEmpty(scenerySc[i]))
            {
                var values = scenerySc[i].ParseFloats(3);
                item.Scale = new Vector3(values[0], values[1], values[2]);
            }
            
            if (sceneryER != null && i < sceneryER.Length && !string.IsNullOrEmpty(sceneryER[i]))
            {
                var values = sceneryER[i].ParseFloats(3);
                item.EulerYaw = values[0];
                item.EulerPitch = values[1];
                item.EulerRoll = values[2];
            }
            
            if (sceneryCR != null && i < sceneryCR.Length && !string.IsNullOrEmpty(sceneryCR[i]))
            {
                var values = sceneryCR[i].ParseFloats(4);
                item.Color1 = new Color4(values[0], values[1], values[2], values[3]);
            }
            
            if (sceneryCG != null && i < sceneryCG.Length && !string.IsNullOrEmpty(sceneryCG[i]))
            {
                var values = sceneryCG[i].ParseFloats(4);
                item.Color2 = new Color4(values[0], values[1], values[2], values[3]);
            }
            
            if (sceneryCB != null && i < sceneryCB.Length && !string.IsNullOrEmpty(sceneryCB[i]))
            {
                var values = sceneryCB[i].ParseFloats(4);
                item.Color3 = new Color4(values[0], values[1], values[2], values[3]);
            }
            
            if (sceneryCA != null && i < sceneryCA.Length && !string.IsNullOrEmpty(sceneryCA[i]))
            {
                var values = sceneryCA[i].ParseFloats(4);
                item.Color4 = new Color4(values[0], values[1], values[2], values[3]);
            }

            item.Position = pos;
            
            items.Add(item);
        }
        
        return items;
    }
}

public class SceneryItem
{
    public Vector3 Position;
    public SceneryType Type { get; set; }
    public TextureType Texture { get; set; }
    public float TextureScale { get; set; }
    
    public Vector3 Scale;
    public float EulerYaw { get; set; }
    public float EulerPitch { get; set; }
    public float EulerRoll { get; set; }
    
    public Color4 Color1 { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 Color2 { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 Color3 { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
    public Color4 Color4 { get; set; } = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
}

public enum SceneryType
{
    SmallRock = 0,
    MediumRock = 1,
    BigRock = 2,
    OakTree = 3,
    PalmTree = 4,
    FirTree = 5,
    Shed1 = 6,
    Shed2 = 7,
    Shed3 = 8,
    SupportTube = 9,
    BoxFooter = 10,
    // 11 is unused and will cause file load failure
    Cylinder = 12,
    Cube = 13,
    Sphere = 14
}

public enum TextureType
{
    None = 0,
    Grain = 1,
    Brick = 2,
    Masonry = 3
}

public class BankedBezier : IParsable
{
    public Vector3[] ControlPoints = new Vector3[4];
    public bool DisableRolls;

    public void Parse(string text)
    {
        var node = UCONNode.ParseBaseNode(text);
        
        node.TryGetArray("controlPoints", out var controlPointsArray);
        for (int i = 0; i < 4 && i < controlPointsArray.Length; i++)
        {
            var values = controlPointsArray[i].ParseFloats(3);
            ControlPoints[i] = new Vector3(values[0], values[1], values[2]);
        }
        
        node.TryGetBool("disableRolls", out DisableRolls);
    }
}

public class CoasterSegment : IParsable
{
    public Vector3 StartPoint;
    public Vector3 StartDerivative;
    public Vector3 StartNormal;
    public BankedBezier PrimaryBezier;
    public int Shape;
    public int Type;
    public int ColorScheme;
    public float StraightLength;
    public float ClassicYaw;
    public float ClassicPitch;
    public float ClassicLen;
    public float FreeformStartAngleWeight;
    public float FreeformEndAngleWeight;
    public float FreeformManualHorizontalAngle;
    public float FreeformManualVerticalAngle;
    public bool FreeformUseAdvanced;
    public bool FreeformKeepHorizontallyStraight;
    public bool FreeformKeepVerticallyStraight;
    public bool KeepPrimaryBezier;
    public float VerticalLoopLength;
    public float VerticalLoopHorizontalShift;
    public float CobraRollLength;
    public float CobraRollHorizontalShift;
    public float CobraRollTopWideness;
    public float CorkscrewHeight;
    public float CorkscrewHorizontalShift;
    public float CorkscrewLength;
    public float CorkscrewLongitudinalShift;
    public float EndBanking;
    public bool ContinuousRoll;
    public bool DisableRolls;
    public int EndBankingStrategy;
    public float ChainSpeed;
    public float BrakeDecel;
    public float BrakeHoldTime;
    public float BrakeSpeed;
    public float LaunchAccel;
    public float LaunchHoldTime;
    public int LaunchType;
    public float LaunchSpeed;
    public bool BlockSection;
    public bool BlockSectionChain;
    public bool RotationFree;
    public bool RotationRelative;
    public bool RotationReduceSpinning;
    public float EndRotationAngle;
    public bool VisualUseDefaults;
    public int VisualDoubleSpineSpines;
    public int VisualGirderTubes;
    public bool VisualTunnel;
    public bool VisualLeftCatwalk;
    public bool VisualRightCatwalk;
    public int VisualIntaminBrakeType;
    public int VisualPremierBrakeType;
    public int VisualBmBrakeType;
    public int VisualGenericBrakeType;
    public int VisualGenericLaunchType;
    public int VisualSpineThickness012;
    public int VisualSpineThickness0123;
    public int VisualSkyrocketBraceType;
    public int VisualIntaminBraceType;
    public int VisualSupportOptionsMetal;
    public int VisualSupportOptionsWood;
    public int VisualSegmentBrakeExtra;
    public int VisualSegmentLaunchExtra;
    public int VisualSegmentExtraOffset;
    public int VisualSegmentExtraSpacing;

    public void Parse(string text)
    {
        var node = UCONNode.ParseBaseNode(text);
        
        // Parse vectors
        float[] tuple;
        if (node.TryGetFloatTuple("startPoint", out tuple) && tuple.Length >= 3)
            StartPoint = new Vector3(tuple[0], tuple[1], tuple[2]);
            
        if (node.TryGetFloatTuple("startDerivative", out tuple) && tuple.Length >= 3)
            StartDerivative = new Vector3(tuple[0], tuple[1], tuple[2]);
            
        if (node.TryGetFloatTuple("startNormal", out tuple) && tuple.Length >= 3)
            StartNormal = new Vector3(tuple[0], tuple[1], tuple[2]);
        
        // Parse primary bezier
        if (node.TryGetParseable("primaryBezier", out BankedBezier bezier))
            PrimaryBezier = bezier;
        
        // Parse primitive properties
        node.TryGetInt("shape", out Shape);
        node.TryGetInt("type", out Type);
        node.TryGetInt("color_scheme", out ColorScheme);
        node.TryGetFloat("straight_length", out StraightLength);
        node.TryGetFloat("classic_yaw", out ClassicYaw);
        node.TryGetFloat("classic_pitch", out ClassicPitch);
        node.TryGetFloat("classic_len", out ClassicLen);
        node.TryGetFloat("freeform_start_angle_weight", out FreeformStartAngleWeight);
        node.TryGetFloat("freeform_end_angle_weight", out FreeformEndAngleWeight);
        node.TryGetFloat("freeform_manual_horizontal_angle", out FreeformManualHorizontalAngle);
        node.TryGetFloat("freeform_manual_vertical_angle", out FreeformManualVerticalAngle);
        node.TryGetBool("freeform_use_advanced", out FreeformUseAdvanced);
        node.TryGetBool("freeform_keep_horizontally_straight", out FreeformKeepHorizontallyStraight);
        node.TryGetBool("freeform_keep_vertically_straight", out FreeformKeepVerticallyStraight);
        node.TryGetBool("keep_primary_bezier", out KeepPrimaryBezier);
        node.TryGetFloat("vertical_loop_length", out VerticalLoopLength);
        node.TryGetFloat("vertical_loop_horizontal_shift", out VerticalLoopHorizontalShift);
        node.TryGetFloat("cobra_roll_length", out CobraRollLength);
        node.TryGetFloat("cobra_roll_horizontal_shift", out CobraRollHorizontalShift);
        node.TryGetFloat("cobra_roll_top_wideness", out CobraRollTopWideness);
        node.TryGetFloat("corkscrew_height", out CorkscrewHeight);
        node.TryGetFloat("corkscrew_horizontal_shift", out CorkscrewHorizontalShift);
        node.TryGetFloat("corkscrew_length", out CorkscrewLength);
        node.TryGetFloat("corkscrew_longitudinal_shift", out CorkscrewLongitudinalShift);
        node.TryGetFloat("end_banking", out EndBanking);
        node.TryGetBool("continuous_roll", out ContinuousRoll);
        node.TryGetBool("disable_rolls", out DisableRolls);
        node.TryGetInt("end_banking_strategy", out EndBankingStrategy);
        node.TryGetFloat("chain_speed", out ChainSpeed);
        node.TryGetFloat("brake_decel", out BrakeDecel);
        node.TryGetFloat("brake_hold_time", out BrakeHoldTime);
        node.TryGetFloat("brake_speed", out BrakeSpeed);
        node.TryGetFloat("launch_accel", out LaunchAccel);
        node.TryGetFloat("launch_hold_time", out LaunchHoldTime);
        node.TryGetInt("launch_type", out LaunchType);
        node.TryGetFloat("launch_speed", out LaunchSpeed);
        node.TryGetBool("block_section", out BlockSection);
        node.TryGetBool("block_section_chain", out BlockSectionChain);
        node.TryGetBool("rotation_free", out RotationFree);
        node.TryGetBool("rotation_relative", out RotationRelative);
        node.TryGetBool("rotation_reduce_spinning", out RotationReduceSpinning);
        node.TryGetFloat("end_rotation_angle", out EndRotationAngle);
        node.TryGetBool("visual_use_defaults", out VisualUseDefaults);
        node.TryGetInt("visual_double_spine_spines", out VisualDoubleSpineSpines);
        node.TryGetInt("visual_girder_tubes", out VisualGirderTubes);
        node.TryGetBool("visual_tunnel", out VisualTunnel);
        node.TryGetBool("visual_left_catwalk", out VisualLeftCatwalk);
        node.TryGetBool("visual_right_catwalk", out VisualRightCatwalk);
        node.TryGetInt("visual_intamin_brake_type", out VisualIntaminBrakeType);
        node.TryGetInt("visual_premier_brake_type", out VisualPremierBrakeType);
        node.TryGetInt("visual_bm_brake_type", out VisualBmBrakeType);
        node.TryGetInt("visual_generic_brake_type", out VisualGenericBrakeType);
        node.TryGetInt("visual_generic_launch_type", out VisualGenericLaunchType);
        node.TryGetInt("visual_spine_thickness_012", out VisualSpineThickness012);
        node.TryGetInt("visual_spine_thickness_0123", out VisualSpineThickness0123);
        node.TryGetInt("visual_skyrocket_brace_type", out VisualSkyrocketBraceType);
        node.TryGetInt("visual_intamin_brace_type", out VisualIntaminBraceType);
        node.TryGetInt("visual_support_options_metal", out VisualSupportOptionsMetal);
        node.TryGetInt("visual_support_options_wood", out VisualSupportOptionsWood);
        node.TryGetInt("visual_segment_brake_extra", out VisualSegmentBrakeExtra);
        node.TryGetInt("visual_segment_launch_extra", out VisualSegmentLaunchExtra);
        node.TryGetInt("visual_segment_extra_offset", out VisualSegmentExtraOffset);
        node.TryGetInt("visual_segment_extra_spacing", out VisualSegmentExtraSpacing);
    }
}

// Update CoasterData class to include segments
public class CoasterData : IParsable
{
    public string Type;
    public int NumCars;
    public List<CoasterSegment> Segments = new List<CoasterSegment>();
    public bool BmHyperThickSpine;
    public bool IntaminSparseSupports;
    public bool RmcTrussSupports;
    public Dictionary<string, Color4> Colors;
    public bool ClosedCircuit;
    public bool UseAdvancedColors;
    public float Gravity;
    public float Friction;
    public float StationWaitTime;
    public int GoalNumTrains;
    
    public void Parse(string text)
    {
        var node = UCONNode.ParseBaseNode(text);
        
        node.TryGetString("type", out Type);
        node.TryGetInt("numCars", out NumCars);
        
        // Parse segments array
        if (node.TryGetArray("segments", out var segmentsArray))
        {
            // The array contains size metadata and then individual segments
            // Format: [array][size:N, 0:[segment]{...}, 1:[segment]{...}, ...]
            for (int i = 0; i < segmentsArray.Length; i++)
            {
                var segmentNode = UCONNode.ParseBaseNode(segmentsArray[i]);
                if (segmentNode != null)
                {
                    var segment = new CoasterSegment();
                    segment.Parse(segmentsArray[i]);
                    Segments.Add(segment);
                }
            }
        }
        
        node.TryGetBool("bmhyperthickspine", out BmHyperThickSpine);
        node.TryGetBool("intaminSparseSupports", out IntaminSparseSupports);
        node.TryGetBool("rmcTrussSupports", out RmcTrussSupports);
        
        Colors = new Dictionary<string, Color4>();
        
        node.TryGetArray("colors_names", out var colorNames);
        node.TryGetArray("colors_values", out var colorValues);

        for (int i = 0; i < colorNames.Length; i++)
        {
            var name = colorNames[i];
            var vals = colorValues[i].ParseFloats(4);
            Colors[name] = new Color4(vals[0], vals[1], vals[2], vals[3]);
        }
        
        node.TryGetBool("closedCircuit", out ClosedCircuit);
        node.TryGetBool("useAdvancedColors", out UseAdvancedColors);
        node.TryGetFloat("gravity", out Gravity);
        node.TryGetFloat("friction", out Friction);
        node.TryGetFloat("stationWaitTime", out StationWaitTime);
        node.TryGetInt("goalNumTrains", out GoalNumTrains);
    }
}