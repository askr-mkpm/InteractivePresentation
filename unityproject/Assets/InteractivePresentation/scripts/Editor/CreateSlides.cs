using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Plane = ProceduralModeling.Plane;

namespace InteractivePresentation.scripts.Editor
{
    public class CreateSlides : EditorWindow
    {
        [SerializeField] public List<Texture2D> slideList;

        [MenuItem("Window/InteractivePresentation/CreateSlides")]
        private static void ShowWindow()
        {
            var window = GetWindow<CreateSlides>("CreateSlides");
        }
        
        void OnGUI()
        { 
            var so = new SerializedObject(this);
            so.Update();
            
            EditorGUILayout.PropertyField(so.FindProperty("slideList"), true);
            
            so.ApplyModifiedProperties();
            
            if( GUILayout.Button( "Create" ) )
            {
                AllignmentSlides();
                Debug.Log("Create_" + slideList.Count + "_Slides!");
            }
        }
        
        private void AllignmentSlides()
        {
            var components = new[]
            {
                typeof(Plane)
            };
            
            GameObject slideRoot = new GameObject("SlideRoot");
            Undo.RegisterCreatedObjectUndo(slideRoot, "Create New GameObject");
            
            slideRoot.transform.localPosition = Vector3.zero;
            
            for (int i = 0; i < slideList.Count; i++)
            {
                GameObject slide = new GameObject("slide_" + i, components);
                Plane sp;
                sp = slide.GetComponent<Plane>();
                
                sp.material = new Material(Shader.Find("InteractivePresentation/GeometrySlide"));
                sp.material.SetFloat("_emission", 1.199f);
                sp.material.SetFloat("_destRange", 2.5f);
                sp.material.SetFloat("alphaRange", 1.291f);
                sp.material.SetFloat("_fadePos", 25f);
                sp.material.SetTexture("_MainTex", slideList[i]);
                
                sp.widthSegments = 12;
                sp.heightSegments = 8;
                sp.size = 3;

                slide.transform.parent = slideRoot.transform;
                slide.transform.localPosition = new Vector3(0, 0, 25 + i * 25);
                slide.transform.eulerAngles = new Vector3(90, 0,0);
                
                Undo.RegisterCreatedObjectUndo(slide, "Create New GameObject");
            }
        }
    }
}