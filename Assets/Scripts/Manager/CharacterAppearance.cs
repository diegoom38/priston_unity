using Assets.Scripts.Manager;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearance : MonoBehaviourPun
{
    [PunRPC]
    public void UpdateCharacterAppearance(float skinR, float skinG, float skinB,
                                          float hairR, float hairG, float hairB,
                                          float eyeR, float eyeG, float eyeB,
                                          string gender, string age)
    {
        Transform characterMeshes = transform;

        ChangeSkinColor(new Color(skinR, skinG, skinB), characterMeshes, gender, age);
        ChangeHairColor(new Color(hairR, hairG, hairB), characterMeshes, gender, age);
        ChangeEyeColor(new Color(eyeR, eyeG, eyeB), characterMeshes, gender, age);
    }

    private void ChangeSkinColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshSkinsList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeHairColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshHairList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeEyeColor(Color color, Transform characterMeshes, string gender, string age)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshEyeList(gender, age))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }
}
