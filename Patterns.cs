using System.Collections.Generic;

namespace WotBot
{
    internal class Patterns
    {
        // Colors for Chest Reticle recognition
        public static readonly List<string> chestColors = new List<string>() { "FF0000FF", "FFFFFFFF" };
        // Pattern for Chest Reticle recognition, top arrow
        public static readonly bool[,] chestTop = new bool[9, 12]
        {
            {true, true, true, true, true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true, true, true, true},
            {false, false, false, false, true, true, true, true, false, false, false, false},
            {false, false, false, false, true, true, true, true, false, false, false, false},
            {false, false, false, false, true, true, true, true, false, false, false, false},
            {false, false, false, false, true, true, true, true, false, false, false, false},
            {false, false, false, false, false, false, false, false, false, false, false, false}
        };
        // Pattern for Chest Reticle recognition, left arrow
        public static readonly bool[,] chestLeft = new bool[12, 8]
        {
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false},
            {true, true, true, true, false, false, false, false}
        };

        // Colors for Taco Truck recognition
        public static readonly List<string> tacoColors = new List<string>() { "818181FF" };
        // Pattern for Taco Truck recognition
        public static readonly bool[,] tacoTruck = new bool[15, 17]
        {
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, false, false, false, false, false, false,false, false, false, false, false, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
            {true, true, true, true, true, true, true, true, true,true, true, true, true, true, true, true, true},
        };
    }
}
