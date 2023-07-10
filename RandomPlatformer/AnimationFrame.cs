using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RandomPlatformer
{
    public struct AnimationFrame
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public char[][] AllLines { get; set; }
        public Application App { get; set; }


        public AnimationFrame(int height, int width, Application app = null)
        {
            Height = height;
            Width = width;
            AllLines = new char[Height][];
            App = app;

            for (int i = 0; i < height; i++)
            {
                AllLines[i] = (new char[Width]);

                for (int j = 0; j < Width; j++)
                {
                    AllLines[i][j] = ' ';
                }
            }
        }

        public AnimationFrame(AnimationFrame frame)
        {
            Height = frame.Height;
            Width = frame.Width;
            AllLines = new char[Height][];

            for (int i = 0; i < Height; i++)
            {
                AllLines[i] = (new char[Width]);

                for (int j = 0; j < Width; j++)
                {
                    AllLines[i][j] = frame.getPixel(i, j);
                }
            }
        }

        char getPixel(int h, int w)
        {
            return AllLines[h][w];
        }

        public void addPixel(int h, int w, char symbol)
        {
            AllLines[h][w] = symbol;
        }

        public void addHorizontalLine(int h, char symbol, int shortenBy = 0, char left = ' ', char right = ' ')
        {
            for (int i = shortenBy; i < Width - shortenBy; i++)
            {
                if (i == shortenBy)
                {
                    AllLines[h][i] = left;
                }
                else if (i == Width - shortenBy - 1)
                {
                    AllLines[h][i] = right;
                }
                else
                {
                    AllLines[h][i] = symbol;
                }
            }
        }

        public void addVerticalLine(int w, char symbol, int shortenBy = 0, char bottom = ' ', char top = ' ')
        {
            for (int i = shortenBy; i < Height - shortenBy; i++)
            {
                if (i == shortenBy)
                {
                    AllLines[i][w] = bottom;
                }
                else if (i == Height - shortenBy - 1)
                {
                    AllLines[i][w] = top;
                }
                else
                {
                    AllLines[i][w] = symbol;
                }
            }
        }

        public void clearFrame()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    AllLines[i][j] = ' ';
                }
            }
        }
        void setWallColors()
        {
            for (int i = 0; i < Height; i++)
            {
                platColors.Add(new List<ConsoleColor>(new ConsoleColor[Width]));
            }
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    platColors[i][j] = ConsoleColor.DarkMagenta;
                }
                
            }
            foreach (GameObject wall in App.WallObjects)
            {
                platColors[wall.yPos][wall.xPos] = wall.ObjectColor;
            }
        }

        List<List<ConsoleColor>> platColors = new List<List<ConsoleColor>>();
        public void printFrame(int ms, ConsoleColor drawColor = ConsoleColor.White, int colorH = 0, bool clearFrame = true)
        {
            if (platColors.Count == 0)
            {
                setWallColors();
            }
            if (clearFrame)
            {
                Console.Clear();
            }

            ConsoleColor color;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Console.ForegroundColor = platColors[i][j];
                    foreach (var gameObject in App.GameObjects)
                    {
                        if (gameObject.yPos == i && gameObject.xPos == j)
                        {
                            Console.ForegroundColor = gameObject.ObjectColor;
                        }
                    }
                    foreach (var col in App.CollectableObjects)
                    {
                        if (col.yPos == i && col.xPos == j)
                        {
                            Console.ForegroundColor = col.ObjectColor;
                        }
                    }
                    if (App.IsPaused)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        foreach (var pos in App.PauseTextPixels)
                        {
                            if (pos[0] == i && pos[1] == j)
                            { Console.ForegroundColor = App.PauseColor; }
                        }
                    }
                    Console.Write(AllLines[i][j]);
                    Console.ResetColor();
                }
                //string pixelRow = new string(AllLines[i].ToArray());
                //Writer.output(pixelRow, 0, 0, color);
            }
            Thread.Sleep(ms);
        }

        public List<List<int>> addSymbolImage(SymbolImage image, int h, int w)
        {
            var pixels = new List<List<int>>();
            int i = h;
            foreach (string s in image.FullImage)
            {
                int j = w;
                foreach (char c in s)
                {
                    pixels.Add(new List<int> { i, j });
                    addPixel(i, j, c);
                    j++;
                }
                i++;
            }
            return pixels;
        }
    }
}

