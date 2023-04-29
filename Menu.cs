using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Button StartGame;
    private Button StartTraining;
    private Button Exit;
    private List<Control> MenuControls;

    public Menu()
    {
        DoubleBuffered = true;
        InitializeComponent();
        UpdForm();
        Program.RModel.MenuAndGarage = this;
        ControlBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
    }

    public void GoToMenu()
    {
        Visible = true;
        MakeMenu();
    }

    private void MakeMenuControls()
    {
        MakeStartGameButton();
        MakeStartTrainingButton();
        MakeExitButton();

        MenuControls = new()
        {
            StartGame,
            StartTraining,
            Exit
        };
        UpdForm();
    }

    private void MakeStartGameButton()
    {
        StartGame = new Button()
        {
            Text = "Начать игру",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        StartGame.Paint += Button_Paint;
        StartGame.FlatAppearance.MouseOverBackColor = Color.Transparent;
        StartGame.FlatAppearance.MouseDownBackColor = Color.Transparent;
        StartGame.MouseEnter += (s, e) => StartGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        StartGame.FlatAppearance.BorderSize = 0;
        StartGame.Click += (s, e) =>
        {
            Controls.Clear();
            if (RaceModel.IsFirstRace)
            {
                GoToStory();
                RaceModel.IsFirstRace = false;
            }
            else
                GoToGarage();
        };
    }

    private void MakeStartTrainingButton()
    {
        StartTraining = new Button()
        {
            Text = "Обучение",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        StartTraining.Paint += Button_Paint;
        StartTraining.FlatAppearance.MouseOverBackColor = Color.Transparent;
        StartTraining.FlatAppearance.MouseDownBackColor = Color.Transparent;
        StartTraining.MouseEnter += (s, e) => StartTraining.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        StartTraining.FlatAppearance.BorderSize = 0;
        StartTraining.Click += (s, e) =>
        {
            Controls.Clear();
            GoToTraining();
        };
    }

    private void MakeExitButton()
    {
        Exit = new Button()
        {
            Text = "Выйти",
            Font = new Font("Ariel", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        Exit.Paint += Button_Paint;
        Exit.FlatAppearance.MouseOverBackColor = Color.Transparent;
        Exit.FlatAppearance.MouseDownBackColor = Color.Transparent;
        Exit.MouseEnter += (s, e) => Exit.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        Exit.FlatAppearance.BorderSize = 0;
        Exit.Click += (s, e) =>
        {
            Application.Exit();
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
        StartGame.Location = new Point((Size.Width - StartGame.Width) / 2, Size.Height / 2);

        StartTraining.Size = new Size(Size.Width / 6, (int)(Size.Width / 6 * 0.25));
        StartTraining.Location = new Point((Size.Width - StartTraining.Width) / 2, StartGame.Bottom);

        Exit.Size = new Size(Size.Width / 7, (int)(Size.Width / 6 * 0.25));
        Exit.Location = new Point((Size.Width - Exit.Width) / 2, StartTraining.Bottom);
    }

    private Image ResizeImage(Image oldImage, Size size) => new Bitmap(oldImage, size);

    private void Menu_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Maximized;
        RaceForm = Program.RForm;
        RaceModel = Program.RModel;

        MakeGarageControls();
        MakeMenuControls();
        MakeResultControls();
        MakeStoryControls();
        MakeTrainingControls();

        MakeMenu();

        FormBorderStyle = FormBorderStyle.FixedSingle;
    }
}