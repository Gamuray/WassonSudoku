using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WassonSudoku
{
    


    class Controller
    {
        private Model _model;
        private View _view;

        public Controller(View view, Model model)
        {
            _view = view;
            _model = model;
        }


        //Iterate through numbers 1-9, check if they are safe, then return the first safe number


        public bool SetupBoard(int difficulty)
        {
            var testBoard = new string[9, 9];

            if (!_model.SolutionBoardInitializer(testBoard, 0, 0))
            {
                return false;
            }

            if (_model.PlayBoardInitializer(testBoard, difficulty))
            {
                return true;
            }

            return false;
        }

        public bool UpdateBoard(Model sudoku, int column, int row, string entry)
        {
            string validatedEntry = ValidateEntry(entry);
            if (validatedEntry != null)
            {
                if (sudoku.UpdateBoard(column, row, validatedEntry))
                {
                    return true;
                }
            }

            return false;
        }

        private string ValidateEntry(string entry)
        {
            try
            {
                var number = Int32.Parse(entry.Substring(0, 1));
                if (number > 0 && number < 10)
                {
                    return number.ToString() + " ";
                }
                else
                {
                    return "--";
                }
            }
            catch (FormatException)
            {
                return "--";
            }
            catch (ArgumentNullException)
            {
                return "--";
            }
        }

        public void UpdateView(Model sudoku)
        {
            _view.ViewBoard(sudoku.PlayBoard);
        }


    }
}
