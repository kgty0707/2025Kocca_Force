using System.Collections.Generic;
using UnityEngine;

public class ShufflePositions : MonoBehaviour
{
    public GameObject[] spheres;

    void Start()
    {
        // 1. 현재 XZ 위치 저장
        List<Vector2> xzPositions = new List<Vector2>();
        foreach (GameObject sphere in spheres)
        {
            Vector3 pos = sphere.transform.position;
            xzPositions.Add(new Vector2(pos.x, pos.z));
        }

        // 2. 위치 셔플
        ShuffleList(xzPositions);

        // 3. 섞인 위치 적용 (Y는 그대로 유지)
        for (int i = 0; i < spheres.Length; i++)
        {
            Vector3 currentPos = spheres[i].transform.position;
            Vector2 newXZ = xzPositions[i];
            spheres[i].transform.position = new Vector3(newXZ.x, currentPos.y, newXZ.y);
        }
    }

    // Fisher-Yates 셔플
    void ShuffleList(List<Vector2> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Vector2 temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}
