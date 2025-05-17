namespace Cursach_1
{
    public static class LupInverter
    {
        public static Matrix Invert(Matrix m)
        {
            int n = m.N;
            var (L, U, P) = Decompose(m);
            if (Math.Abs(Determinant(U)) < 1e-9)
                throw new InvalidOperationException("Матриця вироджена (Визначник матриці рівний нулю)");
            var inv = new Matrix(n);
            for (int i = 0; i < n; i++)
            {
                var e = new double[n];
                e[i] = 1;
                var y = ForwardSolve(L, Permute(P, e));
                var x = BackwardSolve(U, y);
                for (int j = 0; j < n; j++) inv[j, i] = x[j];
            }

            return inv;
        }

        private static (Matrix L, Matrix U, int[] P) Decompose(Matrix m)
        {
            int n = m.N;
            var L = new Matrix(n);
            var U = new Matrix(n);
            var P = new int[n];
            for (int i = 0; i < n; i++)
            {
                P[i] = i;
            }

            for (int k = 0; k < n; k++)
            {
                double max = 0;
                int pivot = k;
                for (int i = k; i < n; i++)
                {
                    if (Math.Abs(m[i, k]) > max)
                    {
                        max = Math.Abs(m[i, k]);
                        pivot = i;
                    }
                }
                if (max < 1e-9) throw new InvalidOperationException("Матриця вироджена (Визначник матриці рівний нулю)");
                
                (P[k], P[pivot]) = (P[pivot], P[k]);
                
                for (int j = 0; j < n; j++)
                {
                    (m[k, j], m[pivot, j]) = (m[pivot, j], m[k, j]);
                }

                for (int j = 0; j < k; j++)
                {
                    (L[k, j], L[pivot, j]) = (L[pivot, j], L[k, j]);
                }

                L[k, k] = 1;

                for (int i = k + 1; i < n; i++)
                {
                    L[i, k] = m[i, k] / m[k, k];
                    for (int j = k; j < n; j++)
                    {
                        m[i, j] -= L[i, k] * m[k, j];
                    }
                }

                for (int j = k; j < n; j++)
                {
                    U[k, j] = m[k, j];
                }
            }

            return (L, U, P);
        }

        private static double[] Permute(int[] P, double[] b)
        {
            int n = P.Length;
            var res = new double[n];
            for (int i = 0; i < n; i++) res[i] = b[P[i]];
            return res;
        }

        private static double[] ForwardSolve(Matrix L, double[] b)
        {
            int n = L.N;
            var y = new double[n];
            for (int i = 0; i < n; i++)
            {
                y[i] = b[i];
                for (int j = 0; j < i; j++) y[i] -= L[i, j] * y[j];
            }

            return y;
        }

        private static double[] BackwardSolve(Matrix U, double[] y)
        {
            int n = U.N;
            var x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = y[i];
                for (int j = i + 1; j < n; j++) x[i] -= U[i, j] * x[j];
                x[i] /= U[i, i];
            }

            return x;
        }

        private static double Determinant(Matrix m)
        {
            double det = 1;
            for (int i = 0; i < m.N; i++)
            {
                det *= m[i, i];
            }
            return det;
        }
        
    }
}