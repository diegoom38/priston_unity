using UnityEngine;

public class NPC : MonoBehaviour
{
    private GameObject warningInstance;

    public bool missionToDelivery = true;

    public void OnMouseDown()
    {
        Debug.Log($@"Click on: {transform.name}");
    }

    public void Update()
    {
        if (warningInstance != null) return;

        string resourceName = missionToDelivery ? "MissionToDelivery" : "MissionToDo";
        var warningPrefab = Resources.Load<GameObject>(resourceName);

        if (warningPrefab != null)
        {
            warningInstance = Instantiate(warningPrefab, transform);
        }
    }
}
