using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Xml;

public class MyCustomEditor : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;
    private VisualElement m_RightPane;

    // Story 리스트
    public List<Story> stories = new List<Story>();

    [MenuItem("Tools/My Custom Editor")]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<MyCustomEditor>();
        wnd.titleContent = new GUIContent("My Custom Editor");

        // Limit size of the window
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    public void CreateGUI()
    {
        var loadButton = new Button(LoadXmlFile) { text = "Load" };
        rootVisualElement.Add(loadButton);

        var saveButton = new Button(SaveXmlFile) { text = "Save" };
        rootVisualElement.Add(saveButton);

        // Create a two-pane view with the left pane being fixed with
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView always needs exactly two child elements
        var leftPane = new ListView();
        splitView.Add(leftPane);
        m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(m_RightPane);

        // Initialize the list view with all story titles
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = stories[index].Text; };
        leftPane.itemsSource = stories;

        // React to the user's selection
        leftPane.onSelectionChange += OnStorySelectionChange;

        // Restore the selection index from before the hot reload
        leftPane.selectedIndex = m_SelectedIndex;

        // Store the selection index when the selection changes
        leftPane.onSelectionChange += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
    }

    private void OnStorySelectionChange(IEnumerable<object> selectedItems)
    {
        // Clear all previous content from the pane
        m_RightPane.Clear();

        // Get the selected story
        var selectedStory = selectedItems.First() as Story;
        if (selectedStory == null)
            return;

        // Add labels for each property of the selected story
        AddLabelToRightPane($"Text: {selectedStory.Text}");
        AddLabelToRightPane($"Type: {selectedStory.Setting.Type}");
        AddLabelToRightPane($"Speaker: {selectedStory.Setting.Speaker}");
        AddLabelToRightPane($"Background: {selectedStory.Setting.Background}");
        AddLabelToRightPane($"BGM: {selectedStory.Setting.BGM}");
        AddLabelToRightPane($"SFX: {selectedStory.Setting.SFX}");
        AddLabelToRightPane($"Left Location: {selectedStory.Location.Left}");
        AddLabelToRightPane($"Middle Location: {selectedStory.Location.Middle}");
        AddLabelToRightPane($"Right Location: {selectedStory.Location.Right}");
        AddLabelToRightPane($"Fade Effect: {selectedStory.Effect.Fade}");
        AddLabelToRightPane($"Camera Effect: {selectedStory.Effect.Camera}");
        AddLabelToRightPane($"UI Effect: {selectedStory.Effect.UI}");
        AddLabelToRightPane($"Left Event: {selectedStory.CharacterEffect.LeftEvent}");
        AddLabelToRightPane($"Middle Event: {selectedStory.CharacterEffect.MiddleEvent}");
        AddLabelToRightPane($"Right Event: {selectedStory.CharacterEffect.RightEvent}");
        AddLabelToRightPane($"Piece Image: {selectedStory.PieceImage.PutPicture}");
    }

    private void AddLabelToRightPane(string text)
    {
        var label = new Label(text);
        m_RightPane.Add(label);
    }

    //public void CreateGUI()
    //{
    //    // Get a list of all sprites in the project
    //    var allObjectGuids = AssetDatabase.FindAssets("t:Sprite");
    //    var allObjects = new List<Sprite>();
    //    foreach (var guid in allObjectGuids)
    //    {
    //        allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
    //    }

    //    // Create a two-pane view with the left pane being fixed with
    //    var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

    //    // Add the panel to the visual tree by adding it as a child to the root element
    //    rootVisualElement.Add(splitView);

    //    // A TwoPaneSplitView always needs exactly two child elements
    //    var leftPane = new ListView();
    //    splitView.Add(leftPane);

    //    m_RightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
    //    splitView.Add(m_RightPane);

    //    // Initialize the list view with all sprites' names
    //    leftPane.makeItem = () => new Label();
    //    leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
    //    leftPane.itemsSource = allObjects;

    //    // React to the user's selection
    //    leftPane.onSelectionChange += OnSpriteSelectionChange;

    //    // Restore the selection index from before the hot reload
    //    leftPane.selectedIndex = m_SelectedIndex;

    //    // Store the selection index when the selection changes
    //    leftPane.onSelectionChange += (items) => { m_SelectedIndex = leftPane.selectedIndex; };
    //}

    //private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    //{
    //    // Clear all previous content from the pane
    //    m_RightPane.Clear();

    //    // Get the selected sprite
    //    var selectedSprite = selectedItems.First() as Sprite;
    //    if (selectedSprite == null)
    //        return;

    //    // Add a new Image control and display the sprite
    //    var spriteImage = new Image();
    //    spriteImage.scaleMode = ScaleMode.ScaleToFit;
    //    spriteImage.sprite = selectedSprite;

    //    // Add the Image control to the right-hand pane
    //    m_RightPane.Add(spriteImage);
    //}


    public class Story
    {
        public string Text;
        public EditorXMLData.Setting Setting;
        public EditorXMLData.Location Location;
        public EditorXMLData.Effect Effect;
        public EditorXMLData.CharacterEffect CharacterEffect;
        public EditorXMLData.PieceImage PieceImage;
    }

    #region XML Data Load, Save

    void LoadXmlFile()
    {
        stories.Clear();

        // XML 파일 경로
        string path = EditorUtility.OpenFilePanel("Import File", $"{Application.streamingAssetsPath}", "xml");
        if(string.IsNullOrEmpty(path)) return;

        // XML 파일을 읽어옴
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        // 모든 <story> 요소를 가져옴
        XmlNodeList storyNodes = xmlDoc.SelectNodes("/storyType/storyTitle/story");

        // 각 <story> 요소를 처리
        foreach (XmlNode storyNode in storyNodes)
        {
            // Story 객체 생성
            Story story = new Story();

            // <Text> 요소의 내용 저장
            story.Text = storyNode.SelectSingleNode(EditorXMLData.StoryXML.TextNode).InnerText;

            // <Setting> 요소의 속성 저장
            XmlNode settingNode = storyNode.SelectSingleNode(EditorXMLData.StoryXML.SettingNode);
            story.Setting.Type = settingNode.Attributes["Type"].Value;
            story.Setting.Speaker = settingNode.Attributes["Speaker"].Value;
            story.Setting.Background = settingNode.Attributes["Background"].Value;
            story.Setting.BGM = settingNode.Attributes["BGM"].Value;
            story.Setting.SFX = settingNode.Attributes["SFX"].Value;

            // <Location> 요소의 속성 저장
            XmlNode locationNode = storyNode.SelectSingleNode(EditorXMLData.StoryXML.LocationNode);
            story.Location.Left = locationNode.Attributes["Left"].Value;
            story.Location.Middle = locationNode.Attributes["Middle"].Value;
            story.Location.Right = locationNode.Attributes["Right"].Value;

            // <Effect> 요소의 속성 저장
            XmlNode effectNode = storyNode.SelectSingleNode(EditorXMLData.StoryXML.EffectNode);
            story.Effect.Fade = effectNode.Attributes["Fade"].Value;
            story.Effect.Camera = effectNode.Attributes["Camera"].Value;
            story.Effect.UI = effectNode.Attributes["UI"].Value;

            // <CharacterEffect> 요소의 속성 저장
            XmlNode characterEffectNode = storyNode.SelectSingleNode(EditorXMLData.StoryXML.CharacterEffectNode);
            story.CharacterEffect.LeftEvent = characterEffectNode.Attributes["LeftEvent"].Value;
            story.CharacterEffect.MiddleEvent = characterEffectNode.Attributes["MiddleEvent"].Value;
            story.CharacterEffect.RightEvent = characterEffectNode.Attributes["RightEvent"].Value;

            // <PieceImage> 요소의 속성 저장
            XmlNode pieceImageNode = storyNode.SelectSingleNode(EditorXMLData.StoryXML.PieceImageNode);
            story.PieceImage.PutPicture = pieceImageNode.Attributes["PutPicture"].Value;

            // 리스트에 Story 추가
            stories.Add(story);
        }

        // XML 파일에서 읽어온 내용을 출력 (디버깅용)
        foreach (var story in stories)
        {
            Debug.Log($"Text: {story.Text}, Type: {story.Setting.Type}, Speaker: {story.Setting.Speaker}, Picture: {story.PieceImage.PutPicture}");
            // 필요한 속성들을 추가로 출력
        }
    }

    private void SaveXmlFile()
    {
        // ... (저장 로직 구현)
    }

    #endregion
}
