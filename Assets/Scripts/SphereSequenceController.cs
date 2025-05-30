using UnityEngine;
using System.Collections.Generic;
using SG;

public class SphereSequenceController : MonoBehaviour
{
    public GameObject noMaterialGroupParent;
    public GameObject materialGroupParent;
    public GameObject confusionMaterialGroupParent; // Confusion 그룹 부모 오브젝트

    public string grabLogFileName = "grab_log.csv";

    public static string currentGroupName = "";

    private Dictionary<GameObject, Vector2> originalPositions = new Dictionary<GameObject, Vector2>();

    private List<GameObject> noMaterialGroup = new List<GameObject>();
    private List<GameObject> materialGroup = new List<GameObject>();
    private List<GameObject> confusionMaterialGroup = new List<GameObject>(); // Confusion 그룹

    private List<int> showSequence = new List<int>();
    private int currentStep = 0;

    private void ResetGrabCountsInGroup(List<GameObject> group)
    {
        foreach (var obj in group)
        {
            var grabScript = obj.GetComponent<SG_Grabable>();
            if (grabScript != null)
            {
                grabScript.ResetGrabCount();
            }
        }
        SG_Grabable.ResetTotalGrabCount();
    }

    private void LoadGroupsFromParents()
    {
        noMaterialGroup.Clear();
        materialGroup.Clear();
        confusionMaterialGroup.Clear();
        originalPositions.Clear();

        foreach (Transform child in noMaterialGroupParent.transform)
        {
            GameObject obj = child.gameObject;
            noMaterialGroup.Add(obj);
            Vector3 pos = obj.transform.position;
            originalPositions[obj] = new Vector2(pos.x, pos.z);
        }

        foreach (Transform child in materialGroupParent.transform)
        {
            GameObject obj = child.gameObject;
            materialGroup.Add(obj);
            Vector3 pos = obj.transform.position;
            originalPositions[obj] = new Vector2(pos.x, pos.z);
        }

        foreach (Transform child in confusionMaterialGroupParent.transform)
        {
            GameObject obj = child.gameObject;
            confusionMaterialGroup.Add(obj);
            Vector3 pos = obj.transform.position;
            originalPositions[obj] = new Vector2(pos.x, pos.z);
        }
    }

    private void AssignRandomMaterialProperties()
    {
        // materialGroup에서 materialProperties 모으기
        List<SG_MaterialProperties> sourceProperties = new List<SG_MaterialProperties>();
        foreach (var obj in materialGroup)
        {
            var mat = obj.GetComponent<SG_Material>();
            if (mat != null && mat.materialProperties != null)
                sourceProperties.Add(mat.materialProperties);
        }

        // === (1) 셔플 ===
        // Fisher-Yates Shuffle
        for (int i = 0; i < sourceProperties.Count; i++)
        {
            int rand = Random.Range(i, sourceProperties.Count);
            var temp = sourceProperties[i];
            sourceProperties[i] = sourceProperties[rand];
            sourceProperties[rand] = temp;
        }

        // === (2) 1:1로 순서대로 할당 ===
        int assignCount = Mathf.Min(confusionMaterialGroup.Count, sourceProperties.Count);
        for (int i = 0; i < assignCount; i++)
        {
            var mat = confusionMaterialGroup[i].GetComponent<SG_Material>();
            if (mat != null)
                mat.materialProperties = sourceProperties[i];
        }
    }

    void GenerateRandomSequence()
    {
        showSequence.Clear();
        List<int> temp = new List<int>();

        // 0: noMaterial, 1: material, 2: confusion
        for (int i = 0; i < 5; i++)
        {
            temp.Add(0); // noMaterial
            temp.Add(1); // material
            temp.Add(2); // confusion
        }

        for (int i = 0; i < 15; i++)
        {
            int randIndex = Random.Range(0, temp.Count);
            showSequence.Add(temp[randIndex]);
            temp.RemoveAt(randIndex);
        }

        Debug.Log("랜덤 시퀀스: " + string.Join(", ", showSequence));
    }

    public void ShowNextGroup()
    {
        if (currentStep >= showSequence.Count)
        {
            Debug.Log("모든 단계를 완료했습니다.");
            return;
        }

        int groupType = showSequence[currentStep];
        currentStep++;

        // 모든 그룹 비활성화
        foreach (var obj in noMaterialGroup) obj.SetActive(false);
        foreach (var obj in materialGroup) obj.SetActive(false);
        foreach (var obj in confusionMaterialGroup) obj.SetActive(false);

        List<GameObject> targetGroup = null;
        if (groupType == 0)
            targetGroup = noMaterialGroup;
        else if (groupType == 1)
            targetGroup = materialGroup;
        else if (groupType == 2)
        {
            AssignRandomMaterialProperties(); // confusion 그룹일 때 랜덤 force 세팅
            targetGroup = confusionMaterialGroup;
        }

        if (groupType == 0)
            currentGroupName = "NoMaterial";
        else if (groupType == 1)
            currentGroupName = "Material";
        else if (groupType == 2)
            currentGroupName = "Confusion";

        SG_Grabable.SetLogFileName(grabLogFileName);
        ResetGrabCountsInGroup(targetGroup);
        ShuffleAndPlace(targetGroup);

        Debug.Log($"Step {currentStep}: " +
            (groupType == 0 ? "No Material" :
             groupType == 1 ? "Material" : "Confusion")
            + " Group 보여줌");
    }

    void ShuffleAndPlace(List<GameObject> group)
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (var obj in group)
        {
            if (originalPositions.ContainsKey(obj))
            {
                positions.Add(originalPositions[obj]);
            }
            else
            {
                Debug.LogWarning($"원래 위치를 찾을 수 없음: {obj.name}");
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            int rand = Random.Range(i, positions.Count);
            (positions[i], positions[rand]) = (positions[rand], positions[i]);
        }

        for (int i = 0; i < group.Count; i++)
        {
            GameObject obj = group[i];
            Vector3 oldPos = obj.transform.position;
            Vector2 newXZ = positions[i];
            obj.transform.position = new Vector3(newXZ.x, oldPos.y, newXZ.y);
            obj.SetActive(true);
        }
    }

    public void ResetSequence()
    {
        currentStep = 0;
        LoadGroupsFromParents();
        GenerateRandomSequence();
    }
}