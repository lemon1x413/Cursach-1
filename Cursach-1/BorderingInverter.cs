namespace Cursach_1;

public static class BorderingInverter
{
    public static readonly IterationCounter IterationCounter = new();

    public static Matrix Invert(Matrix matrix)
    {
        IterationCounter.ResetCurrentMethod();
        if (!IsInvertible(matrix))
            throw new InvalidOperationException("Матриця вироджена (визначник матриці рівний нулю)");
        int n = matrix.N;
        var inverse = new Matrix(n);
        double a00 = matrix[0, 0];
        inverse[0, 0] = 1.0 / a00;

        for (int k = 2; k <= n; k++)
        {
            IterationCounter.AddIteration();
            var u = new double[k - 1];
            var v = new double[k - 1];
            for (int i = 0; i < k - 1; i++)
            {
                IterationCounter.AddIteration();
                u[i] = matrix[i, k - 1];
                v[i] = matrix[k - 1, i];
            }

            double akk = matrix[k - 1, k - 1]; // a_kk

            var invTimesU = Multiply(inverse, u, k - 1);
            var invTimesV = new double[k - 1];
            for (int j = 0; j < k - 1; j++)
            {
                IterationCounter.AddIteration();
                invTimesV[j] = Dot(v, GetColumn(inverse, j, k - 1));
            }
            
            double S = akk - Dot(v, invTimesU); //a_k
            if (Math.Abs(S) < double.Epsilon)
                throw new InvalidOperationException($"Число шура для k={k} рівне нулю, матриця вироджена (визначник матриці рівний нулю)");
            // B^-1_k-1
            var newInvBlock = new double[k, k];
            for (int i = 0; i < k - 1; i++)
            {
                IterationCounter.AddIteration();
                for (int j = 0; j < k - 1; j++)
                {
                    IterationCounter.AddIteration();
                    newInvBlock[i, j] = inverse[i, j] + invTimesU[i] * invTimesV[j] / S;
                }
            }
            // r_k
            for (int i = 0; i < k - 1; i++)
            {
                IterationCounter.AddIteration();
                newInvBlock[i, k - 1] = -invTimesU[i] / S;
            }
            // q_k
            for (int j = 0; j < k - 1; j++)
            {
                IterationCounter.AddIteration();
                newInvBlock[k - 1, j] = -invTimesV[j] / S;
            }
            // 1/a_k
            newInvBlock[k - 1, k - 1] = 1.0 / S;

            for (int i = 0; i < k; i++)
            {
                IterationCounter.AddIteration();
                for (int j = 0; j < k; j++)
                {
                    IterationCounter.AddIteration();
                    inverse[i, j] = newInvBlock[i, j];
                }
            }
        }

        return inverse;
    }

    private static bool IsInvertible(Matrix matrix)
    {
        if (matrix[0, 0] == 0) 
            return false;
        int n = matrix.N;
        var lu = new Matrix(n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                lu[i, j] = matrix[i, j];
            }
        }
        for (int k = 0; k < n; k++)
        {
            double max = 0;
            int pivot = k;
            for (int i = k; i < n; i++)
            {
                if (Math.Abs(matrix[i, k]) > max)
                {
                    max = Math.Abs(matrix[i, k]);
                    pivot = i;
                }
            }

            if (max < 1e-9)
            {
                return false;
            }

            if (pivot != k)
            {
                for (int j = k; j < n; j++)
                {
                    (lu[k, j], lu[pivot, j]) = (lu[pivot, j], lu[k, j]);
                }
            }

            for (int i = k + 1; i < n; i++)
            {
                double factor = lu[i, k] / lu[k, k];
                for (int j = k; j < n; j++)
                {
                    lu[i, j] -= factor * lu[k, j];
                }
            }
        }
        double det = 1;
        for (int i = 0; i < n; i++)
        {
            det *= lu[i, i];
        }

        return Math.Abs(det) > 1e-9;
    }

    
    private static double[] Multiply(Matrix inv, double[] vector, int size)
    {
        var result = new double[size];
        for (int i = 0; i < size; i++)
        {
            double sum = 0;
            for (int j = 0; j < size; j++)
            {
                sum += inv[i, j] * vector[j];
            }

            result[i] = sum;
        }

        return result;
    }

    private static double Dot(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            sum += a[i] * b[i];
        }

        return sum;
    }

    private static double[] GetColumn(Matrix matrix, int col, int size)
    {
        var column = new double[size];
        for (int i = 0; i < size; i++)
        {
            column[i] = matrix[i, col];
        }

        return column;
    }
}