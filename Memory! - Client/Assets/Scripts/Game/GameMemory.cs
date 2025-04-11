using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

public class GameMemory
{
    static (int, bool)[,] DuplicateMatrix((int, bool)[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        (int, bool)[,] copy = new (int, bool)[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copy[i, j] = matrix[i, j];
            }
        }

        return copy;
    }

    public int Height { private set; get; }
    public int Width { private set; get; }
    (int, bool)[,] matrix;
    public GameMemory((int, bool)[,] entry)
    {
        matrix = DuplicateMatrix(entry);
        Height = matrix.GetLength(0);
        Width = matrix.GetLength(1);
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