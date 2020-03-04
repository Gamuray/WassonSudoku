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

        public int Hints { get; set; }


        public bool SolutionBoardInitializer(string[,] testBoard, int givenColumn, int givenRow)
        {
            /*
             * Randomly chooses a number from 1-9. Attempts to add it to the board.
             * On success, move to next square. On failure, remove number from list and retry.
             * Done with backtracking recursion.
             *
             * O(n * m!)
             * ^^^ Where n is how many numbers to test and m is the size of the board. Ahh.
             */
            //List of numbers to try. Failures get weeded out.
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
                        // No more columns or rows.
                        // We've arrived at the end of the board, so we can copy everything over to the SolutionBoard.
                        //Define SolutionBoard size
                        SolutionBoard = new string[testBoard.GetLength(0), testBoard.GetLength(1)];
                        //Traverse testBoard and copy over squares
                        for (int colIndex = 0; colIndex < testBoard.GetLength(0); colIndex++)
                        {
                            for (int rowIndex = 0; rowIndex < testBoard.GetLength(1); rowIndex++)
                            {
                                SolutionBoard[colIndex, rowIndex] = testBoard[colIndex, rowIndex];
                                // string[,] SolutionBoard = testBoard[colIndex,rowIndex]
                            }
                        }

                        return true; //There isn't another column or row after this, so we're done. Return true and cascade
                    }

                }
                tryNumbers.RemoveAt(itemIndex); //remove the number so we don't try it again
            }
            return false; //if we run out of numbers, the board can't be completed this way
        }

        public bool PlayBoardInitializer(string[,] fullBoard, int difficulty)
        {
            /*
             * Takes the generated board from SolutionBoardInitializer and user requested difficulty.
             * Calculates number of squares to eliminate from the board. Randomly selects locations.
             * Copies new board to the PlayBoard.

             */
            if (fullBoard == null) return false;
            if (fullBoard.Length == 0) return false;

            //Minimum # of squares to keep on the play board based on board dimensions
            var minimumKeptSquares = fullBoard.GetLength(0) + fullBoard.GetLength(1) - 1;
            //Requested # of kept squares to keep on board. Minimum of 10
            var requestedKeptSquares = (fullBoard.GetLength(0) * fullBoard.GetLength(1)) - Math.Max((10 * difficulty), 10);
            //The number of squares to be kept. Taking the high of requested and minimum.
            var keepSquares = Math.Max(minimumKeptSquares, requestedKeptSquares);
            var coordsRandom = new Random();

            //O(n logN)
            //keepSquares is x number less than the board total length, so remove squares until it's the same.
            for (; keepSquares < fullBoard.Length; keepSquares++)
            {
                var column = coordsRandom.Next(0, fullBoard.GetLength(0)); //Random x coordinate
                var row = coordsRandom.Next(0, fullBoard.GetLength(1)); //Random y coordinate
                
                while (fullBoard[column, row].Contains("-")) //If this square was already emptied, pick another.
                {
                    column = coordsRandom.Next(0, fullBoard.GetLength(0));
                    row = coordsRandom.Next(0, fullBoard.GetLength(1));
                }
                fullBoard[column, row] = "--"; //Empty the square
            }

            //O(n^2)
            //Define PlayBoard size
            PlayBoard = new string[fullBoard.GetLength(0), fullBoard.GetLength(1)];
            for (int colIndex = 0; colIndex < fullBoard.GetLength(0); colIndex++)
            {
                for (int rowIndex = 0; rowIndex < fullBoard.GetLength(1); rowIndex++)
                {
                    //Copy squares from the 'fullBoard' (not full now) into the PlayBoard
                    PlayBoard[colIndex, rowIndex] = fullBoard[colIndex, rowIndex];
                }
            }

            return true;
        }

        public bool UpdateBoard(int column, int row, string entry)
        {
            /*
             * Requires x/y coordinates and the entry to insert.
             * Checks for null or if it is a permanent entry (marked by '*').
             * Replaces only that square.
             */
            if (PlayBoard == null || PlayBoard.Length <= 0) return false;
            if (PlayBoard[column, row].Contains('*')) return false;
            
            PlayBoard[column, row] = entry;
            return true;

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


    }
}
