

namespace BackendApp.Util;


public static class MatrixOperations
{
    public static double[,] Multiply(double[,] a, double[,] b)
    {
        if(a.GetLongLength(1) != b.GetLongLength(0)) 
            throw new ArgumentException("The 2 matrices must have dimension AxK and KxB respectively.");
        
        long dim0 = a.GetLongLength(0);
        long dim1 = b.GetLongLength(1);
        long commonDim = a.GetLongLength(1);

        double[,] result = new double[dim0,dim1];

        for(long i = 0; i < dim0; i++)
        {
            for(long j = 0; j < dim1; j++)
            {
                result[i,j] = 0;
                for(long k = 0; k < commonDim; k++)
                {
                    result[i,j] += a[i,k]*b[k,j];
                }
            }
        } 
        return result;
    }

    public static double DotProduct(double[] a, double[] b)
    {
        return a.Zip(b, (a,b) => a*b).Aggregate( (x,y) => x + y );
    }

    public static double[,] RandomDoubleMatrix(ulong dim0, ulong dim1, double rangeStart = 0, double rangeEnd = 1)
    {
        var random = new Random();
        var result = new double[dim0, dim1];
        for(ulong i = 0; i < dim0; i ++)
            for(ulong j = 0; j < dim1; j++)
                result[i,j] = random.NextDouble() * (rangeEnd - rangeStart) + rangeStart;

        return result;
    }

    public static double[,] NormalizeMatrixKindOfIdk(double[,] matrix)
    {
        var max = matrix.Cast<double>().Max();
        var min = matrix.Cast<double>().Min();
        var result = new double[matrix.GetLongLength(0), matrix.GetLongLength(1)];
        for(int i = 0; i < matrix.GetLongLength(0); i++)
            for(int j = 0; j < matrix.GetLongLength(1); j++)
                result[i, j] = matrix[i, j] * (min - max) + min;
        return result;
    }

    public static double MatrixMin(double[,] matrix) => matrix.Cast<double>().Min();
    public static double MatrixMax(double[,] matrix) => matrix.Cast<double>().Max();
    public static double[,] Subtract(double[,] a, double[,] b)
    {
        if(
            a.GetLongLength(0) != b.GetLongLength(0) 
            || a.GetLongLength(1) != b.GetLongLength(1)
        ) throw new ArgumentException("Arrays must have the same dimensions.");
        var dim0 = a.GetLongLength(0);
        var dim1 = a.GetLongLength(1);

        var result = new double[dim0, dim1];
        for(long i = 0; i < dim0; i++)
            for(long j = 0; j < dim1; j++)
                result[i,j] = a[i,j] - b[i,j];
        return result;
    }

    public static double SquareError(double[,] data, double[,] approximation)
    {
        return MatrixOperations
            .Subtract(data, approximation)
            .Cast<double>()
            .Select( a => a*a )
            .Sum();
    }

    public static T[] GetColumn<T>(T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRow<T>(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

}