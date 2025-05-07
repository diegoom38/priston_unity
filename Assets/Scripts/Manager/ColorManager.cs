using Assets.Scripts.Manager;
using System;
using System.Collections.Generic;

namespace Scripts.Manager
{
    public class ColorManager
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public ColorManager(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public UnityEngine.Color ToColorEngine() => new UnityEngine.Color(R / 255f, G / 255f, B / 255f);

        public static List<ColorManager> GetHairColors()
        {
            return new List<ColorManager>
            {
                new(255, 245, 225),
                new(229, 220, 168),
                new(230, 206, 168),
                new(220, 208, 186),
                new(214, 196, 194),
                new(202, 191, 177),
                new(183, 166, 158),
                new(222, 188, 153),
                new(184, 151, 120),
                new(183, 65, 14),
                new(167, 133, 106),
                new(151, 121, 97),
                new(145, 85, 61),
                new(165, 107, 70),
                new(106, 78, 66),
                new(85, 72, 56),
                new(83, 61, 50),
                new(80, 68, 68),
                new(78, 67, 63),
                new(59, 48, 36),
                new(44, 34, 43),
                new(9, 8, 6),
                new(0, 0, 0),
            };
        }

        public static List<ColorManager> GetSkinColors()
        {
            return new List<ColorManager>
            {
                new(255, 206, 180),
                new(255, 195, 170),
                new(240, 184, 160),
                new(225, 172, 150),
                new(210, 161, 140),
                new(195, 149, 130),
                new(180, 138, 120),
                new(165, 126, 110),
                new(150, 114, 100),
                new(135, 103, 90),
                new(120, 92, 80),
                new(105, 80, 70),
                new(90, 69, 60),
                new(75, 57, 50),
                new(60, 46, 30),
                new(45, 34, 30),
            };
        }

        public static List<ColorManager> GetEyeColors()
        {
            return new List<ColorManager>
            {
                new(238, 238, 238),
                new(231, 156, 74),
                new(218, 120, 91),
                new(76, 122, 112),
                new(73, 118, 101),
                new(46, 83, 111),
                new(28, 128, 71),
                new(61, 103, 29),
                new(99, 78, 52),
                new(4, 49, 69),
                new(0, 0, 0),
            };
        }

        public static List<ColorManager> GetLipsColors()
        {
            return new List<ColorManager>
            {
                new(157, 41, 51),
                new(255, 216, 190),
                new(210, 180, 140),
                //new(255, 105, 180),
                new(114, 47, 55),
                new(255, 127, 80),
                //new(139, 0, 139),
                new(107, 68, 35),
                new(183, 132, 167),
                //new(183, 132, 167),
            };
        }

        private static ColorManager GetRandomColor(List<ColorManager> colors)
        {
            Random random = new();
            int index = random.Next(colors.Count);
            return colors[index];
        }

        public static CharacterCreationRandomize Randomize()
        {
            ColorManager randomHairColor = GetRandomColor(GetHairColors());
            ColorManager randomSkinColor = GetRandomColor(GetSkinColors());
            ColorManager randomEyeColor = GetRandomColor(GetEyeColors());
            ColorManager randomLipColor = GetRandomColor(GetLipsColors());

            return new()
            {
                HairColor = randomHairColor,
                SkinColor = randomSkinColor,
                EyeColor = randomEyeColor,
                LipColor = randomLipColor
            };
        }
    }
}
