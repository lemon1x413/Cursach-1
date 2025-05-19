using System.IO;

namespace Cursach_1
{
    public static class FileHandler
    {
        public static Matrix LoadMatrix(string path)
        {
            var lines = File.ReadAllLines(path)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();
            int n = lines.Length;
            var matrix = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                var parts = lines[i].Split(new[] { ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != n) throw new FormatException("Некоректна кількість елементів у рядку");
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = double.Parse(parts[j]);
                }
            }
            return matrix;
        }

        public static void SaveMatrix(Matrix matrix, string path)
        {
            using var streamWriterw = new StreamWriter(path);
            for (int i = 0; i < matrix.N; i++)
            {
                var row = string.Join(" ", Enumerable.Range(0, matrix.N).Select(j => matrix[i, j].ToString("F2")));
                streamWriterw.WriteLine(row);
            }
        }
    }
}
