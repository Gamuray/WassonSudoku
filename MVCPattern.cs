using System;

namespace WassonSudoku
{
    class MvcPattern
    {

        private static void Main(string[] args)
        {
            Model sudokuBoard = new Model();
            View comms = new View();
            Controller controller = new Controller(comms, sudokuBoard);



            comms.Introduction(controller, sudokuBoard);


        }

        private static void ViewBoard(Model sudokuBoard)
        {
            Console.WriteLine("");
        }
    }
}