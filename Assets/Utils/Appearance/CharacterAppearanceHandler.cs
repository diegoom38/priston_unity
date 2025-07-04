using Assets.Models;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterAppearanceHandler
{
    public delegate void AppearanceHandler(Transform meshes, Personagem character);

    public static List<AppearanceHandler> GetAppearanceHandlers()
    {
        return new List<AppearanceHandler>
        {
            (meshes, character) =>
                CharacterAppearance.ChangeSkinColor(
                    CharacterAppearance.GetColor(character.configuracao.configuracaoCorPele),
                    meshes,
                    character.configuracao.gender),

            (meshes, character) =>
                CharacterAppearance.ChangeHairColor(
                    CharacterAppearance.GetColor(character.configuracao.configuracaoCorCabelo),
                    meshes,
                    character.configuracao.gender),

            (meshes, character) =>
                CharacterAppearance.ChangeEyeColor(
                    CharacterAppearance.GetColor(character.configuracao.configuracaoCorOlhos),
                    meshes,
                    character.configuracao.gender),

            (meshes, character) =>
                CharacterAppearance.ChangeLipColor(
                    CharacterAppearance.GetColor(character.configuracao.configuracaoCorLabios),
                    meshes,
                    character.configuracao.gender),

            (meshes, character) =>
                CharacterAppearance.DropdownValueChangedDefault(
                    character.configuracao.head,
                    "Head",
                    Scripts.Manager.SpecsManager.GetHeadOptions(),
                    meshes),

            (meshes, character) =>
                CharacterAppearance.DropdownValueChangedDefault(
                    character.configuracao.hair,
                    "Hair",
                    Scripts.Manager.SpecsManager.GetHairOptions(),
                    meshes),

            (meshes, character) =>
                CharacterAppearance.UpdateScale(
                    character.configuracao.scale.x,
                    character.configuracao.scale.y,
                    character.configuracao.scale.z,
                    meshes)
        };
    }
}
