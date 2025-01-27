using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

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
            string attribute,
            string age = null
        )
        {
            var maleMeshes = new List<KeyValuePair<string, string>>();
            var femaleMeshes = new List<KeyValuePair<string, string>>();
            if (age == "Elderly")
            {
                maleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Male_Peasant_01_head", attribute),
                    new("PT_Male_Peasant_01_lower", attribute),
                    new("PT_Male_Peasant_01_upper", attribute)
                };

                femaleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Female_Peasant_01_head", attribute),
                    new("PT_Female_Peasant_01_lower_pants", attribute),
                    new("PT_Female_Peasant_01_upper_short", attribute)
                };
            }
            else if (age == "Adult")
            {
                maleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Male_Armor_head_01", attribute),
                    new("PT_Male_Armor_hair_01", attribute),
                    new("PT_Male_Armor_cloth_00_body", attribute),
                    new("PT_Male_Armor_beard_20", attribute),
                };

                femaleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("PT_Female_Armor_head_01", attribute),
                    new("PT_Female_Armor_hair_01", attribute),
                    new("PT_Female_Armor_cloth_00_body", attribute),
                };
            }

            return gender.ToLower() == "male" ? maleMeshes : femaleMeshes;
        }

        public static List<KeyValuePair<string, string>> MeshSkinsList(string gender, string age)
        {
            return MeshAttributeList(gender, "_SKINCOLOR", age);
        }

        public static List<KeyValuePair<string, string>> MeshHairList(string gender, string age)
        {
            return MeshAttributeList(gender, "_HAIRCOLOR", age);
        }

        public static List<KeyValuePair<string, string>> MeshEyeList(string gender, string age)
        {
            return MeshAttributeList(gender, "_EYESCOLOR", age);
        }
    }
}
