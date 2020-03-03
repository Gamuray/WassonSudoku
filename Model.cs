using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WassonSudoku
{
    class Model
    {
        public string[,] SolutionBoard { get; private set; }

        public string[,] PlayBoard { get; private set; }


        public bool SolutionBoardInitializer(string[,] testBoard, int givenColumn, int givenRow)
        {
            var tryNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var randomIndex = new Random();

            while (tryNumbers.Count > 0)
            {
                //Choose a random number from the list
                var itemIndex = randomIndex.Next(0, tryNumbers.Count - 1);
                if (IsSafe(testBoard, givenColumn, givenRow, tryNumbers.ElementAt(itemIndex))) //if its safe, do stuff
                {
                    testBoard[givenColumn, givenRow] = tryNumbers.ElementAt(itemIndex).ToString() + "*"; //insert it
                    if (givenRow < testBoard.GetLength(1) - 1) //if there is another row in this column...
                    {
                        if (SolutionBoardInitializer(testBoard, givenColumn, givenRow + 1)) //go there and try to insert a number, if true...
                        {
                            return true; //ride cascade
                        }
                        testBoard[givenColumn, givenRow] = null; //if false, this number doesn't work, remove it and try again
                    }
                    else if (givenColumn < testBoard.GetLength(0) - 1) //if there is another column to try, move to the top of it and keep trying. If success...
                    {
                        if (SolutionBoardInitializer(testBoard, givenColumn + 1, 0))
                        {
                            return true; //ride cascade
                        }

                        testBoard[givenColumn, givenRow] = null; //if false, number doesn't work, remove and try another
                    }
                    else
                    {
                        SolutionBoard = testBoard;
                        return true; //if there isn't another row or column after this square, we're done, return true and cascade
                    }

                }
                tryNumbers.RemoveAt(itemIndex); //remove the number so we don't try it again
            }
            return false; //if we run out of numbers, the board can't be completed this way
        }

        public bool PlayBoardInitializer(string[,] fullBoard, int difficulty)
        {
            if (fullBoard == null) return false;
            if (fullBoard.Length == 0) return false;

            var minimumKeptSquares = fullBoard.GetLength(0) + fullBoard.GetLength(1) + 1;
            var requestedKeptSquares = (fullBoard.GetLength(0) * fullBoard.GetLength(1)) - (10 * difficulty);
            var deleteItems = Math.Max(minimumKeptSquares, requestedKeptSquares);
            Random coordRandom = new Random();

            for (; deleteItems < fullBoard.Length; deleteItems++)
            {
                var column = coordRandom.Next(0, fullBoard.GetLength(0));
                var row = coordRandom.Next(0, fullBoard.GetLength(1));
                while (fullBoard[column, row].Contains("-"))
                {
                    column = coordRandom.Next(0, fullBoard.GetLength(0));
                    row = coordRandom.Next(0, fullBoard.GetLength(1));
                }
                fullBoard[column, row] = "--";
            }

            PlayBoard = fullBoard;
            return true;
        }

        public bool UpdateBoard(int column, int row, string entry)
        {
            if (SolutionBoard != null && SolutionBoard.Length > 0)
            {
                SolutionBoard[column, row] = entry;
                return true;
            }

            return false;
        }

        private bool IsSafe(string[,] testBoard, int givenColumn, int givenRow, int testNum)
        {
            if (testBoard == null) return false;
            if (testBoard.Length == 0) return false;


            if (givenColumn > testBoard.GetLength(0) || givenRow > testBoard.GetLength(1))
            {
                return false;
            }

            //Check the given column for the number
            for (var row = 0; row < testBoard.GetLength(1); row++)
            {
                //If the column has a square with the same number as our test number, not safe
                if (testBoard[givenColumn, row] == null) continue;
                if (testBoard[givenColumn, row].StartsWith(testNum.ToString()))
                {
                    return false;
                }
            }

            //Check the given row for the number
            for (var column = 0; column < testBoard.GetLength(0); column++)
            {
                if (testBoard[column, givenRow] == null) continue;
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
                    if (testBoard[cIndex, rIndex] == null) continue;
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


    }
}
