using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CityDBTest;
using Dapper;
using MySql.Data.MySqlClient;

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





        public bool SetupBoard(int difficulty, Controller controller)
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
            return _model.SolutionBoardInitializer(controller, testBoard, 0, 0) && _model.PlayBoardInitializer(testBoard, difficulty);
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
            bool thisIsFine = true;

            for (var column = 0; column < sudokuBoard.SolutionBoard.GetLength(0); column++)
            {
                for (var row = 0; row < sudokuBoard.SolutionBoard.GetLength(1); row++)
                {
                    if (string.IsNullOrEmpty(sudokuBoard.PlayBoard[column, row]) || sudokuBoard.PlayBoard[column, row].Contains("-"))
                    {
                        sudokuBoard.PlayBoard[column, row] = "--"; //If there's nothing in that spot somehow, make it a blank
                        thisIsFine = false;
                    }

                    if (sudokuBoard.PlayBoard[column, row].Contains("*"))
                    {
                        continue; //If it has a '*', it can't be wrong, so move along
                    }

                    int checkNumber;
                    try
                    {
                        //If the first character can be parsed into a number, put it in checkNumber
                        checkNumber = int.Parse(sudokuBoard.PlayBoard[column, row].Substring(0, 1));
                    }
                    catch (FormatException)
                    {
                        //If it can't be parsed, make that entry the first character followed by a '-'
                        sudokuBoard.PlayBoard[column, row] = sudokuBoard.PlayBoard[column, row].Substring(0, 1) + "-";
                        thisIsFine = false;
                        continue;
                    }

                    //Check the number to see if its safe. If it's not, mark it
                    if (!IsSafe(sudokuBoard.PlayBoard, column, row, checkNumber))
                    {
                        sudokuBoard.PlayBoard[column, row] = sudokuBoard.PlayBoard[column, row].Substring(0, 1) + "-";
                        thisIsFine = false;
                    }
                }
            }

            return thisIsFine;
        }

        public bool IsSafe(string[,] testBoard, int givenColumn, int givenRow, int testNum)
        {
            /*
             * Requires a board to be passed, x/y coordinates, and an entry to test.
             * Checks if the board is null or empty.
             * Checks if the coordinates are out of range.
             *
             * Compares number to entries in the same row, column and 'quadrant'
             */
            if (testBoard == null) return false;
            if (testBoard.Length == 0) return false;

            //Check if coordinates are out of range
            if (givenColumn > testBoard.GetLength(0) || givenRow > testBoard.GetLength(1))
            {
                return false;
            }

            //Check the given column for the number
            for (var row = 0; row < testBoard.GetLength(1); row++)
            {

                //If there's nothing there or it's the spot that we're inserting to, move along
                if (testBoard[givenColumn, row] == null || row == givenRow) continue;
                //If the column has a square with the same number as our test number, not safe
                if (testBoard[givenColumn, row].StartsWith(testNum.ToString()))
                {
                    return false;
                }
            }

            //Check the given row for the number
            for (var column = 0; column < testBoard.GetLength(0); column++)
            {
                //If there's nothing there or it's the spot that we're inserting to, move along
                if (testBoard[column, givenRow] == null || column == givenColumn) continue;
                //If the row has a square with the same number as our test number, not safe
                if (testBoard[column, givenRow].StartsWith(testNum.ToString()))
                {
                    return false;
                }
            }

            //This mess figures out which quadrant the coordinates fall in. Divide by 3, round UP
            var startColumn = 0;
            var startRow = 0;
            switch (Math.Ceiling((decimal)(givenColumn + 1) / 3))
            {
                case (1):
                    //starting column index is 0
                    break;

                case (2):
                    startColumn += 3;
                    break;

                case (3):
                    startColumn += 6;
                    break;

                default:
                    return false; //index is out of range, so obviously not safe
            }
            switch (Math.Ceiling((decimal)(givenRow + 1) / 3))
            {
                case (1):
                    //starting row index is 0
                    break;

                case (2):
                    startRow += 3;
                    break;

                case (3):
                    startRow += 6;
                    break;
                default:
                    return false; //index is out of range, so obviously not safe
            }

            //Check the quadrant for the number
            //Start at the adjusted indexes for the quadrant, check the 3x3 quadrant
            //O(n^2), but its small so meh.
            for (var cIndex = startColumn; cIndex <= 2 + startColumn; cIndex++)
            {
                for (var rIndex = startRow; rIndex <= 2 + startRow; rIndex++)
                {
                    //If there's nothing there or it's the spot that we're inserting to, move along
                    if (testBoard[cIndex, rIndex] == null || (cIndex == givenColumn && rIndex == givenRow)) continue;
                    //If the 'quadrant' has a square with the same number as our test number, not safe
                    if (testBoard[cIndex, rIndex].StartsWith(testNum.ToString()))
                    {
                        //IF the number is found, it's not safe
                        return false;
                    }
                }
            }

            //If you get here, the number is safe, woo!
            return true;
        }


        //Retrieves a square from the database. Can be from the complete starting grid or from player moves
        public List<string> GetEntry(int gameId, bool isMove, int column, int row)
        {
            using (MySqlConnection connection = new MySqlConnection(Helper.ConnectionVal("SudokuCloudDB")))
            {
                try
                {
                    //Select rows from the a table according to whether we want moves or the original value
                    var output = connection.Query<string>($"SELECT {(isMove ? "newEntry, MAX(moveNum)" : "entryNum")} " +
                                                          $"FROM {(isMove ? "game_moves" : "grid_square")} " +
                                                          $"WHERE gridID={gameId} AND " +
                                                          $"column={column} AND " +
                                                          $"row={row} " +
                                                          $"{(isMove ? "GROUP BY newEntry" : "")}").ToList();

                    return output;
                }
                catch (NullReferenceException)
                {
                    return null;
                }

            }
        }


        public List<string> ShowGames(string playerName)
        {
            using (MySqlConnection connection = new MySqlConnection(Helper.ConnectionVal("SudokuCloudDB")))
            {
                try
                {
                    //Select rows from the a table according to whether we want moves or the original value
                    var output = connection.Query<string>($"SELECT gridId, playerName, time_stamp " +
                                                          $"FROM fullGrid g, players p " +
                                                          $"WHERE g.playerID = p.playerID " +
                                                          $"{(string.IsNullOrEmpty(playerName) ? "" : "AND p.playerName=" + playerName)}").ToList();

                    return output;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public bool GetPreviousGame(Model sudoku, int gameId, bool isResuming)
        {
            using (MySqlConnection connection = new MySqlConnection(Helper.ConnectionVal("SudokuCloudDB")))
            {
                try
                {
                    //Retrieve original values for the gameId
                    for (int cIndex = 0; cIndex < sudoku.SolutionBoard.GetLength(0); cIndex++)
                    {
                        for (int rIndex = 0; rIndex < sudoku.SolutionBoard.GetLength(1); rIndex++)
                        {
                            //Iterate over solution board and enter the original entries (isMove = false)
                            string retrievedEntry = GetEntry(gameId, false, cIndex, rIndex).First();
                            sudoku.SolutionBoard[cIndex, rIndex] = retrievedEntry;
                            sudoku.PlayBoard[cIndex, rIndex] = retrievedEntry;
                        }
                    }

                    //Get list of empty starting squares and place in playboard
                    
                    
                    //if(isResuming){
                    //Change play board squares to most recent entries
                    //}
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            return true;
        }

        //Called upon board creation, saves squares to DB
        public bool SaveSquare(Model sudoku, bool isBlank, int column, int row, int entry)
        {
            using (MySqlConnection connection = new MySqlConnection(Helper.ConnectionVal("SudokuCloudDB")))
            {
                try
                {
                    
                    var output = connection.Query<string>($"REPLACE INTO {(isBlank ? "start_empty" : "grid_square")} " +
                                                          $"VALUES ({sudoku.GameId}, {column}, {row}{(isBlank ? "" : ", " + entry)}) ").ToList();
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            return true;
        }

        //Automatically saves player moves to the DB
        public bool SaveMove(Model sudoku, int column, int row, int entry)
        {
            using (MySqlConnection connection = new MySqlConnection(Helper.ConnectionVal("SudokuCloudDB")))
            {
                try
                {
                    int moveNum = 1;
                    //Retrieve number for most recent move
                    var output = connection.Query<string>($"SELECT MAX(moveNum)" +
                                                          $"FROM game_moves" +
                                                          $"WHERE gridID={sudoku.GameId}").ToList();

                    //Adjust moveNum if other moves exist
                    if (!string.IsNullOrEmpty(output[0]))
                    {
                        moveNum += int.Parse(output[0]);
                    }

                    //Insert move into database
                    var input = connection.Query<string>($"INSERT INTO game_moves (gridID, columnNum, rowNum, moveNum, newEntry)" +
                                                          $"VALUES ({sudoku.GameId}, {column}, {row}, {moveNum}, {entry})").ToList();
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveBoard(Model sudoku)
        {
            //Iterate over solution board and call SaveSquare on each square
            for (int cIndex = 0; cIndex < sudoku.SolutionBoard.GetLength(0); cIndex++)
            {
                for (int rIndex = 0; rIndex < sudoku.SolutionBoard.GetLength(1); rIndex++)
                {
                    try
                    {
                        int entry = int.Parse(sudoku.SolutionBoard[cIndex, rIndex]);

                        if (!SaveSquare(sudoku, false, cIndex, rIndex, entry))
                        {
                            return false;
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("ERROR: Fault saving grid entry. " + cIndex + "-" + rIndex + " is not a number.");
                        return false;
                    }
                }
            }

            return true;
        }






    }
}
