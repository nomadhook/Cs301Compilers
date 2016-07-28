using Library;
using System;
using System.Linq;
using System.Text.RegularExpressions;

class Sudoku {
    #region Globals
    /// <summary>
    /// Array of Array of string used to find all the cells within the same block.
    /// </summary>
    public static string[][] gridCells = new string[][] {
        new string[] {"00","01","02","10","11","12","20","21","22"},
        new string[] {"03","04","05","13","14","15","23","24","25"},
        new string[] {"06","07","08","16","17","18","26","27","28"},

        new string[] {"30","31","32","40","41","42","50","51","52"},
        new string[] {"33","34","35","43","44","45","53","54","55"},
        new string[] {"36","37","38","46","47","48","56","57","58"},

        new string[] {"60","61","62","70","71","72","80","81","82"},
        new string[] {"63","64","65","73","74","75","83","84","85"},
        new string[] {"66","67","68","76","77","78","86","87","88"},
    };
    /// <summary> 
    /// A 2 dimentional array of Cells representing the entire sudoku board 
    /// </summary>
    static Cell[,] board;
    #endregion

    /// <summary>
    /// Check whether there are no more empty spaces on the board
    /// </summary>
    /// <returns>A boolean representing if it is full</returns>
    private static bool BoardFull() {
        for (int row = 0; row < 9; row++) {
            for (int col = 0; col < 9; col++)
                if (board[row, col].number == 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Reads in a sample file and assigns the board based on the file.
    /// </summary>
    /// <param name="filename">Name of the file which is to be read in.</param>
    private static void ReadDataFile(string filename) {
        int row = 0;
        InFile data = new InFile(filename);
        if (data.OpenError()) {
            Console.WriteLine("Cannot open " + filename);
            Environment.Exit(1);
        }
        string line = data.ReadLine();
        while (!data.NoMoreData()) {
            char[] cells = line.Where(f => new Regex(@"[0-9]").IsMatch("" + f)).ToArray();
            if (cells.Length != 9) {
                Console.WriteLine(filename + " file is of invalid format");
                Environment.Exit(1);
            }
            for (int column = 0; column < 9; column++)
                board[row, column] = new Cell(Convert.ToInt32(cells[column].ToString()));
            if (row++ == 8) break;
            line = data.ReadLine();
        }
        data.Close();
    }

    /// <summary>
    /// Returns an integer refering to the array which contains the coordinate specified by row and col.
    /// </summary>
    /// <param name="row">This is an integer referring to the row or x coordinate</param>
    /// <param name="col">This is an integer referring to the col or y coordinate</param>
    /// <returns>The index at which the correct array can be found in gridCells.</returns>
    private static int GetGridIndex(int row, int col) {
        for (int i = 0; i < 9; i++)
            if (gridCells[i].Contains(string.Format("{0}{1}", row, col)))
                return i;
        return -1;
    }

    /// <summary>
    /// This method prompts the user for input and interprets their input into the correct format. 
    /// </summary>
    private static void UserChoice() {

        Console.WriteLine("Your move - row[0..8] col[0..8] value[1..9] (0 to give up)?");
        string input = Console.ReadLine().Replace(" ", "");
        if (input.Length == 3 &&
            new Regex(@"[0-8]").IsMatch("" + input[0]) &&
            new Regex(@"[0-8]").IsMatch("" + input[1]) &&
            new Regex(@"[1-9]").IsMatch("" + input[2]))
                board[Convert.ToInt32(input[0].ToString()), Convert.ToInt32(input[1].ToString())].SetCell(Convert.ToInt32(input[2].ToString()));
        Console.WriteLine("Invalid Move");
    }

    /// <summary>
    /// Update all of the possibilites for each cell in the board
    /// </summary>
    private static void UpdatePossibilities() {
        for (int row = 0; row < 9; row++) {
            for (int col = 0; col < 9; col++) {
                if (board[row, col].number != 0) continue;
                IntSet possibilities = new IntSet(1, 2, 3, 4, 5, 6, 7, 8, 9);
                int gridPos = GetGridIndex(row, col);
                //Cross of non-possibilities
                for (int num = 1; num < 10; num++) {
                    for (int i = 0; i < 9; i++) {
                        if (board[row, i].number == num || board[i, col].number == num) {
                            possibilities.Excl(num); continue;
                        }
                        if (board[Convert.ToInt32(gridCells[gridPos][i][0].ToString()), Convert.ToInt32(gridCells[gridPos][i][1].ToString())].number == num) {
                            possibilities.Excl(num); continue;
                        }
                    }
                }
                board[row, col] = board[row, col].SetPossibilities(possibilities);
            }
        }
    }

    /// <summary>
    /// Counts the number of values which have been set.
    /// </summary>
    /// <returns></returns>
    private static int CountKnown() {
        int count = 0;
        for (int row = 0; row < 9; row++)
            for (int col = 0; col < 9; col++)
                if (board[row, col].number != 0) count++;
        return count;
    }

    /// <summary>
    /// This method calls the methods which check for Singles and Hidden Singles.
    /// </summary>
    private static void AIChoice() {
        int num = 0;
        if ((num = FindSingles() + FindHiddenSingles()) == 0) {
            DrawTable(board.Cast<Cell>().Take(81).ToArray());
            Console.WriteLine("{0} known, made {1} predictions.\n", CountKnown(), num);
            Console.WriteLine("AI is stuck :'(    plz halp!");
            UserChoice();
        }
        else {
            DrawTable(board.Cast<Cell>().Take(81).ToArray());
            Console.WriteLine("{0} known, made {1} predictions.\n", CountKnown(), num);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// This method finds and sets all the Singles on the board for one loop through all the cells.
    /// </summary>
    /// <returns>The number of Singles found.</returns>
    private static int FindSingles() {
        int count = 0;
        for (int row = 0; row < 9; row++) {
            for (int col = 0; col < 9; col++) {
                if (board[row, col].IsEmptyCell() && board[row, col].possibilities.Members() == 1) {
                    count++;
                    board[row, col].SetCell(Convert.ToInt32(board[row, col].possibilities.ToString()[1].ToString()));
                    UpdatePossibilities();
                }
            }
        }
        return count;
    }

    /// <summary>
    /// This method finds and sets all the Hidden Singles on the board for one loop through all the cells.
    /// </summary>
    /// <returns>The number of Hidden Singles found.</returns>
    private static int FindHiddenSingles() {
        int count = 0;
        for (int num = 1; num < 10; num++) {
            bool OccursOnce = false;
            string cellToChange = "";
            for (int index = 0; index < 9; index++) {
                foreach (string cell in gridCells[index]) {
                    Cell cellToCheck = board[Convert.ToInt32(cell[0].ToString()), Convert.ToInt32(cell[1].ToString())];
                    if (cellToCheck.number == 0 && cellToCheck.possibilities.Contains(num)) {
                        OccursOnce = !OccursOnce;
                        cellToChange = cell;
                        if (!OccursOnce) break;
                    }
                }
                if (OccursOnce == true) {
                    count++;
                    board[Convert.ToInt32(cellToChange[0].ToString()), Convert.ToInt32(cellToChange[1].ToString())].SetCell(num);
                    UpdatePossibilities();
                }
            }
        }
        return count;
    }

    /// <summary>
    /// This method displays the Cell table in the console.
    /// </summary>
    /// <param name="CellArray"></param>
    public static void DrawTable(Cell[] CellArray) { 
	//takes in an array if Cells, top left to right, and displays the potential numbers of each cell on a table 
	//or the final answer.
        Console.SetWindowSize(90, 60); //set window size to display board
        //display the column
		Console.WriteLine("     0       1       2        3       4       5        6       7       8"); 
        Console.WriteLine("||=======================++=======================++=======================||");

        for (int row = 0; row < 9; row++) { //row is each of the 9 rows needed to be checked
            for (int count = 0; count < 3; count++) { //count used later to distinguish between 123, 456, and 789. 
                Console.Write("|| ");
                //cellPos is each cell in a row that can be printed. There are 9 cells in a row
				for (int cellPos = 0; cellPos < 9; cellPos++) { 
                    //possib is each possible number in the cell. Uses count to distinguish between 123, 456, and 789. 
					for (int possib = 1; possib < 4; possib++) { 
					//if the Cell's number is 0 (no answer) and the cell contains 1, 2, or 3, display that number
					if (CellArray[cellPos + (row * 9)].number == 0 && 
						CellArray[cellPos + (row * 9)].possibilities.Contains(possib + count * 3)) { 
                            //change colour to grey for possibilities. Helps distinguish a possibility and an answer
                            Console.ForegroundColor = ConsoleColor.Green; 
							Console.Write(possib + count * 3);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        else if (CellArray[cellPos + (row * 9)].number != 0) { //if the answer is definite, then display
                            int far = (possib + count * 3);//far is how far into the ascii cell you are.
                            if (!CellArray[cellPos + (row * 9)].original)
								//if the number was not original, surround it with brackets
                                Console.Write((far == 4) ? "(" : (far == 5) ? "" + 
								CellArray[cellPos + (row * 9)].number : (far == 6) ? ")" : " "); 
                            //if the number is original, display it in the middle of the cell.
							else Console.Write((far == 5) ? "" + CellArray[cellPos + (row * 9)].number : " "); 
						}
                        else
                            Console.Write(" "); //formatting
                        Console.Write(" ");
                    }
                    if (cellPos == 2 || cellPos == 5 || cellPos == 8) //more formatting
                        Console.Write("|| ");
                    else Console.Write("| ");
                }
                if (count == 1 ) Console.WriteLine(row); //displays the row at the end of the line
                else Console.WriteLine(" ");
            }
            if (row == 2 || row == 5 || row == 8) Console.WriteLine("||=======================++
			=======================++=======================||"); //last bit of formatting.
            else Console.WriteLine("||-----------------------++-----------------------++-----------------------||");
        }
    }

    /// <summary>
    /// This method determines whether the user or AI is playing.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args) {
        board = new Cell[9, 9];
        Console.WriteLine("Please enter a sample");
        ReadDataFile(Console.ReadLine().Trim());
        Console.WriteLine("Input 0 if you would like to play the game yourself and 1 if you would like the AI to solve the puzzle.");
        bool aiPlaying = Console.ReadLine().Trim()[0] == '1';
        UpdatePossibilities();
        while (!BoardFull()) {
            UpdatePossibilities();
            if (aiPlaying) AIChoice();
            else {
                DrawTable(board.Cast<Cell>().Take(81).ToArray());
                UserChoice();
            }
        }
        Console.WriteLine("End of Game");
        Console.ReadLine();
    } //Main
}//Sudoku


/// <summary>
/// A single sudoku cell/block
/// </summary>
class Cell {
    /// <summary> 
    /// The current number within the cell (with 0  representing an empty cell)
    /// </summary>
    public int number { get; private set; }
    /// <summary> 
    /// An IntSet containing the possible numbers 
    /// </summary>
    public IntSet possibilities { get; private set; }
    /// <summary>
    /// A boolean representing whether the cell was set by the sample file 
    /// </summary>
    public bool original { get; private set; }

    /// <summary>
    /// Create a new Cell with a given number and an empty set of possibilities
    /// </summary>
    /// <param name="num">The number to put in the cell (default is zero)</param>
    /// <param name="_original">Whether the cell was originally set by the sample (default is true)</param>
    public Cell(int num = 0, bool _original = true) {
        number = num;
        possibilities = new IntSet();
        original = (num == 0 ? false : _original);
    }

    /// <summary>
    /// Create a new Cell with a given number and a given set of possibilities
    /// </summary>
    /// <param name="num">The number to put in the cell (default is zero)</param>
    /// <param name="pos">An Intset representing the possibilities</param>
    public Cell(int num, IntSet pos) {
        number = num;
        possibilities = new IntSet(pos);
    }

    /// <summary>
    /// Set the Cells number to a new number
    /// </summary>
    /// <param name="num">The number to change the cell to</param>
    /// <returns>A bool refering to whether the cell was successfully or not</returns>
    public bool SetCell(int num) {
        if (num > 0 && num < 10 && !possibilities.Contains(num)) return false;
        number = num;
        return true;
    }

    /// <summary>
    /// Set a cells possibilities. This will return a new instance of the object with the new possibilities
    /// </summary>
    /// <param name="_possibilities">The new possibilities</param>
    /// <returns>A new instance of the cell</returns>
    public Cell SetPossibilities(IntSet _possibilities) {
        return new Cell(number, _possibilities);
    }

    public bool IsEmptyCell() {
        return number == 0;
    }
}//Cell