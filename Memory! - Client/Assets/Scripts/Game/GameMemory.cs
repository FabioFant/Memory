using System;
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
        if (row1 < 0 || row1 >= Height) throw new ArgumentOutOfRangeException($"ERROR : The row index is out of bounds; value received: {row1} (height = {Height})");
        if (row2 < 0 || row2 >= Height) throw new ArgumentOutOfRangeException($"ERROR : The row index is out of bounds; value received: {row1} (height = {Height})");
        if (col1 < 0 || col1 >= Width) throw new ArgumentOutOfRangeException($"ERROR : The column index is out of bounds; value received: {col1} (width = {Width})");
        if (col2 < 0 || col2 >= Width) throw new ArgumentOutOfRangeException($"ERROR : The column index is out of bounds; value received: {col2} (width = {Width})");

        if (row1 == row2 && col1 == col2) throw new InvalidOperationException($"ERROR : The same cell has been selected twice; values received: {row1} (row1), {col1} (col1)");

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

    public int GetID(int row, int col)
    {
        if (row < 0 || row >= Height) throw new ArgumentOutOfRangeException($"ERROR : The row index is out of bounds; value received: {row} (height = {Height})");
        if (col < 0 || col >= Width) throw new ArgumentOutOfRangeException($"ERROR : The column index is out of bounds; value received: {col} (width = {Width})");
        return matrix[row, col].Item1;
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