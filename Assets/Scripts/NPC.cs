using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private GameObject warningInstance;

    public bool missionToDelivery = true;
    public GameObject panelMission;

    public void Start()
    {
        SetMissionPanel();
    }

    private void SetMissionPanel()
    {
        panelMission = GameObject.Find("HandleScene/Canvas/panel_right/panel_mission");
    }

    public void OnMouseDown()
    {
        panelMission.SetActive(!panelMission.activeSelf);
    }

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            panelMission.SetActive(false);
        }

        if (warningInstance != null) return;

        string resourceName = missionToDelivery ? "MissionToDelivery" : "MissionToDo";
        var warningPrefab = Resources.Load<GameObject>(resourceName);

        if (warningPrefab != null)
        {
            warningInstance = Instantiate(warningPrefab, transform);
        }
    }
}
