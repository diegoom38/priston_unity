using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public static class MaterialManager
    {
        public static void ChangeMaterialColor(Transform partTransform, Color color, string property)
        {
            if (partTransform != null && partTransform.TryGetComponent<Renderer>(out Renderer renderer))
            {
                if (renderer.material.HasProperty(property))
                {
                    renderer.material.SetColor(property, color);
                }
            }
        }

        public static List<KeyValuePair<string, string>> MeshAttributeList(
            string gender,
            string attribute
        )
        {
            var maleMeshes = new List<KeyValuePair<string, string>>();
            var femaleMeshes = new List<KeyValuePair<string, string>>();


            maleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Armor_head_01", attribute),
                    new("PT_Armor_hair_01", attribute),
                    new("PT_Armor_cloth_00_body", attribute),
                    new("PT_Armor_beard_20", attribute),
                };

            femaleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Armor_head_01", attribute),
                    new("PT_Armor_hair_01", attribute),
                    new("PT_Armor_cloth_00_body", attribute),
                };


            return gender.ToLower() == "male" ? maleMeshes : femaleMeshes;
        }

        public static List<KeyValuePair<string, string>> MeshSkinsList(string gender)
        {
            return MeshAttributeList(gender, "_SKINCOLOR");
        }

        public static List<KeyValuePair<string, string>> MeshHairList(string gender)
        {
            return MeshAttributeList(gender, "_HAIRCOLOR");
        }

        public static List<KeyValuePair<string, string>> MeshEyeList(string gender)
        {
            return MeshAttributeList(gender, "_EYESCOLOR");
        }
    }
}
