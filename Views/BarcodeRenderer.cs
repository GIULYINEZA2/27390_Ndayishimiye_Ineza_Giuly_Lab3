using System;
using System.Collections.Generic;
using System.Drawing;

namespace LabTestSystem.Views;

public static class BarcodeRenderer
{
    // 9 elements per character (bar, space, bar...), 1 = wide, 0 = narrow
    private static readonly Dictionary<char, string> Patterns = new Dictionary<char, string>
    {
        { '0', "000110100" }, { '1', "100100001" }, { '2', "001100001" }, { '3', "101100000" },
        { '4', "000110001" }, { '5', "100110000" }, { '6', "001110000" }, { '7', "000100101" },
        { '8', "100100100" }, { '9', "001100100" }, { 'A', "100001001" }, { 'B', "001001001" },
        { 'C', "101001000" }, { 'D', "000011001" }, { 'E', "100011000" }, { 'F', "001011000" },
        { 'G', "000001101" }, { 'H', "100001100" }, { 'I', "001001100" }, { 'J', "000011100" },
        { 'K', "100000011" }, { 'L', "001000011" }, { 'M', "101000010" }, { 'N', "000010011" },
        { 'O', "100010010" }, { 'P', "001010010" }, { 'Q', "000000111" }, { 'R', "100000110" },
        { 'S', "001000110" }, { 'T', "000010110" }, { 'U', "110000001" }, { 'V', "011000001" },
        { 'W', "111000000" }, { 'X', "010010001" }, { 'Y', "110010000" }, { 'Z', "011010000" },
        { '*', "010010100" }
    };

    private const int Narrow = 2, Wide = 5, Quiet = 10, BarHeight = 44, TextHeight = 16;

    public static Bitmap Render(string text)
    {
        string data = "*" + text.ToUpperInvariant() + "*";

        int width = Quiet * 2;
        foreach (char ch in data)
        {
            if (!Patterns.TryGetValue(ch, out string pattern))
                throw new ArgumentException("Character '" + ch + "' cannot be shown in a Code 39 barcode.");
            foreach (char element in pattern) width += element == '1' ? Wide : Narrow;
            width += Narrow;
        }

        var bitmap = new Bitmap(width, BarHeight + TextHeight);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            int x = Quiet;
            foreach (char ch in data)
            {
                string pattern = Patterns[ch];
                for (int i = 0; i < pattern.Length; i++)
                {
                    int w = pattern[i] == '1' ? Wide : Narrow;
                    if (i % 2 == 0) g.FillRectangle(Brushes.Black, x, 0, w, BarHeight);
                    x += w;
                }
                x += Narrow;
            }

            using var font = new Font("Segoe UI", 8F);
            using var format = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(text, font, Brushes.Black, new RectangleF(0, BarHeight + 1, width, TextHeight), format);
        }
        return bitmap;
    }
}