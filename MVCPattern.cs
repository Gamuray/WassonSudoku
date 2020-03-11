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

            while (comms.Introduction(controller, sudokuBoard))
            {
                controller.UpdateView(sudokuBoard);

                while (comms.ReceiveInput(sudokuBoard, controller))
                {
                    controller.UpdateView(sudokuBoard);
                }
            }

        }

        private static void ViewBoard(Model sudokuBoard)
        {
            Console.WriteLine("");
        }
    }
}