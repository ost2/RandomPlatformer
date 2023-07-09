using System;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace RandomPlatformer
{
    public class Writer
    {
        public static void output(String text, int emptyLines = 1, int sleepMS = 0, ConsoleColor Color = ConsoleColor.Gray)
        {
            for (int i = 0; i < emptyLines; i++)
            {
                Console.WriteLine();
                Thread.Sleep(sleepMS);
            }
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

