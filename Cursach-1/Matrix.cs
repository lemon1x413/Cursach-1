using System.Data;
namespace Cursach_1
{
    public class Matrix
    {
        public int N { get; }
        private double[,] _matrix;

        public Matrix(int n)
        {
            if (n <= 0) throw new ArgumentException("Розмір повинен бути > 0");
            N = n;
            _matrix = new double[n, n];
        }

        public double this[int i, int j]
        {
            get => _matrix[i, j];
            set => _matrix[i, j] = value;
        }

        public static Matrix RandomGenerate(int size = 3)
        {
            var rand = new Random();
            var m = new Matrix(size);
            for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                m[i, j] = rand.NextDouble() * 10;
            return m;
        }

        public DataTable ToDataTable()
        {
            var dataTable = new DataTable();
            for (int j = 0; j < N; j++) dataTable.Columns.Add(j.ToString(), typeof(double));
            for (int i = 0; i < N; i++)
            {
                var row = dataTable.NewRow();
                for (int j = 0; j < N; j++) row[j] = _matrix[i, j];
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}