namespace RandomPlatformer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var app = new Application(5, 80, true, 120, 10);

            app.runGame();
        }
    }
}

