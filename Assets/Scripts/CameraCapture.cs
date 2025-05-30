using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraCapture : MonoBehaviour
{
    public Camera fixedCamera;
    public string folderPath = "Assets/Captures";
    public string customName = "MyCapture";
    public GameObject[] spheres;

    public void CaptureWithNames()
    {
        SetNameLabelsActive(true);
        Capture();
        SetNameLabelsActive(false);
    }

    private void SetNameLabelsActive(bool isActive)
    {
        foreach (GameObject sphere in spheres)
        {
            Transform label = sphere.transform.Find("NameLabel");
            if (label != null)
                label.gameObject.SetActive(isActive);
        }
    }

    private void Capture()
    {
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string namePart = string.IsNullOrWhiteSpace(customName) ? "Unnamed" : customName;
        string fileName = $"{namePart}_{timestamp}.png";
        string filePath = System.IO.Path.Combine(folderPath, fileName);

        // ✅ 비활성화 상태여도 캡쳐할 수 있도록 설정
        RenderTexture rt = new RenderTexture(1920, 1080, 24);
        Texture2D screenshot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);

        Camera cam = Instantiate(fixedCamera);     // ✅ 복제본 생성 (Scene에 없는 임시 카메라)
        cam.enabled = false;                       // 활성화 방지 (절대 시점 간섭 X)
        cam.targetTexture = rt;

        cam.Render();                              // 수동 렌더
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);

        cam.targetTexture = null;
        RenderTexture.active = null;

        DestroyImmediate(rt);
        DestroyImmediate(cam.gameObject);          // ✅ 임시 카메라 제거

        System.IO.File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
        Debug.Log($"📸 Captured to: {filePath}");
    }

}
