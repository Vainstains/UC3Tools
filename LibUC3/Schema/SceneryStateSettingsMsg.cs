using VProto;

namespace LibUC3.Schema;

public class SceneryStateSettingsMsg : Message
{
    [ProtoField(1)]
    public List<SceneryItemInstanceGroupSettingsMsg> GroupSettings;

    [ProtoField(2)]
    public List<SceneryItemInstanceSettingsMsg> ItemSettings;
}

public class SceneryItemInstanceGroupSettingsMsg : Message
{
    [ProtoField(1)] public string GroupId;
}

public class SceneryItemInstanceGroupTemplateSettingsMsg : Message
{
    [ProtoField(1)] public string GroupId;
    [ProtoField(2)] public List<SceneryItemInstanceSettingsMsg> Items;
}

public class SceneryItemInstanceSettingsMsg : Message
{
    [ProtoField(1)] public float OriginX;
    [ProtoField(2)] public float OriginY;
    [ProtoField(3)] public float OriginZ;
    [ProtoField(4)] public float RotationYaw;
    [ProtoField(5)] public float RotationPitch;
    [ProtoField(6)] public float RotationRoll;
    [ProtoField(7)] public float ScaleX;
    [ProtoField(8)] public float ScaleY;
    [ProtoField(9)] public float ScaleZ;
    [ProtoField(10)] public List<SceneryColorChannelSettingsMsg> ColorChannels;
    [ProtoField(11)] public bool TerrainAttached;
    [ProtoField(12)] public string SceneryItemId;
    [ProtoField(13)] public string GroupId; // optional

    public class EditorExtrasMsg : Message
    {
        [ProtoField(1)] public bool Selected;
        [ProtoField(2)] public bool VisibilityToggledOff;
    }

    [ProtoField(14)] public EditorExtrasMsg EditorExtras; // optional
}

public class SceneryItemPackSettingsMsg : Message
{
    [ProtoField(1)] public string SceneryItemPackId;
    [ProtoField(2)] public List<SceneryItemTypeSettingsMsg> SceneryItemTypes = new();
    [ProtoField(3)] public string SceneryItemPackName;
}

public class SceneryItemTypeSettingsMsg : Message
{
    [ProtoField(1)] public string SceneryItemId;
    [ProtoField(10)] public List<SceneryColorChannelSettingsMsg> ColorChannelDefaults = new();
    [ProtoField(2)] public string SceneryItemName;
    [ProtoField(3)] public string SceneryItemDescription;
    [ProtoField(4)] public string SceneryItemPackId;
    [ProtoField(11)] public List<string> ColorChannelNames = new();
    [ProtoField(12)] public bool ColorChannelTexturesSelectable;
    [ProtoField(13)] public bool HugTerrainByDefault;
}