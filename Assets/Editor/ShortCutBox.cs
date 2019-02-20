using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace JeffXu.Editor.Window
{
    public class ShortCutBox : EditorWindow
    {
        List<Object> objList;
        bool filterFlag;
        List<System.Type> typeList;
        Dictionary<System.Type, bool> typeFlagMap;

        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        private static GUIContent deleteButtonContent = new GUIContent("-", "delete");

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/ShortCutBox")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(ShortCutBox));
        }

        public void OnEnable()
        {
            objList = new List<Object>();
            typeList = new List<System.Type> { typeof(TextAsset), typeof(MonoScript), typeof(SceneAsset), typeof(DefaultAsset),
            typeof(AnimationClip), typeof(UnityEditor.Animations.AnimatorController), typeof(Material), typeof(Shader)};
            typeFlagMap = new Dictionary<System.Type, bool>();
            foreach (var type in typeList)
            {
                typeFlagMap.Add(type, false);
            }
        }

        void OnGUI()
        {
            var objects = DropZone();

            if (objects != null)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    var tempObj = objects[i];
                    if (objList.Contains(tempObj))
                        continue;

                    objList.Add(tempObj);
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            filterFlag = EditorGUILayout.BeginToggleGroup("Filter", filterFlag);
            for (int i = 0; i < typeList.Count; i++)
            {
                var type = typeList[i];
                typeFlagMap[type] = EditorGUILayout.Toggle(GetTypeShortName(type), typeFlagMap[type]);
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (!filterFlag)
            {
                foreach (var tempObj in objList.ToArray())
                {
                    drawSingleObj(tempObj);
                }
            }
            else
            {
                foreach (var tempObj in objList.ToArray())
                {
                    var typeOfObj = tempObj.GetType();
                    
                    if (typeFlagMap.ContainsKey(typeOfObj) && typeFlagMap[typeOfObj])
                        drawSingleObj(tempObj);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // Drag objs
        public static Object[] DropZone()
        {
            EventType eventType = Event.current.type;
            bool isAccepted = false;

            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }
                Event.current.Use();
            }

            return isAccepted ? DragAndDrop.objectReferences : null;
        }

        private string GetTypeShortName(System.Type type)
        {
            string name = type.ToString();
            string[] parts = name.Split('.');
            if (parts.Length > 0)
                return parts[parts.Length - 1];
            else
                return name;
        }

        private void drawSingleObj(Object tempObj)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(tempObj, typeof(Object), false);
            if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
            {
                objList.Remove(tempObj);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}