/*******************************************************************
*
* Gomoku game
*    Version by Anton Yermolayev (anton.yermolayev@outlook.com)
*    03.09.2020
*
*********************************************************************/


using System;
using System.Collections.Generic;

namespace Gomoku
{
    //
    // The class with the Main method
    class Gomoku
    {
        int BoardSize = 15;
        Coordinates nextMove;
        int boardRow, boardCol, textRow, textCol, currentPlayer;

        // Predefined game situation for testing
        string[] arrayOfPredefinedGame = {      "O______________",
                                                "_______________",
                                                "_______________",
                                                "_______________",
                                                "__________O____",
                                                "_______________",
                                                "__________O____",
                                                "_____XX_XXO____",
                                                "__________X____",
                                                "________O__X___",
                                                "_________O__X__",
                                                "__________O__X_",
                                                "___________O___",
                                                "____________O__",
                                                "_______________" };

        static void Main(string[] args)
        {
            // need this instance for "global" non-static variables
            Gomoku GomokuGame = new Gomoku();

            Console.Clear();
            Console.WriteLine("Please select type of game: \n\n1. Computer vs Computer.\n2. Computer vs Human.\n\nPlease type 1 or 2. ");
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    GomokuGame.CompVsComp();
                    break;
                case 2:
                    GomokuGame.CompVsHuman();
                    break;
            } 
        }
        
        void CompVsComp()
        {
            // Board brd = new Board(BoardSize, arrayOfPredefinedGame);
            Board brd = new Board(BoardSize);

            Console.Clear();
            Console.WriteLine("Please choose a strategy for the player 1 (1 or 2): ");
            int Player1Strategy = int.Parse(Console.ReadLine()) - 1;
            Console.WriteLine("Please choose a strategy for the player 2 (1 or 2): ");
            int Player2Strategy= int.Parse(Console.ReadLine()) - 1;
            brd.SetPlayersStrategy(Player1Strategy, Player2Strategy);

            Console.Clear();
            Console.WriteLine("    === The game of Gomoku ===\n");

            // origin for Board print-out
            boardRow = Console.CursorTop;
            boardCol = Console.CursorLeft;
            // origin for supporting text print-out
            textRow = boardRow + BoardSize + 5;
            textCol = boardCol + 3;
            brd.ShowBoard(boardCol, boardRow);

            currentPlayer = 1;
            do
            {
                nextMove = brd.FindNextMove(currentPlayer);
                Console.SetCursorPosition(textCol, textRow);
                Console.WriteLine("The next move by Player {0} (\"{1}\") will be {2,2}{3,2}. Press any key.", 
                    currentPlayer, brd.GetPlayerSign(currentPlayer), nextMove.X_char, nextMove.Y_char);
                Console.ReadKey();

                brd.SetBoard(currentPlayer, nextMove);
                brd.ShowBoard(boardCol, boardRow);

                // toggle the player number
                currentPlayer = 3 - currentPlayer;

            } while (!(brd.DidPlayerWin(1) || brd.DidPlayerWin(2) || brd.BoardIsFull()));

            Console.SetCursorPosition(textCol, textRow + 2);
            if (brd.DidPlayerWin(1))
            {
                Console.WriteLine("Player 1 (\"{0}\") won!", brd.GetPlayerSign(1));
            }

            if (brd.DidPlayerWin(2))
            {
                Console.WriteLine("Player 2 (\"{0}\") won!", brd.GetPlayerSign(2));
            }

            if (brd.BoardIsFull())
            {
                Console.WriteLine("It's a draw! Friendship is the winner.");
            }
        }

        void CompVsHuman()
        {
            // Board brd = new Board(BoardSize, arrayOfPredefinedGame);
            Board brd = new Board(BoardSize);
            
            // Computer is Player1, Human is Player2
            Console.Clear();
            Console.WriteLine("Please choose the game strategy for the computer (1 or 2): ");
            int Player1Strategy = int.Parse(Console.ReadLine()) - 1;
            brd.SetPlayersStrategy(Player1Strategy, Player1Strategy); // the Computer will think that the Human is following the same strategy as him

            Console.WriteLine("Who's going to make the first move: Computer (1) or you (2)?");
            int currentPlayer = int.Parse(Console.ReadLine());

            Console.Clear();
            Console.WriteLine("    === The game of Gomoku ===\n");

            // origin for Board print-out
            boardRow = Console.CursorTop;
            boardCol = Console.CursorLeft;
            // origin for supporting text print-out
            textRow = boardRow + BoardSize + 5;
            textCol = boardCol + 3;
            brd.ShowBoard(boardCol, boardRow);

            do
            {
                switch (currentPlayer)
                {
                    case 1: // The Computer is deciding on his move
                        nextMove = brd.FindNextMove(currentPlayer);
                        Console.SetCursorPosition(textCol, textRow);
                        Console.WriteLine("The Computer (\"{0}\") made his move on {1,2}{2,2}           ", 
                            brd.GetPlayerSign(currentPlayer), nextMove.X_char, nextMove.Y_char);
                        //Console.ReadKey();
                        break;
                    case 2: // The Human is making decisions here
                        Console.SetCursorPosition(textCol, textRow + 1);
                        Console.Write("Please type your (\"{0}\") next move (e.g. E2 or e2): ", brd.GetPlayerSign(currentPlayer));
                        nextMove = new Coordinates(Console.ReadLine());
                        // Clean previous message
                        Console.SetCursorPosition(textCol, textRow + 1);
                        Console.Write("                                                            ");
                        break;
                }

                brd.SetBoard(currentPlayer, nextMove);
                brd.ShowBoard(boardCol, boardRow);

                // toggle player number
                currentPlayer = 3 - currentPlayer;

            } while (!(brd.DidPlayerWin(1) || brd.DidPlayerWin(2) || brd.BoardIsFull()));

            Console.SetCursorPosition(textCol, textRow + 2);
            if (brd.DidPlayerWin(1))
            {
                Console.WriteLine("The Computer won!");
            }

            if (brd.DidPlayerWin(2))
            {
                Console.WriteLine("You won, clever boy!");
            }

            if (brd.BoardIsFull())
            {
                Console.WriteLine("It's a draw! Friendship is the winner.");
            }
        }
    }


    //
    // Class: Board.
    //      Represents the gomoku board.
    class Board
    {
        // The board itself. Any position is BoardArray[y, x], where y <= BoardArray.GetUpperBound(0), x <= BoardArray.GetUpperBound(1)
        char[,] BoardArray;
        float[,] mapOfAttack;
        float[,] mapOfDefence;
        char[] PlayerSign = { 'O', 'X' };
        char EmptySign = '_';
        int player1Params, player2Params;

        // The constructor which resets the board.
        public Board(int brdSize)
        {
            BoardArray = new char[brdSize, brdSize];
            for (int y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                for (int x = 0; x <= BoardArray.GetUpperBound(1); x++)
                {
                    BoardArray[y, x] = EmptySign;
                }
                Console.WriteLine();
            }
        }

        // The constructor which fills the board with predefined array as parameter (FOR TESTING).
        public Board(int brdSize, string[] arrPredefinedGame)
        {
            BoardArray = new char[brdSize, brdSize];
            for (int y = 0; (y <= BoardArray.GetUpperBound(0)); y++)
            {
                for (int x = 0; (x <= BoardArray.GetUpperBound(1)); x++)
                {
                    // if predefined board is less than BoardArray, we fill the space with EmptySign
                    if ((y < arrPredefinedGame.Length) && (x < arrPredefinedGame[y].Length))
                        BoardArray[y, x] = arrPredefinedGame[y][x];
                    else BoardArray[y, x] = EmptySign;
                }
            }
        }

        // Prints the current board.
        public void ShowBoard(int X_origin, int Y_origin)
        {
            int x, y;
            Console.SetCursorPosition(X_origin, Y_origin);

            // Legend of X axis
            Console.Write("   ");
            for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
            {
                Console.Write((char)('A' + x) + " ");
            }
            Console.SetCursorPosition(X_origin, ++Y_origin);

            for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
                {
                    if (x == 0) { Console.Write("{0, 3}", y + 1); };
                    Console.Write(BoardArray[y, x] + " ");
                }
                Console.SetCursorPosition(X_origin, ++Y_origin);
            }
        }

        //
        // Returns True if the specified player won.
        // Receives the number of the player as a parameter.
        public bool DidPlayerWin(int PlayerNo)
        {
            int y, x, y_travelling, x_travelling;
            int count;
            bool victory = false;

            // analyze horizontally
            count = 0;
            for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
                {
                    if (BoardArray[y, x] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5) victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            // analyze vertically

            count = 0;
            for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
            {
                for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
                {
                    if (BoardArray[y, x] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5) victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            // analyze at 45°:
            // 1) left upper "triangle"
            count = 0;
            for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                // y_travelling will go up the board (--) with each x increment
                y_travelling = y;
                for (x = 0; (x <= BoardArray.GetUpperBound(1)) && (y_travelling >= 0); x++, y_travelling--)
                {
                    if (BoardArray[y_travelling, x] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5)
                            victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            // analyze at 45°:
            // 2) right lower "triangle"
            count = 0;
            for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
            {
                // y_travelling will go up the board (--) with each x_travelling increment
                y_travelling = BoardArray.GetUpperBound(0);
                for (x_travelling = x; (x_travelling <= BoardArray.GetUpperBound(1)) && (y_travelling >= 0); x_travelling++, y_travelling--)
                {
                    // Test of analysis spots
                    //Console.SetCursorPosition(x_travelling*2 + 3, y_travelling + 2); Console.Write('@');
                    if (BoardArray[y_travelling, x_travelling] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5)
                            victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            // analyze at -45°:
            // 1) right upper "triangle"
            count = 0;
            for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                // y_travelling will go up the board (--) with each x decrement
                y_travelling = y;
                for (x = BoardArray.GetUpperBound(1); (x >= 0) && (y_travelling >= 0); x--, y_travelling--)
                {
                    if (BoardArray[y_travelling, x] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5)
                            victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            // analyze at -45°:
            // 2) left lower "triangle"
            count = 0;
            for (x = BoardArray.GetUpperBound(1); x >= 0; x--)
            {
                // y_travelling will go up the board (--) with each x_travelling decrement
                y_travelling = BoardArray.GetUpperBound(0);
                for (x_travelling = x; (x_travelling >= 0) && (y_travelling >= 0); x_travelling--, y_travelling--)
                {
                    //Console.SetCursorPosition(x_travelling*2 + 3, y_travelling + 2); Console.Write('@');
                    if (BoardArray[y_travelling, x_travelling] == PlayerSign[PlayerNo - 1])
                    {
                        count++;
                        if (count == 5)
                            victory = true;
                    }
                    else count = 0;
                    if (victory)
                        return true;
                }
                // if the board ends, reset the counter
                count = 0;
            }

            return false;
        }

        //
        // Returns:
        //      - true if the board is full
        //      - false if the board has any free spot
        public bool BoardIsFull()
        {
            foreach (char x in BoardArray)
                if (x == EmptySign) return false;
            return true;
        }


        //
        // NextMove method will analyze the board and recommend the next move for the specified Player
        // The coordinates of the next move will be sent back via Coordinates instance
        public Coordinates FindNextMove(int PlayerNo)
        {
            int x, y, otherPlayerNo;
            float maxFactor;
            List<Coordinates> possibleAttack = new List<Coordinates>();
            Random rand = new Random();

            // map of recommendation factors for attack, the bigger the value of the cell the more benefits
            mapOfAttack = new float[BoardArray.GetUpperBound(0) + 1, BoardArray.GetUpperBound(1) + 1];

            // map of recommendation factors for defence, the bigger the value of the cell the more benefits
            mapOfDefence = new float[BoardArray.GetUpperBound(0) + 1, BoardArray.GetUpperBound(1) + 1];

            // calculate the number of the other player
            otherPlayerNo = 3 - PlayerNo;

            MakeAnalysisMap(PlayerNo, true, mapOfAttack);
            MakeAnalysisMap(otherPlayerNo, false, mapOfDefence);

            // for testing
            //ShowAnalysisMap(mapOfAttack);
            //ShowAnalysisMap(mapOfDefence);

            // Make full analysis:
            // sum up values in corresponding cells of mapOfAttack and mapOfDefence; Result is in mapOfAttack array
            for (y = 0; (y <= mapOfAttack.GetUpperBound(0)); y++)
            {
                for (x = 0; (x <= mapOfAttack.GetUpperBound(1)); x++)
                {
                    mapOfAttack[y, x] += mapOfDefence[y, x];
                }
            }

            // for testing again
            //ShowAnalysisMap(mapOfAttack);

            // find all coordinates with max values (there may be several of them) in mapOfAttack 
            // and them into possibleAtack List
            maxFactor = 0;
            foreach (float coef in mapOfAttack)
            {
                if (coef > maxFactor) maxFactor = coef;
            }

            for (y = 0; (y <= mapOfAttack.GetUpperBound(0)); y++)
            {
                for (x = 0; (x <= mapOfAttack.GetUpperBound(1)); x++)
                {
                    if (mapOfAttack[y, x] == maxFactor)
                        possibleAttack.Add(new Coordinates(x, y));
                }
            }

            // randomly select one instance of Coordinates from possibleAttack List
            return possibleAttack[rand.Next(possibleAttack.Count)];
        }

        //
        // Analyzes the BoardArray and returns a map of efficiency coefficients 
        // Input: 
        //      - number of the player for whom to perform the analysis, 
        //      - boolean Attack to indicate is it for Attack or for Defence analysis?
        //      - the reference to array which will hold the result analysis map
        // Output: 
        //      - modifications of the array which was received.
        public void MakeAnalysisMap(int PlayerNo, bool Attack, float[,] analysisMap)
        {
            int countForward, countBackward;
            bool emptyChain; // equals 1 if the found chain is open, 0 if thr chain is limited by opposite player's sign or end of board
            float factor;
            int x, y, x_travelling, y_travelling;
            int currentPlayerParams = PlayerNo == 1 ? player1Params : player2Params;
            char currentSymbol; 

            // make an instance of class holding a collection of parameters for calculation of the map
            NextMoveParameters NextMove_parameters = new NextMoveParameters();

            // Check through all cells.
            // Analysis is done in 4 steps: horizontally, vertically, diagonally +45°, diagonally -45°
            // the results are summed up in counterForward, counterBackward and then in factor 
            // which is a sum of counters with strategic parameters applied as multiplier
            for (y = 0; y <= BoardArray.GetUpperBound(0); y++)
            {
                for (x = 0; x <= BoardArray.GetUpperBound(1); x++)
                {
                    if (BoardArray[y, x] == EmptySign)
                    {
                        factor = 0;
                        // analyze horizontally:
                        // 1) look to the right by 4 cells, count the chain stones
                        countForward = 0;
                        countBackward = 0;
                        emptyChain = false;
                        for (x_travelling = 1; (x_travelling <= 4) && (x + x_travelling <= BoardArray.GetUpperBound(1)); x_travelling++)
                        {
                            currentSymbol = BoardArray[y, x + x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countForward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countForward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                                
                        }

                        // analyze horizontally:
                        // 2) look to the left by 4 cells, continue to count the chain stones
                        for (x_travelling = 1; (x_travelling <= 4) && (x - x_travelling >= 0); x_travelling++)
                        {
                            currentSymbol = BoardArray[y, x - x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countBackward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countBackward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                        }

                        CollectCounts(countForward + countBackward);

                        // analyze vertically:
                        // 1) look down by 4 cells, count the chain length
                        countForward = 0;
                        countBackward = 0;
                        emptyChain = false;
                        for (y_travelling = 1; (y_travelling <= 4) && (y + y_travelling <= BoardArray.GetUpperBound(0)); y_travelling++)
                        {
                            currentSymbol = BoardArray[y + y_travelling, x];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countForward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countForward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                        }
                        
                        // analyze vertically:
                        // 2) look up by 4 cells, continue to count the chain length
                        for (y_travelling = 1; (y_travelling <= 4) && (y - y_travelling >= 0); y_travelling++)
                        {
                            currentSymbol = BoardArray[y - y_travelling, x];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countBackward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countBackward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                        }

                        CollectCounts(countForward + countBackward);

                        // analyze diagonally +45°:
                        // 1) look down-left by 4 cells, count the chain length
                        countForward = 0;
                        countBackward = 0;
                        emptyChain = false;
                        y_travelling = 1;
                        x_travelling = 1;
                        while ((y_travelling <= 4) && (y + y_travelling <= BoardArray.GetUpperBound(0)) &&
                                (x - x_travelling >= 0))
                        {
                            currentSymbol = BoardArray[y + y_travelling, x - x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countForward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countForward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                            x_travelling++;
                            y_travelling++;
                        }
                        
                        // analyze diagonally +45°:
                        // 2) look up-right by 4 cells, count the chain length
                        y_travelling = 1;
                        x_travelling = 1;
                        while ((y_travelling <= 4) && (y - y_travelling >= 0) &&
                                (x + x_travelling <= BoardArray.GetUpperBound(1)))
                        {
                            currentSymbol = BoardArray[y - y_travelling, x + x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countBackward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countBackward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                            x_travelling++;
                            y_travelling++;
                        }

                        CollectCounts(countForward + countBackward);

                        // analyze diagonally -45°:
                        // 1) look down-right by 4 cells, count the chain length
                        countForward = 0;
                        countBackward = 0;
                        emptyChain = false;
                        y_travelling = 1;
                        x_travelling = 1;
                        while ((y_travelling <= 4) && (y + y_travelling <= BoardArray.GetUpperBound(0)) &&
                                (x + x_travelling <= BoardArray.GetUpperBound(1)))
                        {
                            currentSymbol = BoardArray[y + y_travelling, x + x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countForward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countForward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                            x_travelling++;
                            y_travelling++;
                        }
                        // analyze diagonally -45°:
                        // 2) look up-left by 4 cells, count the chain length
                        y_travelling = 1;
                        x_travelling = 1;
                        while ((y_travelling <= 4) && (y - y_travelling >= 0) &&
                                (x - x_travelling >= 0))
                        {
                            currentSymbol = BoardArray[y - y_travelling, x - x_travelling];
                            if (currentSymbol == PlayerSign[PlayerNo - 1])
                            {
                                countBackward++;
                            }
                            else
                            {
                                if ((currentSymbol == EmptySign) && (countBackward > 0)) // chain was broken with EmptySign
                                    emptyChain = true;
                                break;
                            }
                            x_travelling++;
                            y_travelling++;
                        }

                        CollectCounts(countForward + countBackward);

                        // Apply strategic parameters common to all directions
                        if (Attack) factor *= NextMove_parameters[currentPlayerParams].ImportanceOfAttack;
                            else factor *= NextMove_parameters[currentPlayerParams].ImportanceOfDefence;
                        analysisMap[y, x] = factor;

                        // Collect counts in factor. Apply strategic parameters as multipliers
                        void CollectCounts(int count)
                        {
                            switch (count)
                            {
                                case 1: factor += count * NextMove_parameters[currentPlayerParams].ImportanceOf1; break;
                                case 2: factor += count * NextMove_parameters[currentPlayerParams].ImportanceOf2; break;
                                case 3: factor += count * NextMove_parameters[currentPlayerParams].ImportanceOf3; break;
                                case 4: factor += count * NextMove_parameters[currentPlayerParams].ImportanceOf4; break;
                                default: factor += count; break;
                            }
                            if (emptyChain) factor *= NextMove_parameters[currentPlayerParams].ImportanceOfEmptyChain;
                        }
                    }
                }
            }
        }

        //
        // Print analysis map (for testing)
        void ShowAnalysisMap(float[,] analysisMap)
        {
            int x, y;

            Console.Write("\n   ");

            // Legend of X axis
            for (x = 0; x <= analysisMap.GetUpperBound(1); x++)
            {
                Console.Write("{0,8}", (char)('A' + x));
            }
            Console.WriteLine();

            for (y = 0; y <= mapOfAttack.GetUpperBound(0); y++)
            {
                for (x = 0; x <= mapOfAttack.GetUpperBound(1); x++)
                {
                    if (x == 0) { Console.Write("{0,3}", y + 1); };
                    Console.Write("{0,8:F2}", analysisMap[y, x]);
                }
                Console.WriteLine();
            }
        }

        //
        // Make changes to the Board (specifically, make a move)
        public void SetBoard(int PlayerNo, Coordinates MoveCoor)
        {
            BoardArray[MoveCoor.Y, MoveCoor.X] = PlayerSign[PlayerNo - 1];
        }

        //
        // Set class variables which are used in construction of analysis maps 
        public void SetPlayersStrategy(int Player1Str, int Player2Str)
        {
            player1Params = Player1Str;
            player2Params = Player2Str;
        }

        //
        // Returns the symbol of a player
        public char GetPlayerSign(int PlayerNo)
        {
            return PlayerSign[PlayerNo - 1];
        }

    }

    //
    // Class for sharing the coordinates of next move etc, zero-based.
    class Coordinates
    {
        int x, y;

        public Coordinates(int x_inp, int y_inp)
        {
            x = x_inp;
            y = y_inp;
        }

        // may be used in conversion of human input into proper coordinates
        public Coordinates(string InputString)
        {
            x = (int)char.ToUpper(InputString[0]) - (int)'A';
            y = int.Parse(InputString.Substring(1)) - 1;
        }

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public char X_char
        {
            get
            {
                return (char)('A' + x);
            }

        }
        public string Y_char
        {
            get
            {
                return (y + 1).ToString();
            }
        }
    }

    //
    // Class of strategic parameters in one bunch
    class SetOfNextMoveParameters
    {
        public float ImportanceOf1;
        public float ImportanceOf2;
        public float ImportanceOf3;
        public float ImportanceOf4;
        public float ImportanceOfAttack;
        public float ImportanceOfDefence;
        public float ImportanceOfEmptyChain;
    }

    //
    // List of sets of parameters
    class NextMoveParameters
    {
        List<SetOfNextMoveParameters> par = new List<SetOfNextMoveParameters>();
        // indexer for retrieving elements of "par" List
        public SetOfNextMoveParameters this[int index]
        {
            get
            {
                return par[index];
            }
        }
        // Constructor which sets some parameters as elements of "par" List
        public NextMoveParameters()
        {
            // parameters which make emphasis on ...
            par.Add(new SetOfNextMoveParameters());
            par[0].ImportanceOf1 = 1F;
            par[0].ImportanceOf2 = 1.1F;
            par[0].ImportanceOf3 = 1.3F;
            par[0].ImportanceOf4 = 10F;
            par[0].ImportanceOfAttack = 1F;
            par[0].ImportanceOfDefence = 1F;
            par[0].ImportanceOfEmptyChain = 1.3F;

            // parameters which makes a small emphasis on Attacking vs Defence
            par.Add(new SetOfNextMoveParameters());
            par[1].ImportanceOf1 = 1F;
            par[1].ImportanceOf2 = 1F;
            par[1].ImportanceOf3 = 1F;
            par[1].ImportanceOf4 = 1F;
            par[1].ImportanceOfAttack = 1.1F;
            par[1].ImportanceOfDefence = 1F;
            par[1].ImportanceOfEmptyChain = 1F;
        }
    }

}
