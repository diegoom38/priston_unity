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
                    new("Head/Head", attribute),
                    new("Hair/Hair", attribute),
                    new("Body/Body", attribute),
                    new("Hair/Beard_01", attribute),
                    new("Hair/Beard_02", attribute),
                };

            femaleMeshes = new List<KeyValuePair<string, string>>
                {
                    new("Head/Head", attribute),
                    new("Hair/Hair", attribute),
                    new("Body/Body", attribute),
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

        public static List<KeyValuePair<string, string>> MeshLipList(string gender)
        {
            return MeshAttributeList(gender, "_LIPSCOLOR");
        }
    }
}
