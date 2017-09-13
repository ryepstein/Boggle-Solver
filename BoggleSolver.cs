//Ryan Epstein 7/18/2017

//Program that will solve an MxN sized board of Boggle
//-Read inputs as command line arguments
//-Output words found in output.txt

using System;

namespace Boggle
{
    class BoggleSolver
    {
        static string[,] gameBoard;         //MxN matrix of letters
        static bool[,] visited;         //mark position if the letter has been visited in a word check already
        static string[] dictionary;         //list of valid words to find
        static bool[] foundDict;        //mark dictionary word positions as found to exclude from searching
        static string[] foundBoggle;        //hold list of words found in the Boggle board

        static System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(@"output.txt");

        static void createBoard(string boardFile)
        {
            string[] letters = System.IO.File.ReadAllLines(boardFile);

            int rowCount = letters.Length;      //count elements to get number of rows of the board (M)
            int colCount = letters[0].Split(null).Length;   //count characters on a line to get number of columns (N)

            gameBoard = new string[rowCount, colCount];     //intialize gameBoard to MxN size
            visited = new bool[gameBoard.GetLength(0),gameBoard.GetLength(1)];      //intialize visited to size of gameBoard

            fillBoard(letters);
        }

        static void fillBoard(string[] boardInput)
        {
            string[] temp;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                temp = boardInput[row].Split(null);         //split characters on each row into new array

                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    gameBoard[row, col] = temp[col];        //fill gameboard with each row's contents
                }
            }
        }

        static void printBoard(string[,] board)
        {
            //print 2d array
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    Console.Write("{0} ", board[row, col]);
                }
                Console.WriteLine();
            }
        }

        static void createDictionary(string dictFile)
        {
            dictionary = System.IO.File.ReadAllLines(dictFile);     //store every line of file as a new word of dictionary array

            mergeSort(dictionary);      //sort dictionary to help search optimization

            foundBoggle = new string[dictionary.Length];       //list of words found in the Boggle
            foundDict = new bool[dictionary.Length];        //list to mark off a found word from the dictionary
        }

        static void solveBoggle()
        {
            //check Array against dictionary words
            //loop entire board and run a word check on each letter as a starting position.
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Array.Clear(visited, 0, visited.Length);        //clear array of visited letters on each new starting letter
                    recursiveCheck(gameBoard, row, col, "");
                }
            }
        }

        static void recursiveCheck(string[,] board, int index1, int index2, string theString)
        {
            int rowStart = index1;
            int colStart = index2;
            visited[rowStart, colStart] = true;
            theString += board[rowStart, colStart];

            if (theString.Length >= 3 && checkDictionary(theString))
            {
                fileWriter.WriteLine(theString);      //print the found word out to the output.txt
            }

            if(isPrefix(theString))
            {
                for (int row = rowStart - 1; row <= rowStart + 1 && row < board.GetLength(0); row++)        //check adjacent letters; Ranging from rowStart-1 to rowStart+1
                {
                    for (int col = colStart - 1; col <= colStart + 1 && col < board.GetLength(1); col++)        //check adjacent letters; Ranging from colStart-1 to colStart+1
                    {
                        if (row >= 0 && col >= 0 && !visited[row, col])         //if current position has not been visited, call recursively
                            recursiveCheck(board, row, col, theString);
                    }
                }
            }
            visited[rowStart, colStart] = false;
        }

        static bool isPrefix(string prefix)
        {
            prefix = prefix.ToUpper();
            
            for (int i = 0; i < dictionary.Length; i++)
            {
                string comparedWord = dictionary[i].ToUpper();
                if (comparedWord[0].Equals(prefix[0]))      //check if first letter of a word matches with first character in prefix
                {
                    if (comparedWord.Contains(prefix))          //further check to see if the prefix is contained in the word
                        return true;
                }
            }
            return false;
        }
        
        static bool checkDictionary(string theString)
        {
            for (int i = 0; i < dictionary.Length; i++)
            {
                if (foundDict[i] == true)       //if current word has arleady been found in Boggle, skip to next word
                    continue;
                if (theString.Length >= 3 && dictionary[i].Equals(theString, StringComparison.OrdinalIgnoreCase))       //if word is at least 3 length and found in dictionary
                {
                    foundDict[i] = true;        //mark the word as found to avoid unnecessary comparisons
                    return true;
                }
            }
            return false;
        }

        static void mergeSort(string[] words)
        {
            if (words.Length >= 2)      //base case of recursive splitting
            {
                string[] left = new string[words.Length / 2];
                string[] right = new string[words.Length - (words.Length / 2)];

                for (int i = 0; i < left.Length; i++)       //fill left half of array
                {
                    left[i] = words[i];
                }

                for (int i = 0; i < right.Length; i++)      //fill right half of array
                {
                    right[i] = words[i + words.Length / 2];
                }

                mergeSort(left);        //recursively call mergeSort on left half of Array
                mergeSort(right);       //recursively call mergeSort on right half of Array
                merge(words, left, right);      //recursively merge mini-arrays back together
            }
        }

        static void merge(string[] words, string[] left, string[] right)
        {
            int leftPos = 0;
            int rightPos = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (rightPos >= right.Length || (leftPos < left.Length && left[leftPos].CompareTo(right[rightPos]) < 0))        //check if string on left comes before string on right
                {
                    words[i] = left[leftPos];
                    leftPos++;
                }
                else
                {
                    words[i] = right[rightPos];
                    rightPos++;
                }
            }
        }


        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("Proper usage: .\\boggle.exe board.txt dictionary.txt");
                Console.ReadKey();
                System.Environment.Exit(0);
            }
            createBoard(args[0]);
            createDictionary(args[1]);

            solveBoggle();

            Console.WriteLine("Solver is finished! \nCheck output.txt");

            fileWriter.Close();
            Console.ReadKey();
        }
    }
}
