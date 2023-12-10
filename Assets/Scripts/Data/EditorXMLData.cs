public class EditorXMLData
{
    public struct Setting
    {
        public string Type;
        public string Speaker;
        public string Background;
        public string BGM;
        public string SFX;
    }

    public struct Location
    {
        public string Left;
        public string Middle;
        public string Right;
    }

    public struct Effect
    {
        public string Fade;
        public string Camera;
        public string UI;
    }

    public struct CharacterEffect
    {
        public string LeftEvent;
        public string MiddleEvent;
        public string RightEvent;
    }

    public struct PieceImage
    {
        public string PutPicture;
    }

    static public class StoryXML
    {
        static public string TextNode = "Text";
        static public string SettingNode = "Setting";
        static public string LocationNode = "Location";
        static public string EffectNode = "Effect";
        static public string CharacterEffectNode = "CharacterEffect";
        static public string PieceImageNode = "PieceImage";

    }
}
