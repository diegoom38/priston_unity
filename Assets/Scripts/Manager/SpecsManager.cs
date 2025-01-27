using System.Collections.Generic;

namespace Scripts.Manager
{
    public class HairManager
    {
        public static List<string> GetHairOptions() {
            return new List<string>
            {
                "Social",
            };        
        }
    }

    public class AgeManager
    {
        public static List<string> GetAgeOptions()
        {
            return new List<string>
            {
                "Adulto",
                "Ancião",

            };
        }
    }
}
