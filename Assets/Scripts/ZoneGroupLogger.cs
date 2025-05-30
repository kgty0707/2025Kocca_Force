using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class ZoneGroupLogger : MonoBehaviour
{
    public Transform zonesParent; // 5개의 콜라이더 부모
    public string outputFileName = "zone_log.csv";

    [ContextMenu("📝 Export All Zone Data to CSV")]
    public void ExportAllToCSV()
    {
        if (zonesParent == null)
        {
            Debug.LogError("❌ zonesParent가 설정되지 않았습니다.");
            return;
        }

        string folderPath = Path.Combine(Application.dataPath, "ZoneLogs");
        Directory.CreateDirectory(folderPath);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(folderPath, $"{timestamp}_{outputFileName}");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Zone,ObjectName,Timestamp");

            foreach (Transform zone in zonesParent)
            {
                var logger = zone.GetComponent<ZoneTriggerLogger>();
                if (logger != null)
                {
                    var entries = logger.GetCurrentEntries();
                    foreach (var entry in entries)
                    {
                        writer.WriteLine($"{entry.zoneName},{entry.objectName},{entry.timestamp:yyyy-MM-dd HH:mm:ss}");
                    }

                    // ✅ 저장 후 초기화
                    logger.ClearCurrentObjects();
                }
            }
        }

        Debug.Log($"✅ 모든 존 데이터가 저장되었습니다: {filePath}");
    }
}
