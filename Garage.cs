using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private static RaceModel RaceModel;
    private static Race RaceForm;
    private static PictureBox Car;
    private static ProgressBar SpeedLevel;
    private static ProgressBar DRSTimeLevel = new();
    private static ProgressBar DRSBoostLevel = new();
    private static ProgressBar BoostLevel = new();
    private static ProgressBar ControlLevel = new();
    private static Button UpSpeed;
    private static Button UpDRSTime = new();
    private static Button UpDRSBoost = new();
    private static Button UpBoost = new();
    private static Button UpControl = new();
    private static Button StartRace;
    private static Label Balance;
    private static List<Control> GarageControls;

    public void GoToGarage()
    {   
        MakeGarage();
        UpdateButtons();
        UpdateProgressBars();
        foreach (Specification s in new Specification[5] { Specification.Speed, Specification.DRSTime, Specification.DRSBoost, Specification.Boost, Specification.Control })
            UpdateBalanceInfo(s);
    }

    private void MakeGarage()
    {
        BackgroundImage = null;
        foreach (var e in GarageControls)
            Controls.Add(e);
    }

    private void MakeGarageControls()
    {
        Balance = new()
        {
            Text = "$0",
            Top = 0,
            Left = 0,
            Font = new Font("Times New Roman", 30),
            BackColor = Color.Transparent,
            AutoSize = true,
        };

        MakeGarageCar();
        MakeGarageButtons();
        MakeGarageProgressBars();

        MakeGarageControlsList();
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

    private void MakeGarageCar()
    {
        Car = new()
        {
            BackColor = Color.Transparent,
            Size = new Size(Size.Width / 5, (int)((Size.Width / 5) * (88.0 / 237)))
        };
        Image CarImage = RaceForm.bitmaps["McLaren.png"];

        CarImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
        CarImage = ResizeImage(CarImage, Car.Size);
        Car.Location = new Point((Size.Width - Car.Width) / 2, Car.Height);
        Car.Image = CarImage;
    }

    private void MakeGarageProgressBars()
    {
        SpeedLevel = new()
        {
            Style = ProgressBarStyle.Continuous,
            Size = new Size(UpSpeed.Width, Size.Width / 46),
            Location = new Point(UpSpeed.Location.X, (int)(UpSpeed.Location.Y + UpSpeed.Height + Size.Height * 0.05)),
            Value = 1,
            Step = 1,
            Minimum = 0,
            Maximum = 5,
        };

        var barsWithOutFirst = new List<ProgressBar>() { DRSTimeLevel, DRSBoostLevel, BoostLevel, ControlLevel };
        var previousBar = SpeedLevel;
        foreach (var bar in barsWithOutFirst)
        {
            bar.Style = SpeedLevel.Style;
            bar.Size = SpeedLevel.Size;
            bar.Location = new Point(previousBar.Location.X + previousBar.Width + (int)(Size.Width * 0.05), SpeedLevel.Location.Y);
            bar.Value = 1;
            bar.Maximum = 5;
            previousBar = bar;
        }
    }

    private void MakeGarageButtons()
    {
        StartRace = new()
        {
            Size = new Size(Size.Width / 10, Size.Height / 15),
            Top = (int)(Size.Height * 0.85),
            Left = (int)(Size.Width * 0.85),
            Text = "Начать гонку",
            Font = new Font("Ariel", Size.Width > 2000 ? 20 : 15)
        };

        StartRace.Click += (s, e) =>
        {
            Controls.Clear();
            Visible = false;
            Program.RModel.Start();
        };

        UpSpeed = new()
        {
            Size = new Size(Size.Width / 10, Size.Width / 10),
            Location = new Point((int)(Size.Width * 0.15), Car.Location.Y + Car.Height + Size.Height / 15 + Size.Width / 46 + (int)(Size.Height * 0.05)),
        };

        var texts = new Dictionary<Button, string>();
        texts[UpSpeed] = "Скорость";
        texts[UpDRSTime] = "Время DRS";
        texts[UpDRSBoost] = "Ускорение DRS";
        texts[UpBoost] = "Ускорение";
        texts[UpControl] = "Управление";

        var buttons = new List<Button>() { UpSpeed, UpDRSTime, UpDRSBoost, UpBoost, UpControl };
        var previousButton = UpSpeed;
        foreach (var button in buttons)
        {
            button.Enabled = true;
            button.Text = $"{texts[button]}\n\n\n Улучшение: 100$";
            button.Font = new Font("Ariel", Size.Width > 2000 ? 20 : 15);
            button.Click += UpButton_Click;
            if (button == UpSpeed) continue;
            button.Size = UpSpeed.Size;
            button.Location = new Point(previousButton.Location.X + previousButton.Width + (int)(Size.Width * 0.05), UpSpeed.Location.Y);
            previousButton = button;
        }
        UpdateButtons();
        UpdateBalanceInfo(Specification.Speed);
    }

    private void UpButton_Click(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var dict = new Dictionary<Button, Specification>
        {
            [UpDRSTime] = Specification.DRSTime,
            [UpDRSBoost] = Specification.DRSBoost,
            [UpBoost] = Specification.Boost,
            [UpControl] = Specification.Control,
            [UpSpeed] = Specification.Speed
        };
        RaceModel.Economy.Buy(dict[button]);
    }

    public void UpdateButtons()
    {
        Balance.Text = "$" + RaceModel.Economy.Balance;
        var dict = new Dictionary<Specification, Button>
        {
            [Specification.DRSTime] = UpDRSTime,
            [Specification.DRSBoost] = UpDRSBoost,
            [Specification.Boost] = UpBoost,
            [Specification.Control] = UpControl,
            [Specification.Speed] = UpSpeed
        };

        foreach (var specification in dict.Keys)
            if (!RaceModel.Economy.CanBuy(specification))
            {
                if (RaceModel.PlayerCar.SpecificationsLevels[specification] == 5)
                    dict[specification].BackColor = Color.LimeGreen;
                else
                    dict[specification].BackColor = Color.Red;
                dict[specification].Enabled = false;
            }
            else
            {
                dict[specification].BackColor = Color.LawnGreen;
                dict[specification].Enabled = true;
            }
    }

    public void UpdateBalanceInfo(Specification updateSpecification)
    {
        var dict = new Dictionary<Specification, Button>
        {
            [Specification.DRSTime] = UpDRSTime,
            [Specification.DRSBoost] = UpDRSBoost,
            [Specification.Boost] = UpBoost,
            [Specification.Control] = UpControl,
            [Specification.Speed] = UpSpeed
        };
        var texts = new Dictionary<Button, string>();
        texts[UpSpeed] = "Скорость";
        texts[UpDRSTime] = "Время DRS";
        texts[UpDRSBoost] = "Ускорение DRS";
        texts[UpBoost] = "Ускорение";
        texts[UpControl] = "Управление";
        var updateButton = dict[updateSpecification];

        Balance.Text = "$" + RaceModel.Economy.Balance;

        if (RaceModel.PlayerCar.SpecificationsLevels[updateSpecification] == 5)
        {
            updateButton.Text = $"{texts[updateButton]}\n\n\n Максимальный\n уровень";
            updateButton.BackColor = Color.LimeGreen;
            updateButton.Enabled = false;
        }
        else 
            updateButton.Text = $"{texts[updateButton]}\n\n\n Улучшение: {RaceModel.Economy.Price[updateSpecification]}$";
    }

    public void UpdateProgressBars()
    {
        SpeedLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Speed];
        DRSTimeLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSTime];
        DRSBoostLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSBoost];
        BoostLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Boost];
        ControlLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Control];
    }
}