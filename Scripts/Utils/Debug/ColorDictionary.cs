using System.Collections.Generic;

namespace Game.Scripts.Utils.Debug
{
    public enum DebugColor : byte
    {
        Black = 0,
        Gray = 1,
        Green = 2,
        Yellow = 3,
        Orange = 4,
        Blue = 5,
        Red = 6,
        Magenta = 7,
        White = 8
    }
    
    public sealed class ColorDictionary
    {
        private const string Black = "000000";
        private const string Gray = "adadad";
        private const string Green = "19e619";
        private const string Yellow = "f0f409";
        private const string Orange = "ff9900";
        private const string Blue = "00bfff";
        private const string Red = "e34234";
        private const string Magenta = "ce29ff";
        private const string White = "ffffff";

        public static readonly Dictionary<DebugColor, string> Colors = new()
        {
            { DebugColor.Black, Black },
            { DebugColor.Gray, Gray },
            { DebugColor.Green, Green },
            { DebugColor.Yellow, Yellow },
            { DebugColor.Orange, Orange },
            { DebugColor.Blue, Blue },
            { DebugColor.Red, Red },
            { DebugColor.Magenta, Magenta },
            { DebugColor.White, White },
        };
    }
}