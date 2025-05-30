using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraCapture))]
public class CameraCaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraCapture script = (CameraCapture)target;

         if (GUILayout.Button("📸 캡쳐 (이름 포함)"))
        {
            script.CaptureWithNames();
        }
    }
}