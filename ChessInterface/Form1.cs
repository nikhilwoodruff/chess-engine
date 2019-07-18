using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessInterface
{
    public partial class Form1 : Form
    {
        PictureBox[][] boxes = new PictureBox[8][];
        List<State> history = new List<State>();
        int index = 0;
        public Form1()
        {
            
            for (int a = 0; a < 8; a++)
            {
                boxes[a] = new PictureBox[8];
                for (int b = 0; b < 8; b++)
                {
                    boxes[a][b] = new PictureBox();
                    boxes[a][b].Size = new Size(150, 150);
                    boxes[a][b].Location = new Point(a * 150, (7 - b) * 150);
                    boxes[a][b].Visible = true;
                    boxes[a][b].BackColor = Color.Transparent;
                    boxes[a][b].SizeMode = PictureBoxSizeMode.StretchImage;
                    boxes[a][b].Parent = pictureBox1;
                    boxes[a][b].Click += HandleClick;
                    Controls.Add(boxes[a][b]);

                }
            }
            InitializeComponent();
            Initialise();
            UpdatePieces();
        }
        public void HandleClick(object sender, EventArgs e)
        {
            int x = (MousePosition.X - 224) / 56;
            int y = (MousePosition.Y - 88) / 62;
            textBox1.Text += MoveCode(new int[] { x, 7 - y});
        }
        void Act()
        {
            while (index < history.Count - 1)
            {
                history.RemoveAt(history.Count - 1);
            }
            if (textBox1.Text == "")
            {
                HaveGo(null);
            }
            else
            {
                char[] chars = textBox1.Text.ToCharArray();
                int[] coordinate = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    switch (chars[i])
                    {
                        case 'a':
                            coordinate[i] = 0;
                            break;
                        case 'b':
                            coordinate[i] = 1;
                            break;
                        case 'c':
                            coordinate[i] = 2;
                            break;
                        case 'd':
                            coordinate[i] = 3;
                            break;
                        case 'e':
                            coordinate[i] = 4;
                            break;
                        case 'f':
                            coordinate[i] = 5;
                            break;
                        case 'g':
                            coordinate[i] = 6;
                            break;
                        case 'h':
                            coordinate[i] = 7;
                            break;
                        case '1':
                            coordinate[i] = 0;
                            break;
                        case '2':
                            coordinate[i] = 1;
                            break;
                        case '3':
                            coordinate[i] = 2;
                            break;
                        case '4':
                            coordinate[i] = 3;
                            break;
                        case '5':
                            coordinate[i] = 4;
                            break;
                        case '6':
                            coordinate[i] = 5;
                            break;
                        case '7':
                            coordinate[i] = 6;
                            break;
                        case '8':
                            coordinate[i] = 7;
                            break;
                    }
                }
                HaveGo(new int[][] { new int[] { coordinate[0], coordinate[1] }, new int[] { coordinate[2], coordinate[3] } });
                textBox1.Text = "";
            }
            UpdatePieces();
            UpdateLabels();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Act();
        }
        private void UpdatePieces()
        {
            String player = "W";
            String opponent = "B";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) + @"\images\";
                    switch (history[index].board[j][i])
                    {
                        case 0:
                            boxes[j][i].Visible = false;
                            break;
                        case 1:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_P.png");
                            break;
                        case -1:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_P.png");
                            break;
                        case 2:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_B.png");
                            break;
                        case -2:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_B.png");
                            break;
                        case 3:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_N.png");
                            break;
                        case -3:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_N.png");
                            break;
                        case 4:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_R.png");
                            break;
                        case -4:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_R.png");
                            break;
                        case 5:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_Q.png");
                            break;
                        case -5:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_Q.png");
                            break;
                        case 6:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + player + "_K.png");
                            break;
                        case -6:
                            boxes[j][i].Visible = true;
                            boxes[j][i].Image = Image.FromFile(path + opponent + "_K.png");
                            break;
                    }
                }
            }
        }

        public float progress = 0;
        public State board;
        public void Initialise()
        {
            history.Clear();
            history.Add(State.startBoard);
        }
        public void HaveGo(int[][] coordinates)
        {
            bool divisible = (index % 2 == 0);
            if (coordinates == null)
            {
                List<State> nextBoards = history[index].GetNextBoards(divisible);
                List<float> scores = new List<float>();
                List<List<FutureMove>> evaluations = new List<List<FutureMove>>();
                for (int i = 0; i < nextBoards.Count; i++)
                {
                    int depth = 3;
                    if(index < 4)
                    {
                        depth = 4;
                    }
                    List<FutureMove> evaluation = StateMinimax(nextBoards[i], depth, !divisible, -1000, 1000);
                    scores.Add(evaluation[0].value);
                    progressBar1.Value = (int)Math.Floor(100f * i / (nextBoards.Count - 1));
                    evaluations.Add(evaluation);
                }
                List<List<FutureMove>> shortlist = new List<List<FutureMove>>();
                float target;
                if (divisible)
                {
                    target = scores.Max();
                    label4.Text = "White predicted future advantage: " + target;
                }
                else
                {
                    target = scores.Min();
                    label3.Text = "Black predicted future advantage: " + target;
                }
                for (int i = 0; i < evaluations.Count; i++)
                {
                    if (evaluations[i][0].value == target)
                    {
                        shortlist.Add(evaluations[i]);
                    }
                }
                Random generator = new Random();
                int moveIndex = generator.Next(0, shortlist.Count);
                history.Add(shortlist[moveIndex][shortlist[moveIndex].Count - 1].state);
                shortlist[moveIndex].Reverse();
                richTextBox1.Text += "Move " + (index + 1) + ":    " + PointsToMove(State.FindMove(history[index], shortlist[moveIndex][0].state, divisible)) + "\n{\n";
                for (int i = 0; i < shortlist[moveIndex].Count - 1; i++)
                {
                    richTextBox1.Text += "  " + PointsToMove(State.FindMove(shortlist[moveIndex][i].state, shortlist[moveIndex][i + 1].state, !Alternate(divisible, i))) + ", " + shortlist[moveIndex][i + 1].state.GetScore() + "\n ";
                }
                richTextBox1.Text += "}\n";
            }
            else
            {
                history.Add(State.ChangeState(history[index], coordinates[0][0], coordinates[0][1], coordinates[1][0] - coordinates[0][0], coordinates[1][1] - coordinates[0][1], history[index].board[coordinates[0][0]][coordinates[0][1]]));
                richTextBox1.Text += "Move " + (index + 1) + ":    " + PointsToMove(coordinates) + "\n";
            }
            index++;
        }
        
        public List<FutureMove> StateMinimax(State board, int depth, bool max, float alpha, float beta)
        {
            float maxEval;
            float minEval;
            if (depth == 0)
            {
                List<FutureMove> sequence = new List<FutureMove>();
                sequence.Add(new FutureMove(board, board.GetScore()));
                return sequence;

            }
            if (max)
            {
                maxEval = -1000;
                List<State> moves = board.GetNextBoards(max);
                List<FutureMove> moveSequence = new List<FutureMove>();
                for (int i = 0; i < moves.Count; i++)
                {
                    List<FutureMove> minimaxOutput = StateMinimax(moves[i], depth - 1, false, alpha, beta);
                    float eval = minimaxOutput[minimaxOutput.Count - 1].value;
                    if(eval > maxEval)
                    {
                        maxEval = eval;
                        moveSequence = minimaxOutput;
                    }
                    alpha = Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                moveSequence.Add(new FutureMove(board, maxEval));
                return moveSequence;
            }
            else
            {
                minEval = 1000;
                List<State> moves = board.GetNextBoards(max);
                List<FutureMove> moveSequence = new List<FutureMove>();
                for (int i = 0; i < moves.Count; i++)
                {
                    List<FutureMove> minimaxOutput = StateMinimax(moves[i], depth - 1, true, alpha, beta);
                    float eval = minimaxOutput[minimaxOutput.Count - 1].value;
                    if (eval < minEval)
                    {
                        minEval = eval;
                        moveSequence = minimaxOutput;
                    }
                    beta = Min(minEval, beta);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                moveSequence.Add(new FutureMove(board, minEval));
                return moveSequence;
            }
        }
        public List<FutureMove> StateMinimaxNoPrune(State board, int depth, bool max)
        {
            float maxEval;
            float minEval;
            if (depth == 0)
            {
                List<FutureMove> sequence = new List<FutureMove>();
                sequence.Add(new FutureMove(board, board.GetScore()));
                return sequence;

            }
            if (max)
            {
                maxEval = -1000;
                List<State> moves = board.GetNextBoards(max);
                List<FutureMove> moveSequence = new List<FutureMove>();
                if(moves.Count == 0)
                {
                    moveSequence.Add(new FutureMove(board, -1000));
                    return moveSequence;
                }
                for (int i = 0; i < moves.Count; i++)
                {
                    List<FutureMove> minimaxOutput = StateMinimaxNoPrune(moves[i], depth - 1, false);
                    float eval = minimaxOutput[minimaxOutput.Count - 1].value;
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        moveSequence = minimaxOutput;
                    }
                }
                moveSequence.Add(new FutureMove(board, maxEval));
                return moveSequence;
            }
            else
            {
                minEval = 1000;
                List<State> moves = board.GetNextBoards(max);
                List<FutureMove> moveSequence = new List<FutureMove>();
                if (moves.Count == 0)
                {
                    moveSequence.Add(new FutureMove(board, +1000));
                    return moveSequence;
                }
                for (int i = 0; i < moves.Count; i++)
                {
                    List<FutureMove> minimaxOutput = StateMinimax(moves[i], depth - 1, true, -100000, 1000);
                    float eval = minimaxOutput[minimaxOutput.Count - 1].value;
                    if (eval < minEval)
                    {
                        minEval = eval;
                        moveSequence = minimaxOutput;
                    }
                }
                moveSequence.Add(new FutureMove(board, minEval));
                return moveSequence;
            }
        }
        private static bool Alternate(bool value, int num)
        {
            for(int i = 0; i < num; i++)
            {
                value = !value;
            }
            return value;
        }
        public float Max(float a, float b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }
        private static float Min(float a, float b)
        {
            if (a < b)
            {
                return a;
            }
            return b;
        }
        

        private void Button2_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index--;
            }
            UpdatePieces();
            UpdateLabels();
        }
        void UpdateLabels()
        {
            label2.Text = "Move " + index + " of " + (history.Count - 1);
            label5.Text = "Evaluation: " + history[index].GetScore();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (index < history.Count - 1)
            {
                index++;
            }
            UpdatePieces();
            UpdateLabels();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boxes[i][j].BackColor = Color.Transparent;
                    boxes[i][j].Parent = pictureBox1;
                }
            }
        }
        private static String PointsToMove(int[][] coordinates)
        {
            return MoveCode(coordinates[0]) + MoveCode(coordinates[1]);
        }
        private static String MoveCode(int[] coordinates)
        {
            String move = "";
            switch (coordinates[0])
            {
                case 0:
                    move += "a";
                    break;
                case 1:
                    move += "b";
                    break;
                case 2:
                    move += "c";
                    break;
                case 3:
                    move += "d";
                    break;
                case 4:
                    move += "e";
                    break;
                case 5:
                    move += "f";
                    break;
                case 6:
                    move += "g";
                    break;
                case 7:
                    move += "h";
                    break;
            }
            move += (coordinates[1] + 1);
            return move;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            textBox1.Text += "a";
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length == 4)
            {
                Act();
            }
        }
    }
}
public class FutureMove
{
    public State state;
    public float value;
    public FutureMove(State state, float value)
    {
        this.state = state;
        this.value = value;
    }
}
public class State : IComparable<State>
{   // 1=P, 2=B, 3=N, 4=R, 5=Q, 6=K (White)
    public int[][] board;
    public bool[] castlingRight = new bool[] { true, true };
    public State Invert()
    {
        State inverted = new State(board);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                inverted.board[i][j] = board[7 - i][7 - j] * -1;
            }
        }
        return inverted;
    }
    
    
    public static int[][] FindMove(State state1, State state2, bool whiteMoved)
    {
        int[][] coordinates = new int[2][];
        coordinates[0] = new int[2];
        coordinates[1] = new int[2];
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                int difference = state2.board[i][j] - state1.board[i][j];
                if(difference != 0)
                {
                    if(whiteMoved)
                    {
                        if(difference < 0)
                        {
                            coordinates[0][0] = i;
                            coordinates[0][1] = j;
                        }
                        else
                        {
                            coordinates[1][0] = i;
                            coordinates[1][1] = j;
                        }
                    }
                    else
                    {
                        if (difference < 0)
                        {
                            coordinates[1][0] = i;
                            coordinates[1][1] = j;
                        }
                        else
                        {
                            coordinates[0][0] = i;
                            coordinates[0][1] = j;
                        }
                    }
                }
            }
        }
        return coordinates;
    }
    public float GetScore()
    {
        float[][] pawnValues = new float[][] { new float[] { 0, 0, 0, 0, 0, 0, 0, 0 }, new float[] { 0.5f, 1, 1, -2, -2, 1, 1, 0.5f }, new float[] { 0.5f, -0.5f, -0.4f, 0, 0, -0.4f, -0.5f, 0.5f }, new float[] { 0, 0, 0, 2, 2, 0, 0, 0 }, new float[] { 0.5f, 0.5f, 1, 2.5f, 2.5f, 1, 0.5f, 0.5f }, new float[] { 1, 1, 2, 3, 3, 2, 1, 1 }, new float[] { 5, 5, 5, 5, 5, 5, 5, 5 }, new float[] { 0, 0, 0, 0, 0, 0, 0, 0 } };
        float[][] knightValues = new float[][] { new float[] { -5, -4, -3, -3, -3, -3, -4, -5 }, new float[] { -4, -2, 0, 0.5f, 0.5f, 0, -2, -4 }, new float[] { -3, 0.5f, 1, 1.5f, 1.5f, 1, 0.5f, -3 }, new float[] { -3, 0, 1.5f, 2, 2, 1.5f, 0, -3 }, new float[] { -3, 0.5f, 1.5f, 2, 2, 1.5f, 0.5f, -3 }, new float[] { -3, 0, 1, 1.5f, 1.5f, 1, 0, -3 }, new float[] { -4, -2, 0, 0, 0, 0, -2, -4 }, new float[] { -5, -4, -3, -3, -3, -3, -4, -5 } };
        float[][] bishopValues = new float[][] { new float[] { -2, -1, -1, -1, -1, -1, -1, -2 }, new float[] { -1, 0.5f, 0, 0, 0, 0, 0.5f, -1 }, new float[] { -1, 1, 1, 1, 1, 1, 1, -1 }, new float[] { -1, 0, 1, 1, 1, 1, 0, -1 }, new float[] { -1, 0.5f, 0.5f, 1, 1, 0.5f, 1, -1 }, new float[] { -1, 0, 0.5f, 1, 1, 0.5f, 0, -1 }, new float[] { -1, 0, 0, 0, 0, 0, 0, -1 }, new float[] { -2, -1, -1, -1, -1, -1, -1, -2 } };
        float[][] queenValues = new float[][] { new float[] { -2, -1, -1, -0.5f, -0.5f, -1, -1, -2}, new float[] { -1, 0, 0.5f, 0, 0, 0.5f, 0, -1}, new float[] { -1, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, -1}, new float[] { 0, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, 0}, new float[] { -0.5f, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, -0.5f}, new float[] { -1, 0, 0.5f, 0.5f, 0.5f, 0.5f, 0, -1}, new float[] { -1, 0, 0, 0, 0, 0, 0, -1}, new float[] { -2, -1, -1, -0.5f, -0.5f, -1, -1, -2} };
        float[][] kingValues = new float[][] { new float[] { 5, 6, 0, -0.3f, -0.3f, 0, 6, 5}, new float[] { 2, 2, 0, 0, 0, 0, 2, 2}, new float[] { -1, -2, -2, -2, -2, -2, -2, -1}, new float[] { -2, -3, -3, -4, -4, -3, -3, -2}, new float[] { -3, -4, -4, -5, -5, -4, -4, -3}, new float[] { -3, -4, -4, -5, -5, -4, -4, -3}, new float[] { -3, -4, -4, -5, -5, -4, -4, -3}, new float[] { -3, -4, -4, -5, -5, -4, -4, -3} };
        float[][] rookValues = new float[][] { new float[] { 0, 0, 0, 0.5f, 0.5f, 0, 0, 0}, new float[] { -0.5f, 0, 0, 0, 0, 0, 0, -0.5f}, new float[] { -0.5f, 0, 0, 0, 0, 0, 0, -0.5f}, new float[] { -0.5f, 0, 0, 0, 0, 0, 0, -0.5f }, new float[] { -0.5f, 0, 0, 0, 0, 0, 0, -0.5f }, new float[] { -0.5f, 0, 0, 0, 0, 0, 0, -0.5f }, new float[] { 0.5f, 1, 1, 1, 1, 1, 1, 0.5f}, new float[] { 0, 0, 0, 0, 0, 0, 0, 0} };
        float total = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int player = 1;
                if (board[i][j] < 1)
                {
                    player = -1;
                }
                if (board[i][j] > 1)
                {
                    player = 1;
                }
                switch (Math.Abs(board[i][j]))
                {
                    case 0:
                        break;
                    case 1:
                        if(player == 1)
                        {
                            total += pawnValues[j][i] * 0.06f;
                        }
                        else
                        {
                            total -= pawnValues[7 - j][i] * 0.06f;
                        }
                        total += 1 * player;
                        break;
                    case 2:
                        if (player == 1)
                        {
                            total += bishopValues[j][i] * 0.02f;
                        }
                        else
                        {
                            total -= bishopValues[7 - j][i] * 0.02f;
                        }
                        total += 3 * player;
                        break;
                    case 3:
                        if (player == 1)
                        {
                            total += knightValues[j][i] * 0.02f;
                        }
                        else
                        {
                            total -= knightValues[7 - j][i] * 0.02f;
                        }
                        total += 3 * player;
                        break;
                    case 4:
                        if (player == 1)
                        {
                            total += rookValues[j][i] * 0.03f;
                        }
                        else
                        {
                            total -= rookValues[7 - j][i] * 0.03f;
                        }
                        total += 5 * player;
                        break;
                    case 5:
                        if (player == 1)
                        {
                            total += queenValues[j][i] * 0.01f;
                        }
                        else
                        {
                            total -= queenValues[7 - j][i] * 0.01f;
                        }
                        total += 9 * player;
                        break;
                    case 6:
                        total += 1000 * player;
                        if (player == 1)
                        {
                            total += kingValues[j][i] * 0.03f;
                        }
                        else
                        {
                            total -= kingValues[7 - j][i] * 0.03f;
                        }
                        break;
                }
            }
        }
        return total;
    }

    public List<State> GetNextBoards(bool whiteToMove)
    {
        if (whiteToMove)
        {
            return GetMoves();
        }
        else
        {
            List<State> blackMoves = Invert().GetMoves();
            for(int i = 0; i < blackMoves.Count; i++)
            {
                blackMoves[i] = blackMoves[i].Invert();
            }
            return blackMoves;
        }
    }
    //public static State startBoard = new State(new int[][] { new int[] { 4, 1, 0, 0, 0, 0, -1, -4 }, new int[] { 3, 1, 0, 0, 0, 0, -1, -3 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -2 }, new int[] { 6, 1, 0, 0, 0, 0, -1, -6 }, new int[] { 5, 1, 0, 0, 0, 0, -1, -5 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -2 }, new int[] { 3, 1, 0, 0, 0, 0, -1, -3 }, new int[] { 4, 1, 0, 0, 0, 0, -1, -4 } });
    public static State startBoard = new State(new int[][] { new int[] { 4, 1, 0, 0, 0, 0, -1, -4 }, new int[] { 3, 1, 0, 0, 0, 0, -1, -3 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -2 }, new int[] { 5, 1, 0, 0, 0, 0, -1, -5 }, new int[] { 6, 1, 0, 0, 0, 0, -1, -6 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -2 }, new int[] { 3, 1, 0, 0, 0, 0, -1, -3 }, new int[] { 4, 1, 0, 0, 0, 0, -1, -4 } });

    public static State testBoard2 = new State(new int[][] { new int[] { 0, 1, 0, 0, 0, 0, -1, -4 }, new int[] { 6, 0, 1, 0, 0, 0, -1, 0 }, new int[] { 0, 0, 5, 1, 0, 0, -1, -2 }, new int[] { 0, 0, 0, 4, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 1, 0, 0, -3, -4 }, new int[] { 2, 0, 1, 0, 0, 0, -1, -6 }, new int[] { 3, 1, 0, -5, 0, 0, -1, 0 }, new int[] { 4, 1, 0, 0, 0, 0, -1, 0 } });
    public static State testBoard = new State(new int[][] { new int[] { 4, 1, 0, 0, 0, 5, 1, 0 }, new int[] { 0, 1, 0, 0, 0, -1, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, -1, -3 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 6, 0, 0, 0, -1, 0, 0 }, new int[] { 2, 1, 0, 0, 0, 0, -1, -4 }, new int[] { 0, 1, 0, 0, 0, 0, -1, -6 }, new int[] { 4, 1, 0, 3, 0, -1, 0, 0 } });
    public State(int[][] board)
    {
        this.board = CopyArray(board);
    }
    public static State ChangeState(State state, int i, int j, int x, int y, int newPieceCode)
    {
        State newState = new State(state.board);
        newState.board[i + x][j + y] = newPieceCode;
        newState.board[i][j] = 0;
        return newState;
    }
    public static int[][] CopyArray(int[][] array)
    {
        int[][] newArray = new int[8][];
        for (int i = 0; i < 8; i++)
        {
            newArray[i] = new int[8];
            for (int j = 0; j < 8; j++)
            {
                newArray[i][j] = array[i][j];
            }
        }
        return newArray;
    }
    public List<State> GetMoves()
    {
        int numKings = 0;
        List<State> moves = new List<State>();
        int[][][] displacements = new int[2][][];
        displacements[0] = new int[][] { new int[] { 1, 2 }, new int[] { 2, 1 }, new int[] { 1, -2 }, new int[] { 2, -1 }, new int[] { -1, -2 }, new int[] { -2, -1 }, new int[] { -1, 2 }, new int[] { -2, 1 } };
        displacements[1] = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { -1, 0 }, new int[] { 1, 1 }, new int[] { 1, -1 }, new int[] { -1, 1 }, new int[] { -1, -1 } };
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int pieceCode = board[i][j];
                if (Math.Abs(board[i][j]) == 6)
                {
                    numKings++;
                }
                switch (pieceCode)
                {
                    case 1:
                        if (BoardSpaceExists(i, j, 1, 1))
                        {
                            if (IsEnemyOccupied(i, j, i + 1, j + 1))
                            {
                                if (j + 1 == 7)
                                {
                                    State promoted = new State(board);
                                    promoted.board[i + 1][7] = 5;
                                    promoted.board[i + 1][6] = 0;
                                    moves.Add(promoted);
                                }
                                else
                                {
                                    moves.Add(ChangeState(this, i, j, 1, 1, 1));
                                }
                            }
                        }
                        if (BoardSpaceExists(i, j, -1, 1))
                        {
                            if (IsEnemyOccupied(i, j, i - 1, j + 1))
                            {
                                if (j + 1 == 7)
                                {
                                    State promoted = new State(board);
                                    promoted.board[i - 1][7] = 5;
                                    promoted.board[i - 1][6] = 0;
                                    moves.Add(promoted);
                                }
                                else
                                {
                                    moves.Add(ChangeState(this, i, j, -1, 1, 1));
                                }
                            }
                        }
                        if (BoardSpaceExists(i, j, 0, 1))
                        {
                            if (!IsEnemyOccupied(i, j, i, j + 1) && IsCapturable(new int[] { board[i][j], board[i][j + 1] }))
                            {
                                if (j + 1 == 7)
                                {
                                    State promoted = new State(board);
                                    promoted.board[i][7] = 5;
                                    promoted.board[i][6] = 0;
                                    moves.Add(promoted);
                                }
                                else
                                {
                                    moves.Add(ChangeState(this, i, j, 0, 1, 1));
                                }
                            }
                        }
                        if (j == 1)
                        {
                            if (BoardSpaceExists(i, j, 0, 2))
                            {
                                if (!IsEnemyOccupied(i, j, i, j + 2) && IsCapturable(new int[] { board[i][j], board[i][j + 2] }) && !IsEnemyOccupied(i, j, i, j + 1) && IsCapturable(new int[] { board[i][j], board[i][j + 1] }))
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
                        for (int k = 0; k < 8; k++)
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
        if (board[7][0] == 4 && board[6][0] == 0 && board[5][0] == 0 && board[4][0] == 6 && castlingRight[0])
        {
            State castle = new State(board);
            castle.board[7][0] = 0;
            castle.board[6][0] = 6;
            castle.board[5][0] = 4;
            castle.board[4][0] = 0;
            moves.Add(castle);
        }
        if (board[0][0] == 4 && board[1][0] == 0 && board[2][0] == 0 && board[3][0] == 0 && board[4][0] == 6 && castlingRight[1])
        {
            State castle = new State(board);
            castle.board[0][0] = 0;
            castle.board[1][0] = 0;
            castle.board[2][0] = 6;
            castle.board[3][0] = 4;
            castle.board[4][0] = 0;
            moves.Add(castle);
        }
        if (board[0][0] == 4 && board[1][0] == 0 && board[2][0] == 0 && board[3][0] == 6 && castlingRight[0])
        {
            State castle = new State(board);
            castle.board[0][0] = 0;
            castle.board[1][0] = 6;
            castle.board[2][0] = 4;
            castle.board[3][0] = 0;
            moves.Add(castle);
        }
        if (board[7][0] == 4 && board[6][0] == 0 && board[5][0] == 0 && board[4][0] == 0 && board[3][0] == 6 && castlingRight[1])
        {
            State castle = new State(board);
            castle.board[7][0] = 0;
            castle.board[6][0] = 0;
            castle.board[5][0] = 6;
            castle.board[4][0] = 4;
            castle.board[3][0] = 0;
            moves.Add(castle);
        }
        if(numKings < 2)
        {
            moves.Clear();
        }
        return moves;
    }
    List<State> AddLegalMovesUntil(int i, int j, int x, int y, List<State> moves)
    {
        for (int k = 1; k < 8; k++)
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
            if (IsCapturable(new int[] { board[i][j], board[i + x][j + y] }))
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
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (coordinates[i][j] < 0 || coordinates[i][j] > 7)
                {
                    return false;
                }
            }
        }
        return true;
    }
    bool IsCapturable(int[] pieceCodes)
    {
        if (pieceCodes[0] > 0 && pieceCodes[1] > 0)
        {
            return false;
        }
        if (pieceCodes[0] < 0 && pieceCodes[1] < 0)
        {
            return false;
        }
        return true;
    }
    bool IsEnemyOccupied(int x, int y, int a, int b)
    {
        return (board[x][y] > 0 && board[a][b] < 0 || board[x][y] < 0 && board[a][b] > 0);
    }

    public int CompareTo(State other)
    {
        return (int) Math.Floor(other.GetScore() - GetScore());
    }
}