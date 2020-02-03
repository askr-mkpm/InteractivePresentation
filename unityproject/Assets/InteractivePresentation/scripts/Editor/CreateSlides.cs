using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractivePresentation.scripts.Editor
{
    public class CreateSlides : EditorWindow
    {
        [SerializeField] public List<Texture2D> slideNum;

        [MenuItem("Window/InteractivePresentation/CreateSlides")]
        private static void ShowWindow()
        {
            var window = GetWindow<CreateSlides>("CreateSlides");
        }
        
        void OnGUI()
        { 
            var so = new SerializedObject(this);
            so.Update();
            
            EditorGUILayout.PropertyField(so.FindProperty("slideNum"), true);
            
            so.ApplyModifiedProperties();
            
            if( GUILayout.Button( "Create" ) )
            {
                AllignmentSlides();
            }
        }
        
        private void AllignmentSlides()
        {
            for (int i = 0; i < slideNum.Count; i++)
            {
                GameObject newGameObject = new GameObject("slide_" + i);
                Undo.RegisterCreatedObjectUndo(newGameObject, "Create New GameObject");
            }
        }
    }
}