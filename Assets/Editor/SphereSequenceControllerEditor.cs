using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SphereSequenceController))]
public class SphereSequenceControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SphereSequenceController controller = (SphereSequenceController)target;

        if (GUILayout.Button("▶ 다음 그룹 보여주기"))
        {
            controller.ShowNextGroup();
        }

        if (GUILayout.Button("🔁 시퀀스 리셋"))
        {
            controller.ResetSequence();
        }
    }
}
