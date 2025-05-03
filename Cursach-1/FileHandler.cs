using System;
using System.IO;
using System.Linq;

namespace Cursach_1
{
    public static class FileHandler
    {
        public static Matrix LoadMatrix(string path)
        {
            var lines = File.ReadAllLines(path)
                .Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            int n = lines.Length;
            var m = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                var parts = lines[i].Split(new[] { ' ', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != n) throw new FormatException("Некоректна кількість елементів у рядку");
                for (int j = 0; j < n; j++) m[i, j] = double.Parse(parts[j]);
            }

            return m;
        }

        public static void SaveMatrix(Matrix m, string path)
        {
            using var sw = new StreamWriter(path);
            for (int i = 0; i < m.N; i++)
            {
                var row = string.Join(" ", Enumerable.Range(0, m.N).Select(j => m[i, j].ToString()));
                sw.WriteLine(row);
            }
        }
    }
}
