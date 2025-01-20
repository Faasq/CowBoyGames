namespace CowAek;

struct Coordinate
{
    public int X;
    public int Y;

    public Coordinate(int x,int y)
    {
        X=x;
        Y=y;
    }
}

class GameObject
{
    public Coordinate Position;
    public char Symbol;
    public ConsoleColor Color;
    public int Lifes;
    public bool Enabled;
}
class Program
{
    static int W = 40;
    static int H = 20;
    static GameObject Me;
    static List<GameObject> target;
    static int Bullets;
    static int CurrentLevel = 1; 
    static int MaxLevel = 4; 
    static Random RG;
    static int Score;
    static bool GameRun = true;
    static void Main(string[] args)
    {
        SetupScreen();
        //Set up player
        Me = new GameObject();
        Me.Lifes = 2;
        Me.Symbol = 'M';
        Me.Color = ConsoleColor.Gray;
        Me.Position.X = W/2;
        Me.Position.Y = H-2;
        RG = new Random();
        //setuptarget
        target = new List<GameObject>();

        GameRun =true;
        while (GameRun == true)
        {
            GetInput();

            Console.Clear();

            
            foreach (var t in target.Where(t => t.Enabled))
            {
                DrawSymbol(t.Position, t.Symbol, t.Color);
            }

            
            DrawSymbol(Me.Position, Me.Symbol, Me.Color);
            DrawWord(new Coordinate(5, 1), "Bullet:" + Bullets, ConsoleColor.White);
            DrawWord(new Coordinate(27, 1), "Life:" + Me.Lifes, ConsoleColor.White);
            DrawWord(new Coordinate(15, 1), $"Level: {CurrentLevel}", ConsoleColor.White);

            
            CheckTargetsAek();

            Thread.Sleep(200);
        }

        
    }

    static void SetupScreen()
    {
        Console.WriteLine(Console.LargestWindowHeight);
        Console.WriteLine(Console.LargestWindowWidth);
        Console.Title = "CowAek";
        Console.BufferHeight = Console.WindowHeight = H;
        Console.BufferWidth = Console.WindowWidth = W;
        Console.CursorVisible = false;
    }

    static void RestoreScreen()
    {
        Console.CursorVisible = true;
    }

    static void DrawSymbol(Coordinate pos,char Symbol,ConsoleColor color)
    {
        Console.SetCursorPosition(pos.X,pos.Y);
        Console.ForegroundColor = color;
        Console.Write(Symbol);
    }

    static void DrawWord(Coordinate pos,string text,ConsoleColor color )
    {
        Console.SetCursorPosition(pos.X,pos.Y);
        Console.ForegroundColor = color;
        Console.Write(text);
    }

    static void GetInput()
    {
        if(Console.KeyAvailable)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            while(Console.KeyAvailable)
            {
                Console.ReadKey();
            }

            if(keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if(Me.Position.X > 0)
                {
                   Me.Position.X--; 
                }
                
            }

            if(keyInfo.Key == ConsoleKey.RightArrow)
            {
                if(Me.Position.X < W-1)
                {
                   Me.Position.X++; 
                }
                
            }

            if(keyInfo.Key == ConsoleKey.R)
            {
                Res();
            }

            if(keyInfo.Key == ConsoleKey.E)
            {
                Shoot();
            }
        }
    }

    static void CreateTargetsAek()
    {
        target = new List<GameObject>();
        int targetCount = CurrentLevel + 2; 
        Bullets = targetCount;

        for (int i = 0; i < targetCount; i++)
        {
            GameObject Aek = new GameObject();
            Coordinate newPosition;
            bool isDuplicate;

            do
            {
                newPosition = new Coordinate(RG.Next(1, W - 1), RG.Next(5, H - 2));
                isDuplicate = target.Any(t => t.Position.X == newPosition.X && t.Position.Y == newPosition.Y);
            } while (isDuplicate); 

            Aek.Position = newPosition;
            Aek.Symbol = 'A';
            Aek.Color = ConsoleColor.Red;
            Aek.Enabled = true;
            target.Add(Aek);
        }
    }
    
    static void Shoot()
    {
        if (Bullets > 0) 
        {
            Bullets--; 

            
            for (int y = Me.Position.Y - 3; y >= 0; y--)
            {
                Console.SetCursorPosition(Me.Position.X, y);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('|');
                Thread.Sleep(50); 
                Console.SetCursorPosition(Me.Position.X, y);
                Console.Write(' '); 
            }

            
            var hitTarget = target.FirstOrDefault(t => t.Enabled && t.Position.X == Me.Position.X && t.Position.Y < Me.Position.Y);
            if (hitTarget != null)
            {
                hitTarget.Enabled = false; 
    
                Console.Beep(500, 200); 
            }
        }
        else
        {
            Console.Beep(200, 300);
        }
    }


    static void CheckTargetsAek()
        {
            if (target.All(t => !t.Enabled))
            {
                CurrentLevel++;
                if (CurrentLevel > MaxLevel)
                {
                    Win();
                }
                else
                {
                    CreateTargetsAek();
                }
            }
            else if (Bullets == 0)
            {
                Console.Clear();
                DrawWord(new Coordinate(15, 10), "I need more bullets!", ConsoleColor.Yellow);
                Thread.Sleep(2000);
                Me.Lifes--;
                Console.Beep(200, 500);
                Console.BackgroundColor = ConsoleColor.Red;
                Thread.Sleep(200);
                Console.BackgroundColor = ConsoleColor.Black;
                if (Me.Lifes > 0)
                {
                    CreateTargetsAek();
                }
                else
                {
                    Endgame();
                }
            }
        }

    


    static void Endgame()
    {
        if(Me.Lifes <=0)
        {
            GameRun = false;
            Console.Clear();
            DrawWord(new Coordinate(20, 10), "U loose (T-T)", ConsoleColor.Red);
            DrawWord(new Coordinate(25, 10), "R to RESTART THE GAME", ConsoleColor.Red);
            Thread.Sleep(3000);

            while (Console.ReadKey(true).Key != ConsoleKey.R)
            {
                Res();
            } 
            
            
        }
    }

     static void Win()
    {
        if(CurrentLevel > MaxLevel)
        {
            GameRun = false;
            Console.Clear();
            DrawWord(new Coordinate(20, 10), "U WIN !!!", ConsoleColor.Red);
            DrawWord(new Coordinate(25, 10), "R to RESTART THE GAME", ConsoleColor.Red);
            Thread.Sleep(3000);

            while (Console.ReadKey(true).Key != ConsoleKey.R)
            {
                Res();
            }
            
        }
    }

    static void Res()
    {
        CurrentLevel = 1;
        Me.Lifes = 2;
        target.Clear();
        CreateTargetsAek();
        GameRun = true;
    }


    
}
