using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WassonSudoku
{
    class Model
    {
        private String[,] _solutionBoard;
        private String[,] _playBoard;


        public void SetupBoard(int difficulty)
        {
            _solutionBoard = SolutionBoardInitializer();
            _playBoard = PlayBoardInitializer(_solutionBoard, 1);
        }

        private String[,] SolutionBoardInitializer()
        {
            String[,] tempBoard = new String[9, 9];


            
            for (int column = 0; column < tempBoard.GetLength(0); column++)
            {
                for (int row = 0; row < tempBoard.GetLength(1); row++)
                {
                    int numberToInsert = GetSafeNumber(tempBoard, column, row);
                    if (numberToInsert > 0)
                    {
                        tempBoard[column, row] = numberToInsert.ToString() + "*";
                    }

                }
            }

            return tempBoard;
        }

        //Iterate through numbers 1-9, check if they are safe, then return the first safe number
        private int GetSafeNumber(String[,] checkBoard, int column, int row)
        {
            
            List<int> tryNumbers = new List<int>(){1,2,3,4,5,6,7,8,9};
            Random randomIndex = new Random();
            
            while (tryNumbers.Count > 0)
            {
                //Choose a random number from the list, test it, then remove it if it won't work
                int itemIndex = randomIndex.Next(0, tryNumbers.Count);
                if (IsSafe(checkBoard, column, row, tryNumbers.ElementAt(itemIndex)))
                {
                    return tryNumbers.ElementAt(itemIndex);
                }
                tryNumbers.RemoveAt(itemIndex);
            }

            //If no numbers would be valid, return zero
            return 0;
        }

        private bool IsSafe(String[,] testBoard, int givenColumn, int givenRow, int testNum)
        {
            if (givenColumn > testBoard.GetLength(0) || givenRow > testBoard.GetLength(1))
            {
                return false;
            }

            //Check the given column for the number
            for (int row = 0; row < testBoard.GetLength(1); row++)
            {
                //If the column has a square with the same number as our test number, not safe
                if (testBoard[givenColumn, row] == null) continue;
                if (testBoard[givenColumn, row].StartsWith(testNum.ToString()))
                {
                    return false;
                }
            }

            //Check the given row for the number
            for (int column = 0; column < testBoard.GetLength(0); column++)
            {
                if (testBoard[column, givenRow] == null) continue;
                if (testBoard[column, givenRow].StartsWith(testNum.ToString()))
                {
                    return false;
                }
            }

            //This mess figures out which quadrant the coordinates fall in. Divide by 3, round UP
            int startColumn = 0, startRow = 0;
            switch (Math.Round((decimal) (givenColumn/3), MidpointRounding.AwayFromZero))
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
            switch (Math.Round((decimal)(givenRow / 3), MidpointRounding.AwayFromZero))
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
            for (int cIndex = startColumn; cIndex <= 2 + startColumn; cIndex++)
            {
                for (int rIndex = startRow; rIndex <= 2 + startRow; rIndex++)
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

        private String[,] PlayBoardInitializer(String[,] fullBoard, int difficulty)
        {



            return fullBoard;
        }

        public Boolean UpdatePlayBoard(String[,] playBoard, int column, int row, int entry)
        {
            if (IsSafe(playBoard, column, row, entry))
            {
                playBoard[column, row] = entry.ToString();
            }

            return false;
        }




    }
}
