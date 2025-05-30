using UnityEngine;
using System;
using System.Collections.Generic;

public class ZoneTriggerLogger : MonoBehaviour
{
    public string zoneName;

    private Dictionary<GameObject, DateTime> currentObjects = new Dictionary<GameObject, DateTime>();

    public void ClearCurrentObjects()
    {
        currentObjects.Clear();
        Debug.Log($"🔄 {zoneName} 오브젝트 목록 초기화됨");
    }

    public class ZoneEntry
    {
        public string zoneName;
        public string objectName;
        public DateTime timestamp;
    }

    public List<ZoneEntry> GetCurrentEntries()
    {
        List<ZoneEntry> entries = new List<ZoneEntry>();
        foreach (var kv in currentObjects)
        {
            entries.Add(new ZoneEntry
            {
                zoneName = this.zoneName,
                objectName = kv.Key.name,
                timestamp = kv.Value
            });
        }
        return entries;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!currentObjects.ContainsKey(other.gameObject))
        {
            currentObjects[other.gameObject] = DateTime.Now;
            // Debug.Log($"🟢 Entered: {other.name} in {zoneName}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObjects.ContainsKey(other.gameObject))
        {
            currentObjects.Remove(other.gameObject);
        }
    }
}
