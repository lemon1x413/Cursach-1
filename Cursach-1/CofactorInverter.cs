namespace Cursach_1
{
    public static class CofactorInverter
    {
        public static Matrix Invert(Matrix matrix)
        {
            int n = matrix.N;
            double det = Determinant(matrix);
            if (Math.Abs(det) < 1e-9) throw new InvalidOperationException("Матриця вироджена");
            var cof = CofactorMatrix(matrix);
            var inv = Transpose(cof);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    inv[i, j] /= det;
                }
            }
            return inv;
        }
        
        private static double Determinant(Matrix matrix)
        {
            int n = matrix.N;
            if (n == 1) return matrix[0, 0];
            if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            double det = 0;
            for (int j = 0; j < n; j++)
                det += Math.Pow(-1, j) * matrix[0, j] * Determinant(Submatrix(matrix, 0, j));
            return det;
        }
        
        private static Matrix CofactorMatrix(Matrix matrix)
        {
            int n = matrix.N;
            var cof = new Matrix(n);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                cof[i, j] = Math.Pow(-1, i + j) * Determinant(Submatrix(matrix, i, j));
            return cof;
        }

        private static Matrix Submatrix(Matrix matrix, int row, int col)
        {
            int n = matrix.N;
            var sub = new Matrix(n - 1);
            int r = 0;
            for (int i = 0; i < n; i++)
                if (i != row)
                {
                    int c = 0;
                    for (int j = 0; j < n; j++)
                        if (j != col)
                            sub[r, c++] = matrix[i, j];
                    r++;
                }
            return sub;
        }

        private static Matrix Transpose(Matrix matrix)
        {
            int n = matrix.N;
            var t = new Matrix(n);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                t[i, j] = matrix[j, i];
            return t;
        }
        
    }
}