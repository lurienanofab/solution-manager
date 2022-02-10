using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SolutionManager;

public static class ColorExtensions
{
    private static readonly Dictionary<ConsoleColor, Color> ColorMap = new Dictionary<ConsoleColor, Color>
    {
        { ConsoleColor.Black, Color.Black }, //Black = 0
        { ConsoleColor.DarkBlue, Color.DarkBlue }, //DarkBlue = 1
        { ConsoleColor.DarkGreen, Color.DarkGreen }, //DarkGreen = 2
        { ConsoleColor.DarkCyan, Color.DarkCyan }, //DarkCyan = 3
        { ConsoleColor.DarkRed, Color.DarkRed}, //DarkRed = 4
        { ConsoleColor.DarkMagenta, Color.DarkMagenta }, //DarkMagenta = 5
        { ConsoleColor.DarkYellow, Color.FromArgb(0x808000) }, //DarkYellow = 6
        { ConsoleColor.Gray, Color.Gray }, //Gray = 7
        { ConsoleColor.DarkGray, Color.DarkGray }, //DarkGray = 8
        { ConsoleColor.Blue, Color.Blue }, //Blue = 9
        { ConsoleColor.Green, Color.Green }, //Green = 10
        { ConsoleColor.Cyan, Color.Cyan }, //Cyan = 11
        { ConsoleColor.Red, Color.Red }, //Red = 12
        { ConsoleColor.Magenta, Color.Magenta }, //Magenta = 13
        { ConsoleColor.Yellow, Color.Yellow }, //Yellow = 14
        { ConsoleColor.White, Color.White }  //White = 15
    };

    public static Color FromConsoleColor(this ConsoleColor c)
    {
        if (ColorMap.ContainsKey(c))
            return ColorMap[c];
        else
            return ColorMap[ConsoleColor.White];
    }

    public static ConsoleColor ToConsoleColor(this Color c)
    {
        var map = ColorMap.ToList().ToDictionary(x => x.Value, x => x.Key);

        if (map.ContainsKey(c))
            return map[c];
        else
            return map[Color.White];
    }
}