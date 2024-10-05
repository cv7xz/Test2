using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Status))]
public class StatusEditor : Editor
{
    private SerializedObject test;

    private SerializedProperty isField,isAttachStatus,isAttachSkill,isSpecial,hasCounter,hasTrigger,depend;
    private SerializedProperty field,attachStatus,attachSkill,Special,Counter,Trigger;

    private List<SerializedProperty> boolProperty = new List<SerializedProperty>();
    private List<SerializedProperty> contentProperty = new List<SerializedProperty>();
    private void OnEnable()
    {
        test = new SerializedObject(target);


        boolProperty.Add(test.FindProperty("IsAttached"));
        boolProperty.Add(test.FindProperty("IsAttachSkill"));
        boolProperty.Add(test.FindProperty("isSpecialStatus"));
        boolProperty.Add(test.FindProperty("hasCounter"));


        contentProperty.Add(test.FindProperty("attachOtherStatus"));
        contentProperty.Add(test.FindProperty("attachOtherSkill"));
        contentProperty.Add(test.FindProperty("specialType"));
        contentProperty.Add(test.FindProperty("counter"));


        isField = test.FindProperty("isField");
        field = test.FindProperty("fieldEffects");
        hasTrigger = test.FindProperty("hasTrigger");
        Trigger = test.FindProperty("trigger");

        depend = test.FindProperty("dependValues");
    }

    private bool showTriggerFoldout = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        test.Update();

        if (hasTrigger.boolValue)
        {
            showTriggerFoldout = EditorGUILayout.Foldout(showTriggerFoldout, "Trigger Settings", true);

            if (showTriggerFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerCondition"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerLayer"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("SelfLimit"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("limitRelation"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("limitValue"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerEffect"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerStatus"), true); // true allows for lists
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerSkill"));
                EditorGUILayout.PropertyField(Trigger.FindPropertyRelative("triggerAction"));

                EditorGUI.indentLevel--;
            }
        }
        //if (isField.boolValue)
        //{
        //    EditorGUILayout.PropertyField(field, new GUIContent("Field Effects"),true);
        //}
        //foreach(var a in boolProperty)
        //{
        //    if (a.boolValue)
        //    {
        //        EditorGUILayout.PropertyField(contentProperty[boolProperty.IndexOf(a)],true);
        //    }
        //}
        serializedObject.ApplyModifiedProperties();
        test.ApplyModifiedProperties();
    }
}
