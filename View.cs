using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WassonSudoku
{
    class View
    {

        public bool Introduction(Controller controller, Model sudoku)
        {
            /*
             * Asks the user if they want to play.
             * If they do, it gets a difficulty and randomly generates
             * a sudoku board with values removed according to their input.
             * If they don't, the program ends.
             * If they respond irrationally, it notifies them and repeats.
             *
             * Requires pass of controller so that it can have it set up
             * the board.
             *
             * O(n)
            */

            string input = "";

            while (input != "N")
            {
                Console.WriteLine("Wanna play a game? (Y/N)");
                input = Console.ReadLine()?.ToUpper();

                if (string.IsNullOrEmpty(input)) continue;
                switch (input.Substring(0, 1))
                {
                    case "Y":
                        input = "";

                        while(input!="X")
                        {
                            Console.WriteLine("\n\nType your choice...\n\n" +
                                              "1: Show Games\n" +
                                              "2: Load Game from Last Move (ID)\n" +
                                              "3: Load Game from Start (ID)\n" +
                                              "4: New Game (Difficulty)\n" +
                                              "X: Return to intro\n\n");

                            input = Console.ReadLine()?.ToUpper();

                            if (string.IsNullOrEmpty(input)) continue;
                            switch (input.Substring(0, 1))
                            {
                                case "1":
                                    string name = "";
                                    Console.WriteLine("\nEnter player name or press enter for all games...");
                                    name = Console.ReadLine();

                                    foreach (var row in sudoku.ShowGames(name))
                                    {
                                        Console.WriteLine(row.FullInfo);
                                    }

                                    break;

                                case "2":
                                    Console.WriteLine("Enter the ID of the game you wish to resume...");
                                    int resumeId = 0;
                                    try
                                    {
                                        resumeId = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("ERROR: Entry contained non-numeric characters.");
                                        break;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Console.WriteLine("ERROR: No entry detected.");
                                        break;
                                    }

                                    if (sudoku.GetPreviousGame(sudoku, resumeId, true))
                                    {
                                        Console.WriteLine("Resuming Game " + resumeId + "...");
                                        UpdateView(sudoku.PlayBoard);
                                        
                                        while (ReceiveInput(sudoku, controller))
                                        {
                                            UpdateView(sudoku.PlayBoard);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR: Unable to locate requested game. Verify the requested ID.");
                                    }
                                    break;

                                case "3":
                                    Console.WriteLine("Enter the ID of the game you wish to restart...");
                                    int restartId = 0;
                                    try
                                    {
                                        restartId = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("ERROR: Entry contained non-numeric characters.");
                                        break;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Console.WriteLine("ERROR: No entry detected.");
                                        break;
                                    }

                                    if (sudoku.GetPreviousGame(sudoku, restartId, false))
                                    {
                                        Console.WriteLine("Starting new attempt for game ");
                                        UpdateView(sudoku.PlayBoard);

                                        while (ReceiveInput(sudoku, controller))
                                        {
                                            UpdateView(sudoku.PlayBoard);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("ERROR: Unable to locate requested game. Verify the requested ID.");
                                    }
                                    break;

                                case "4":
                                    try
                                    {
                                        Console.WriteLine("Enter your starting difficulty (1-7)...");

                                        int difficulty =
                                            int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

                                        if (controller.SetupBoard(difficulty, controller, sudoku))
                                        {
                                            Console.WriteLine("Creating New Board...");

                                            sudoku.SaveBoard(sudoku);
                                            UpdateView(sudoku.PlayBoard);

                                            while (ReceiveInput(sudoku, controller))
                                            {
                                                UpdateView(sudoku.PlayBoard);
                                            }
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("ERROR: Entry contained non-numeric characters.");
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        Console.WriteLine("ERROR: No entry detected.");
                                    }

                                    break;

                                case "X":

                                    break;

                                default:
                                    Console.WriteLine("ERROR: Invalid command.");
                                    break;
                            }
                        }
                        break;

                    case "N":
                        //No, exit
                        Console.WriteLine("Aww... That's too bad. Next time... maybe.");
                        //        Thread.Sleep(2000);
                        return false;

                    default:
                        //Invalid response
                        Console.WriteLine("Answer the questions correctly...");
                        Thread.Sleep(2000);
                        break;
                }
            }

            return false;
        }

        public void UpdateView(string[,] sudokuBoard)
        {
            /*
             * Requires pass of a sudokuBoard to read from.
             * Traverses the board and prints out the contents with 3x3 'quadrant' formatting.
             * (It places vertical and horizontal bars at 3 entry intervals)
             *
             * O(n^2)
             */

            for (int row = 0; row < sudokuBoard.GetLength(0); row++)
            {
                for (int column = 0; column < sudokuBoard.GetLength(1); column++)
                {

                    if ((column + 1) % 3 == 0)
                    {
                        Console.Write(sudokuBoard[column, row] + " | ");
                    }
                    else
                    {
                        Console.Write(sudokuBoard[column, row] + "  ");
                    }
                }

                if ((row + 1) % 3 == 0)
                {

                    Console.WriteLine();

                    for (int barLength = 0; barLength < sudokuBoard.GetLength(0) * 4 + 2; barLength++)
                    {
                        if ((barLength + 2) % 13 == 0)
                        {
                            Console.Write("|");
                        }
                        else
                        {
                            Console.Write("_");
                        }

                    }

                }
                Console.WriteLine();
            }

        }

        public bool ReceiveInput(Model sudokuBoard, Controller controller)
        {
            /*
             * Requires a sudoku to references solution and play boards.
             * Requires a controller to refer to some methods.
             *
             * Takes user string input and reads first character to determine course of action.
             *
             * O(n)
             */

            string input = "";

            //Thread.Sleep(2000);

            Console.WriteLine("\nActions:\n" +
                              "P: Place entry (column#, row#, entry)\n" +
                              "H: Get a hint (column#, row#)\n" +
                              "G: Give up\n" +
                              "C: Check your solution\n" +
                              "X: Stop playing\n\n");

            input = Console.ReadLine()?.ToUpper();
            if (string.IsNullOrEmpty(input)) return true;

            switch (input.Substring(0, 1))
            {
                case "P":
                    //Place entry in defined column if able
                    Console.WriteLine("Column: ");
                    var pInputC = Console.ReadLine()?.Substring(0, 1);
                    int pColumn;
                    if (!string.IsNullOrEmpty(pInputC))
                    {
                        pColumn = int.Parse(pInputC) % sudokuBoard.SolutionBoard.GetLength(0);
                    }
                    else
                    {
                        pColumn = -1;
                    }

                    Console.WriteLine("Row: ");
                    var pInputR = Console.ReadLine()?.Substring(0, 1);
                    int pRow;
                    if (!string.IsNullOrEmpty(pInputR))
                    {
                        pRow = int.Parse(pInputR) % sudokuBoard.SolutionBoard.GetLength(0);
                    }
                    else
                    {
                        pRow = -1;
                    }

                    try
                    {
                        Console.WriteLine("Entry: ");
                        var entry = Console.ReadLine()?.Substring(0, 1);

                        sudokuBoard.UpdateBoard(controller, sudokuBoard, pColumn, pRow, entry);

                        //Thread.Sleep(2000);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("ERROR: Unable to accept entry.");
                    }
                    break;

                case "H":
                    //get help
                    Console.WriteLine("Column: ");
                    var hInputC = Console.ReadLine()?.Substring(0, 1);
                    int hColumn;
                    if (!string.IsNullOrEmpty(hInputC))
                    {
                        hColumn = int.Parse(hInputC) % sudokuBoard.SolutionBoard.GetLength(0);
                    }
                    else
                    {
                        hColumn = 0;
                    }

                    Console.WriteLine("Row: ");
                    var hInputR = Console.ReadLine()?.Substring(0, 1);
                    int hRow;
                    if (!string.IsNullOrEmpty(hInputR))
                    {
                        hRow = int.Parse(hInputR) % sudokuBoard.SolutionBoard.GetLength(0);
                    }
                    else
                    {
                        hRow = 0;
                    }

                    Console.WriteLine(controller.ShowHint(sudokuBoard, controller, hColumn, hRow)
                    ? "\nHint provided...\n\n\n"
                    : "\nNo hints available...\n\n\n");
                    //Thread.Sleep(2000);
                    break;

                case "G":
                    //give up
                    UpdateView(sudokuBoard.SolutionBoard);
                    return false;

                case "N":
                    //new game
                    Console.WriteLine("Too hard? Let's try another one...");
                    //Thread.Sleep(2000);
                    Console.WriteLine("Choose a difficulty level (1-7).");

                    var nInputDifficulty = Console.ReadLine();
                    var difficulty = 0;

                    if (!string.IsNullOrEmpty(nInputDifficulty))
                    {
                        difficulty = int.Parse(nInputDifficulty);
                    }

                    controller.SetupBoard(difficulty, controller, sudokuBoard);

                    //Thread.Sleep(2000);
                    Console.WriteLine("Here's your new board... Do better.");
                    //Thread.Sleep(2000);
                    break;

                case "C":
                    //check completeness
                    Console.WriteLine("\nYou think you've beaten my game? I'll see about that...\n");
                    //Thread.Sleep(2000);

                    //If the solution is valid
                    if (controller.CheckSolution(sudokuBoard))
                    {
                        Console.WriteLine("\nCongratulations... You've beaten it.\n\n\n");
                        return false; //back to main 'screen'
                    }

                    //If something is invalid in the solution
                    Console.WriteLine("\nYou are mistaken. Some of this is incorrect. Try again.\n\n\n");
                    //Thread.Sleep(2000);
                    break;

                case "X":
                    //exit
                    Console.WriteLine("\n\n\n\n\n");
                    return false;

                default:
                    Console.WriteLine("\nThat's not a valid entry...\n\n\n");
                    break;
            }

            return true;

        }

        public void PrintList(List<string> inputList, int columnCount)
        {
            //if (inputList.Count < columnCount || inputList.Count == 0)
            //{
            //    Console.WriteLine("\nEmpty List.\n\n\n");
            //    return;
            //}
            foreach (var thing in inputList)
            {
                Console.WriteLine(thing);
            }

            for (int index = 0; index < inputList.Count; index++)
            {
                if (index % columnCount == 0)
                {
                    Console.WriteLine(inputList.ElementAt(index));
                }
                else
                {
                    Console.Write(inputList.ElementAt(index));
                }
            }
        }
    }
}
