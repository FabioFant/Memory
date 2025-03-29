using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

public class Memory
{
    static void InitMatrix((int, bool)[,] matrix)
    {
        System.Random random = new System.Random();
        List<(int, int)> indexes = new List<(int, int)>(matrix.Length);

        for (int i = 0; i < matrix.GetLength(0); i++)
            for (int j = 0; j < matrix.GetLength(1); j++)
                indexes.Add((i, j));

        for (int i = 0; i < matrix.Length / 2; i++)
        {
            int randomIndex; (int, int) index;

            randomIndex = random.Next(0, indexes.Count);
            index = indexes[randomIndex];
            indexes.RemoveAt(randomIndex);
            matrix[index.Item1, index.Item2] = (i, false);

            randomIndex = random.Next(0, indexes.Count);
            index = indexes[randomIndex];
            indexes.RemoveAt(randomIndex);
            matrix[index.Item1, index.Item2] = (i, false);
        }
    }

    int Height { set; get; }
    int Width { set; get; }
    (int, bool)[,] matrix;
    public Memory(int height, int width)
    {
        if (height < 2) throw new ArgumentException($"ERROR : The height of the matrix must be greater or equal than 2; value received: {height}");
        // else if (height % 2 != 0) throw new ArgumentException($"ERROR : The height of the matrix must be even; value received: {height}");

        if (width < 2) throw new ArgumentException($"ERROR : The width of the matrix must be greater or equal than 2; value received: {width}");
        // else if (width % 2 != 0) throw new ArgumentException($"ERROR : The width of the matrix must be even; value received: {width}");

        if (height * width % 2 != 0) throw new ArgumentException($"ERROR : The area of the matrix must be even; values received: {height} (height) * {width} (width) = {height * width}");

        Height = height;
        Width = width;
        matrix = new (int, bool)[height, width];
        InitMatrix(matrix);

#if false
        string debug = "";
        for(int i = 0; i < matrix.GetLength(0); i++)
        {
            for(int j = 0; j < matrix.GetLength(1); j++)
            {
                debug += $"({matrix[i, j].Item1}, {matrix[i, j].Item2}) ";
            }
            debug += "\n";
        }
        UnityEngine.Debug.Log(debug);
#endif
    }

    public bool Pick(int row1, int col1, int row2, int col2)
    {
        (int, bool) cell1 = matrix[row1, col1];
        (int, bool) cell2 = matrix[row2, col2];

        if (cell1.Item2 || cell2.Item2) throw new InvalidOperationException($"ERROR : One or both of the two cells has already been selected; values received: {cell1.Item2} (cell1), {cell2.Item2} (cell2)");

        if (cell1.Item1 == cell2.Item1)
        {
            matrix[row1, col1].Item2 = true;
            matrix[row2, col2].Item2 = true;
            return true;
        }

        return false;
    }

    public bool IsFinished()
    {
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                if (!matrix[i, j].Item2)
                    return false;

        return true;
    }
}