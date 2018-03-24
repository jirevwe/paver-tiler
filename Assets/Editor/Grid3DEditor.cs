using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Grid3D))]
public class Grid3DEditor : Editor
{
    Grid3D grid3D;

    private void OnEnable()
    {
        grid3D = target as Grid3D;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // EditorGUI.BeginChangeCheck();
        // EditorGUILayout.BeginVertical();
        // {
        //     serializedObject.Update();

        //     GUI.enabled = false;
        //     EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Grid3D)target), typeof(Grid3D), false);
        //     GUI.enabled = true;

        //     grid3D.editMode = EditorGUILayout.Toggle("Edit Mode", grid3D.editMode, GUILayout.MinHeight(20), GUILayout.MinWidth(100));
        //     EditorGUILayout.Space();

        //     grid3D.gridZ = (int)EditorGUILayout.Slider("Grid (Z)", grid3D.gridZ, 1, 10, GUILayout.MinHeight(20), GUILayout.MinWidth(100));
        //     grid3D.gridX = (int)EditorGUILayout.Slider("Grid (X)", grid3D.gridX, 1, 10, GUILayout.MinHeight(20), GUILayout.MinWidth(100));

        //     grid3D.min

        //     EditorGUILayout.Space();
        //     EditorGUILayout.LabelField("Other Types", EditorStyles.boldLabel);
        //     EditorGUILayout.Space();

        //     grid3D.tileHolder = EditorGUILayout.ObjectField("Tile Holder", grid3D.tileHolder, typeof(GameObject), true, GUILayout.MinHeight(20), GUILayout.MinWidth(100)) as GameObject;
        //     grid3D.tile = EditorGUILayout.ObjectField("Box Cross", grid3D.tile, typeof(GameObject), true, GUILayout.MinHeight(20), GUILayout.MinWidth(100)) as GameObject;

        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("nodes"), true);
        //     EditorGUILayout.PropertyField(serializedObject.FindProperty("nodeGrid"), true);

        //     serializedObject.ApplyModifiedProperties();
        // }
        // EditorGUILayout.EndVertical();

        // if (EditorGUI.EndChangeCheck() && GUI.changed)
        // {
        //     EditorUtility.SetDirty(target);
        // }
    }

    protected virtual void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;
            
        EditorGUI.BeginChangeCheck();

        var z = grid3D.gridZ % 2 == 0 ? grid3D.gridZ / 2 - .5f : grid3D.gridZ / 2;
        var x = grid3D.gridX % 2 == 0 ? grid3D.gridX / 2 - .5f : grid3D.gridX / 2;

        grid3D.gridZ = (int)Handles.DoPositionHandle(new Vector3(x, 0, grid3D.gridZ), Quaternion.identity).z;
        grid3D.gridX = (int)Handles.DoPositionHandle(new Vector3(grid3D.gridX, 0, z), Quaternion.identity).x;

        grid3D.gridX = Mathf.Clamp(grid3D.gridX, (int)grid3D.min.x, (int)grid3D.max.y);
        grid3D.gridZ = Mathf.Clamp(grid3D.gridZ, (int)grid3D.min.x, (int)grid3D.max.y);

        var start = GameObject.FindGameObjectWithTag("start");
        if (start == null)
        {
            start = GameObject.CreatePrimitive(PrimitiveType.Cube);
            start.GetComponent<MeshRenderer>().enabled = false;
            start.name = "Start";
            start.tag = "start";
        }

        for (int i = 0; i < grid3D.max.x; i++)
        {
            for (int j = 0; j < grid3D.max.y; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                if (child != null)
                {
                    GameObject tile = child.gameObject;
                    L.SafeDestroy(tile);
                }
            }
        }

        for (int i = 0; i < grid3D.gridX; i++)
        {
            for (int j = 0; j < grid3D.gridZ; j++)
            {
                Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
                if (child == null)
                {
                    GameObject g = GameObject.Instantiate(grid3D.tile, grid3D[i, j].worldPosition, Quaternion.identity);
                    g.name = string.Format("{0}, {1}", i, j);
                    g.transform.parent = start.transform;
                }
            }
        }

        // Handles.BeginGUI();
        // {
        //     GUILayout.BeginVertical();
        //     {
        //         GUILayout.BeginArea(new Rect(20, 20, 100, 500));
        //         {
        //             var start = GameObject.FindGameObjectWithTag("start");
        //             if (start == null)
        //             {
        //                 start = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //                 start.GetComponent<MeshRenderer>().enabled = false;
        //                 start.name = "Start";
        //                 start.tag = "start";
        //             }

        //             if (GUILayout.Button("Init All Cross", GUILayout.MinWidth(200), GUILayout.MinHeight(50)))
        //             {
        //                 for (int i = 0; i < 11; i++)
        //                 {
        //                     for (int j = 0; j < 11; j++)
        //                     {
        //                         Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
        //                         if (child != null)
        //                         {
        //                             GameObject tile = child.gameObject;
        //                             L.SafeDestroy(tile);
        //                         }
        //                     }
        //                 }

        //                 for (int i = 0; i < grid3D.gridX; i++)
        //                 {
        //                     for (int j = 0; j < grid3D.gridZ; j++)
        //                     {
        //                         Transform child = start.transform.Find(string.Format("{0}, {1}", i, j).ToString());
        //                         if (child == null)
        //                         {
        //                             GameObject g = GameObject.Instantiate(grid3D.tile, grid3D[i, j].worldPosition, Quaternion.identity);
        //                             g.name = string.Format("{0}, {1}", i, j);
        //                             g.transform.parent = start.transform;
        //                         }
        //                         else
        //                         {
        //                             L.d(child.position);
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         GUILayout.EndArea();
        //     }
        //     GUILayout.EndVertical();
        // }
        // Handles.EndGUI();

        if (GUI.changed && EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            // EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
