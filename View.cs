using System;
using System.Threading;
using System.Threading.Tasks;

namespace WassonSudoku
{
    class View
    {
        public bool Introduction()
        {
            string input = "";

            while (input != "N")
            {
                Console.WriteLine("Wanna play a game? (Y/N)");
                input = Console.ReadLine()?.ToUpper();

                if (input == null) continue;
                switch (input.Substring(0, 1))
                {
                    case "Y":
                        Console.WriteLine("Good...");
                        Thread.Sleep(1000);
                        Console.WriteLine("\nHere are your options...");
                        Thread.Sleep(2000);
                        Console.WriteLine("P - Place a number from 1-9 or _ to clear a number. (column#, row#, insert#/_)");
                        Thread.Sleep(2000);
                        Console.WriteLine("H - Get help. (column#, row#)");
                        Thread.Sleep(2000);
                        Console.WriteLine("G - Give up.");
                        Thread.Sleep(2000);
                        Console.WriteLine("N - New game. Same rules.");
                        Thread.Sleep(2000);
                        Console.WriteLine("X - Try to leave.");
                        Thread.Sleep(2000);
                        Console.WriteLine("\n\nRemember your options. I don't repeat myself...\n\n\n\n\n");


                        Thread.Sleep(4000);
                        Console.WriteLine(" Your board is here... Let's play...\n\n\n");
                        Thread.Sleep(2000);
                        return true;


                    case "N":
                        Console.WriteLine("Aww... That's too bad. Next time... maybe.");
                        Thread.Sleep(2000);
                        return false;

                    default:
                        Console.WriteLine("Answer the questions correctly...");
                        Thread.Sleep(2000);
                        break;
                }
            }

            return false;
        }



        public void ViewBoard(String[,] sudokuBoard)
        {
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
                        if ((barLength+2) % 13 == 0)
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
            String input = "";

            input = Console.ReadLine()?.ToUpper();
            if (input == null) return true;

            switch (input.Substring(0, 1))
            {
                case "P":
                    Console.WriteLine("Column: ");
                    var column = Console.Read() % sudokuBoard.SolutionBoard.GetLength(0);
                    Console.WriteLine("Row: ");
                    var row = Console.Read() % sudokuBoard.SolutionBoard.GetLength(1);
                    Console.WriteLine("New Entry: ");
                    var entry = Console.Read().ToString().Substring(0, 1);

                    Console.WriteLine(controller.UpdateBoard(sudokuBoard, column, row, entry)
                        ? "Added..."
                        : "Invalid Entry...");
                    break;

                case "H":
                    //get help
                    break;

                case "G":
                    //give up
                    break;

                case "N":
                    //new game
                    break;

                case "X":
                    return false;
            }

            return true;

        }
    }
}
