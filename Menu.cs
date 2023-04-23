﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Button StartGame;
    private Button StartTraining;
    private List<Control> MenuControls;

    public Menu()
    {
        DoubleBuffered = true;
        InitializeComponent();
        UpdForm();
        Program.RModel.MenuAndGarage = this;
    }

    public void GoToMenu()
    {
        Controls.Clear();
        MakeMenu();
    }

    public void RemoveMenu()
    {
        foreach (var e in MenuControls)
            Controls.Remove(e);
    }

    private void MakeMenuControls()
    {
        StartGame = new Button()
        {
            Text = "Начать игру",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        StartGame.Paint += StartGame_Paint;
        StartGame.FlatAppearance.MouseOverBackColor = Color.Transparent;
        StartGame.FlatAppearance.MouseDownBackColor = Color.Transparent;
        StartGame.MouseEnter += (s, e) => StartGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        StartGame.FlatAppearance.BorderSize = 0;
        StartGame.Click += (s, e) =>
        {
            RemoveMenu();
            GoToGarage();
        };

        StartTraining = new Button()
        {
            Text = "Обучение",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        StartTraining.Paint += StartTraining_Paint;
        StartTraining.FlatAppearance.MouseOverBackColor = Color.Transparent;
        StartTraining.FlatAppearance.MouseDownBackColor = Color.Transparent;
        StartTraining.MouseEnter += (s, e) => StartTraining.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        StartTraining.FlatAppearance.BorderSize = 0;
        StartTraining.Click += (s, e) =>
        {
            RemoveMenu();
            GoToResults();
        };

        MenuControls = new()
        {
            StartGame,
            StartTraining
        };
        UpdForm();
    }

    private void MakeResultControlsList()
    {
        ResultControls = new()
        {
            PlayerPlace,
            Income,
            MaxPlace,
            LewisPlace,
            FernandoPlace,
            CharlePlace,
            RepeatRace,
            ToGarage,
            ToMenu
        };
    }

    private void MakeGarageControlsList()
    {
        GarageControls = new()
        {
            Car,
            SpeedLevel,
            DRSTimeLevel,
            DRSBoostLevel,
            BoostLevel,
            ControlLevel,
            UpSpeed,
            UpDRSTime,
            UpDRSBoost,
            UpBoost,
            UpControl,
            StartRace,
            Balance
        };
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        UpdForm();
        base.OnSizeChanged(e);
    }

    private void MakeMenu()
    {
        BackgroundImage = ResizeImage(Program.RForm.bitmaps["Menu.png"], Size);
        foreach (var e in MenuControls)
            Controls.Add(e);
    }

    private void UpdForm()
    {
        if (StartGame == null) return;
        BackgroundImage = ResizeImage(Program.RForm.bitmaps["Menu.png"], Size);

        StartGame.Size = new Size(Size.Width / 5, (int)(Size.Width / 6 * 0.25));
        StartGame.Location = new Point((Size.Width - StartGame.Size.Width) / 2, Size.Height / 2);

        StartTraining.Size = new Size(Size.Width / 6, (int)(Size.Width / 6 * 0.25));
        StartTraining.Location = new Point((Size.Width - StartTraining.Size.Width) / 2, StartGame.Location.Y + StartGame.Height);
    }

    private Image ResizeImage(Image oldImage, Size size) => new Bitmap(oldImage, size);

    private void StartGame_Paint(object sender, PaintEventArgs e)
    {
        float fontSize = NewFontSize(e.Graphics, StartGame.Size, StartGame.Font, StartGame.Text);
        Font f = new Font("Times New Roman", fontSize, FontStyle.Regular);
        StartGame.Font = f;
    }

    private void StartTraining_Paint(object sender, PaintEventArgs e)
    {
        float fontSize = NewFontSize(e.Graphics, StartTraining.Size, StartTraining.Font, StartTraining.Text);
        Font f = new Font("Times New Roman", fontSize, FontStyle.Regular);
        StartTraining.Font = f;
    }

    public static float NewFontSize(Graphics graphics, Size size, Font font, string str)
    {
        SizeF stringSize = graphics.MeasureString(str, font);
        float wRatio = size.Width / stringSize.Width;
        float hRatio = size.Height / stringSize.Height;
        float ratio = Math.Min(hRatio, wRatio);
        return font.Size * ratio;
    }

    private void Menu_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Maximized;
        RaceForm = Program.RForm;
        RaceModel = Program.RModel;

        MakeGarageControls();
        MakeMenuControls();
        MakeResultControls();

        MakeMenu();
    }
}