using System;
namespace RandomPlatformer
{
	public class GameLevel
	{
		public Application App { get; set; }
		public AnimationFrame LevelLayout { get; set; }

		public void generateRandomLevel(int h, int w, int pixelNum, char symbol)
		{
			var level = new AnimationFrame(h, w, App);
			var rand = new Random();
			for (int i = 0; i < pixelNum; i++)
			{
				var y = rand.Next(2, h - 2);
				var x = rand.Next(2, w - 2);
				level.addPixel(y, x, symbol);
			}
			LevelLayout = level;
		}

		public void generateLevelPlatforms(int h, int w, int platformNum)
		{
            var level = new AnimationFrame(h, w, App);
            var rand = new Random();
			
            for (int i = 0; i < platformNum; i++)
            {
				var length = rand.Next(0, 8);
				var pString = "" + App.PlatformEdgeChar + App.PlatformChar;
				for (int k = 0; k < length; k++) { pString += App.PlatformChar; }
				pString += App.PlatformEdgeChar;
                var platform = new SymbolImage(pString);
                var y = rand.Next(2, h - 2);
                var x = rand.Next(2, w - 10);
                level.addSymbolImage(platform, y, x);
            }
			level.addHorizontalLine(h - 1, App.PlatformChar);
            LevelLayout = level;
        }
		private void checkLevelPossible()
		{

		}
		public GameLevel(Application app)
		{
			App = app;
		}
	}
}

