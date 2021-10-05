using System;
using System.Collections.Generic;
using System.Threading;

namespace MagicSquares
{
    class Program
    {
        static void Main(string[] args)
        {
            SquareNode node = new SquareNode(4);
            List<SquareNode> magicSquareList = new List<SquareNode>();
            FindMagicSquare(node, magicSquareList);
            Console.WriteLine("###########");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Drumroll please.....");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("###########");

            Thread.Sleep(3000);

            foreach (SquareNode magicSquare in magicSquareList)
            {
                magicSquare.PrintSquare();
                Console.WriteLine();
                Console.WriteLine("###########");
                Console.WriteLine();
            }
                
        }

         static void FindMagicSquare(SquareNode node, List<SquareNode> magicSquareList)
         {
            //node.PrintSquare();
            if (node.IsFilled())
            {
                magicSquareList.Add(node);
            }
            else
            {
                node.ProduceChildren();
                foreach (SquareNode child in node.Children)
                {
                    FindMagicSquare(child, magicSquareList);
                }
                   
            }
            return;
         }

    }

    class SquareNode
    {
        public int[,] Matrix { get; private set; }

        public int Level { get; private set; }

        public int SideLength { get; private set; }

        public List<SquareNode> Children { get; private set; }

        public List<int> UsedNumbers { get; private set; }

        public int FirstRowSum { get; private set; }

        // constructor to be used for the root SquareNode
        public SquareNode(int length)
        {
            Level = 0;
            SideLength = length;
            Children = new List<SquareNode>();
            Matrix = new int[length, length];
            UsedNumbers = new List<int>();

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Matrix[i, j] = 0;
                }
            }
        }

        // constructor to be used for child SquareNodes, where we will want to copy the matrix of the parent SquareNode 
        // and then insert a new value at the next blank square (x and y will be its coordinates, 'value' will be value to fill)
        // we will also copy the parent's list of used numbers and add 'value' to it 
        public SquareNode(int x, int y, int value, SquareNode parent)
        {
            SideLength = parent.SideLength;
            
            Matrix = new int[SideLength, SideLength];
            
            // copy parent matrix
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    Matrix[i, j] = parent.Matrix[i, j];
                }
            }

            Matrix[x, y] = value;
            Level = parent.Level + 1;
            Children = new List<SquareNode>();
            UsedNumbers = new List<int>(parent.UsedNumbers);
            UsedNumbers.Add(value);
            FirstRowSum = parent.FirstRowSum;

        }

        public void ProduceChildren()
        {
            int lengthSquared = SideLength * SideLength;
            int x = (Level ) / SideLength;
            int y = (Level) % SideLength;
            for (int i = 1; i <= lengthSquared; i++)
            {
                if (UsedNumbers.Contains(i))
                    continue;
                else
                {
                    SquareNode child = new SquareNode(x, y, i, this);
                    if (child.IsIllegal())
                        continue;
                    else
                        Children.Add(child);
                }
            }
        }

        public bool IsFilled()
        {
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    if (Matrix[i, j] == 0)
                        return false;
                }
            }

            return true;
        }


        // This method will check whether a (partially filled) square is illegal, meaning it will return 'true' if a square is illegal and false otherwise. 
        // The method should ignore partially filled rows and columns (that is, rows and columns which contain a 0).
        // Due to the order in which squares are filled (left to right, starting at the top left corner), it only makes sense to start checking once at least two rows
        // are filled (meaning at least 2 * SideLength values have been filled. The count of filled numbers is equal to the Level of the SquareNode).
        // Once the leftmost square of the very last row is filled, we can start checking columns (as well as the diagonal going from bottom-left to top-right).
        public bool IsIllegal()
        {
            if (Level == SideLength)
            {
                int firstRowSum = 0;

                for (int j = 0; j < SideLength; j++)
                {
                    firstRowSum += Matrix[0, j];
                }

                FirstRowSum = firstRowSum;
            }
            
            int filledRows = Level / SideLength;

            int modulo = Level % SideLength;

            if (filledRows < 2)
                return false;
            else if (filledRows >= 2 && filledRows < SideLength - 1)
            {
                if (modulo == 0)
                {
                    if (CheckRowIllegal(filledRows - 1))
                        return true;
                }
                else
                    return false;

            }
            else if (filledRows == SideLength - 1)
            {
                if (modulo == 0)
                {
                    if (CheckRowIllegal(filledRows - 1))
                        return true;
                }
                else
                {
                    if (CheckColumnIllegal(modulo - 1))
                        return true;
                    if (modulo == 1)
                    {
                        if (CheckDiagonalIllegal(DiagonalType.TopRight))
                            return true;
                    }
                }
            }
            else if (filledRows == SideLength)
            {
                //PrintSquare();
                if (CheckRowIllegal(filledRows - 1) || CheckColumnIllegal(SideLength - 1) || CheckDiagonalIllegal(DiagonalType.TopLeft))
                    return true;
            }

            return false;
        }

        // Given a rownumber (zero-indexed), thiis function will return 'true' if the row is illegal (does not sum to what it's supposed to) and false otherwise
        public bool CheckRowIllegal(int rowNumber)
        {
            int rowSum = 0;
            for (int j = 0; j < SideLength; j++)
            {
                rowSum += Matrix[rowNumber, j];
            }

            if (rowSum != FirstRowSum)
                return true;
            else
                return false;

        }

        public bool CheckColumnIllegal(int colNumber)
        {
            int colSum = 0;
            for (int i = 0; i < SideLength; i++)
            {
                colSum += Matrix[i, colNumber];
            }

            if (colSum != FirstRowSum)
                return true;
            else
                return false;
        }

        public bool CheckDiagonalIllegal(DiagonalType diagonalType)
        {
            int diagonalSum = 0;
            
            if (diagonalType == DiagonalType.TopLeft)
            {
                for (int i = 0; i < SideLength; i++)
                {
                    diagonalSum += Matrix[i, i];
                }
            }
            else if (diagonalType == DiagonalType.TopRight)
            {
                int j = SideLength - 1;
                for (int i = 0; i < SideLength; i++)
                {
                    diagonalSum += Matrix[i, j];
                    j--;
                }
            }

            if (diagonalSum != FirstRowSum)
                return true;
            else
                return false;
        }


        public void PrintSquare()
        {
            for (int i = 0; i < SideLength; i++)
            {
                for (int j = 0; j < SideLength; j++)
                {
                    if (Matrix[i, j] < 10)
                    {
                        if (j == SideLength - 1)
                            Console.WriteLine($" {Matrix[i, j]} ");
                        else
                            Console.Write($" {Matrix[i, j]} |");
                    }
                    else if (Matrix[i,j] > 9 && Matrix[i,j] < 100)
                    {
                        if (j == SideLength - 1)
                            Console.WriteLine($"{Matrix[i, j]} ");
                        else
                            Console.Write($"{Matrix[i, j]} |");
                    }
                    else
                    {
                        if (j == SideLength - 1)
                            Console.WriteLine($"{Matrix[i, j]}");
                        else
                            Console.Write($"{Matrix[i, j]}|");
                    }
                }
                if (i != SideLength - 1)
                {
                    for (int n = 0; n < SideLength; n++)
                    {
                        if (n == SideLength - 1)
                        {
                            Console.Write("---");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.Write("---+");
                        }
                    }
                }
            }
        }
    }

    public enum DiagonalType { TopRight, TopLeft}
}
