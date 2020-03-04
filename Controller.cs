using System;
using System.Collections.Generic;
using System.Globalization;
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


        


        public bool SetupBoard(int difficulty)
        {
            /*
             * Creates a blank board.
             * Sets the amount of hints based on difficulty.
             * Calls model methods to initialize the solution and play boards
             * based on received difficulty from user.
             * Returns true/false by success of the calls.
             *
             * O(n)
             */
            var testBoard = new string[9, 9];
            _model.Hints = 10 - difficulty;
            return _model.SolutionBoardInitializer(testBoard, 0, 0) && _model.PlayBoardInitializer(testBoard, difficulty);
        }

        public bool UpdateBoard(Model sudoku, int column, int row, string entry)
        {
            var validatedEntry = ValidateEntry(entry);
            return validatedEntry != null && sudoku.UpdateBoard(column, row, validatedEntry);
        }

        private string ValidateEntry(string entry)
        {
            try
            {
                if (entry.StartsWith("-"))
                {
                    return "--";
                }
                var number = int.Parse(entry.Substring(0, 1));
                if (number > 0 && number < 10)
                {
                    return number.ToString() + " ";
                }

                return null;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public void UpdateView(Model sudoku)
        {
            _view.ViewBoard(sudoku.PlayBoard);
        }

        public bool ShowHint(Model sudokuBoard, int column, int row)
        {
            try
            {
                if (sudokuBoard.Hints > 0)
                {
                    sudokuBoard.Hints--;
                    if (sudokuBoard.UpdateBoard(column, row, sudokuBoard.SolutionBoard[column, row]))
                    {
                        return true;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                return false;
            }

            return false;

        }

        public bool CheckSolution(Model sudokuBoard)
        {
            for (var column = 0; column < sudokuBoard.SolutionBoard.GetLength(0); column++)
            {
                for (var row = 0; row < sudokuBoard.SolutionBoard.GetLength(1); row++)
                {
                    if (string.IsNullOrEmpty(sudokuBoard.PlayBoard[column, row]) || sudokuBoard.PlayBoard[column, row].Contains("-"))
                    {
                        return false;
                    }

                    int checkNumber;
                    try
                    {
                        checkNumber = int.Parse(sudokuBoard.PlayBoard[column, row].Substring(0, 1));
                    }
                    catch(FormatException)
                    {
                        return false;
                    }

                    //User input is validated and formatted before putting in play board -> only care about first character
                    if (!sudokuBoard.IsSafe(sudokuBoard.PlayBoard, column, row, checkNumber))
                        return false;
                }
            }

            return true;
        }
    }
}
