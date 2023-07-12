using System;
using System.Reflection.Emit;

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

        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public int RefreshRate { get; set; }
        public int ObjectUpdateLimit { get; set; }

        public int LevelNumber { get; set; } = 1;

        public bool HasPlayer { get; set; }


        Random rand = new Random();

        public Application(int updateLimit, int refreshRate, bool hasPlayer = false, int spawnEnemy = 100, int moveEnemy = 10)
        {
            ScreenHeight = Console.WindowHeight - 9;
            ScreenWidth = Console.WindowWidth;
            GameObjects = new List<GameObject>();
            WallObjects = new List<GameObject>();
            WallObj = new GameObject(this, PlatformChar, 0, ConsoleColor.Gray, true);
            HasPlayer = hasPlayer;
            RefreshRate = refreshRate;
            ObjectUpdateLimit = updateLimit;
            DefEnemyMoveTime = moveEnemy;
            DefEnemySpawnTime = spawnEnemy;
            EnemySpawnTime = spawnEnemy;
            EnemyMoveTime = moveEnemy;
            PlatformNumber = DefPlatformNumber;
        }
        public int PlatformNumber { get; set; }
        public int DefPlatformNumber { get; set; } = Console.WindowHeight * Console.WindowWidth / 120;
        void createLevel()
        {
            GameLevel = new GameLevel(this);
            GameLevel.generateLevelPlatforms(ScreenHeight, ScreenWidth, PlatformNumber);
            GameBoard = new AnimationFrame(ScreenHeight, ScreenWidth, this);
            loadLevelLayout();
        }

        public int DefJumpHeight { get; set; } = 9;
        public int DefEnemySpawnTime { get; set; }
        public int DefEnemyMoveTime { get; set; }
        public void runGame()
        {
            createLevel();
            LastRefresh = DateTime.Now;
            KillerPos = new int[2] { 0, 0 };
            if (HasPlayer)
            {
                PlayerChar = new GameObject(this, '∆');
                PlayerChar.ObjectColor = ConsoleColor.DarkCyan;
                PlayerChar.yPos = ScreenHeight - 5;
                PlayerChar.xPos = ScreenWidth / 2;
                PlayerChar.MovementSpeed = 1;
                PlayerChar.JumpHeight = DefJumpHeight;
                addGameObject(PlayerChar);
            }
            PlayerDead = false;
            CollectableNumber = 2;
            EnemySpawnTimer = 0;
            runGameLoop();
        }

        public bool IsPaused { get; set; } = false;
        void runGameLoop()
        {
            while (true)
            {
                parseInput();
                if ((DateTime.Now - LastRefresh).TotalMilliseconds >= RefreshRate && !IsPaused)
                {
                   
                    LastRefresh = DateTime.Now;
                    GameTimer += (new TimeSpan(0, 0, 0, 0, RefreshRate));

                    handleCollectables();
                    handleEnemyMovement();
                    handleJump();

                    updatePlayerPosition();
                    generateFrame();
                    drawFrame();

                    if (WinConditionMet)
                    {
                        playerWin();
                    }
                    if (PlayerDead)
                    {
                        playerLose();
                    }
                }


            }
        }

        public int CollectableNumber { get; set; }
        public int LastCollectedNumber { get; set; } = 1;
        public int FlickerTimer { get; set; } = 0;
        public int TotalCollected { get; set; } = 0;

        public char CollectableSymbol { get; set; }
        public ConsoleColor CollectableColor { get; set; }
        public List<GameObject> CollectableObjects { get; set; } = new List<GameObject>();

        public int WinCondition { get { return 6 + (LevelNumber * 2); } }
        public bool WinConditionMet { get { if (TotalCollected >= WinCondition) { return true; } else return false; } }
        public bool MatchPoint { get { if (TotalCollected == WinCondition - 1) return true; else return false; } }
        void handleCollectables()
        {
            if (TotalEnemies / 3 > CollectableNumber)
            {
                CollectableNumber++;
            }
            if (MatchPoint)
            {
                CollectableNumber = 1;
            }
            handleFlickerAnimations();
            if (LastCollectedNumber > 0)
            {
                clearCollectables();
                spawnCollectables();
                LastCollectedNumber = 0;
            }
        }

        void clearCollectables()
        {
            CollectableObjects.Clear();
        }
        void spawnCollectables()
        {
            var collectable = new GameObject(this, CollectableSymbol, 0, CollectableColor, false); collectable.IsCollectable = true;

            for (int i = 0; i < CollectableNumber; i++)
            {
                var pos = getCollectablePos();
                var obj = new GameObject(collectable, pos[0], pos[1]);
                CollectableObjects.Add(obj);
            }
        }

        public List<List<List<int>>> PlatformPositions { get; set; } = new List<List<List<int>>>();
        int[] getCollectablePos()
        {
            int[] pos;
            while (true)
            {
                var centerPos = CenterPoints[rand.Next(0, CenterPoints.Count)];
                if (!isSolidGround(centerPos.yPos - 1, centerPos.xPos) && isFarAway(PlayerChar, centerPos.yPos, centerPos.xPos, 15, 15))
                {
                    var canPlace = true;
                    foreach (var col in CollectableObjects)
                    {
                        if (!isFarAway(col, centerPos.yPos - 1, centerPos.xPos, ScreenHeight / CollectableNumber, ScreenWidth / (CollectableNumber * 2)))
                        {
                            canPlace = false;
                        }
                    }
                    if (canPlace)
                    {
                        pos = new int[2] { centerPos.yPos - 1, centerPos.xPos };
                        return pos;
                    }
                }
            }
        }
        public bool isFarAway(GameObject obj, int y, int x, int dy, int dx)
        {
            if (Math.Abs(y - obj.yPos) > dy || Math.Abs(x - obj.xPos) > dx)
            {
                return true;
            }
            else return false;
        }
        public bool isFarAway(int sy, int sx, int y, int x, int dy, int dx)
        {
            if (Math.Abs(y - sy) > dy || Math.Abs(x - sx) > dx)
            {
                return true;
            }
            else return false;
        }

        public void collectPoint()
        {
            LastCollectedNumber++;
            TotalCollected++;
            PlayerChar.HasSuperJump = true;
        }
        void handleFlickerAnimations()
        {
            if (FlickerTimer == 0 || FlickerTimer == 1)
            {
                FlickerTimer++;

                if (MatchPoint)
                {
                    CollectableSymbol = '◊';
                    CollectableColor = ConsoleColor.Green;
                }
                else
                {
                    CollectableSymbol = '•';
                    CollectableColor = ConsoleColor.Green;
                }

                PlayerChar.ObjectColor = ConsoleColor.DarkCyan;
                if (PlayerChar.HasSuperJump)
                {
                    PlayerChar.ObjectColor = ConsoleColor.Cyan;
                }
                foreach (var obj in GameObjects)
                {
                    if (obj.IsHostile)
                    {
                        obj.Symbol = '+';
                    }
                }
            }
            else if (FlickerTimer == 2 || FlickerTimer == 3)
            {
                FlickerTimer++;
                if (FlickerTimer == 4)
                { FlickerTimer = 0; }

                if (MatchPoint)
                {
                    CollectableColor = ConsoleColor.DarkGreen;
                }
                else
                {
                    CollectableColor = ConsoleColor.DarkGreen;
                    CollectableSymbol = '°';
                }

                if (PlayerChar.HasSuperJump)
                {
                    PlayerChar.ObjectColor = ConsoleColor.Magenta;
                }
                foreach (var obj in GameObjects)
                {
                    if (obj.IsHostile)
                    {
                        obj.Symbol = '*';
                    }
                }
            }

            foreach (var col in CollectableObjects)
            {

                col.Symbol = CollectableSymbol;
                col.ObjectColor = CollectableColor;
            }
        }

        public int EnemyMoveTimer { get; set; }
        public int EnemySpawnTimer { get; set; }

        public int EnemyMoveTime { get; set; }
        public int EnemySpawnTime { get; set; }

        public int TotalEnemies { get; set; } = 0;

        void handleEnemyMovement()
        {
            EnemyMoveTimer++;
            EnemySpawnTimer++;
            if (EnemyMoveTimer >= EnemyMoveTime)
            {
                selectObjectsToMove();
                updateEnemyPositions();
                EnemyMoveTimer = 0;
            }
            if (EnemySpawnTimer >= EnemySpawnTime)
            {
                int y = 0;
                int x = 0;
                while (y == 0 || x == 0)
                {
                    var pY = rand.Next(3, ScreenHeight - 2);
                    var pX = rand.Next(2, ScreenWidth - 2);

                    if (Math.Abs(pY - PlayerChar.yPos) > 15) { y = pY; }
                    if (Math.Abs(pX - PlayerChar.xPos) > 15) { x = pX; }
                }
                var enemy = new GameObject(this, '*', 1, ConsoleColor.Red, false, true, true);
                var obj = new GameObject(enemy, y, x);
                addGameObject(obj);
                EnemySpawnTimer = 0;
                TotalEnemies++;
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
                    case ConsoleKey.K:
                        resetGame(false);
                        break;
                    case ConsoleKey.P:
                    case ConsoleKey.Enter:
                    case ConsoleKey.E:
                        pauseGame();
                        break;
                    case ConsoleKey.L:
                        if (!IsPaused)
                        PlayerDead = true;
                        break;
                    case ConsoleKey.O:
                        if (!IsPaused)
                        TotalCollected = WinCondition;
                        break;
                    case ConsoleKey.B:
                        PlayLevelBuildAnimation = !PlayLevelBuildAnimation;
                        break;
                    default:
                        break;
                }
            }
        }
        void pauseGame()
        {
            IsPaused = !IsPaused;
            generateFrame();
            drawFrame();
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
        public bool isSolidGround(int y, int x)
        {
            if (
                GameBoard.AllLines[y][x] == PlatformChar ||
                GameBoard.AllLines[y][x] == PlatformEdgeChar ||
                GameBoard.AllLines[y][x] == PlatformCenterChar
                )
            {
                return true;
            }
            else return false;
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

        public void playerWin()
        {
            waitForKey(ConsoleKey.Enter, ConsoleKey.E);
            resetGame(true);
        }

        public bool PlayerDead { get; set; }
        public int[] KillerPos { get; set; }
        public void playerLose()
        {
            waitForKey(ConsoleKey.Enter, ConsoleKey.E);
            resetGame(false);
        }

        public bool IsIntro { get; set; } = false;
        string[] getTopBotText()
        {
            var text = new string[2];
            text[1] = "    ‹« Enter to Continue »›";
            if (PlayerDead)
            {
                text[0] = "      .:[ -YOU  DIED- ]:.";
            }
            else if (WinConditionMet)
            {
                text[0] = "      *±{ ~YOU * WIN~ }±*";
            }
            else if (IsPaused)
            {
                text[0] = "      ~≈{ • RESPITE • }≈~";    
            }
            else if (IsIntro)
            {
                text[0] = "                         ";
                text[1] = "                         ";
            }

            return text;
        }

        void waitForKey(ConsoleKey key, ConsoleKey key2 = ConsoleKey.NoName)
        {
            ConsoleKey keyOpt;
            if (key2 == ConsoleKey.NoName)
            {
                keyOpt = key;
            }
            else
            {
                keyOpt = key2;
            }
            var pressed = false;
            while (!pressed)
            {
                var read = Console.ReadKey().Key;
                if (read == key || read == keyOpt) { pressed = true; }
            }
        }


        void resetGame(bool win)
        {
            SymCol = ConsoleColor.Magenta;
            TextCol = ConsoleColor.Yellow;
            TotalCollected = 0;
            LastCollectedNumber = 1;
            GameObjects.Clear();
            WallObjects.Clear();
            CollectableObjects.Clear();
            CenterPoints.Clear();
            PlatformPositions.Clear();
            GameTimer = new TimeSpan(0);
            TotalEnemies = 0;
            if (win)
            {
                LevelNumber++;
                PlatformNumber -= 2;
                EnemySpawnTime -= 10;
                EnemyMoveTime -= 1;
                PlayerChar.JumpHeight += 1;
            }
            else
            {
                PlayerChar.JumpHeight = DefJumpHeight;
                PlatformNumber = DefPlatformNumber;
                EnemySpawnTime = DefEnemySpawnTime;
                EnemyMoveTime = DefEnemyMoveTime;
                LevelNumber = 1;
            }
            runGame();
        }
        public GameObject WallObj { get; set; }
        public char PlatformChar { get; set; } = '‾';
        public char PlatformEdgeChar { get; set; } = 'T';
        public char PlatformCenterChar { get; set; } = '˘';
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
                        wall.ObjectColor = ConsoleColor.DarkYellow;
                        WallObjects.Add(wall);
                    }
                    if (GameLevel.LevelLayout.AllLines[i][j] == PlatformCenterChar)
                    {
                        var wall = new GameObject(WallObj, i, j);
                        wall.Symbol = PlatformCenterChar;
                        wall.ObjectColor = ConsoleColor.DarkMagenta;
                        WallObjects.Add(wall);
                        CenterPoints.Add(wall);
                    }
                }
            }
        }

        string[] getSpaces()
        {
            var spacesArr = new string[5];

            var spaces0 = "";
            var lvl = LevelNumber.ToString();
            if (lvl.Length <= 1)
            { spaces0 = " "; }
            spacesArr[0] = spaces0;

            var spaces1 = "";
            var spaces2 = "";
            var min = "" + GameTimer.Minutes;
            var sec = "" + GameTimer.Seconds;
            if (min.Length == 1)
            {
                spaces1 = " ";
            }
            if (sec.Length == 1)
            {
                spaces2 = " ";
            }
            spacesArr[1] = spaces1;
            spacesArr[2] = spaces2;

            var spaces3 = "";
            var spaces4 = "";
            var points = TotalCollected.ToString();
            var req = WinCondition.ToString();
            if (points.Length <= 1)
            {
                spaces3 = " ";
            }
            if (req.Length <= 1)
            {
                spaces4 = " ";
            }
            spacesArr[3] = spaces3;
            spacesArr[4] = spaces4;
            return spacesArr;
        }


        public List<GameObject> CenterPoints { get; set; } = new List<GameObject>();
        SymbolImage pausedText = new SymbolImage("    .~ •/\\• ~. ; ~•:˙[ PAUSED ]˙:•~ ;    ˙˚~ •\\/• ~˚˙");
        public List<List<int>> PauseTextPixels { get; set; }
        public List<List<int>> StatsTextPixels { get; set; }
        public ConsoleColor PauseColor { get; set; } = ConsoleColor.Yellow;

        void drawPausedText()
        {
            PauseTextPixels = GameBoard.addSymbolImage(pausedText, (GameBoard.Height / 2), (GameBoard.Width / 2) - 8);
        }

        public SymbolImage StatsImage { get; set; }
        public void drawStatsText(bool intro = false)
        {
            var spaces = getSpaces();
            var text = getTopBotText();
            StatsImage = new SymbolImage("");
            StatsImage.createImage(text[0] + " ;  ;  ;      . -~ • LVL: " + spaces[0] + + LevelNumber + " • ~- . ;  ; .~•/˙  Survived: " + spaces[1] + GameTimer.Minutes + "m " + spaces[2] + GameTimer.Seconds + "s  ˙\\•~. ;  ;  ˙~•\\.  Points: " + spaces[3] + spaces[4] + TotalCollected + " / " + WinCondition + "  ./•~˙  ;  ;          ˙ -~ •.• ~- ˙    ;  ;  ; " + text[1]);
            if (!intro)
            StatsTextPixels = GameBoard.addSymbolImage(StatsImage, (ScreenHeight / 2) - 5, (ScreenWidth / 2) - 15);
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
            foreach (var collectable in CollectableObjects)
            {
                GameBoard.addPixel(collectable.yPos, collectable.xPos, collectable.Symbol);
            }
            GameBoard.addHorizontalLine(2, PlatformChar);
            GameBoard.addVerticalLine(ScreenWidth - 1, '│', 2, '§', '§');
            GameBoard.addVerticalLine(0, '│', 2, '§', '§');
            if (PlayerDead || WinConditionMet || IsPaused)
            { drawStatsText(); }
        }


        public ConsoleColor TextCol { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor SymCol { get; set; } = ConsoleColor.Magenta;
        public bool PlayLevelBuildAnimation { get; set; } = true;

        void displayStats()
        {
            
            Writer.output(". -~ •Å LVL: " + LevelNumber + " Å• ~- .", 0, 0, SymCol, TextCol, SymCol);
            Writer.output(".~•/˙   ÅSurvived: " + GameTimer.Minutes + "m " + GameTimer.Seconds+ "sÅ   ˙\\•~.", 1, 0, SymCol, TextCol, SymCol);
            Writer.output("˙~•\\.  ÅPoints: " + TotalCollected + " / " + WinCondition + "Å  ./•~˙", 1, 0, SymCol, TextCol, SymCol);
            Writer.output("˙ -~ •.• ~- ˙", 1, 0, SymCol);
            Console.WriteLine();

        }

        public void drawFrame()
        {
            GameBoard.printFrame(0);
            if (!PlayerDead && !WinConditionMet && !IsPaused)
            {
                displayStats();
            }
        }
    }
}

