using System;
namespace RandomPlatformer
{
    public class Application
    {
        public AnimationFrame GameBoard { get; set; }
        public GameLevel GameLevel { get; set; }
        public List<GameObject> GameObjects { get; set; }
        public List<GameObject> WallObjects { get; set; }

        public GameObject PlayerChar { get; set; }
        public GameObject NullObject { get; set; }

        public DateTime LastRefresh { get; set; }
        public TimeSpan GameTimer { get; set; }

        public Random Rand { get; set; }

        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public int RefreshRate { get; set; }
        public int ObjectUpdateLimit { get; set; }

        public bool HasPlayer { get; set; }

        public Application(int updateLimit, int refreshRate, bool hasPlayer = false)
        {
            Rand = new Random();
            ScreenHeight = Console.WindowHeight - 1;
            ScreenWidth = Console.WindowWidth;
            GameObjects = new List<GameObject>();
            WallObjects = new List<GameObject>();
            WallObj = new GameObject(this, PlatformChar, 0, ConsoleColor.Gray, true);
            HasPlayer = hasPlayer;
            RefreshRate = refreshRate;
            ObjectUpdateLimit = updateLimit;
            
            if (HasPlayer)
            {
                PlayerChar = new GameObject(this, 'O');
                PlayerChar.ObjectColor = ConsoleColor.Cyan;
                PlayerChar.yPos = ScreenHeight - 5;
                PlayerChar.xPos = ScreenWidth / 2;
                PlayerChar.MovementSpeed = 1;
                addGameObject(PlayerChar);
            }
        }
        void createLevel()
        {
            var pixelNum = (ScreenHeight * ScreenWidth) / 5;
            var platformNum = (ScreenHeight * ScreenWidth / 82);
            GameLevel = new GameLevel(this);
            GameLevel.generateLevelPlatforms(ScreenHeight, ScreenWidth, platformNum);
            GameBoard = new AnimationFrame(ScreenHeight, ScreenWidth, this);
            loadLevelLayout();
        }

        public void runGame()
        {
            createLevel();
            LastRefresh = DateTime.Now;
            runGameLoop();
        }
        public int EnemyMoveTimer { get; set; } = 0;

        void runGameLoop()
        {
            while (true)
            {
                parseInput();

                if ((DateTime.Now - LastRefresh).TotalMilliseconds >= RefreshRate)
                {
                    LastRefresh = DateTime.Now;
                    GameTimer += (new TimeSpan(0, 0, 0, 0, RefreshRate));

                    handleEnemyMovement();
                    handleJump();

                    updatePlayerPosition();
                    generateFrame();
                    drawFrame();
                }
            }
        }

        void handleEnemyMovement()
        {
            EnemyMoveTimer++;
            if (EnemyMoveTimer == 10)
            {
                selectObjectsToMove();
                updateEnemyPositions();
                EnemyMoveTimer = 0;
            }
        }
        void handleJump()
        {
            if (PlayerChar.IsInAir && PlayerChar.isOnGround())
            {
                PlayerChar.IsInAir = false; handleLanding();
            }
            if (PlayerChar.JumpTimer > 0)
            {
                PlayerChar.JumpTimer--;
            }
            if (PlayerChar.JumpTimer == 0)
            {
                PlayerChar.setObjectMovement(1, PlayerChar.xMovement);
            }

            var yMove = Math.Clamp(PlayerChar.yMovement, -100, 0);
            if (PlayerChar.isOnGround())
            {
                PlayerChar.setObjectMovement(yMove, PlayerChar.xMovement);
            }
            if (!PlayerChar.isOnGround())
            {
                if (!PlayerChar.IsInAir) { PlayerChar.setObjectMovement(yMove, PlayerChar.xMovement); }
                PlayerChar.IsInAir = true;
            }
        }
        void handleLanding()
        {
            PlayerChar.setObjectMovement(PlayerChar.yMovement, 0);
        }

        public void addGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
        }

        ConsoleKey takeInput()
        {
            return Console.ReadKey(true).Key;
        }

        public void parseInput()
        {
            if (Console.KeyAvailable && HasPlayer)
            {
                switch (takeInput())
                {
                    case ConsoleKey.W:
                        if (!PlayerChar.IsInAir)
                        { PlayerChar.performJump(); }
                        break;
                    case ConsoleKey.A:
                        PlayerChar.setObjectMovement(PlayerChar.yMovement, -PlayerChar.MovementSpeed);
                        break;
                    case ConsoleKey.S:
                        PlayerChar.setObjectMovement(PlayerChar.MovementSpeed, 0);
                        break;
                    case ConsoleKey.D:
                        PlayerChar.setObjectMovement(PlayerChar.yMovement, PlayerChar.MovementSpeed);
                        break;
                    case ConsoleKey.Spacebar:
                        if (!PlayerChar.IsInAir)
                        { PlayerChar.performJump(); }
                        break;
                    default:
                        break;
                }
            }
        }
        

        public void updateObjectMovement(GameObject gameObject)
        {
            //if (gameObject == PlayerChar)
            //{
            //    parseInput();
            //}

            if (gameObject.AvoidsNearest)
            {
                gameObject.setMovementAwayFromObject(gameObject.nearestNotRunner());
            }

            else if (gameObject.FollowsNearest)
            {
                gameObject.setMovementTowardsObject(gameObject.nearestNotFollower());
            }

            else if (gameObject.FollowsPlayer)
            {
                gameObject.setMovementTowardsObject(PlayerChar);
            }
        }

        public void selectObjectsToMove()
        {
            foreach (var obj in GameObjects) { updateObjectMovement(obj); }
            //for (int i = 0; i < ObjectUpdateLimit; i++)
            //{
            //    updateObjectMovement(GameObjects[Rand.Next(0, GameObjects.Count - 1)]);
            //}
        }

        public void updatePlayerPosition()
        {
            PlayerChar.collisionCheck();
            PlayerChar.OOBCheck();
            PlayerChar.moveObject(PlayerChar.yMovement, PlayerChar.xMovement);
        }
             
        public void updateEnemyPositions()
        {
            {
                foreach (GameObject gameObject in GameObjects)
                {
                    if (gameObject != PlayerChar)
                    {
                        gameObject.collisionCheck();
                        gameObject.OOBCheck();
                        gameObject.moveObject(gameObject.yMovement, gameObject.xMovement);
                        gameObject.setObjectMovement(0, 0);
                    }
                }
            }
        }

        public void playerLose()
        {
            Console.Clear();
            Writer.output("YOU DIED");
            Writer.output("Survived " + GameTimer.TotalSeconds + "s");
            Console.ReadKey();
        }
        public GameObject WallObj { get; set; } 
        public char PlatformChar { get; set; } = '‾';
        public char PlatformEdgeChar { get; set; } = '+';
        void loadLevelLayout()
        {
            for (int i = 0; i < ScreenHeight; i++)
            {
                for (int j = 0; j < ScreenWidth; j++)
                {
                    if (GameLevel.LevelLayout.AllLines[i][j] == PlatformChar)
                    {
                        var wall = new GameObject(WallObj, i, j);
                        wall.ObjectColor = ConsoleColor.DarkMagenta;
                        WallObjects.Add(wall);
                    }
                    if (GameLevel.LevelLayout.AllLines[i][j] == PlatformEdgeChar)
                    {
                        var wall = new GameObject(WallObj, i, j);
                        wall.Symbol = PlatformEdgeChar;
                        wall.ObjectColor = ConsoleColor.Yellow;
                        WallObjects.Add(wall);
                    }
                }
            }
        }

        public void generateFrame()
        {
            GameBoard.clearFrame();
            foreach (var wall in WallObjects)
            {
                GameBoard.addPixel(wall.yPos, wall.xPos, wall.Symbol);
            }
            foreach (GameObject gameObject in GameObjects)
            {
                GameBoard.addPixel(gameObject.yPos, gameObject.xPos, gameObject.Symbol);
            }

        }

        public void drawFrame()
        {
            GameBoard.printFrame(0);
        }
    }
}

