using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ConsoleGSwitch
{
    class Program
    {
        static char[,] board;
        static int playerRow;
        static int playerCol;
        static string gravity = "floor";
        static int rows = 10;
        static int cols = 20;
        static Random rand = new Random();
        static List<(int, int)> obstacles = new List<(int, int)>();
        static int score = 0;
        static Stopwatch gameTimer = new Stopwatch();

        static void Main(string[] args)
        {
            bool playAgain = true;

            while (playAgain)
            {
                InitializeBoard();
                PlaceObstacles();
                PlaceGoal();
                playerRow = gravity == "floor" ? rows - 1 : 0;
                playerCol = 0;

                PrintBoard();
                Console.WriteLine("Press SPACEBAR to start the game or Q to quit...");


                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Spacebar)
                        {
                            gameTimer.Start();
                            PlayGame();
                            break;
                        }
                        else if (key.Key == ConsoleKey.Q)
                        {
                            playAgain = false;
                            break;
                        }
                    }
                }

                if (playAgain)
                {
                    Console.WriteLine("Press R to retry or Q to quit...");
                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.R)
                            {
                                score = 0;
                                gameTimer.Reset();
                                obstacles.Clear();
                                break;
                            }
                            else if (key.Key == ConsoleKey.Q)
                            {
                                playAgain = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        static void InitializeBoard()
        {
            board = new char[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    board[i, j] = ' ';
                }
            }
        }

        static void PlaceObstacles()
        {
            for (int i = 0; i < 10; i++)
            {
                int row = rand.Next(rows);
                int col = rand.Next(cols - 1);
                obstacles.Add((row, col));
                board[row, col] = '#';
            }
        }

        static void PlaceGoal()
        {
            for (int i = 0; i < rows; i++)
            {
                board[i, cols - 1] = 'G';
            }
        }

        static void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine("How to play:");
            Console.WriteLine("W: Switch Gravity");
            Console.WriteLine($"Score: {score} | Time: {gameTimer.Elapsed.TotalSeconds:F1} seconds");
            Console.WriteLine();


            Console.WriteLine(new string('-', cols * 2) + " Ceiling");

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (i == playerRow && j == playerCol)
                        Console.Write("P ");
                    else
                        Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }


            Console.WriteLine(new string('-', cols * 2) + " Floor");

            Console.WriteLine($"Gravity: {gravity}");
        }

        static void PlayGame()
        {
            while (true)
            {
                PrintBoard();
                Thread.Sleep(500); 

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.W) 
                    {
                        gravity = (gravity == "floor") ? "ceiling" : "floor";
                        playerRow = gravity == "floor" ? rows - 1 : 0;
                    }
                }

                MovePlayer(0, 1); 

                MoveObstacles();
                score++;

                if (playerRow < 0 || playerRow >= rows || playerCol < 0 || playerCol >= cols)
                {
                    Console.WriteLine("Game Over! You moved out of bounds.");
                    break;
                }

                if (board[playerRow, playerCol] == '#')
                {
                    Console.WriteLine("Game Over! You hit an obstacle.");
                    break;
                }

                if (board[playerRow, playerCol] == 'G')
                {
                    Console.WriteLine($"Congratulations! You reached the goal. Final Score: {score}");
                    break;
                }
            }

            gameTimer.Stop();
        }

        static void MovePlayer(int dx, int dy)
        {
            int newRow = playerRow + dx;
            int newCol = playerCol + dy;

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                playerRow = newRow;
                playerCol = newCol;
            }
        }

        static void MoveObstacles()
        {
            foreach (var obstacle in obstacles.ToList())
            {
                int newRow = obstacle.Item1 + rand.Next(-1, 2); 
                int newCol = obstacle.Item2 + rand.Next(-1, 2); 

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && board[newRow, newCol] != '#' && board[newRow, newCol] != 'G')
                {
                    board[obstacle.Item1, obstacle.Item2] = ' ';
                    obstacles.Remove(obstacle);
                    obstacles.Add((newRow, newCol));
                    board[newRow, newCol] = '#'; 
                }
            }
        }
    }
}
