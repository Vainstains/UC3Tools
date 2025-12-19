using System.Net.Sockets;
using LibUC2;
using LibUC2.Schema;
using LibUC3;
using LibUC3.Schema;
using OpenTK.Mathematics;

namespace UC3Tools;

public static class UC2ToUC3Converter
{
    private static readonly Dictionary<string, string> CoasterTypesMap = new()
    {
        { "arrow", "ar_corkscrew" },
        { "arrow4d", "bm_wing" },
        { "arrowhyper", "ar_corkscrew" },
        { "bmdive6", "bm_dive_10" },
        { "bmdive8", "bm_dive_10" },
        { "bmdive10", "bm_dive_10" },
        { "bmfloorless", "bm_floorless" },
        { "bmflyer", "bm_flyer" },
        { "bmflyerlead", "bm_flyer" },
        { "bmhyper", "bm_hyper" },
        { "bminvert", "bm_invert" },
        { "bmsitdown", "bm_sitdown" },
        { "bmstaggered", "bm_hyper" },
        { "bmstandup", "bm_sitdown" },
        { "bmwing", "bm_wing" },
        { "flyingdutchman", "vk_dutchman" },
        { "gci", "ptc2" },
        { "gerstlauerinfinity", "in_blitz_lapbar" },
        { "girderblitzlapbar", "in_blitz_lapbar" },
        { "girderblitztosr", "in_rocket_otsr" },
        { "girderinvert", "in_impulse" },
        { "hypergtx", "ar_corkscrew" },
        { "intamin2spine", "in_blitz_lapbar" },
        { "intamin2spinelapbar", "in_blitz_lapbar" },
        { "intamin4", "in_rocket_lapbar" },
        { "intaminrocket", "in_rocket_lapbar" },
        { "newekoma", "in_blitz_lapbar" },
        { "premierlim", "pr_lim" },
        { "ptc2", "ptc2" },
        { "ptc3", "ptc3" },
        { "raptor", "rmc_raptor" },
        { "rmc", "rmc_hybrid_modern" },
        { "schwarzkopf", "in_rocket_lapbar" },
        { "slc", "vk_slc" },
        { "sr3", "pr_sr3" },
        { "stc", "vk_stc" },
        { "vekoma", "ar_corkscrew" },
        { "xtremespinner", "in_blitz_lapbar" }
    };
    public struct UC2ToUC3ConverterArgs
    {
        public string Path;
        public string Output;
    }

    public static void Convert(UC2ToUC3ConverterArgs args)
    {
        var twoWorld = UC2.LoadWorld(args.Path);
        var threeWorld = UC3.CreateBlankWorld();
        
        var terrain = threeWorld.World.Terrain;
        terrain.ResetTerrain(UC2.VertsPerDirection, new Color4(20, 117, 65, 255));

        var uc2SideLength = UC2.VertsPerSide;
        for (var r = 0; r < uc2SideLength; r++)
        {
            for (var c = 0; c < uc2SideLength; c++)
            {
                var h = twoWorld.GetHeightAtCoord(c, r);
                terrain.SetHeightAtCoord(r, c, h);
                
                // compensate for uc3 having odd numbered edges (one more than uc2)
                if (r == uc2SideLength - 1)
                    terrain.SetHeightAtCoord(r + 1, c, h);
                if (c == uc2SideLength - 1)
                    terrain.SetHeightAtCoord(r, c + 1, h);
                if (r == uc2SideLength - 1 && c == uc2SideLength - 1)
                    terrain.SetHeightAtCoord(r + 1, c + 1, h);
            }
        }
        
        terrain.WaterYLevel = twoWorld.WaterHeight;
        terrain.WaterColor = twoWorld.WaterColor;
        if (!twoWorld.EnableWater)
            terrain.WaterYLevel = -2;
        
        terrain.VertexSpacing = UC2.VertexSpacing * UC2.TerrainCorrectionFactor;

        threeWorld.World.InitialCameraPos = twoWorld.CameraStartPosition + UC2.OddEvenCompensationOffset;

        Vector3 camDir = twoWorld.CameraStartDirection;
        
        // roll will always be 0
        threeWorld.World.InitialCameraPitch = MathF.Asin(camDir.Y);
        threeWorld.World.InitialCameraYaw = MathF.Atan2(camDir.X, camDir.Z);


        var threeScenery = new List<SceneryItemInstanceSettingsMsg>();
        
        foreach (var sceneryItem in twoWorld.SceneryItems)
        {
            threeScenery.Add(ConvertSceneryItem(sceneryItem, twoWorld));
        }

        threeWorld.World.SceneryState.ItemSettings = threeScenery;

        
        var coaster = UC3.CreateCoaster(twoWorld.Name);
        
        BuildCoaster(coaster, twoWorld.Coaster);
        threeWorld.World.Coasters = [coaster];
        
        File.WriteAllBytes(args.Output, threeWorld.SerializeToBytes());
    }

    private static void BuildCoaster(CoasterSettingsMsg coaster, CoasterData twoCoaster)
    {
        var colorSchemes = new CoasterSegmentColorSchemeSettingsMsg[5];
        for (var i = 0; i < 5; i++)
        {
            var indexStr = i > 0 ? $":{i + 1}" : "";
            var scheme = new CoasterSegmentColorSchemeSettingsMsg()
            {
                LeftRailColor = twoCoaster.Colors[$"track_rails{indexStr}"],
                RightRailColor = twoCoaster.Colors[$"track_rails{indexStr}"],
                SpineColor = twoCoaster.Colors[$"track_spine_1{indexStr}"],
                SpineAltColor = twoCoaster.Colors[$"track_spine_2{indexStr}"],
                BraceColor = twoCoaster.Colors[$"track_brace{indexStr}"],
                CatwalkColor = twoCoaster.Colors["catwalk"],
                MachineryColor = Color4.DarkGray,
                MagneticFinColor = twoCoaster.Colors["fin"],
                SupportColor = twoCoaster.Colors["supports"],
                SupportFooterColor = Color4.LightSlateGray,
                TunnelColor = twoCoaster.Colors["tunnel"]
            };
            if (!twoCoaster.UseAdvancedColors)
            {
                scheme.LeftRailColor = scheme.RightRailColor = scheme.BraceColor = 
                    scheme.SpineColor = scheme.SpineAltColor = twoCoaster.Colors["track"];
            }
            scheme.UseSimpleEditor = !twoCoaster.UseAdvancedColors;
            colorSchemes[i] = scheme;
        }

        coaster.DefaultSegmentColorScheme = colorSchemes[0];
        
        coaster.MaxNumTrains = twoCoaster.GoalNumTrains;
        coaster.NumCarsWithSeatsInTrain = twoCoaster.NumCars;
        coaster.TypeId = CoasterTypesMap[twoCoaster.Type];

        var part = coaster.PartGroups[0].Part;

        part.IsClosedCircuit = twoCoaster.ClosedCircuit;
        if (part.IsClosedCircuit)
        {
            part.InitialStartConnection = new CoasterPartSettingsMsg.ConnectNodeSettingsMsg()
            {
                PartUuid = part.Uuid,
                IsStartOfPart = false
            };
            part.InitialEndConnection = new CoasterPartSettingsMsg.ConnectNodeSettingsMsg()
            {
                PartUuid = part.Uuid,
                IsStartOfPart = true
            };
        }

        var nurbsPoints = part.Curves[0].NurbsSequence.ControlPoint;
        var bankNodes = part.Curves[0].BankNodes;
        var segmentNodes = part.Curves[0].SegmentNodes;
        
        nurbsPoints.Clear();
        bankNodes.Clear();
        segmentNodes.Clear();

        part.Curves[0].ShapeTypeId = "NURBS";
        
        for (var i = 0; i < twoCoaster.Segments.Count; i++)
        {
            var seg = twoCoaster.Segments[i];
            
            var bez0 = seg.PrimaryBezier.ControlPoints[0] + UC2.OddEvenCompensationOffset;
            var bez1 = seg.PrimaryBezier.ControlPoints[1] + UC2.OddEvenCompensationOffset;
            var bez2 = seg.PrimaryBezier.ControlPoints[2] + UC2.OddEvenCompensationOffset;
            var bez3 = seg.PrimaryBezier.ControlPoints[3] + UC2.OddEvenCompensationOffset;

            var endBank = seg.EndBanking;
            var continuous = seg.ContinuousRoll;
            // TODO: Add relative banking

            if (i == 0)
            {
                nurbsPoints.AddPoint(bez0, strict: true);
                bankNodes.Add(new()
                {
                    Angle = 0,
                    ContinuousRoll = false,
                    DisableRolls = true,
                    ControlPointIndex = 0,
                    HeartlinePercentage = 1,
                    Relative = false,
                    Smoothness = 1
                });
            }
            
            segmentNodes.Add(CreateSegmentNode(nurbsPoints.Count - 1, seg,
                twoCoaster.UseAdvancedColors ? colorSchemes : null));
            
            nurbsPoints.AddPoint(bez1);
            nurbsPoints.AddPoint(bez2);
            nurbsPoints.AddPoint(bez3, strict: true);
            
            const float Deg2Rad = 0.01745329f;
            bankNodes.Add(new()
            {
                Angle = seg.EndBanking * Deg2Rad,
                ContinuousRoll = seg.ContinuousRoll,
                ControlPointIndex = nurbsPoints.Count - 1,
                HeartlinePercentage = 1,
                Relative = false,
                Smoothness = 1
            });
        }
    }

    private static SegmentNodeSettingsMsg CreateSegmentNode(int controlPointId, CoasterSegment twoSeg,
        CoasterSegmentColorSchemeSettingsMsg[]? colorSchemes)
    {
        var threeSeg = new SegmentNodeSettingsMsg();
        threeSeg.ControlPointIndex = controlPointId;
        threeSeg.Uuid = Guid.NewGuid().ToString();
        threeSeg.ChainLiftSettings = new()
        {
            Accel = 3,
            Speed = twoSeg.ChainSpeed
        };
        threeSeg.StationSettings = new()
        {
            LaunchSpeed = 2,
            LaunchAccel = 4 * 9.81f,
            BrakeSpeed = 2,
            BrakeDecel = 4 * 9.81f,
            StationLaunchMode = SegmentNodeSettingsMsg.StationSettingsMsg.LaunchMode.Forward
        };
        threeSeg.BrakeSettings = new()
        {
            BrakeDecel = twoSeg.BrakeDecel * 9.81f,
            BrakeSpeed = twoSeg.BrakeSpeed,
            IsBlockSection = twoSeg.BlockSection
        };
        threeSeg.LaunchSettings = new()
        {
            LaunchSpeed = twoSeg.LaunchSpeed,
            LaunchAccel = twoSeg.LaunchAccel * 9.81f,
            BrakeSpeed = 2,
            BrakeDecel = 4 * 9.81f,
            LaunchModeValue = SegmentNodeSettingsMsg.LaunchSettingsMsg.LaunchMode.Standard
        };

        if (colorSchemes != null)
        {
            threeSeg.OverrideColorSettings = colorSchemes[twoSeg.ColorScheme - 1];
        }

        threeSeg.SegmentType = twoSeg.Type switch
        {
            0 => SegmentNodeSettingsMsg.Type.Standard,
            1 => SegmentNodeSettingsMsg.Type.Brake,
            2 => SegmentNodeSettingsMsg.Type.Launch,
            3 => SegmentNodeSettingsMsg.Type.Brake,
            4 => SegmentNodeSettingsMsg.Type.Station,
            _ => SegmentNodeSettingsMsg.Type.Standard
        };
        
        return threeSeg;
    }

    private static SceneryItemInstanceSettingsMsg ConvertSceneryItem(SceneryItem sceneryItem, World world, bool useBillboardTrees = true)
    {
        var threeItem = new SceneryItemInstanceSettingsMsg();

        threeItem.TerrainAttached = true;

        threeItem.OriginX = sceneryItem.Position.X + UC2.OddEvenCompensationOffset.X;
        threeItem.OriginY = sceneryItem.Position.Y;
        threeItem.OriginY += world.GetHeightAtFloatCoord(
            sceneryItem.Position.X / UC2.TerrainCorrectionFactor, 
            sceneryItem.Position.Z / UC2.TerrainCorrectionFactor);
        threeItem.OriginZ = sceneryItem.Position.Z + UC2.OddEvenCompensationOffset.Z;

        threeItem.ScaleX = sceneryItem.Scale.X;
        threeItem.ScaleY = sceneryItem.Scale.Y;
        threeItem.ScaleZ = sceneryItem.Scale.Z;
        
        const float Deg2Rad = MathF.PI / 180;
        threeItem.RotationPitch = sceneryItem.EulerPitch * Deg2Rad;
        threeItem.RotationYaw = sceneryItem.EulerYaw * Deg2Rad;
        threeItem.RotationRoll = sceneryItem.EulerRoll * Deg2Rad;

        string oakTree = "com.phonygames.coasterthree.scenery.trees.oaktree0";
        string firTree = "com.phonygames.coasterthree.scenery.trees.firtree0";
        string palmTree = "com.phonygames.coasterthree.scenery.trees.palmtree0";

        if (useBillboardTrees)
        {
            oakTree = "com.phonygames.coasterthree.scenery.billboardtrees.oaktree0";  
            firTree = "com.phonygames.coasterthree.scenery.billboardtrees.firtree0";  
            palmTree = "com.phonygames.coasterthree.scenery.billboardtrees.palmtree0";
        }
        
        var type = sceneryItem.Type;
        switch (type)
        {
            case SceneryType.Cube:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.primitives.cube";
                break;
            case SceneryType.Cylinder:
            {
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.primitives.cylinder";
                
                float radiusX = threeItem.ScaleX;
                float radiusZ = threeItem.ScaleY;
                float length  = threeItem.ScaleZ;

                threeItem.ScaleX = radiusX;
                threeItem.ScaleY = length;
                threeItem.ScaleZ = radiusZ;
                
                var rotYaw = Quaternion.FromAxisAngle(Vector3.UnitY, threeItem.RotationYaw);
                var rotPitch = Quaternion.FromAxisAngle(Vector3.UnitX, threeItem.RotationPitch);
                var rotRoll = Quaternion.FromAxisAngle(Vector3.UnitZ, threeItem.RotationRoll);

                var uc2Rot = rotYaw * rotPitch * rotRoll;
                
                var halfLengthOffset = new Vector3(0f, 0f, length * -0.5f);
                var rotatedOffset = Vector3.Transform(halfLengthOffset, uc2Rot);
                threeItem.OriginX += rotatedOffset.X;
                threeItem.OriginY += rotatedOffset.Y;
                threeItem.OriginZ += rotatedOffset.Z;
                
                // calculate yaw and pitch based off the offset as a sort of "look direction"
                threeItem.RotationYaw = MathF.Atan2(rotatedOffset.X, rotatedOffset.Z);
                threeItem.RotationPitch   = MathF.Asin(rotatedOffset.Y) + MathF.PI * 0.5f;
                threeItem.RotationRoll  = 0; // frick you uc2 cylinder for being different and therefore difficult :(
                
                break;
            }
            case SceneryType.Sphere:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.primitives.sphere";
                break;
            case SceneryType.SmallRock:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.rocks.Srock0";
                threeItem.ScaleX *= 2.3f;
                threeItem.ScaleY *= 2.3f;
                threeItem.ScaleZ *= 2.3f;
                break;
            case SceneryType.MediumRock:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.rocks.Mrock0";
                threeItem.ScaleX *= 2.3f;
                threeItem.ScaleY *= 2.3f;
                threeItem.ScaleZ *= 2.3f;
                break;
            case SceneryType.BigRock:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.rocks.Lrock0";
                threeItem.ScaleX *= 2.3f;
                threeItem.ScaleY *= 2.3f;
                threeItem.ScaleZ *= 2.3f;
                break;
            case SceneryType.OakTree:
                threeItem.SceneryItemId = oakTree;
                threeItem.ScaleX *= 0.8f;
                threeItem.ScaleY *= 0.8f;
                threeItem.ScaleZ *= 0.8f;
                break;
            case SceneryType.FirTree:
                threeItem.SceneryItemId = firTree;
                threeItem.ScaleX *= 0.8f;
                threeItem.ScaleY *= 0.95f;
                threeItem.ScaleZ *= 0.8f;
                break;
            case SceneryType.PalmTree:
                threeItem.SceneryItemId = palmTree;
                threeItem.ScaleX *= 0.8f;
                threeItem.ScaleY *= 0.8f;
                threeItem.ScaleZ *= 0.8f;
                break;
            case SceneryType.SupportTube:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.supports.cylindertube";
                threeItem.ScaleX *= 0.25f;
                threeItem.ScaleY *= 0.25f;
                threeItem.ScaleZ *= 2.0f;
                break;
            case SceneryType.BoxFooter:
            {
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.primitives.cube";
                var moveVec = Vector3.UnitY * -4f * threeItem.ScaleY;
                threeItem.ScaleX *= 2.0f;
                threeItem.ScaleY *= 9.5f;
                threeItem.ScaleZ *= 2.0f;

                // uc3 and uc2 use a right-handed coordinate system; y-up
                // uc2 used the order yaw, pitch, roll
                var rotYaw = Quaternion.FromAxisAngle(Vector3.UnitY, threeItem.RotationYaw);
                var rotPitch = Quaternion.FromAxisAngle(Vector3.UnitX, threeItem.RotationPitch);
                var rotRoll = Quaternion.FromAxisAngle(Vector3.UnitZ, threeItem.RotationRoll);

                var itemRot = rotYaw * rotPitch * rotRoll;

                var rotatedMoveVec = Vector3.Transform(moveVec, itemRot);

                threeItem.OriginX += rotatedMoveVec.X;
                threeItem.OriginY += rotatedMoveVec.Y;
                threeItem.OriginZ += rotatedMoveVec.Z;
                break;
            }
            default:
                threeItem.SceneryItemId = "com.phonygames.coasterthree.scenery.primitives.cube";
                break;
        }

        var allChannels = new List<SceneryColorChannelSettingsMsg>()
        {
            new SceneryColorChannelSettingsMsg()
            {
                Color = sceneryItem.Color1,
                TextureScale = sceneryItem.TextureScale,
            },
            new SceneryColorChannelSettingsMsg()
            {
                Color = sceneryItem.Color2,
                TextureScale = sceneryItem.TextureScale,
            },
            new SceneryColorChannelSettingsMsg()
            {
                Color = sceneryItem.Color3,
                TextureScale = sceneryItem.TextureScale,
            },
            new SceneryColorChannelSettingsMsg()
            {
                Color = sceneryItem.Color4,
                TextureScale = sceneryItem.TextureScale,
            }
        };
        
        threeItem.ColorChannels = allChannels;
        return threeItem;
    }
}