using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal; // ReorderableList 기능을 사용하기 위해 추가합니다.

[Serializable]
public struct CharacterData
{
    public int Level;
    public string Text;
}

public class DataObject : ScriptableObject
{
    public CharacterData[] data;
}

public class StoryTool : EditorWindow
{
    ReorderableList strings_ro_list;
    SerializedObject serializedObject;
    SerializedProperty stringsProperty;

    private void OnEnable()
    {
        DataObject obj = CreateInstance<DataObject>();

        serializedObject = new UnityEditor.SerializedObject(obj);
        stringsProperty = serializedObject.FindProperty("data");

        strings_ro_list = new ReorderableList(serializedObject, stringsProperty, true, true, true, true);
        strings_ro_list.drawElementCallback = StringsDrawListItems;
        strings_ro_list.drawHeaderCallback = StringsDrawHeader;      
    }

    void StringsDrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = stringsProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // 레이블과 필드의 너비 비율을 정의합니다.
        float labelWidth = 60f;  // 필요에 따라 조정할 수 있습니다.
        float fieldWidth = (rect.width - labelWidth - spacing * 3) * 0.5f;

        // 항목 번호를 그립니다.
        Rect indexRect = new Rect(rect.x, rect.y, 20f, lineHeight);
        EditorGUI.LabelField(indexRect, index.ToString());

        // Level 레이블을 그립니다.
        Rect levelLabelRect = new Rect(rect.x, rect.y, labelWidth, lineHeight);
        EditorGUI.LabelField(levelLabelRect, "Level");

        // Level 입력 필드를 그립니다.
        Rect levelRect = new Rect(rect.x + labelWidth + spacing, rect.y, fieldWidth, lineHeight);
        EditorGUI.PropertyField(levelRect, element.FindPropertyRelative("Level"), GUIContent.none);

        // Text 레이블을 그립니다.
        Rect textLabelRect = new Rect(rect.x + labelWidth + spacing + fieldWidth + spacing, rect.y, labelWidth, lineHeight);
        EditorGUI.LabelField(textLabelRect, "Text");

        // Text 입력 필드를 그립니다.
        Rect textRect = new Rect(rect.x + labelWidth + spacing * 3 + fieldWidth, rect.y, fieldWidth, lineHeight);
        EditorGUI.PropertyField(textRect, element.FindPropertyRelative("Text"), GUIContent.none);
    }
    void StringsDrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Character Data List");
    }

    [MenuItem("Window/StoryTool")]
    public static void ShowWindow()
    {
        GetWindow(typeof(StoryTool));
    }

    private void OnGUI()
    {
        EditorGUIUtility.labelWidth = 70f;

        if (this.serializedObject == null)
        {
            return;
        }
        serializedObject.Update();
        strings_ro_list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}