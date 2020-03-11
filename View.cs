using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WassonSudoku
{
    class View
    {

        public bool Introduction(Controller controller, Model model)
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
                            Console.WriteLine("Type your choice...\n\n" +
                                              "1: Show Games\n" +
                                              "2: Load Game (ID)\n" +
                                              "3: New Game (Difficulty)" +
                                              "X: Return to intro");

                            input = Console.ReadLine()?.ToUpper();

                            if (string.IsNullOrEmpty(input)) continue;
                            switch (input.Substring(0, 1))
                            {
                                case "1":
                                    string name = "";
                                    Console.WriteLine("\nEnter player name or press enter for all games...");
                                    name = Console.ReadLine();

                                    foreach (var row in model.ShowGames(name))
                                    {
                                        Console.WriteLine(row.FullInfo);
                                    }

                                    break;

                                case "2":



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




                //switch (input.Substring(0, 1))
                //{
                //    case "Y":
                //        //Yes, controls, difficulty
                //        Console.WriteLine("Good...");
                //        //        Thread.Sleep(1000);
                //        Console.WriteLine("\nHere are your options...");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("P - Place a number from 1-9 or _ to clear a number. (column#, row#, insert#/_)");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("H - Get help. (column#, row#)");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("G - Give up.");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("N - New game. Same rules.");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("C - Check your solution.");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("X - Try to leave.");
                //        //        Thread.Sleep(2000);
                //        Console.WriteLine("\n\nRemember your options. I don't repeat myself...\n\n\n\n\n");

                //        Console.WriteLine("How difficult should this be? (1-7)");

                //        string tempInput = Console.ReadLine();
                //        int difficulty = 0;
                //        if (!string.IsNullOrEmpty(tempInput))
                //        {
                //            difficulty = int.Parse(tempInput.Substring(0, 1));
                //        }

                //        //        Thread.Sleep(2000);
                //        Console.WriteLine(controller.SetupBoard(difficulty, controller)
                //        ? "Your board is here... Let's play...\n\n\n"
                //        : "Failed to build board...");
                //        Thread.Sleep(2000);
                //        return true;


                //    case "N":
                //        //No, exit
                //        Console.WriteLine("Aww... That's too bad. Next time... maybe.");
                //        //        Thread.Sleep(2000);
                //        return false;

                //    default:
                //        //Invalid response
                //        Console.WriteLine("Answer the questions correctly...");
                //        Thread.Sleep(2000);
                //        break;
                //}
            }

            return false;
        }

        public void ViewBoard(string[,] sudokuBoard)
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
             * Requires a model to references solution and play boards.
             * Requires a controller to refer to some methods.
             *
             * Takes user string input and reads first character to determine course of action.
             *
             * O(n)
             */

            string input = "";

            Thread.Sleep(2000);
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

                    Console.WriteLine("Entry: ");
                    var entry = Console.ReadLine()?.Substring(0, 1);

                    Console.WriteLine(controller.UpdateBoard(sudokuBoard, pColumn, pRow, entry)
                        ? "\nAdded...\n\n\n"
                        : "\nInvalid Entry...\n\n\n");
                    Thread.Sleep(2000);
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

                    Console.WriteLine(controller.ShowHint(sudokuBoard, hColumn, hRow)
                    ? "\nHint provided...\n\n\n"
                    : "\nNo hints available...\n\n\n");
                    Thread.Sleep(2000);
                    break;

                case "G":
                    //give up
                    ViewBoard(sudokuBoard.SolutionBoard);
                    return false;

                case "N":
                    //new game
                    Console.WriteLine("Too hard? Let's try another one...");
                    Thread.Sleep(2000);
                    Console.WriteLine("Choose a difficulty level (1-7).");

                    var nInputDifficulty = Console.ReadLine();
                    var difficulty = 0;

                    if (!string.IsNullOrEmpty(nInputDifficulty))
                    {
                        difficulty = int.Parse(nInputDifficulty);
                    }

                    controller.SetupBoard(difficulty, controller);

                    Thread.Sleep(2000);
                    Console.WriteLine("Here's your new board... Do better.");
                    Thread.Sleep(2000);
                    break;

                case "C":
                    //check completeness
                    Console.WriteLine("\nYou think you've beaten my game? I'll see about that...\n");
                    Thread.Sleep(2000);

                    //If the solution is valid
                    if (controller.CheckSolution(sudokuBoard))
                    {
                        Console.WriteLine("\nCongratulations... You've beaten it.\n\n\n");
                        return false; //back to main 'screen'
                    }

                    //If something is invalid in the solution
                    Console.WriteLine("\nYou are mistaken. Some of this is incorrect. Try again.\n\n\n");
                    Thread.Sleep(2000);
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
