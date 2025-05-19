namespace Cursach_1;

public static class BorderingInverter
{
    public static Matrix Invert(Matrix matrix)
    {
        int n = matrix.N;
        if (n <= 0)
            throw new ArgumentException("Матриця повинна мати розмір більше 0");

        var inverse = new Matrix(n);
        double a00 = matrix[0, 0];
        if (Math.Abs(a00) < double.Epsilon)
            throw new InvalidOperationException("Матриця вироджена");
        inverse[0, 0] = 1.0 / a00;

        for (int k = 2; k <= n; k++)
        {
            var u = new double[k - 1];
            var v = new double[k - 1];
            for (int i = 0; i < k - 1; i++)
            {
                u[i] = matrix[i, k - 1];
                v[i] = matrix[k - 1, i];
            }
            double akk = matrix[k - 1, k - 1];

            var invTimesU = Multiply(inverse, u, k - 1);
            var invTimesV = new double[k - 1];
            for (int j = 0; j < k - 1; j++)
                invTimesV[j] = Dot(v, GetColumn(inverse, j, k - 1));

            double vInvU = Dot(v, invTimesU);
            double S = akk - vInvU;
            if (Math.Abs(S) < double.Epsilon)
                throw new InvalidOperationException($"Schur-комплемент для k={k} рівний нулю, матриця вироджена");

            var newInvBlock = new double[k, k];
            for (int i = 0; i < k - 1; i++)
                for (int j = 0; j < k - 1; j++)
                    newInvBlock[i, j] = inverse[i, j] + invTimesU[i] * invTimesV[j] / S;

            for (int i = 0; i < k - 1; i++)
                newInvBlock[i, k - 1] = -invTimesU[i] / S;

            for (int j = 0; j < k - 1; j++)
                newInvBlock[k - 1, j] = -invTimesV[j] / S;

            newInvBlock[k - 1, k - 1] = 1.0 / S;

            for (int i = 0; i < k; i++)
                for (int j = 0; j < k; j++)
                    inverse[i, j] = newInvBlock[i, j];
        }

        return inverse;
    }

    private static double[] Multiply(Matrix inv, double[] vector, int size)
    {
        var result = new double[size];
        for (int i = 0; i < size; i++)
        {
            double sum = 0;
            for (int j = 0; j < size; j++)
                sum += inv[i, j] * vector[j];
            result[i] = sum;
        }
        return result;
    }

    private static double Dot(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
            sum += a[i] * b[i];
        return sum;
    }

    private static double[] GetColumn(Matrix mat, int col, int size)
    {
        var column = new double[size];
        for (int i = 0; i < size; i++)
            column[i] = mat[i, col];
        return column;
    }
}
