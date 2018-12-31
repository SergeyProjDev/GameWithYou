using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Media;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace GameWithYou
{
    class Program
    {
        static void Main(string[] args)
        {
            Program console = new Program();
            Gamer gamer = new Gamer();
            console.init();

            gamer.Start();
        }

        [DllImport("user32.dll")] static extern bool SetCursorPos(int X, int Y);

        void init()
        {
            Console.Title = "404";

            DisableConsoleQuickEdit dcqe = new DisableConsoleQuickEdit(); dcqe.Go();
            DisableActionBarButtons dabb = new DisableActionBarButtons(); dabb.Go();
            FormPosition fp = new FormPosition(); fp.SetConsoleSize();

            Task myTask = new Task(new Action(blockCoursor));
            myTask.Start();
        }

        
        void blockCoursor()
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            SetCursorPos(screen.Width/2, screen.Height/2);

            while (true)
            {
                if (Cursor.Position.X < (int)screen.Width / 2 - 230)
                    SetCursorPos(Cursor.Position.X + 1, Cursor.Position.Y);
                if (Cursor.Position.X > (int)screen.Width / 2 + 230)
                    SetCursorPos(Cursor.Position.X - 1, Cursor.Position.Y);
                if (Cursor.Position.Y < (int)screen.Height / 2 - 210)
                    SetCursorPos(Cursor.Position.X, Cursor.Position.Y + 1);
                if (Cursor.Position.Y > (int)screen.Height / 2 + 230)
                    SetCursorPos(Cursor.Position.X, Cursor.Position.Y - 1);
            }
        }
        
    }

    class DisableConsoleQuickEdit
    {
        const uint ENABLE_QUICK_EDIT = 0x0040;
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll")] static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll")] static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal bool Go()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode))
                return false;

            consoleMode &= ~ENABLE_QUICK_EDIT;

            if (!SetConsoleMode(consoleHandle, consoleMode))
                return false;

            return true;
        }
    }
    class DisableActionBarButtons
    {
        const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")] static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")] static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)] static extern IntPtr GetConsoleWindow();

        internal void Go()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            DeleteMenu(sysMenu, SC_CLOSE,   MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MINIMIZE,MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MAXIMIZE,MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE,    MF_BYCOMMAND);
        }
    }
    class FormPosition
    {
        const int SWP_NOZORDER = 0x4;
        const int SWP_NOACTIVATE = 0x10;

        [DllImport("kernel32")] static extern IntPtr GetConsoleWindow();
        [DllImport("user32")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        internal void SetConsoleSize()
        {
            Console.WindowWidth = 50;
            Console.WindowHeight = 3;
            Console.BufferWidth = 50;
            Console.BufferHeight = 3;
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            SetWindowPosition(screen.Width/2-250, screen.Height/2-250, 500, 500);
        }

        public void SetWindowPosition(int x, int y, int width, int height)
        {
            SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        public IntPtr Handle
        {
            get
            {
                return GetConsoleWindow();
            }
        }
    }



    class Gamer
    {
        SoundPlayer SoundClick = new SoundPlayer("click.wav");
        SoundPlayer SoundType = new SoundPlayer("typing.wav");

        public void Start()
        {
            CPUOutputTextLine("Привет, обычный юзер!");

            Console.ReadLine();
        }

        void CPUOutputTextLine(string inp)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("CPU: ");
            Console.ForegroundColor = ConsoleColor.Green;
            SoundType.Play();
            for (int i = 0; i < inp.ToString().Length; i++)
            {
                Console.Write(inp.ToString()[i]);
                Thread.Sleep(200);
            }
            SoundType.Stop();
            SoundClick.Play();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
