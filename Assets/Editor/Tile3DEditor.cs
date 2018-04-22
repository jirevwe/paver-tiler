using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Tile3D))]
public class Tile3DEditor : Editor
{
    Tile3D tile3D;

    // public override void OnInspectorGUI()
    // {
    //     tile3D = target as Tile3D;

    //     EditorGUILayout.BeginVertical();
    //     {
    //         serializedObject.Update();

    //         GUI.enabled = false;
    //         EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Tile3D)target), typeof(Tile3D), false);
    //         GUI.enabled = true;
    //     }
    //     EditorGUILayout.EndVertical();
    // }

    protected virtual void OnSceneGUI()
    {
        tile3D = target as Tile3D;

        Handles.Label(tile3D.transform.position + (Vector3.forward / 2) + (Vector3.right / 4), string.Format("{0},{1}", tile3D.transform.position.x, tile3D.transform.position.z));

        Handles.BeginGUI();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.BeginArea(new Rect(260, 20, 100, 500));
                {

                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(140, 20, 100, 500));
                {
                    var start = GameObject.FindGameObjectWithTag("start");
                    if (start == null)
                    {
                        start = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        start.GetComponent<MeshRenderer>().enabled = false;
                        start.name = "Start";
                        start.tag = "start";
                    }

                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(20, 20, 100, 500));
                {

                }
                GUILayout.EndArea();
            }
            GUILayout.EndVertical();
        }
        Handles.EndGUI();

        if (GUI.changed && !Application.isPlaying)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
