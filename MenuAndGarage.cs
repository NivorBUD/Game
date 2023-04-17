using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class MenuAndGarage : Form
    {
        #region Меню
        private Button StartGame;
        private Button StartTraining;

        public MenuAndGarage()
        {
            DoubleBuffered = true;
            MakeMenu();
            InitializeComponent();
            UpdForm();
            Program.RModel.MenuAndGarage = this;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdForm();
            base.OnSizeChanged(e);
        }

        private void MakeMenu()
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
                
            };

            Controls.Add(StartGame);
            Controls.Add(StartTraining);
        }

        private void UpdForm()
        {
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

        public void GoToGarage()
        {
            Controls.Remove(StartGame);
            Controls.Remove(StartTraining);
            MakeGarage();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        #endregion

        #region Гараж
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

        private void MakeGarage()
        {
            RaceForm = Program.RForm;
            RaceModel = Program.RModel;
            BackgroundImage = null;

            Balance = new()
            {
                Text = "$0",
                Top = 0,
                Left = Size.Width - 100,
                Font = new Font("Times New Roman", 16),
                BackColor = Color.Transparent
            };

            StartRace = new()
            {
                Size = new Size(Size.Width / 10, Size.Height / 15),
                Top = (int)(Size.Height * 0.85),
                Left = (int)(Size.Width * 0.85),
                Text = "Начать гонку"
            };

            StartRace.Click += (s, e) =>
            {
                Visible = false;
                Program.RModel.Start();
            };

            MakeGarageCar();
            MakeGarageButtons();
            MakeGarageProgressBars();

            Controls.Add(Car);
            Controls.Add(SpeedLevel);
            Controls.Add(DRSTimeLevel);
            Controls.Add(DRSBoostLevel);
            Controls.Add(BoostLevel);
            Controls.Add(ControlLevel);
            Controls.Add(UpSpeed);
            Controls.Add(UpDRSTime);
            Controls.Add(UpDRSBoost);
            Controls.Add(UpBoost);
            Controls.Add(UpControl);
            Controls.Add(Balance);
            Controls.Add(StartRace);
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
                button.Font = new Font("Ariel", 20);
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
            var dict = new Dictionary<Specification, Button>
            {
                [Specification.DRSTime] = UpDRSTime,
                [Specification.DRSBoost] = UpDRSBoost,
                [Specification.Boost] = UpBoost,
                [Specification.Control] = UpControl,
                [Specification.Speed] = UpSpeed
            };

            foreach (var specification in dict.Keys)
                if (!RaceModel.Economy.CanBuy(specification) || RaceModel.PlayerCar.SpecificationsLevels[specification] >= 5)
                {
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

            updateButton.Text = $"{texts[updateButton]}\n\n\n Улучшение: {RaceModel.Economy.Price[updateSpecification]}$";
            updateButton.Font = new Font("Ariel", 20);
        }

        public void UpdateProgressBars()
        {
            SpeedLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Speed];
            DRSTimeLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSTime];
            DRSBoostLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.DRSBoost];
            BoostLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Boost];
            ControlLevel.Value = RaceModel.PlayerCar.SpecificationsLevels[Specification.Control];
        }
        #endregion
    }
}
