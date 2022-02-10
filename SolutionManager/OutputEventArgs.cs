using System;
using System.Collections.Generic;
using System.Drawing;

namespace SolutionManager;

public class OutputEventArgs : EventArgs
{
    private readonly List<Format> _formats = new List<Format>();

    public OutputEventArgs(string text, Color? foreColor, Color? backColor = null) : this(text)
    {
        AddFormat(0, text.Length, foreColor, backColor);
    }

    public OutputEventArgs(string text)
    {
        Text = text;
    }

    public string Text { get; set; }
    public IEnumerable<Format> Formats => _formats;

    public void AddFormat(int index, int length, Color? foreColor, Color? backColor)
    {
        _formats.Add(new Format(index, length, foreColor, backColor));
    }

    public Color GetForeColorAtPosition(int pos, Color defval)
    {
        foreach(var fmt in _formats)
        {
            if (pos >= fmt.Index && pos < fmt.Index + fmt.Length)
                return fmt.ForeColor ?? defval;
        }

        return defval;
    }

    public Color GetBackColorAtPosition(int pos, Color defval)
    {
        foreach (var fmt in _formats)
        {
            if (pos >= fmt.Index && pos < fmt.Index + fmt.Length)
                return fmt.BackColor ?? defval;
        }

        return defval;
    }
}

public struct Format
{
    public Format(int index, int length, Color? foreColor, Color? backColor)
    {
        Index = index;
        Length = length;
        ForeColor = foreColor;
        BackColor = backColor;
    }

    public int Index { get; }
    public int Length { get; }
    public Color? ForeColor { get; }
    public Color? BackColor { get; }
}