using System;
using System.Collections.Generic;
using System.Text;

namespace WassonSudoku
{
    


    class Controller
    {
        private static void Main(string[] args)
        {
            Model sudokuBoard = new Model();
            String input = "";

            

            while (input != "N")
            {
                Console.WriteLine("Wanna play a game? (Y/N)");
                input = Console.ReadLine().ToUpper();

                if (input == null) continue;
                switch (input.Substring(0, 1))
                {
                    case "Y":
                        sudokuBoard.SetupBoard(1);
                        Console.WriteLine("Good... Your board is here... Let's play...");
                        Console.WriteLine("\nHere are your options..." +
                                          "\n1 - Place a number from 1-9. (column#, row#, insert#)" +
                                          "\n2 - Get help. (column#, row#)" +
                                          "\n3 - Give up." +
                                          "\n4 - New game. Same rules." +
                                          "\nX - Try to leave." +
                                          "\n\n Here's your board. Remember your options. I don't repeat myself...\n\n\n\n\n");

                        //sudokuBoard.ViewBoard();

                        //while (input != "X")
                        //{
                        //    Console.


                        //}
                        
                        
                        
                        
                        break;
                        
                    case "N":
                        Console.WriteLine("Aww... That's too bad. Next time... maybe.");
                        break;

                    default:
                        Console.WriteLine("Answer the questions correctly...");
                        break;
                }
                
            }




        }
    }
}
