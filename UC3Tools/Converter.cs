using System.Globalization;
using LibUC3;
using OpenTK.Mathematics;

namespace UC3Tools;

public static class Converter
{
    public struct FVDConverterArgs
    {
        public string Path;
        public string Output;
        public float Heartlining;
    }
    
    public static void ConvertFVDCsvToUC3(FVDConverterArgs args)
    {
        var world = UC3.CreateBlankWorld();
        var coaster = UC3.CreateCoaster(Path.GetFileName(args.Path));
        var fvdData = FVDFrameParser.ParseFramesFromFile(args.Path);

        // Adjust heartline height
        for (var i = 0; i < fvdData.Count; i++)
        {
            var frame = fvdData[i];
            frame.Position -= frame.Up * args.Heartlining;
            fvdData[i] = frame;
        }

        // Fix frame basis vectors
        for (var i = 0; i < fvdData.Count; i++)
        {
            var frame = fvdData[i];
            var up = frame.Up;
            var newFwd = frame.Fwd;

            if (i < fvdData.Count - 1)
            {
                var nextFramePos = fvdData[i + 1].Position;
                var pos = fvdData[i].Position;
                newFwd = nextFramePos - pos;

                if (newFwd.LengthSquared > 1e-12f)
                    newFwd = Vector3.Normalize(newFwd);
                else
                    newFwd = Vector3.Normalize(frame.Fwd);
            }
            else if (i > 0)
            {
                var prevPos = fvdData[i - 1].Position;
                newFwd = Vector3.Normalize(frame.Position - prevPos);
            }

            var right = Vector3.Cross(up, newFwd);

            if (right.LengthSquared < 1e-12f)
            {
                right = Vector3.Cross(Vector3.UnitY, newFwd);
                if (right.LengthSquared < 1e-12f)
                    right = Vector3.Cross(Vector3.UnitZ, newFwd);
            }

            right = Vector3.Normalize(right);
            var newUp = Vector3.Normalize(Vector3.Cross(newFwd, right));

            frame.Fwd = newFwd;
            frame.Left = -right;
            frame.Up = newUp;
            fvdData[i] = frame;
        }

        var curve = coaster.PartGroups[0].Part.Curves[0];
        curve.BankNodes.Clear();
        curve.NurbsSequence.ControlPoint.Clear();

        void AddNode(Vector3 position, float bankAngle, bool relative)
        {
            position.Y += 10;
            int idx = curve.NurbsSequence.ControlPoint.Count;

            curve.NurbsSequence.ControlPoint.Add(new()
            {
                Position = position,
                Strict = false,
                Weight = 1.0f
            });
            
            curve.BankNodes.Add(new()
            {
                Angle = bankAngle,
                ContinuousRoll = true,
                ControlPointIndex = idx,
                Smoothness = 1.0f,
                HeartlinePercentage = 0.0f,
                Relative = relative, // Use the relative parameter
                DisableRolls = true
            });
        }

        var bankAngles = new float[fvdData.Count];
        var relativeBanking = new bool[fvdData.Count];
        float lastAngle = 0f;
        const float steepThresholdRadians = 80 * MathF.PI / 180f;

        for (int i = 0; i < fvdData.Count; i++)
        {
            float verticalAngle = MathF.Acos(MathF.Abs(Vector3.Dot(fvdData[i].Fwd, Vector3.UnitY)));
            
            if (verticalAngle > steepThresholdRadians)
            { // relative banking
                Vector3 referenceUp = (i > 0) ? fvdData[i - 1].Up : Vector3.UnitY;
                float angle = fvdData[i].CalculateRoll(referenceUp);
                relativeBanking[i] = true;
                
                bankAngles[i] = angle;
                lastAngle = angle;
            }
            else
            { // global banking
                float angle = fvdData[i].CalculateGlobalRoll();
                
                while (angle - lastAngle > MathF.PI) angle -= MathF.PI * 2;
                while (angle - lastAngle < -MathF.PI) angle += MathF.PI * 2;
                
                bankAngles[i] = angle;
                relativeBanking[i] = false;
                lastAngle = angle;
            }
        }

        for (int i = 0; i < fvdData.Count; i++)
        {
            AddNode(
                fvdData[i].Position,
                bankAngles[i],
                relativeBanking[i]
            );
        }

        world.World.Coasters = [coaster];
        File.WriteAllBytes(args.Output, world.SerializeToBytes());
    }
}

public struct FVDFrame(Vector3 position, Vector3 fwd, Vector3 left, Vector3 up)
{
    public Vector3 Position = position;
    public Vector3 Fwd = fwd;
    public Vector3 Left = left;
    public Vector3 Up = up;
    
    public float CalculateGlobalRoll() => CalculateRoll(Vector3.UnitY);
    public float CalculateRoll(Vector3 refUp)
    {
        Vector3 fwd = Vector3.Normalize(Fwd);
        
        Vector3 projGlobalUp = refUp - Vector3.Dot(refUp, fwd) * fwd;
        
        if (projGlobalUp.LengthSquared < 1e-12f)
        {
            return 0f;
        }
        projGlobalUp = Vector3.Normalize(projGlobalUp);
        
        Vector3 projFrameUp = Up - Vector3.Dot(Up, fwd) * fwd;
    
        if (projFrameUp.LengthSquared < 1e-12f)
        {
            return 0f;
        }
        projFrameUp = Vector3.Normalize(projFrameUp);
        
        float dot = Vector3.Dot(projGlobalUp, projFrameUp);
        dot = Math.Clamp(dot, -1f, 1f);
        float angle = MathF.Acos(dot);
    
        
        Vector3 cross = Vector3.Cross(projGlobalUp, projFrameUp);
        float sign = MathF.Sign(Vector3.Dot(cross, fwd));
    
        return sign * angle;
    }
}

public static class FVDFrameParser
{
    public static List<FVDFrame> ParseFrames(string fileContent)
    {
        var frames = new List<FVDFrame>();
        var lines = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        // Skip header line (assuming first line is header)
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            var columns = line.Split('\t');
            
            // Skip the index column (first column)
            var values = columns.Skip(1).Select(s => 
                double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            
            if (values.Length < 12)
            {
                throw new FormatException($"Line {i} does not contain enough values. Expected 12, got {values.Length}");
            }
            
            var frame = new FVDFrame(
                position: new Vector3((float)values[0], (float)values[1], (float)values[2]),
                fwd: new Vector3((float)values[3], (float)values[4], (float)values[5]),
                left: new Vector3((float)values[6], (float)values[7], (float)values[8]),
                up: new Vector3((float)values[9], (float)values[10], (float)values[11])
            );
            
            frames.Add(frame);
        }
        
        return frames;
    }
    
    public static List<FVDFrame> ParseFramesFromFile(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return ParseFrames(content);
    }
}