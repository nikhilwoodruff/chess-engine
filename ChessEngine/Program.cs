using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessEngine
{
    public class Chess
    {
        public State board;
        public void Initialise()
        {
            board = State.startBoard;
            /*board.PrintState();
            for(int i = 0; i < 25; i++)
            {
                Console.WriteLine(i + ".");
                PrintDifference(board, Search(board));
                board = Search(board);
                board.PrintState();
                board = State.Invert(board);
                Console.WriteLine();
                Console.WriteLine(i + ". ...");
                PrintDifferenceInverted(board, Search(board));
                board = Search(board);
                board.PrintStateInverted();
                Console.WriteLine();
                board = State.Invert(board);
            }*/
        }
        public State HaveGoes()
        {
            board = Search(board);
            board = State.Invert(board);
            board = Search(board);
            board = State.Invert(board);
            return board;
        }
        static State Search(State board)
        {
            List<State> moveList = board.GetMoves();
            List<int> moveScores = new List<int>();
            for(int i = 0; i < moveList.Count; i++)
            {
                State opponentMove = State.Invert(moveList[i]);
                List<State> opponentMoveList = opponentMove.GetMoves();
                opponentMoveList.Sort();
                moveScores.Add(-opponentMoveList[opponentMoveList.Count - 1].GetScore());
            }
            List<State> maxScoring = new List<State>();
            for(int i = 0; i < moveScores.Count; i++)
            {
                if(moveScores[i] == moveScores.Max())
                {
                    maxScoring.Add(moveList[i]);
                }
            }
            Random generator = new Random();
            return maxScoring[generator.Next(0, maxScoring.Count)];
        }
        public static void PrintDifference(State state1, State state2)
        {
            for(int i = 0; i < 8; i++)
            {
                Console.WriteLine();
                for(int j = 0; j < 8; j++)
                {
                    Console.Write(" " + (state2.board[j][7 - i] - state1.board[j][7 - i]));
                }
            }
        }
        public static void PrintDifferenceInverted(State state1, State state2)
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(" " + (state2.board[7 - j][i] - state1.board[7 - j][i]));
                }
            }
        }
    }
    public class State : IComparable<State>
    {   // 1=P, 2=B, 3=N, 4=R, 5=Q, 6=K (White)
        public int[][] board;
        public int CompareTo(State state)
        {
            return GetScore().CompareTo(state.GetScore());
        }
        public static State Invert(State state)
        {
            State inverted = new State(CopyArray(state.board));
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    inverted.board[i][j] = state.board[7 - i][7 - j] * -1;
                }
            }
            return inverted;
        }
        public int GetScore()
        {
            int total = 0;
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    int player = 0;
                    if(board[i][j] < 1)
                    {
                        player = -1;
                    }
                    else if(board[i][j] > 1)
                    {
                        player = 1;
                    }
                    switch(Math.Abs(board[i][j]))
                    {
                        case 0:
                            break;
                        case 1:
                            total += 1 * player;
                            break;
                        case 2:
                            total += 3 * player;
                            break;
                        case 3:
                            total += 3 * player;
                            break;
                        case 4:
                            total += 5 * player;
                            break;
                        case 5:
                            total += 9 * player;
                            break;
                        case 6:
                            total += 1000 * player;
                            break;
                    }
                }
            }
            return total;
        }
        public static State startBoard = new State(new int[][] { new int[] { 4, 1, 0, 0, 0, 0, -1, -4 }, new int[] { 3, 1, 0, 0, 0, 0, -1, -3}, new int[] { 2, 1, 0, 0, 0, 0, -1, -2}, new int[] { 5, 1, 0, 0, 0, 0, -1, -5}, new int[] { 6, 1, 0, 0, 0, 0, -1, -6 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -2}, new int[] { 3, 1, 0, 0, 0, 0, -1, -3}, new int[] { 4, 1, 0, 0, 0, 0, -1, -4} });
        public State(int[][] board)
        {
            this.board = board;
        }
        public void PrintState()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Console.Write(" " + board[j][7 - i]);
                }
                Console.WriteLine();
            }
        }
        public void PrintStateInverted()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(" " + board[7 - j][i] * -1);
                }
                Console.WriteLine();
            }
        }
        public State ChangeState(State state, int i, int j, int x, int y, int newPieceCode)
        {
            State newState = new State(CopyArray(state.board));
            newState.board[i+x][j+y] = newPieceCode;
            newState.board[i][j] = 0;
            return newState;
        }
        public static int[][] CopyArray(int[][] array)
        {
            int[][] newArray = new int[8][];
            for(int i = 0; i < 8; i++)
            {
                newArray[i] = new int[8];
                for(int j = 0; j < 8; j++)
                {
                    newArray[i][j] = array[i][j];
                }
            }
            return newArray;
        }
        public List<State> GetMoves()
        {
            List<State> moves = new List<State>();
            int[][][] displacements = new int[2][][]; //displacements to try for some major pieces: Kn, Ki
            displacements[0] = new int[][] { new int[] { 1, 2 }, new int[] { 2, 1 }, new int[] { 1, -2 }, new int[] { 2, -1 }, new int[] { -1, -2 }, new int[] { -2, -1 }, new int[] { -1, 2 }, new int[] { -2, 1 } };
            displacements[1] = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, 1 }, new int[] { -1, -1 } };
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0;  j < 8; j++)
                {
                    int pieceCode = board[i][j];
                    switch(pieceCode)
                    {
                        case 1:
                            if (BoardSpaceExists(i, j, 1, 1))
                            {
                                if (IsEnemyOccupied(i, j, i + 1, j + 1))
                                {
                                    moves.Add(ChangeState(this, i, j, 1, 1, 1));
                                }
                            }
                            if (BoardSpaceExists(i, j, -1, 1))
                            {
                                if (IsEnemyOccupied(i, j, i - 1, j + 1))
                                {
                                    moves.Add(ChangeState(this, i, j, -1, 1, 1));
                                }
                            }
                            if (BoardSpaceExists(i, j, 0, 1))
                            {
                                if (!IsEnemyOccupied(i, j, i, j + 1) && IsCapturable(new int[] { board[i][j], board[i][j + 1] }))
                                {
                                    moves.Add(ChangeState(this, i, j, 0, 1, 1));
                                }
                            }
                            if(j == 1)
                            {
                                if (BoardSpaceExists(i, j, 0, 2))
                                {
                                    if (!IsEnemyOccupied(i, j, i, j + 2) && IsCapturable(new int[] { board[i][j], board[i][j + 2] }))
                                    {
                                        moves.Add(ChangeState(this, i, j, 0, 2, 1));
                                    }
                                }
                            }
                            break;
                        case 2:
                            moves = AddLegalMovesUntil(i, j, 1, 1, moves);
                            moves = AddLegalMovesUntil(i, j, 1, -1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, 1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, -1, moves);
                            break;
                        case 3:
                            for(int k = 0; k < 8; k++)
                            {
                                moves = AddLegalMove(i, j, displacements[0][k][0], displacements[0][k][1], moves);
                            }
                            break;
                        case 4:
                            moves = AddLegalMovesUntil(i, j, 0, 1, moves);
                            moves = AddLegalMovesUntil(i, j, 1, 0, moves);
                            moves = AddLegalMovesUntil(i, j, 0, -1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, 0, moves);
                            break;
                        case 5:
                            moves = AddLegalMovesUntil(i, j, 0, 1, moves);
                            moves = AddLegalMovesUntil(i, j, 1, 0, moves);
                            moves = AddLegalMovesUntil(i, j, 0, -1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, 0, moves);
                            moves = AddLegalMovesUntil(i, j, 1, 1, moves);
                            moves = AddLegalMovesUntil(i, j, 1, -1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, 1, moves);
                            moves = AddLegalMovesUntil(i, j, -1, -1, moves);
                            break;
                        case 6:
                            for (int k = 0; k < 8; k++)
                            {
                                moves = AddLegalMove(i, j, displacements[1][k][0], displacements[1][k][1], moves);
                            }
                            break;
                    }
                }
            }
            return moves;
        }
        List<State> AddLegalMovesUntil(int i, int j, int x, int y, List<State> moves)
        {
            for(int k = 1; k < 8; k++)
            {
                if (BoardSpaceExists(i, j, x * k, y * k))
                {
                    if (!IsEnemyOccupied(i, j, i + x * k, j + y * k) && IsCapturable(new int[] { board[i][j], board[i + x * k][j + y * k] }))
                    {
                        moves.Add(ChangeState(this, i, j, x * k, y * k, board[i][j]));
                    }
                    else if (IsEnemyOccupied(i, j, i + x * k, j + y * k) && IsCapturable(new int[] { board[i][j], board[i + x * k][j + y * k] }))
                    {
                        moves.Add(ChangeState(this, i, j, x * k, y * k, board[i][j]));
                        k = 8;
                    }
                    else
                    {
                        k = 8;
                    }
                }
                else
                {
                    k = 8;
                }
            }
            return moves;
        }
        List<State> AddLegalMove(int i, int j, int x, int y, List<State> moves)
        {
            if (BoardSpaceExists(i, j, x, y))
            {
                if (!IsEnemyOccupied(i, j, i + x, j + y) && IsCapturable(new int[] { board[i][j], board[i + x][j + y] }))
                {
                    moves.Add(ChangeState(this, i, j, x, y, board[i][j]));
                }
            }
            return moves;
        }
        bool IsMove(int[] position, int[] displacement)
        {
            return (BoardSpaceExists(position[0], position[1], displacement[0], displacement[1]) && IsCapturable(new int[] { board[position[0]][position[1]], board[position[0] + displacement[0]][position[1] + displacement[1]] }));
        }
        bool BoardSpaceExists(int a, int b, int x, int y)
        {
            int[][] coordinates = new int[2][];
            coordinates[0] = new int[] { a, b };
            coordinates[1] = new int[] { a + x, b + y };
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    if(coordinates[i][j] < 0 || coordinates[i][j] > 7)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        bool IsCapturable(int[] pieceCodes)
        {
            if(pieceCodes[0] > 0 && pieceCodes[1] > 0)
            {
                return false;
            }
            if(pieceCodes[0] < 0 && pieceCodes[1] < 0)
            {
                return false;
            }
            return true;
        }
        bool IsEnemyOccupied(int x, int y, int a, int b)
        {
            return (board[x][y] > 0 && board[a][b] < 0 || board[x][y] < 0 && board[a][b] > 0);
        }
    }
}
