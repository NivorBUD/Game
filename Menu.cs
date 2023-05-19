using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Button StartNewGame;
    private Button ContinueGame;
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
        MakeStartNewGameButton();
        MakeContinueGameButton();
        MakeStartTrainingButton();
        MakeExitButton();

        MenuControls = new()
        {
            StartNewGame,
            ContinueGame,
            StartTraining,
            Exit
        };
        UpdForm();
    }

    private void MakeStartNewGameButton()
    {
        StartNewGame = new()
        {
            Text = "Новая игра",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        StartNewGame.Paint += Button_Paint;
        StartNewGame.FlatAppearance.MouseOverBackColor = Color.Transparent;
        StartNewGame.FlatAppearance.MouseDownBackColor = Color.Transparent;
        StartNewGame.MouseEnter += (s, e) => StartNewGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        StartNewGame.FlatAppearance.BorderSize = 0;
        StartNewGame.Click += (s, e) =>
        {
            RaceModel.Save.RewriteSave(new int[9] { 1, 1, 100, 5, 1, 1, 1, 1, 1 });
            RaceModel.LoadSave();
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

    private void MakeContinueGameButton()
    {
        ContinueGame = new()
        {
            Text = "Продолжить",
            Font = new Font("Times New Roman", 16),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
        };
        ContinueGame.Paint += Button_Paint;
        ContinueGame.FlatAppearance.MouseOverBackColor = Color.Transparent;
        ContinueGame.FlatAppearance.MouseDownBackColor = Color.Transparent;
        ContinueGame.MouseEnter += (s, e) => ContinueGame.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, Color.Black);
        ContinueGame.FlatAppearance.BorderSize = 0;
        ContinueGame.Click += (s, e) =>
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
        StartTraining = new()
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
        Exit = new()
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
            RaceModel.Save.RewriteSave(new int[9] { RaceModel.IsFirstRace ? 1 : 0, RaceModel.IsFirstWin ? 1 : 0, 
                RaceModel.Economy.Balance, 
                RaceModel.StartPlace, 
                RaceModel.PlayerCar.SpecificationsLevels[Specification.Speed],
                RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSTime],
                RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSBoost],
                RaceModel.PlayerCar.SpecificationsLevels[Specification.Boost], 
                RaceModel.PlayerCar.SpecificationsLevels[Specification.Control] });
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
        if (StartNewGame == null) return;
        BackgroundImage = ResizeImage(Program.RForm.bitmaps["Menu.png"], Size);

        ContinueGame.Size = new Size((int)(Size.Width / 4.5), (int)(Size.Width / 6 * 0.25));
        ContinueGame.Location = new Point((Size.Width - ContinueGame.Width) / 2, Size.Height / 2);

        StartNewGame.Size = new Size(Size.Width / 5, (int)(Size.Width / 6 * 0.25));
        StartNewGame.Location = new Point((Size.Width - StartNewGame.Width) / 2, ContinueGame.Top - StartNewGame.Height);

        StartTraining.Size = new Size(Size.Width / 6, (int)(Size.Width / 6 * 0.25));
        StartTraining.Location = new Point((Size.Width - StartTraining.Width) / 2, ContinueGame.Bottom);

        Exit.Size = new Size(Size.Width / 7, (int)(Size.Width / 6 * 0.25));
        Exit.Location = new Point((Size.Width - Exit.Width) / 2, StartTraining.Bottom);
    }

    private Image ResizeImage(Image oldImage, Size size) => new Bitmap(oldImage, size);

    private void Menu_Load(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Maximized;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        RaceForm = Program.RForm;
        RaceModel = Program.RModel;

        MakeGarageControls();
        MakeMenuControls();
        MakeResultControls();
        MakeStoryControls();
        MakeTrainingControls();

        MakeMenu();
    }
}