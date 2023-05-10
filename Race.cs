using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Game;
public partial class Race : Form
{
    public readonly Dictionary<string, Bitmap> bitmaps = new();
    private static KeyEventArgs KeyWS;
    private static KeyEventArgs KeyAD;
    private readonly RaceModel RaceModel;
    private static Label RaceTimeBox;
    private static ProgressBar DRSBar;
    private static PictureBox DRSIcon;
    private static PictureBox Light;
    public PictureBox Player;
    private static Image ActualImage;
    private static Image NextImage;
    private static PictureBox Speedometer;
    public int TimeOffTheRoad;
    public int TimeNotMaxSpeed;
    public int TimeWithOutOvertakes;
    private List<uint> RoadValues;
    private bool IsOvertaking;
    public bool BotIsOvertaking;


    public Timer DRSTimer;
    public Timer Stopwatch; // секундомер 
    public int MiliSeconds;
    public int LightN;
    public int PrevUsed;

    private void MyForm_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Maximized;
        ResizeBackghroundImages();
        ChangeImages();
        MakePauseButtons();
        ControlBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
    }

    public Race()
    {
        RoadValues = new() { 12698049, 7237230, 1973790, 1907997, 12632256, 0, 1118481, 12829635 }; // числа - номера цветов по которым можно ездить
        DoubleBuffered = true;
        MakeDir();
        RaceModel = Program.RModel;
        RaceModel.RaceForm = this;
        RaceModel.PlayerCar.Location = new Point(Size.Width / 2, Size.Height - 200);
        MakeTable();
        InitializeTimers();
        ChangeBackground();
        KeyPreview = true;
        PrevUsed = 0;
        LightN = 0;
        Keyboard();
        ChangeImages();
        Paint += new PaintEventHandler(OnPaint);
    }

    public void Start()
    {
        IsOvertaking = false;
        BotIsOvertaking = false;
        Visible = true;
        BackColor = Color.GhostWhite;
        BackColor = Color.White;
        Light.Size = new Size(Size.Width / 5, (int)(Size.Width / 5 * 0.424));
        LightN = 0;
        MiliSeconds = 0;
        TimeOffTheRoad = 0;
        TimeNotMaxSpeed = 0;
        TimeWithOutOvertakes = 20;
        Light.Image = ResizeImage(bitmaps["Light" + LightN + ".png"], Light.Size);
        Light.Location = new Point(Size.Width / 2 - Light.Size.Width / 2, Size.Height / 10);
        Controls.Add(Light); 
        ChangeImages();
        ChangeBackground();
        MakeBotsPictureBoxs();
        RaceModel.PlayerCar.Location = new Point(Size.Width / 2, 0);
        Player.Size = new((int)(Size.Width * 1.0 / 19), (int)(Size.Width * 2.693 / 19)); ;
        Player.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(Size.Height - 2.5 * Player.Height));
        SetBotsLocs();
        Speedometer.Size = new Size(Size.Width / 8, (int)(Size.Width / 8 * 0.98));
        Speedometer.Location = new Point(0, (int)(Size.Height - Speedometer.Height * 1.1));

        StartTimers();

        Stopwatch = new() { Interval = 100 };
        Stopwatch.Tick += (s, e) => MiliSeconds++;
        UpdatePlacesLables();
        LightTimer.Start();
    }

    public void Finish()
    {
        Visible = false;
        StopTimers();
        RaceModel.MenuAndGarage.UpdateButtons();
        RaceModel.MenuAndGarage.MakeResults();
    }

    private void Keyboard()
    {
        KeyAD = new KeyEventArgs(Keys.Z);
        KeyWS = new KeyEventArgs(Keys.Z);
        KeyDown += (s, e) =>
        {
            if (e.KeyData.ToString() == "E")
                RaceModel.DRS();
            else if (e.KeyData.ToString() == "W" || e.KeyData.ToString() == "S")
                KeyWS = e;
            else if (e.KeyData.ToString() == "A" || e.KeyData.ToString() == "D")
                KeyAD = e;
        };
        KeyUp += (s, e) =>
        {
            if (e.KeyData.ToString() == "W" || e.KeyData.ToString() == "S")
                KeyWS = new KeyEventArgs(Keys.Z);
            else if (e.KeyData.ToString() == "A" || e.KeyData.ToString() == "D")
                KeyAD = new KeyEventArgs(Keys.Z);
            if (e.KeyData.ToString() == "Escape")
                GoToPause();
        };
    }

    private Bitmap ResizeImage(Image oldImage, Size size) => new Bitmap(oldImage, size);

    private string GetTime()
    {
        if (MiliSeconds == 0) return "0:0:0";
        var mili = (MiliSeconds % 10).ToString();
        var sec = (MiliSeconds / 10 % 60).ToString();
        var min = (MiliSeconds / 10 / 60).ToString();
        return min + ':' + sec + ':' + mili;
    }

    private void MakeTable()
    {
        DRSBar = new()
        {
            Value = 0,
            Size = new Size(100, 30),
            Step = 1,
            Style = ProgressBarStyle.Continuous,
            Top = 0,
            BackColor = Color.Green,
            ForeColor = Color.Red,
            Minimum = 0,
            Maximum = 100,
            RightToLeftLayout = true,
        };
        
        RaceTimeBox = new()
        {
            BackColor = Color.Transparent,
            Top = 0,
            Font = new Font("Times New Roman", 16),
        };

        DRSIcon = new()
        {
            Size = new Size(155/2, 94/2),
            BackColor= Color.Transparent,
        };
        DRSIcon.Image = ResizeImage(bitmaps["DRSoff.png"], DRSIcon.Size);

        Light = new()
        {
            BackColor= Color.Transparent,
            Size = new Size(Size.Width / 5, (int)(Size.Width / 5 * 0.424))
        };
        Light.Image = ResizeImage(bitmaps["Light0.png"], Light.Size);
        Light.Location = new Point(Size.Width / 2 - Light.Size.Width / 2, Size.Height / 10);

        Player = new()
        {
            Size = new Size(Size.Width / 18, (int)(Size.Width / 18 * 2.693)),
            BackColor = Color.Transparent
        };

        Speedometer = new()
        {
            Size = new Size(Size.Width / 18, (int)(Size.Width / 18 * 0.98)),
            BackColor = Color.Transparent
        };

        Controls.Add(Speedometer);
        Controls.Add(DRSIcon);
        Controls.Add(DRSBar);
        Controls.Add(RaceTimeBox);
        Controls.Add(Player);
        MakePlacesLables();
    }

    private void MakeDir(DirectoryInfo imagesDirectory = null)
    {
        var way = @"..\..\..\Game\GameImages\";
        if (imagesDirectory == null)
            imagesDirectory = new DirectoryInfo(way);
        foreach (var e in imagesDirectory.GetFiles("*.png"))
            bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
        InitializeComponent();
    }

    private void DrawCar()
    {
        var loc = RaceModel.PlayerCar.Location;
        var k = 1.0 / 19;
        var coefficientsWidth = new double[] { k, k * 1.091, k * 1.341, k * 1.477, k * 1.659, k * 1.761, k * 1.932, k * 2.045, k * 2.159, k * 2.273, k * 2.386, k * 2.466, k * 2.5}; // k * (высота 0го / высота нынешнего)
        var coefficientsHeight = new double[] { k * 2.693, k * 2.636 ,k * 2.682, k * 2.613, k * 2.591, k * 2.568, k * 2.500, k * 2.450, k * 2.386, k * 2.273, k * 2.159, k * 2.046, k * 1.932}; // k * (кф ширины * высота нынешнего / ширина нынешнего)
        Player.Location = new Point(loc.X, Player.Location.Y);
        var kw = coefficientsWidth[Math.Abs((int)RaceModel.PlayerCar.Velocity.X)];
        var kh = coefficientsHeight[Math.Abs((int)RaceModel.PlayerCar.Velocity.X)];
        Player.Size = new Size((int)(Size.Width * kw), (int)(Size.Width * kh));
        Player.Image = ResizeImage(bitmaps[RaceModel.PlayerCar.CarBrand + (int)RaceModel.PlayerCar.Velocity.X + ".png"], Player.Size);
    }

    public void ChangeBackground()
    {
        MoveDownBackground();

        RaceTimeBox.Left = Size.Width - RaceTimeBox.Width + 25;

        DRSBar.Top = Size.Height - DRSBar.Height - 39;
        DRSBar.Left = Size.Width - DRSBar.Width - 16;
        DRSBar.Size = new Size(Size.Width / 7, (int)(Size.Width / 7 * 0.2));

        Light.Size = new Size(Size.Width / 5, (int)(Size.Width / 5 * 0.424));
        Light.Location = new Point(Size.Width / 2 - Light.Size.Width / 2, Light.Location.Y);

        DRSIcon.Location = new Point(DRSBar.Location.X + (DRSBar.Size.Width - DRSIcon.Width) / 2, (int)(DRSBar.Location.Y - DRSBar.Size.Height / 2 - DRSIcon.Size.Height / 1.5));
        DRSIcon.Size = new Size(Size.Width / 15, (int)(Size.Width / 15 * 0.606));
        DRSIcon.Image = ResizeImage(DRSIcon.Image, DRSIcon.Size);

        if (RaceModel.PlayerCar.DRSOn)
            if ((int)(RaceModel.PlayerCar.Velocity.Y * RaceModel.PlayerCar.DRSMultiplier) >= 30)
                Speedometer.Image = ResizeImage(bitmaps["Speedometer30.png"], Speedometer.Size);
            else
                Speedometer.Image = ResizeImage(bitmaps["Speedometer" + (int)(RaceModel.PlayerCar.Velocity.Y * RaceModel.PlayerCar.DRSMultiplier) + ".png"], Speedometer.Size);
        else
            Speedometer.Image = ResizeImage(bitmaps["Speedometer" + (int)RaceModel.PlayerCar.Velocity.Y + ".png"], Speedometer.Size);
    }

    public void ChangeImages()
    {
        var background = "Background" + RaceModel.ActualSectorId + ".png";
        var nextbackground = "Background" + RaceModel.NextSectorId + ".png";
        ActualImage = bitmaps[background];
        if (RaceModel.NextSectorId == -1) return;
        NextImage = bitmaps[nextbackground];
    }

    private void MoveDownBackground()
    {
        if (!RaceModel.GameIsGo) return;
        var k = Size.Height * 0.1 / 100;
        if (RaceModel.PlayerCar.DRSOn) k *= RaceModel.PlayerCar.DRSMultiplier;
        if (RaceModel.PlayerCar.Velocity.Y <= 0) return;

        if (RaceModel.NextSectorId == -1)
        {
            MovePlayer(k);
            return;
        }

        RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(RaceModel.PlayerCar.Location.Y + Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
        Light.Location = new Point(Light.Location.X, (int)(Light.Location.Y + Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
        if (RaceModel.PlayerCar.Location.Y >= Size.Height)
        {
            RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, 0);
            ChangeSector();
            ChangeImages();
        }

        if (((RaceModel.ActualSectorId == 11 && RaceModel.NextSectorId == 11) ||
            (RaceModel.ActualSectorId == 22 && RaceModel.NextSectorId == 22) ||
            (RaceModel.ActualSectorId == 0 && RaceModel.NextSectorId == 0)) &&
            RaceModel.SectorNumberActual >= 6 && !Controls.Contains(OvertakenBot))
            if (CanBeOvertaken())
                OvertakeBot();
            else if (CanOvertake())
                OvertakeBot();

       MoveBotAtOvertaking();

        BackColor = Color.White;
        BackColor = Color.Wheat;
    }

    private void MovePlayer(double k) //двигает игрока, только перед финишем 
    {
        Player.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(Player.Location.Y - Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
        if (Player.Location.Y <= Size.Height * 0.14)
            Stopwatch.Stop();
        if (Player.Bottom <= 0)
            RaceModel.FinishRace(RaceModel.PlayerCar.Place);
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
        var gr = e.Graphics;
        gr.DrawImage(ActualImage, new Point(0, RaceModel.PlayerCar.Location.Y));
        gr.DrawImage(NextImage, new Point(0, RaceModel.PlayerCar.Location.Y - Size.Height));
    }

    private void ChangeDRSStatus()
    {
        if (RaceModel.PlayerCar.DRSOn)
            DRSIcon.Image = ResizeImage(bitmaps["DRSon.png"], DRSIcon.Size);
        else
            DRSIcon.Image = ResizeImage(bitmaps["DRSoff.png"], DRSIcon.Size);

        if (100.0 * RaceModel.PlayerCar.DRSUsedTime / RaceModel.PlayerCar.DRSMaxTime <= 99.999999)
            DRSBar.Value = (int)(100.0 * RaceModel.PlayerCar.DRSUsedTime / RaceModel.PlayerCar.DRSMaxTime);
    }

    [DllImport("gdi32")]
    public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);
    private IntPtr dc = GetWindowDC(IntPtr.Zero);
    private bool CheckRoad() // проверка нахождения на трассе 
    {
        var leftSide = GetPixel(dc, Player.Left, Player.Top);
        var rightSide = GetPixel(dc, Player.Right, Player.Top);
        var leftSideOnRoad = RoadValues.Contains(leftSide);
        var rightSideOnRoad = RoadValues.Contains(rightSide);
        return leftSideOnRoad || rightSideOnRoad;
    }

    public void ChangeSector()
    {
        if (Controls.Contains(OvertakenBot))
            return;
        if (RaceModel.SectorNumberNext + 1 == RaceModel.Track.Count)
        {
            RaceModel.NextSectorId = -1;
            RaceModel.ActualSectorId = RaceModel.Track[RaceModel.SectorNumberActual + 1];
        }
        else
        {
            RaceModel.SectorNumberActual = RaceModel.SectorNumberNext;
            RaceModel.ActualSectorId = RaceModel.Track[RaceModel.SectorNumberActual];
            RaceModel.SectorNumberNext += 1;
            RaceModel.NextSectorId = RaceModel.Track[RaceModel.SectorNumberNext];
        }
    }

    private void ResizeBackghroundImages()
    {
        var a = new List<int> { 0, 1, 2, 10, 11, 12, 20, 21, 22, 100 };
        foreach (var e in a)
            bitmaps["Background" + e + ".png"] = ResizeImage(bitmaps["Background" + e + ".png"], Size);
    }

    protected override CreateParams CreateParams // ???
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;    // WS_EX_COMPOSITED
            return cp;
        }
    }
}