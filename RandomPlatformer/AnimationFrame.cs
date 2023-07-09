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

        public void addVerticalLine(int w, char symbol)
        {
            for (int i = 0; i < Height; i++)
            {
                AllLines[i][w] = symbol;
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

        public void printFrame(int ms, ConsoleColor drawColor = ConsoleColor.White, int colorH = 0, bool clearFrame = true)
        {
            if (clearFrame)
            {
                Console.Clear();
            }

            ConsoleColor color;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    foreach (GameObject wall in App.WallObjects)
                    {
                        if (wall.yPos == i && wall.xPos == j)
                        {
                            Console.ForegroundColor = wall.ObjectColor;
                            //Console.BackgroundColor = gameObject.ObjectColor;
                        }
                    }
                    foreach (GameObject gameObject in App.GameObjects)
                    {
                        if (gameObject.yPos == i && gameObject.xPos == j)
                        {
                            Console.ForegroundColor = gameObject.ObjectColor;
                            //Console.BackgroundColor = gameObject.ObjectColor;
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

        public void addSymbolImage(SymbolImage image, int h, int w)
        {
            int i = h;
            foreach (string s in image.FullImage)
            {
                int j = w;
                foreach (char c in s)
                {
                    addPixel(i, j, c);
                    j++;
                }
                i++;
            }
        }
    }
}

