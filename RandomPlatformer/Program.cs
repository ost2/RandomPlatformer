namespace RandomPlatformer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var rand = new Random();
            var app = new Application(5, 80, true);
            var enemy = new GameObject(app, '*', 1, ConsoleColor.Red, false, true, true);
            var enemyNum = 10;
            for (int i = 0; i < enemyNum; i++)
            {
                var obj = new GameObject(enemy, rand.Next(2, app.ScreenHeight - 2), rand.Next(2, app.ScreenWidth - 2));
                app.addGameObject(obj);
            }

            app.runGame();
        }
    }
}

