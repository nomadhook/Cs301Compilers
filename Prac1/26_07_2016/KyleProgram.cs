using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
//int set empty, cell is empty
//If it 
namespace ConsoleApplication1
{
    class Cell
    {
        public int number { get; private set; }
        public IntSet possibilities { get; private set; }

        public Cell(int num, IntSet pos)
        {
            number = num;
            possibilities = pos;
        }

    }
    class Program
    {
        static void Main(string[] args){

            Cell a = new Cell(0, new IntSet(1,2,7,8,9));
            Cell b = new Cell(0, new IntSet(1, 2, 3, 4, 5, 6, 7,  9));
            Cell c = new Cell(4, new IntSet(1, 2, 3, 4, 5, 7, 8, 9));
            Cell d = new Cell(0, new IntSet(1,  3, 4, 5, 6, 7, 8, 9));
            Cell e = new Cell(0, new IntSet(1, 2, 3, 4, 5, 6, 7,  9));
            Cell f = new Cell(4, new IntSet(1, 2, 3, 4,  6, 7, 8, 9));
            Cell g = new Cell(0, new IntSet(1, 2,  4, 5, 6, 7, 8, 9));
            Cell h = new Cell(0, new IntSet(2, 3,  7, 8));
            Cell i = new Cell(9, new IntSet(1, 2, 3, 4, 5, 6,  8, 9));

            Cell[] arr;
            arr = new Cell[81] { a, b, c, b, e, f, g, a, i, a, b, c, f, e, c, g,c, i, a, a, c, d, e, f, g, b, a, b, b, c, d, a, f, g, a, c, a, b, c, d, e, f, g, a, i, b, b, c, d, e, b, g, h, i, a, b, c, d, e, c, g, h, i, a, b, c, d,c, f, g, h, i, a, b, c, d, e, f, g, h, a };
            DrawRow(arr, 4);
        }




        public static void DrawRow(Cell[] CellArray, int RowNum)
        {
            Console.SetWindowSize(80, 40);
            Console.WriteLine("     0       1       2        3       4       5        6       7       8");
            Console.WriteLine("||=======================++=======================++=======================||");
            for (int z = 0; z < 9; z++)
            {
                Console.Write("|| ");
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 1; x < 4; x++)
                    {
                        if (CellArray[y + (z * 9)].number == 0 && CellArray[y + (z * 9)].possibilities.Contains(x))
                            Console.Write(x);
                        else
                            Console.Write(" ");
                        Console.Write(" ");
                    }
                    if (y == 2 || y == 5 || y == 8)
                        Console.Write("|| ");
                    else Console.Write("| ");
                }
                Console.WriteLine(" ");
                Console.Write("|| ");
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 4; x < 7; x++)
                    {
                        if (CellArray[y + (z * 9)].number == 0 && CellArray[y + (z * 9)].possibilities.Contains(x))
                            Console.Write(x);
                        else
                            if (x == 5 && CellArray[y + z].number != 0)
                            Console.Write(CellArray[y + z].number);
                        else Console.Write(" ");
                        Console.Write(" ");
                    }
                    if (y == 2 || y == 5 || y == 8)
                        Console.Write("|| ");
                    else Console.Write("| ");
                }
                Console.WriteLine(" ");
                Console.Write("|| ");
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 7; x < 10; x++)
                    {
                        if (CellArray[y + (z * 9)].number == 0 && CellArray[y + (z * 9)].possibilities.Contains(x))
                            Console.Write(x);
                        else
                            Console.Write(" ");
                        Console.Write(" ");
                    }
                    if (y == 2 || y == 5 || y == 8)
                        Console.Write("|| ");
                    else Console.Write("| ");
                }
                Console.WriteLine(" ");
                if (z == 2 || z == 5 || z == 8) Console.WriteLine("||=======================++=======================++=======================||");
                else Console.WriteLine("||-----------------------++-----------------------++-----------------------||");
            }
            Console.ReadLine();
        }
    }
}