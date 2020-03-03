using System;

namespace WassonSudoku
{
    class MVCPattern
    {

        private static void Main(string[] args)
        {
            Model sudokuBoard = new Model();
            View comms = new View();
            Controller controller = new Controller(comms, sudokuBoard);

            if (controller.SetupBoard(5))
            {
                if (comms.Introduction())
                {
                    controller.UpdateView(sudokuBoard);
                }

                while (comms.ReceiveInput(sudokuBoard, controller))
                {
                    controller.UpdateView(sudokuBoard);
                }


                




            }
            else
            {
                Console.WriteLine("Failed to make board.");
            }







        }

        private static void ViewBoard(Model sudokuBoard)
        {
            Console.WriteLine("");
        }
    }
}