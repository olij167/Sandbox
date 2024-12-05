using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(Rename))]
public class RenameEditor : Editor
{
    public List<GameObject> renameList = new List<GameObject>();

    public string newName;

    public string prefix;
    public string suffix;

    //>> ADD A METHOD TO ADD / REMOVE ITEM COUNT SUFFIX

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Rename rename = (Rename)target;
        if (rename == null) return;

        for (int i = 0; i < rename.objectsToRename.Length; i++)
        {
            if (!renameList.Contains(rename.objectsToRename[i]))
            {
                renameList.Add(rename.objectsToRename[i]);
                Debug.Log(rename.objectsToRename[i] + " added for renaming");
            }
        }

        for (int i = 0; i < renameList.Count; i++)
        {
            for (int r = 0; r < rename.objectsToRename.Length; r++)
            {
                if (rename.objectsToRename[r] != (renameList[i]) && r++ > rename.objectsToRename.Length)
                {
                    Debug.Log(renameList[i] + " removed from renaming");
                    renameList.RemoveAt(i);
                }
            }
        }
        //GUILayout.Space(10f);

        if (GUILayout.Button("Clear List"))
        {
            rename.objectsToRename = new GameObject[0];

        }

        GUILayout.Space(10f);


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Name:", GUILayout.MaxWidth(50f));
        newName = EditorGUILayout.TextField(newName, GUILayout.MinWidth(100f));
        //GUILayout.EndVertical();
        if (GUILayout.Button("Rename", GUILayout.MinWidth(75f)))
        {
            foreach (GameObject g in renameList)
            {

                g.name = newName;

            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);


        GUILayout.BeginHorizontal();
        //GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Prefix:", GUILayout.MaxWidth(50f));
        prefix = EditorGUILayout.TextField(prefix, GUILayout.MinWidth(100f));
        //GUILayout.EndVertical();
        if (GUILayout.Button("Add", GUILayout.MinWidth(50f)))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(prefix))
                {
                    string old = g.name;
                    g.name = prefix + old;
                }
            }
        }

        if (GUILayout.Button("Remove", GUILayout.MinWidth(75f)))
        {
            if (prefix.Length > 0)
                foreach (GameObject g in renameList)
                {

                    while (g.name.StartsWith(prefix))
                    {
                        g.name = g.name.Substring(prefix.Length);
                    }


                }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Suffix:", GUILayout.MaxWidth(50f));
        suffix = EditorGUILayout.TextField(suffix, GUILayout.MinWidth(100f));

        if (GUILayout.Button("Add ", GUILayout.MinWidth(50f)))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(suffix))
                    g.name += suffix;

            }
        }

        if (GUILayout.Button("Remove", GUILayout.MinWidth(75f)))
        {
            if (suffix.Length > 0)
                foreach (GameObject g in renameList)
                {

                    while (g.name.EndsWith(suffix))
                    {
                        g.name = g.name.Substring(0, g.name.Length - suffix.Length);
                    }

                }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();
        GUILayout.Space(50f);

        if (GUILayout.Button("Add Both"))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(prefix))
                {
                    string old = g.name;
                    g.name = prefix + old;
                }

                if (!g.name.Contains(suffix))
                    g.name += suffix;
            }
        }

        if (GUILayout.Button("Remove Both"))
        {
            foreach (GameObject g in renameList)
            {
                if (prefix.Length > 0)
                {
                    while (g.name.StartsWith(prefix))
                    {
                        g.name = g.name.Substring(prefix.Length);
                    }
                }

                if (suffix.Length > 0)
                {
                    while (g.name.EndsWith(suffix))
                    {
                        g.name = g.name.Substring(0, g.name.Length - suffix.Length);
                    }
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(25f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Index Count"))
        {
            for (int i = 0; i < renameList.Count; i++)
            {
                renameList[i].name += " #" + (i + 1);
            }
        }

        if (GUILayout.Button("Remove Index Count"))
        {
            foreach (GameObject g in renameList)
            {
                for (int i = 0; i < 9; i++)
                {

                    if (g.name.EndsWith("(" + i.ToString() + ")"))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 3);
                    }
                    else if (g.name.EndsWith("#" + i.ToString()))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 2);
                    }
                    else if (g.name.EndsWith(i.ToString()))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 1);
                    }

                    if (g.name.EndsWith(" "))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 1);
                    }

                }
            }
        }
        GUILayout.EndHorizontal();
    }
}


public class RenameEditorWindow : EditorWindow
{
    private const string _helpText = "Cannot find 'Rename' component on any GameObject in the scene!";

    private static Vector2 _windowsMinSize = Vector2.one * 500f;
    private static Rect _helpRect = new Rect(0f, 0f, 400f, 100f);
    private static Rect _listRect = new Rect(Vector2.zero, _windowsMinSize);

    private bool _isActive;

    SerializedObject _objectSO = null;
    ReorderableList _listRE = null;

    Rename rename;

    public List<GameObject> renameList = new List<GameObject>();
    public string newName;
    public string prefix;
    public string suffix;

    [MenuItem("Tools/RenameTool")]
    public static void OpenSimulatorWindow()
    {
        GetWindow<RenameEditorWindow>(true, "Rename Window");
    }

    private void OnEnable()
    {
        if (FindObjectOfType<Rename>())
        {
            rename = FindObjectOfType<Rename>();
        }
        else
        {
            GameObject g = new GameObject();
            g.name = "Renamer";
            g.AddComponent<Rename>();

            rename = g.GetComponent<Rename>();
        }

        if (rename)
        {
            _objectSO = new SerializedObject(rename);

            //init list
            _listRE = new ReorderableList(_objectSO, _objectSO.FindProperty("objectsToRename"), true,
                true, true, true);

            //handle drawing
            _listRE.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Game Objects");
            _listRE.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUIContent objectLabel = new GUIContent($"GameObject {index}");
                //the index will help numerate the serialized fields
                EditorGUI.PropertyField(rect, _listRE.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
            };
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (_objectSO == null)
        {
            //EditorGUI.HelpBox(_helpRect, _helpText, MessageType.Warning);
            //return;
            if (FindObjectOfType<Rename>())
            {
                rename = FindObjectOfType<Rename>();
            }
            else
            {
                //GameObject r = new GameObject();
                //Rename g = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity).AddComponent<Rename>();
                //g.name = "Renamer";
                //rename = g;

                EditorGUI.HelpBox(_helpRect, _helpText, MessageType.Warning);
                return;
            }
        }
        else if (_objectSO != null)
        {
            _objectSO.Update();
            _listRE.DoList(_listRect);
            _objectSO.ApplyModifiedProperties();
        }

        GUILayout.Space(_listRE.GetHeight() + 0f);
        GUILayout.Label("Add the GameObjects you wish to rename to the list");
        GUILayout.Space(5f);
        if (GUILayout.Button("Clear List"))
        {
            rename.objectsToRename = new GameObject[0];

        }

        GUILayout.Space(10f);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Name:", GUILayout.MaxWidth(50f));
        newName = EditorGUILayout.TextField(newName, GUILayout.MinWidth(100f));
        //GUILayout.EndVertical();
        if (GUILayout.Button("Rename", GUILayout.MinWidth(75f)))
        {
            foreach (GameObject g in renameList)
            {

                g.name = newName;

            }
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);

        GUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Prefix:", GUILayout.MaxWidth(50f));

        prefix = EditorGUILayout.TextField(prefix, GUILayout.MinWidth(100f));

        if (GUILayout.Button("Add", GUILayout.MinWidth(50f)))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(prefix))
                {
                    string old = g.name;
                    g.name = prefix + old;
                }
            }
        }

        if (GUILayout.Button("Remove", GUILayout.MinWidth(75f)))
        {
            if (prefix.Length > 0)
                foreach (GameObject g in renameList)
                {

                    while (g.name.StartsWith(prefix))
                    {
                        g.name = g.name.Substring(prefix.Length);
                    }


                }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Suffix:", GUILayout.MaxWidth(50f));
        suffix = EditorGUILayout.TextField(suffix);

        if (GUILayout.Button("Add", GUILayout.MinWidth(50f)))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(suffix))
                    g.name += suffix;

            }
        }


        if (GUILayout.Button("Remove", GUILayout.MinWidth(75f)))
        {
            if (suffix.Length > 0)
                foreach (GameObject g in renameList)
                {

                    while (g.name.EndsWith(suffix))
                    {
                        g.name = g.name.Substring(0, g.name.Length - suffix.Length);
                    }

                }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();
        GUILayout.Space(50f);

        if (GUILayout.Button("Add Both"))
        {
            foreach (GameObject g in renameList)
            {
                if (!g.name.Contains(prefix))
                {
                    string old = g.name;
                    g.name = prefix + old;
                }

                if (!g.name.Contains(suffix))
                    g.name += suffix;
            }
        }

        if (GUILayout.Button("Remove Both"))
        {
            foreach (GameObject g in renameList)
            {
                if (prefix.Length > 0)
                {
                    while (g.name.StartsWith(prefix))
                    {
                        g.name = g.name.Substring(prefix.Length);
                    }
                }

                if (suffix.Length > 0)
                {
                    while (g.name.EndsWith(suffix))
                    {
                        g.name = g.name.Substring(0, g.name.Length - suffix.Length);
                    }
                }
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Index Count"))
        {
            for (int i = 0; i < renameList.Count; i++)
            {
                renameList[i].name += " " + i;
            }
        }

        if (GUILayout.Button("Remove Index Count"))
        {
            foreach (GameObject g in renameList)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (g.name.EndsWith(i.ToString()))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 1);
                    }
                    else if (g.name.EndsWith("(" + i.ToString() + ")"))
                    {
                        g.name = g.name.Substring(0, g.name.Length - 3);
                    }

                }
            }
        }
        GUILayout.EndHorizontal();
    }
}

