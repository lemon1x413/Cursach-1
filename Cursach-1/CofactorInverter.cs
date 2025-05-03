namespace Cursach_1
{
    public static class CofactorInverter
    {
        public static Matrix Invert(Matrix m)
        {
            int n = m.N;
            double det = Determinant(m);
            if (Math.Abs(det) < 1e-9) throw new InvalidOperationException("Матриця вироджена");
            var cof = CofactorMatrix(m);
            var inv = Transpose(cof);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                inv[i, j] /= det;
            return inv;
        }

        private static double Determinant(Matrix m)
        {
            int n = m.N;
            if (n == 1) return m[0, 0];
            if (n == 2) return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            double det = 0;
            for (int j = 0; j < n; j++)
                det += Math.Pow(-1, j) * m[0, j] * Determinant(Submatrix(m, 0, j));
            return det;
        }

        private static Matrix CofactorMatrix(Matrix m)
        {
            int n = m.N;
            var cof = new Matrix(n);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                cof[i, j] = Math.Pow(-1, i + j) * Determinant(Submatrix(m, i, j));
            return cof;
        }

        private static Matrix Submatrix(Matrix m, int row, int col)
        {
            int n = m.N;
            var sub = new Matrix(n - 1);
            int r = 0;
            for (int i = 0; i < n; i++)
                if (i != row)
                {
                    int c = 0;
                    for (int j = 0; j < n; j++)
                        if (j != col)
                            sub[r, c++] = m[i, j];
                    r++;
                }

            return sub;
        }

        private static Matrix Transpose(Matrix m)
        {
            int n = m.N;
            var t = new Matrix(n);
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                t[i, j] = m[j, i];
            return t;
        }
    }
}