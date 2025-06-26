using System.Collections.Generic;

namespace Scripts.Manager
{
    public class SpecsManager
    {
        public static Dictionary<string, string> GetHeadOptions() {
            return new Dictionary<string, string>
            {
                { "Head_01", "Nova" },
                { "Head_02", "Judiada"},
            };        
        }

        public static Dictionary<string, string> GetHairOptions()
        {
            return new Dictionary<string, string>
            {
                { "Hair_01", "Arrumado" },
                { "Hair_02", "Bagunçado"},
            };
        }
    }
}
