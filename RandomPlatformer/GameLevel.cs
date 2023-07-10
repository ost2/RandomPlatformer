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
			
            for (int i = 0; i < platformNum;)
            {
				var length = 0;
				while (length == 0)
				{
					var val = rand.Next(5, 11);
					if (val % 2 != 0) { length = val; }
				}

				var pString = "" + App.PlatformEdgeChar;
				for (int k = 1; k < length + 1; k++)
				{
					if (k + (k - 1) == length || length == 1) { pString += App.PlatformCenterChar; }
					else { pString += App.PlatformChar; }					
				}
                pString += App.PlatformEdgeChar;
                var platform = new SymbolImage(pString);
                var y = rand.Next(5, h - 5);
                var x = rand.Next(2, w - 12);

				var canPlace = true;
				foreach (var plat in App.PlatformPositions)
				{
					foreach (var pixel in plat)
					{
						if (!App.isFarAway(pixel[0], pixel[1], y, x + length / 2, 4, 4))
						{
							canPlace = false;
						}
					}
				}
				if (canPlace)
				{
					App.PlatformPositions.Add(level.addSymbolImage(platform, y, x));
					i++;
				}
            }
            level.addHorizontalLine(h - 2, App.PlatformChar);
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

