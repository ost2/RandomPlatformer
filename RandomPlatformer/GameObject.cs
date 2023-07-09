using System;
namespace RandomPlatformer
{
    public class GameObject
    {
        public string Name { get; set; }
        public Application App { get; set; }
        public int yPos { get; set; }
        public int xPos { get; set; }
        public int yMovement { get; set; }
        public int xMovement { get; set; }
        public int MovementSpeed { get; set; }
        public int JumpHeight { get; set; }

        public char Symbol { get; set; }

        public bool IsSolid { get; set; }
        public bool IsHostile { get; set; }
        public bool FollowsPlayer { get; set; }
        public bool AvoidsNearest { get; set; }
        public bool FollowsNearest { get; set; }
        public bool IsInfectious { get; set; }

        public bool IsInAir { get; set; }
        public int JumpTimer { get; set; } = 0;

        public ConsoleColor ObjectColor { get; set; }

        public GameObject(Application app, char symbol, int movementSpeed = 1, ConsoleColor objColor = ConsoleColor.White, bool isSolid = true, bool isHostile = false, bool followsPlayer = false, bool avoidsNearest = false, bool followsNearest = false, bool isInfectious = false)
        {
            App = app;
            MovementSpeed = movementSpeed;
            JumpHeight = 9;
            Symbol = symbol;
            ObjectColor = objColor;
            IsSolid = isSolid;
            IsHostile = isHostile;
            FollowsPlayer = followsPlayer;
            AvoidsNearest = avoidsNearest;
            FollowsNearest = followsNearest;
            IsInfectious = isInfectious;
        }

        public GameObject(GameObject gameObject, int y, int x, List<char> randomSymbolsOverride = null, ConsoleColor colorOverride = ConsoleColor.Black)
        {
            App = gameObject.App;
            yPos = y;
            xPos = x;

            if (randomSymbolsOverride == null)
            {
                Symbol = gameObject.Symbol;
            }
            else
            {
                Symbol = randomSymbolsOverride[App.Rand.Next(0, randomSymbolsOverride.Count)];
            }

            ObjectColor = gameObject.ObjectColor;
            IsSolid = gameObject.IsSolid;
            IsHostile = gameObject.IsHostile;
            FollowsPlayer = gameObject.FollowsPlayer;
            FollowsNearest = gameObject.FollowsNearest;
            AvoidsNearest = gameObject.AvoidsNearest;
            IsInfectious = gameObject.IsInfectious;
            MovementSpeed = gameObject.MovementSpeed;

            if (colorOverride != ConsoleColor.Black)
            {
                ObjectColor = colorOverride;
            }
        }

        public void moveObject(int dy, int dx)
        {
            yPos += dy;
            xPos += dx;
            //yPos = Math.Clamp(yPos, 1, App.ScreenHeight - 1);
            //xPos = Math.Clamp(xPos, 1, App.ScreenWidth - 1);
        }

        public void doCollision(GameObject Mover, GameObject Stander)
        {
            if (Stander.IsSolid)
            {
                Mover.setMovementAwayFromObject(Stander);
                Stander.setMovementAwayFromObject(Mover);
  
                //Mover.yMovement = 0;
                //Mover.xMovement = 0;
            }
            if (Mover.IsHostile || Stander.IsHostile)
            {
                if (Stander == App.PlayerChar || Mover == App.PlayerChar)
                {
                    App.playerLose();
                }
            }
            if (Mover.IsInfectious && Stander != App.PlayerChar && !Stander.IsInfectious)
            {
                Stander.infectObject(Mover);
            }
            else if (Stander.IsInfectious && Mover != App.PlayerChar && !Mover.IsInfectious)
            {
                Mover.infectObject(Stander);
            }
        }

        public void infectObject(GameObject infector)
        {
            Symbol = infector.Symbol;
            IsSolid = infector.IsSolid;
            IsHostile = infector.IsHostile;
            IsInfectious = infector.IsInfectious;
            AvoidsNearest = infector.AvoidsNearest;
            FollowsNearest = infector.FollowsNearest;
            MovementSpeed = infector.MovementSpeed;
            ObjectColor = infector.ObjectColor;
        }


        public void deleteObject()
        {
            Symbol = ' ';
            IsSolid = false;
            IsHostile = false;
            IsInfectious = false;
            AvoidsNearest = false;
            FollowsNearest = false;
            MovementSpeed = 0;
        }

        public void collisionCheck()
        {
            int nextYPos = yPos + yMovement;
            int nextXPos = xPos + xMovement;

            foreach (GameObject gameObject in App.GameObjects)
            {
                if (gameObject != this)
                {
                    if (gameObject.yPos == nextYPos && gameObject.xPos == nextXPos)
                    {
                        doCollision(this, gameObject);
                    }
                    if (gameObject.yPos == nextYPos - (yMovement / 2) && gameObject.xPos == nextXPos - (xMovement / 2))
                    {
                        doCollision(this, gameObject);
                    }
                }
            }
            if (IsSolid)
            {
                foreach (var wall in App.WallObjects)
                {
                    if (wall.yPos == nextYPos && wall.xPos == nextXPos)
                    {
                        doCollisionWall(wall);
                    }
                }
            }
        }
        public bool isOnGround()
        {
            if (App.GameBoard.AllLines[yPos + 1][xPos] != ' ') { return true; }
            return false;
        }

        public void doCollisionWall(GameObject wall)
        {
            if (wall.xPos == xPos)
            {
                yMovement = 0;
                return;
            }
            if (wall.yPos == yPos)
            {
                xMovement = 0;
                return;
            }
            yMovement = 0;
            xMovement = 0;
        }

        public void OOBCheck()
        {
            int newYPos = yPos + yMovement;
            int newXPos = xPos + xMovement;

            if (newYPos < 0 || newYPos >= App.ScreenHeight || newXPos < 0 || newXPos >= App.ScreenWidth)
            {
                
                //scoobyDoo(newYPos, newXPos);
                //reverseMovement();
                nullifyMovement();
                //teleportRandom();
                //teleportToMiddle();
            }
        }

        public void scoobyDoo(int y, int x)
        {
            if (y >= App.ScreenHeight)
            {
                yPos = 1;
            }
            else if (y <= 0)
            {
                yPos = App.ScreenHeight - 1;
            }
            else if (x >= App.ScreenWidth)
            {
                xPos = 1;
            }
            if (x <= 0)
            {
                xPos = App.ScreenWidth - 1;
            }
        }

        public void reverseMovement()
        {
            yMovement = -yMovement;
            xMovement = -xMovement;
        }
        public void nullifyMovement()
        {
            yMovement = 0;
            xMovement = 0;
        }

        public void teleportRandom()
        {
            yPos = App.Rand.Next(0, App.ScreenHeight - 1);
            xPos = App.Rand.Next(0, App.ScreenWidth - 1);
        }

        public void teleportToMiddle()
        {
            yPos = App.ScreenHeight / 2;
            xPos = App.ScreenWidth / 2;
        }

        public void performJump()
        {
            JumpTimer = JumpHeight;
            setObjectMovement(-1, xMovement);
        }

        public void setMovementAwayFromObject(GameObject objectToAvoid)
        {
            int dy = yPos - objectToAvoid.yPos;
            int dx = xPos - objectToAvoid.xPos;

            int yMove = 0;
            int xMove = 0;

            if (dy > 0)
            {
                yMove = MovementSpeed;
            }
            else if (dy < 0)
            {
                yMove = -MovementSpeed;
            }
            if (dx > 0)
            {
                xMove = MovementSpeed;
            }
            else if (dx < 0)
            {
                xMove = -MovementSpeed;
            }
            if (Math.Abs(dy) > Math.Abs(dx) * 1.5)
            {
                xMove = 0;
            }
            else if (Math.Abs(dx) > Math.Abs(dy) * 1.5)
            {
                yMove = 0;
            }
            setObjectMovement(yMove, xMove);
        }

        public void setMovementTowardsObject(GameObject objectToFollow)
        {
            int dy = yPos - objectToFollow.yPos;
            int dx = xPos - objectToFollow.xPos;

            int yMove = 0;
            int xMove = 0;

            if (dy > 0)
            {
                yMove = -MovementSpeed;
            }
            else if (dy < 0)
            {
                yMove = MovementSpeed;
            }
            if (dx > 0)
            {
                xMove = -MovementSpeed;
            }
            else if (dx < 0)
            {
                xMove = MovementSpeed;
            }
            if (Math.Abs(dy) > Math.Abs(dx) * 1.5)
            {
                xMove = 0;
            }
            else if (Math.Abs(dx) > Math.Abs(dy) * 1.5)
            {
                yMove = 0;
            }
            setObjectMovement(yMove, xMove);
        }

        double[] getPytDistances()
        {
            List<double> pytDistances = new List<double>(App.GameObjects.Count - 1);

            for (int i = 0; i < App.GameObjects.Count; i++)
            {
                if (App.GameObjects[i].yPos != yPos || App.GameObjects[i].xPos != xPos)
                {
                    pytDistances.Add(getPytDistance(App.GameObjects[i]));
                }
            }
            pytDistances.Sort();
            return pytDistances.ToArray();
        }

        double getPytDistance(GameObject gameObject)
        {
            int[] distanceVector = getDistanceVector(gameObject);

            int lowerVal = distanceVector.Min();
            int higherVal = distanceVector.Max();

            return Math.Abs(distanceVector[0]) + Math.Abs(distanceVector[1]);
        }

        int[] getDistanceVector(GameObject gameObject)
        {
            int dy = Math.Abs(yPos) - Math.Abs(gameObject.yPos);
            int dx = Math.Abs(xPos) - Math.Abs(gameObject.xPos);

            int[] distanceVector = new int[] { dy, dx };
            return distanceVector;
        }

        List<GameObject> nearestToFurthest()
        {
            List<GameObject> nearestToFurthestObjects = new List<GameObject>(getPytDistances().Count());
            foreach (Double distance in getPytDistances())
            {
                foreach (GameObject gameObject in App.GameObjects)
                {
                    if (gameObject != this)
                    {
                        double objectPytDistance = getPytDistance(gameObject);
                        if (distance > objectPytDistance - 1 && distance < objectPytDistance + 1)
                        {
                            nearestToFurthestObjects.Add(gameObject);
                        }
                    }
                }
            }
            return nearestToFurthestObjects;
        }

        public GameObject nearestNotFollower()
        {
            var nearestToFurthestArr = nearestToFurthest();
            for (int i = 0; i < nearestToFurthestArr.Count; i++)
            {
                if (!nearestToFurthestArr[i].FollowsNearest)
                {
                    return nearestToFurthestArr[i];
                }
            }
            return App.GameObjects[App.Rand.Next(0, App.GameObjects.Count)];
        }

        public GameObject nearestNotRunner()
        {
            var nearestToFurthestArr = nearestToFurthest();
            for (int i = 0; i < nearestToFurthestArr.Count; i++)
            {
                if (!nearestToFurthestArr[i].AvoidsNearest)
                {
                    return nearestToFurthestArr[i];
                }
            }
            return App.GameObjects[App.Rand.Next(0, App.GameObjects.Count)];
        }

        public void setObjectMovement(int yMove, int xMove)
        {
            yMovement = yMove;
            xMovement = xMove;
        }
    }
}

