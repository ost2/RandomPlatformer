using System;
namespace RandomPlatformer
{
	public class Animation
	{
        //koble til incrementalgame hver ascension større verden.
        public List<AnimationFrame> AnimationFrames { get; set; }
		public int RefreshRate { get; set; }

		public Animation()
		{
			AnimationFrames = new List<AnimationFrame>();
		}

		public Animation(int framesNumber, int h, int w, AnimationFrame background)
		{
			AnimationFrames = new List<AnimationFrame>();
            for (int i = 0; i < framesNumber; i++)
            {
				AnimationFrames.Add(new AnimationFrame(background));
            }
        }

		public void addAnimationFrame(AnimationFrame frame, int frameNumber)
		{
			AnimationFrames[frameNumber] = frame;
		}

		public void playAnimation(int refreshRate, ConsoleColor color = ConsoleColor.White, int colorH = 0)
		{
			for (int i = 0; i < AnimationFrames.Count; i++)
			{
				AnimationFrames[i].printFrame(refreshRate, color, colorH);
			}
		}
	}
}

