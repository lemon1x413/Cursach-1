﻿using System.IO;

namespace Cursach_1;

public static class FileHandler
{
    public static Matrix LoadMatrix(string path)
    {
        var lines = File.ReadAllLines(path)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();
        int n = lines.Length;
        if (n > 10) throw new FormatException("Матриця повинна бути розміром не більше 10 х 10.");
        var matrix = new Matrix(n);
        for (int i = 0; i < n; i++)
        {
            var parts = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != n) throw new FormatException("Некоректна кількість елементів у рядку, або файл не містить матриці.");
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = double.Parse(parts[j]);
            }
        }

        return matrix;
    }

    public static void SaveMatrix(Matrix matrix, string path)
    {
        using var streamWriter = new StreamWriter(path);
        var n = matrix.N;
        for (int i = 0; i < n; i++)
        {
            var row = string.Join(" ", Enumerable.Range(0, matrix.N).Select(j => matrix[i, j].ToString("F2")));
            streamWriter.WriteLine(row);
        }
    }
}