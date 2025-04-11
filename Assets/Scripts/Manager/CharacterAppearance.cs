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
                                          string gender)
    {
        Transform characterMeshes = transform;

        ChangeSkinColor(new Color(skinR, skinG, skinB), characterMeshes, gender);
        ChangeHairColor(new Color(hairR, hairG, hairB), characterMeshes, gender);
        ChangeEyeColor(new Color(eyeR, eyeG, eyeB), characterMeshes, gender);
    }

    private void ChangeSkinColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshSkinsList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeHairColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshHairList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }

    private void ChangeEyeColor(Color color, Transform characterMeshes, string gender)
    {
        foreach (KeyValuePair<string, string> mesh in MaterialManager.MeshEyeList(gender))
            MaterialManager.ChangeMaterialColor(characterMeshes.Find(mesh.Key), color, mesh.Value);
    }
}
