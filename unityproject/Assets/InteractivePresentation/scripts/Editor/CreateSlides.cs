using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ProceduralModeling;
using Plane = ProceduralModeling.Plane;

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
            var components = new[]
            {
                typeof(Plane)
            };
            
            for (int i = 0; i < slideNum.Count; i++)
            {
                GameObject slide = new GameObject("slide_" + i, components);
                Plane sp;
                sp = slide.GetComponent<Plane>();
                
                sp.material = new Material(Shader.Find("InteractivePresentation/GeometrySlide"));
                sp.material.SetFloat("_emission", 1.199f);
                sp.material.SetFloat("_destRange", 2.5f);
                sp.material.SetFloat("alphaRange", 1.291f);
                sp.material.SetFloat("_fadePos", 25f);
                
                sp.widthSegments = 12;
                sp.heightSegments = 8;
                sp.size = 3;
                Undo.RegisterCreatedObjectUndo(slide, "Create New GameObject");
            }
        }
    }
}