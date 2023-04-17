﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Game;
public partial class Race : Form
{
    public readonly Dictionary<string, Bitmap> bitmaps = new();
    private readonly Dictionary<string, string> idFiles = new();
    private static KeyEventArgs KeyWS;
    private static KeyEventArgs KeyAD;
    private readonly RaceModel RaceModel;
    private static Label RaceTimeBox;
    private static ProgressBar DRSBar;
    private static PictureBox DRSIcon;
    private static PictureBox Light;
    private static PictureBox Player;
    private static Image ActualImage;
    private static Image NextImage;
    private static PictureBox Speedometer;

    public Timer DRSTimer;
    public Timer Stopwatch; // секундомер 
    public int MiliSeconds;
    public int LightN;
    public int PrevUsed;

    public Race()
    {
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
        Visible = false;
        Keyboard();
        ChangeImages();
    }

    public void Start()
    {
        Visible = true;
        Light.Size = new Size(Size.Width / 5, (int)(Size.Width / 5 * 0.424));
        Light.Image = ResizeImage(bitmaps[idFiles["Light" + LightN]], Light.Size);
        RaceModel.PlayerCar.Location = new Point(Size.Width / 2, 0);
        Player.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(Size.Height - 2.6 * Player.Height));
        Speedometer.Size = new Size(Size.Width / 8, (int)(Size.Width / 8 * 0.98));
        Speedometer.Location = new Point(0, (int)(Size.Height - Speedometer.Height * 1.1));

        Paint += new PaintEventHandler(OnPaint);

        var lightTimer = new Timer()
        {
            Interval = 1000,
        };
        lightTimer.Tick += LightTimer_Tick;
        lightTimer.Start();

        Stopwatch = new() { Interval = 100 };
        Stopwatch.Tick += (s, e) => MiliSeconds++;
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
        };
    }

    #region Таймеры
    private void InitializeTimers()
    {
        var timer = new Timer
        {
            Interval = 1
        };
        timer.Tick += Timer_Tick;
        timer.Start();

        var speedTimer = new Timer
        {
            Interval = 100,
        };
        speedTimer.Tick += SpeedTimer_Tick;
        speedTimer.Start();

        var updateTimer = new Timer
        {
            Interval = 10,
        };
        updateTimer.Tick += UpdateTimer_Tick;
        updateTimer.Start();

        DRSTimer = new Timer
        {
            Interval = 20,
        };
        DRSTimer.Tick += DRSTimer_Tick;
        DRSTimer.Start();
    }

    private void DRSTimer_Tick(object sender, EventArgs e)
    {
        if (RaceModel.PlayerCar.DRSOn) RaceModel.PlayerCar.DRSUsedTime++;
        if (RaceModel.PlayerCar.DRSOn && RaceModel.PlayerCar.DRSUsedTime >= RaceModel.PlayerCar.DRSMaxTime)
            RaceModel.DRS();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        RaceTimeBox.Text = GetTime();
        RaceModel.Move(Size);

        DrawCar();

        if (LightN >= 6 && RaceModel.ActualSectorId != 1234)
        {
            Light.Image = null;
            Controls.Remove(Light);
        }
    }

    public void LightTimer_Tick(object sender, EventArgs e)
    {
        var timer = (Timer)sender;
        LightN++;
        if (LightN <= 5)
            Light.Image = ResizeImage(bitmaps[idFiles["Light" + LightN]], Light.Size);
        if (LightN == 6)
        {
            Light.Image = ResizeImage(bitmaps[idFiles["Light0"]], Light.Size);
            RaceModel.StartRace();
            timer.Stop();
        }
    }

    private void SpeedTimer_Tick(object sender, EventArgs e)
    {
        RaceModel.ChangeSpeed(KeyWS.KeyCode.ToString(), KeyAD.KeyCode.ToString());
    }

    private void UpdateTimer_Tick(object sender, EventArgs e)
    {
        ChangeBackground();
        ChangeDRSStatus();
    }
    #endregion

    private Image ResizeImage(Image oldImage, Size size) => new Bitmap(oldImage, size);

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
        DRSIcon.Image = ResizeImage(bitmaps[idFiles["DRSoff"]], DRSIcon.Size);

        Light = new()
        {
            BackColor= Color.Transparent,
            Size = new Size(Size.Width / 5, (int)(Size.Width / 5 * 0.424))
        };
        Light.Image = ResizeImage(bitmaps[idFiles["Light0"]], Light.Size);
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
        Controls.Add(Light);
        Player.Location = new Point(Player.Location.X, Size.Height - Player.Height);
    }

    private void MakeDir(DirectoryInfo imagesDirectory = null)
    {
        if (imagesDirectory == null)
            imagesDirectory = new DirectoryInfo("Images");
        foreach (var e in imagesDirectory.GetFiles("*.png"))
            bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
        foreach (var e in bitmaps.Keys)
            idFiles[e.Substring(0, e.Length - 4)] = e;
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
        Player.Image = ResizeImage(bitmaps[idFiles[RaceModel.PlayerCar.CarBrand + RaceModel.PlayerCar.Velocity.X]], Player.Size);
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

        Speedometer.Image = ResizeImage(bitmaps[idFiles["Speedometer" + (int)RaceModel.PlayerCar.Velocity.Y]], Speedometer.Size);
    }

    public void ChangeImages()
    {
        var background = "Background" + RaceModel.ActualSectorId;
        var nextbackground = "Background" + RaceModel.NextSectorId;
        ActualImage = ResizeImage(bitmaps[idFiles[background]], Size);
        if (RaceModel.NextSectorId == -1) return;
        NextImage = ResizeImage(bitmaps[idFiles[nextbackground]], Size);
    }

    private void MoveDownBackground()
    {
        var k = Size.Height * 0.1 / 100;
        if (RaceModel.PlayerCar.DRSOn) k *= 1.0 + RaceModel.PlayerCar.DRSMultiplier;
        if (RaceModel.PlayerCar.Velocity.Y <= 0) return;

        if (RaceModel.NextSectorId == -1)
        {
            Player.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(Player.Location.Y - Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
            return;
        }

        RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(RaceModel.PlayerCar.Location.Y + Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
        Light.Location = new Point(Light.Location.X, (int)(Light.Location.Y + Math.Abs(RaceModel.PlayerCar.Velocity.Y) * k));
        if (RaceModel.PlayerCar.Location.Y >= Size.Height)
        {
            RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, 0);
            RaceModel.ChangeSector();
            ChangeImages();
        }
        BackColor = Color.White;
        BackColor = Color.Wheat;

        //for (int i = 0; i < Math.Abs(RaceModel.PlayerCar.Velocity.Y); i++)
        //{
        //    RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, (int)(RaceModel.PlayerCar.Location.Y + k));
        //    Light.Location = new Point(Light.Location.X, (int)(Light.Location.Y + k));
        //    if (RaceModel.PlayerCar.Location.Y >= Size.Height)
        //    {
        //        RaceModel.PlayerCar.Location = new Point(RaceModel.PlayerCar.Location.X, 0);
        //        RaceModel.ChangeSector();
        //        ChangeImages();
        //    }
        //}
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
            DRSIcon.Image = ResizeImage(bitmaps[idFiles["DRSon"]], DRSIcon.Size);
        else
            DRSIcon.Image = ResizeImage(bitmaps[idFiles["DRSoff"]], DRSIcon.Size);

        if (100.0 * RaceModel.PlayerCar.DRSUsedTime / RaceModel.PlayerCar.DRSMaxTime <= 99.999999)
            DRSBar.Value = (int)(100.0 * RaceModel.PlayerCar.DRSUsedTime / RaceModel.PlayerCar.DRSMaxTime);
    }

    private void MyForm_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Maximized;
        ChangeImages();
    }
}