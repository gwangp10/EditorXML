using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

public class Test : EditorWindow
{
    private string loadStoryTxtFilePath;       // 불러온 Story 파일의 경로
    private List<string> loadStoryList = new List<string>();
    private Vector2 scrollPosition;
    private ReorderableList reorderableList;
    private string defaultValue = "x";

    [MenuItem("Tools/TestEditor", false, 1000)]
    public static void ShowWindow()
    {
        Test communicationEditor = (Test)GetWindow(typeof(Test));
        communicationEditor.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Text File Content", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Text File", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            LoadTextFile();
        }

        if (GUILayout.Button("Save Text File", GUILayout.MinWidth(100), GUILayout.MaxWidth(200)))
        {
            SaveTextFile();
        }

        GUILayout.EndHorizontal();

        if (loadStoryList != null)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            reorderableList.DoLayoutList();

            EditorGUILayout.EndScrollView();
        }
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(loadStoryList, typeof(string), true, true, true, true);
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Text List");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = loadStoryList[index];
            EditorGUI.LabelField(rect, index.ToString());
            element = EditorGUI.TextField(new Rect(rect.x + 30, rect.y, rect.width - 20, rect.height), element);
            loadStoryList[index] = element;
        };
    }

    #region XML 파일 불러오기, 저장하기
    private void LoadTextFile()
    {
        loadStoryTxtFilePath = EditorUtility.OpenFilePanel("Import File", $"{Application.streamingAssetsPath}/Txt", "txt");
        if (string.IsNullOrEmpty(loadStoryTxtFilePath)) return;
        loadStoryList.Clear();

        string[] lines = File.ReadAllLines(loadStoryTxtFilePath);

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))   // 빈 칸이 아닐 시에만 List에 넣는다.
            {
                loadStoryList.Add(line);
            }
        }
    }

    private void SaveTextFile()
    {
        // XML 문서를 만든다.
        XmlDocument xmlDoc = new XmlDocument();

        // XML을 선언한다.
        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
        xmlDoc.AppendChild(xmlDeclaration);

        // storyType을 만든다.
        XmlElement storyType = xmlDoc.CreateElement("storyType");
        storyType.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
        xmlDoc.AppendChild(storyType);

        // storyTitle을 만든다.
        XmlElement storyTitle = xmlDoc.CreateElement("storyTitle");
        storyType.AppendChild(storyTitle);
        EditorXMLData.Setting setting = new EditorXMLData.Setting();
        EditorXMLData.Location location = new EditorXMLData.Location();
        EditorXMLData.Effect effect = new EditorXMLData.Effect();
        EditorXMLData.CharacterEffect characterEffect = new EditorXMLData.CharacterEffect();
        EditorXMLData.PieceImage pieceImage = new EditorXMLData.PieceImage();

        for (int i = 0; i < loadStoryList.Count; i++)
        {
            // 'story' 엘리먼트 생성
            XmlElement storyElement = xmlDoc.CreateElement("story");
            storyTitle.AppendChild(storyElement);

            // 'Text' 엘리먼트 추가
            AddElement(xmlDoc, storyElement, EditorXMLData.StoryXML.TextNode, loadStoryList[i]);

            // 속성 엘리먼트들 추가
            AddElement(xmlDoc, storyElement, nameof(EditorXMLData.Setting), setting);
            AddElement(xmlDoc, storyElement, nameof(EditorXMLData.Location), location);
            AddElement(xmlDoc, storyElement, nameof(EditorXMLData.Effect), effect);
            AddElement(xmlDoc, storyElement, nameof(EditorXMLData.CharacterEffect), characterEffect);
            AddElement(xmlDoc, storyElement, nameof(EditorXMLData.PieceImage), pieceImage);
        }

        string saveFilePath = EditorUtility.SaveFilePanel("Save File", $"{Application.streamingAssetsPath}", "", "xml");
        xmlDoc.Save(saveFilePath);
    }
    #endregion

    // 텍스트 컨텐츠를 포함한 엘리먼트를 추가하는 도우미 메서드
    private void AddElement(XmlDocument xmlDoc, XmlElement parentElement, string elementName, string textContent = "")
    {
        XmlElement element = xmlDoc.CreateElement(elementName);
        element.InnerText = textContent;
        parentElement.AppendChild(element);
    }

    private void AddElement<T>(XmlDocument xmlDoc, XmlElement parentElement, string elementName, T elementProperty)
    {
        XmlElement element = xmlDoc.CreateElement(elementName);
        CreateXMLProperty(elementProperty, element);
        parentElement.AppendChild(element);
    }

    private void CreateXMLProperty<T>(T val, XmlElement element)
    {
        foreach (var field in typeof(T).GetFields())
        {
            string fieldName = field.Name;
            element.SetAttribute(fieldName, defaultValue);
        }
    }
}