using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InteractivePresentation.scripts.Editor
{
    public class CreateSlides : EditorWindow
    {
        [MenuItem("InteractivePresentation/CreateSlides")]
        public static void Create()
        {
            GameObject newGameObject = new GameObject("New GameObject");
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create New GameObject");
        }
    }
}