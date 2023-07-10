using System;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace RandomPlatformer
{
    public class Writer
    {
        public static void output(string text, int emptyLines = 1, int sleepMS = 0, ConsoleColor color1 = ConsoleColor.Gray, ConsoleColor color2 = ConsoleColor.Gray, ConsoleColor color3 = ConsoleColor.Gray)
        {
            for (int i = 0; i < emptyLines; i++)
            {
                Console.WriteLine();
                Thread.Sleep(sleepMS);
            }

            Console.ForegroundColor = color1;
            //foreach (var c in text)
            //{
            //    Console.Write(String.Format("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", c));
            //}
            //return;
            var splitText = text.Split("Å");
            Console.WriteLine("");
            Console.Write(new string(' ', (Console.WindowWidth - (text.Length - (splitText.Count() - 1))) / 2));
            int j = 0;
            var colors = new List<ConsoleColor> { color1, color2, color3 };
            for (int g = 0; g < splitText.Count(); g++)
            {
                foreach (var c in splitText[g])
                {
                    Console.ForegroundColor = colors[g];
                    if (c != 'Å') { Console.Write(c); }
                    j++;
                }
            }

            //Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text));

        }
    }
}

