//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(Skill_SO))]
//public class TestEditor : Editor
//{
//    private SerializedObject test;

//    private SerializedProperty type;
//    private SerializedProperty randomAttackNumber;

//    private void OnEnable()
//    {
//        test = new SerializedObject(target);

//        type = test.FindProperty("attackType");
//        randomAttackNumber = test.FindProperty("RandomAttackNumber");
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        test.Update();
//        if (type.enumValueIndex == 3)
//        {
//            EditorGUILayout.PropertyField(randomAttackNumber);
//        }

//        test.ApplyModifiedProperties();
//    }
//}
