using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Game;
public partial class Race : Form
{
    private Label MaxPlace;
    private Label LewisPlace;
    private Label FernandoPlace;
    private Label CharlePlace;
    private Label PlayerPlace;
    private PictureBox Max;
    private PictureBox Lewis;
    private PictureBox Fernando;
    private PictureBox Charle;
    private List<PictureBox> Bots;
    private List<PictureBox> FrontBots;
    private List<PictureBox> BackBots;
    public PictureBox OvertakenBot;

    public void MakeBotsPictureBoxs()
    {
        var kw = 1.0 / 19;
        var kh = kw * 2.693;
        var size = new Size((int)(Size.Width * kw), (int)(Size.Width * kh));
        Max = new(); Max.Name = "Max";
        Lewis = new(); Lewis.Name = "Lewis";
        Fernando = new(); Fernando.Name = "Fernando";
        Charle = new(); Charle.Name = "Charle";
        Bots = new() { Max, Lewis, Fernando, Charle };
        FrontBots = new() { Max, Lewis, Fernando, Charle };
        BackBots = new();
        if (RaceModel.StartPlace <= 4) 
        { 
            FrontBots.Remove(Charle);
            BackBots.Add(Charle);
        }
        if (RaceModel.StartPlace <= 3)
        {
            FrontBots.Remove(Fernando);
            BackBots.Add(Fernando);
        }
        if (RaceModel.StartPlace <= 2)
        {
            FrontBots.Remove(Lewis);
            BackBots.Add(Lewis);
            BackBots.Remove(Charle);
        }
        if (RaceModel.StartPlace == 1)
        {
            FrontBots.Remove(Max);
            BackBots.Add(Max);
            BackBots.Remove(Fernando);
        }
        foreach (var pictureBox in Bots)
        {
            pictureBox.Image = ResizeImage(bitmaps[pictureBox.Name + ".png"], size);
            pictureBox.Size = size;
            pictureBox.BackColor = Color.Transparent;
        }

        foreach (var pictureBox in FrontBots)
            Controls.Add(pictureBox);

        foreach (var pictureBox in BackBots)
            Controls.Add(pictureBox);
    }

    public void MoveBotsFromStart()
    {
        foreach (var pictureBox in FrontBots)
        {
            pictureBox.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y - 8);
            if (pictureBox.Bottom <= 0)
                Controls.Remove(pictureBox);
        }

        foreach (var pictureBox in BackBots)
        {
            pictureBox.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + 8);
            if (pictureBox.Top >= Size.Height)
                Controls.Remove(pictureBox);
        }
    }

    public bool CanOvertake()
    {
        if (RaceModel.PlayerCar.SpecificationsLevels[Specification.Speed] > 6 - RaceModel.PlayerCar.Place && 
            TimeOffTheRoad <= 10 && TimeWithOutOvertakes >= 50 && TimeNotMaxSpeed < 10)
            IsOvertaking = true;
        return IsOvertaking;
    }

    public bool CanBeOvertaken()
    {
        if ((TimeOffTheRoad > 15 || TimeNotMaxSpeed > 10) && RaceModel.PlayerCar.Place <= 4 && TimeWithOutOvertakes >= 50)
        {
            TimeOffTheRoad = 5;
            BotIsOvertaking = true;
        }
        return BotIsOvertaking;
    }

    public void OvertakeBot()
    {
        TimeNotMaxSpeed = 0;
        TimeWithOutOvertakes = 0;
        OvertakenBot = new PictureBox();
        if (RaceModel.PlayerCar.Place == 5 && IsOvertaking) OvertakenBot = Charle;
        if (RaceModel.PlayerCar.Place == 4) OvertakenBot = IsOvertaking ? Fernando : Charle;
        if (RaceModel.PlayerCar.Place == 3) OvertakenBot = IsOvertaking ? Lewis : Fernando;
        if (RaceModel.PlayerCar.Place == 2) OvertakenBot = IsOvertaking ? Max : Lewis;
        if (RaceModel.PlayerCar.Place == 1 && BotIsOvertaking) OvertakenBot = Max;
        
        OvertakenBot.Location = new Point(Player.Left, - Player.Height);

        if (BotIsOvertaking)
            OvertakenBot.Location = new Point(OvertakenBot.Location.X, Size.Height);

        Controls.Add(OvertakenBot);
    }

    private void MoveBotAtOvertaking()
    {
        if (OvertakenBot != null && RaceModel.GameIsGo)
        {
            if (IsOvertaking)
                MoveOvertakenBot();
            if (BotIsOvertaking)
                MoveOvertakingBot();

            ChangeOvertakenPlace();

            if (OvertakenBot.Top >= Size.Height + 100)
            {
                Controls.Remove(OvertakenBot);
                IsOvertaking = false;
                BotIsOvertaking = false;
                TimeOffTheRoad = 9;
            }
            if (OvertakenBot.Bottom <= -50)
            {
                IsOvertaking = false;
                BotIsOvertaking = false;
                Controls.Remove(OvertakenBot);
                TimeOffTheRoad = 9;
            }
        }
    }

    public void MoveOvertakenBot()
    {
        var a = (int)(RaceModel.PlayerCar.MaxVelocity.Y - RaceModel.PlayerCar.Velocity.Y * (RaceModel.PlayerCar.DRSOn ? RaceModel.PlayerCar.DRSMultiplier : 1));
        var delt = 10 - 3 * a;
        if (delt >= 0 && Player.Top - OvertakenBot.Bottom <= 20 &&
            Player.Top - OvertakenBot.Bottom >= 0 &&
            ((OvertakenBot.Left >= Player.Left && OvertakenBot.Left <= Player.Right) ||
            (OvertakenBot.Right >= Player.Left && OvertakenBot.Right <= Player.Right)))
            return;

        if (delt <= 0 && OvertakenBot.Top - Player.Bottom <= 20 &&
            OvertakenBot.Top - Player.Bottom >= 0 &&
            ((OvertakenBot.Left >= Player.Left && OvertakenBot.Left <= Player.Right) ||
            (OvertakenBot.Right >= Player.Left && OvertakenBot.Right <= Player.Right)))
            return;

        OvertakenBot.Location = new Point(OvertakenBot.Location.X, OvertakenBot.Location.Y + delt);

        MoveOvertakenBotHorizontal();
    }

    public void MoveOvertakenBotHorizontal()
    {
        var playerCenterLoc = (Player.Left + Player.Right) / 2;
        var botCenter = (OvertakenBot.Left + OvertakenBot.Right) / 2;
        var k = (int)(Size.Width * 3.0 / 1100);
        if (botCenter < playerCenterLoc && !InPlayerAtHeight())
            OvertakenBot.Location = new Point(OvertakenBot.Left + k, OvertakenBot.Top);
        else if (botCenter > playerCenterLoc && !InPlayerAtHeight())
            OvertakenBot.Location = new Point(OvertakenBot.Left - k, OvertakenBot.Top);
    }

    public void MoveOvertakingBot()
    {
        var delt = - 2 + (int)(RaceModel.PlayerCar.Velocity.Y * (RaceModel.PlayerCar.DRSOn ? RaceModel.PlayerCar.DRSMultiplier : 0) - RaceModel.PlayerCar.MaxVelocity.Y);
        if (delt >= 0 && Player.Top - OvertakenBot.Bottom <= 20 &&
            Player.Top - OvertakenBot.Bottom >= 0 &&
            ((OvertakenBot.Left >= Player.Left && OvertakenBot.Left <= Player.Right) ||
            (OvertakenBot.Right >= Player.Left && OvertakenBot.Right <= Player.Right)))
            return;

        if (delt <= 0 && OvertakenBot.Top - Player.Bottom <= 20 &&
            OvertakenBot.Top - Player.Bottom >= 0 &&
            ((OvertakenBot.Left >= Player.Left && OvertakenBot.Left <= Player.Right) ||
            (OvertakenBot.Right >= Player.Left && OvertakenBot.Right <= Player.Right)))
            return;

        OvertakenBot.Location = new Point(OvertakenBot.Location.X, OvertakenBot.Location.Y + delt);
        if (delt < 0)
            MoveOvertakingBotHorizontal();
    }

    public void MoveOvertakingBotHorizontal()
    {
        var playerCenterLoc = (Player.Left + Player.Right) / 2;
        var k = (int)(Size.Width * 6.0 / 1100);
        if (RaceModel.ActualSectorId == 0)
        {
            var roadCenter = Size.Width / 2;
            if (playerCenterLoc >= roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Left - k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left - k, OvertakenBot.Top);
            if (playerCenterLoc < roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Right + k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left + k, OvertakenBot.Top);
        }

        if (RaceModel.ActualSectorId == 11)
        {
            var roadCenter = Size.Width * 0.73;
            if (playerCenterLoc >= roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Left - k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left - k, OvertakenBot.Top);
            if (playerCenterLoc < roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Right + k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left + k, OvertakenBot.Top);
        }

        if (RaceModel.ActualSectorId == 22)
        {
            var roadCenter = Size.Width * 0.26;
            if (playerCenterLoc <= roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Left - k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left - k, OvertakenBot.Top);
            if (playerCenterLoc > roadCenter && RoadValues.Contains(GetPixel(dc, OvertakenBot.Right + k, OvertakenBot.Top)))
                OvertakenBot.Location = new Point(OvertakenBot.Left + k, OvertakenBot.Top);
        }
    }

    private bool InPlayerAtHeight()
    {
        if ((OvertakenBot.Top > Player.Top && OvertakenBot.Top < Player.Bottom) ||
            (OvertakenBot.Bottom > Player.Top && OvertakenBot.Bottom < Player.Bottom))
            return true;
        return false;
    }

    private void ChangeOvertakenPlace()
    {
        if (OvertakenBot.Top >= Player.Top)
        {
            if (OvertakenBot.Name == "Max") RaceModel.PlayerCar.Place = 1;
            else if (OvertakenBot.Name == "Lewis") RaceModel.PlayerCar.Place = 2;
            else if (OvertakenBot.Name == "Fernando") RaceModel.PlayerCar.Place = 3;
            else RaceModel.PlayerCar.Place = 4;
            UpdatePlacesLables();
        }
        else
        {
            if (OvertakenBot.Name == "Max") RaceModel.PlayerCar.Place = 2;
            else if (OvertakenBot.Name == "Lewis") RaceModel.PlayerCar.Place = 3;
            else if (OvertakenBot.Name == "Fernando") RaceModel.PlayerCar.Place = 4;
            else RaceModel.PlayerCar.Place = 5;
            UpdatePlacesLables();
        }
    }

    private void SetBotsLocs()
    {
        if (RaceModel.StartPlace == 5)
        {
            Charle.Location = new Point((int)(Player.Left - Charle.Width * 1.5), (int)(Player.Top - Charle.Height * 0.5));
            Fernando.Location = new Point(Player.Left, (int)(Player.Top - Fernando.Height * 1.5));
            Lewis.Location = new Point(Charle.Left, (int)(Charle.Top - Lewis.Height * 1.5));
            Max.Location = new Point(Player.Left, (int)(Fernando.Top - Max.Height * 1.5));
        }
        else if (RaceModel.StartPlace == 4)
        {
            Charle.Location = new Point((int)(Player.Left - Charle.Width * 1.5), Player.Bottom);
            Fernando.Location = new Point((int)(Player.Left - Fernando.Width * 1.5), (int)(Player.Top - Fernando.Height * 0.5));
            Lewis.Location = new Point(Player.Left, (int)(Player.Top - Fernando.Height * 1.5));
            Max.Location = new Point(Fernando.Left, (int)(Fernando.Top - Max.Height * 1.5));
        }
        else if (RaceModel.StartPlace == 3)
        {
            Charle.Location = new Point(Player.Left, (int)(Player.Bottom + Charle.Height * 0.5));
            Fernando.Location = new Point((int)(Player.Left - Fernando.Width * 1.5), Player.Bottom);
            Lewis.Location = new Point((int)(Player.Left - Lewis.Width * 1.5), (int)(Player.Top - Lewis.Height * 0.5));
            Max.Location = new Point(Player.Left, (int)(Player.Top - Lewis.Height * 1.5));
        }
        else if (RaceModel.StartPlace == 2)
        {
            Fernando.Location = new Point(Player.Left, (int)(Player.Bottom + Charle.Height * 0.5));
            Lewis.Location = new Point((int)(Player.Left - Lewis.Width * 1.5), Player.Bottom);
            Max.Location = new Point((int)(Player.Left - Max.Width * 1.5), (int)(Player.Top - Max.Height * 0.5));
        }
        else
        {
            Lewis.Location = new Point(Player.Left, (int)(Player.Bottom + Charle.Height * 0.5));
            Max.Location = new Point((int)(Player.Left - Lewis.Width * 1.5), Player.Bottom);
        }
    }

    private void MakePlacesLables()
    {
        MaxPlace = new()
        {
            Size = new Size(Size.Width / 5, Size.Width / 20),
            Text = "Max",
            Font = new Font("Ariel", 30),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
        };

        LewisPlace = new()
        {
            Size = MaxPlace.Size,
            Text = "Lewis",
            Font = new Font("Ariel", 30),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
        };

        FernandoPlace = new()
        {
            Size = MaxPlace.Size,
            Text = "Fernando",
            Font = new Font("Ariel", 30),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
        };

        CharlePlace = new()
        {
            Size = MaxPlace.Size,
            Text = "Charle",
            Font = new Font("Ariel", 30),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
        };

        PlayerPlace = new()
        {
            Size = MaxPlace.Size,
            Text = "You",
            Font = new Font("Ariel", 30),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
        };
    }

    private void UpdatePlacesLables()
    {
        var left = 0;
        switch (RaceModel.PlayerCar.Place)
        {
            case (1): PlayerPlace1(left); break;
            case (2): PlayerPlace2(left); break;
            case (3): PlayerPlace3(left); break;
            case (4): PlayerPlace4(left); break;
            case (5): PlayerPlace5(left); break;
        }
        Controls.Add(MaxPlace);
        Controls.Add(LewisPlace);
        Controls.Add(FernandoPlace);
        Controls.Add(CharlePlace);
        Controls.Add(PlayerPlace);
    }

    private void PlayerPlace1(int left)
    {
        PlayerPlace.Location = new Point(left, (int)(Size.Height * 0.05));
        MaxPlace.Location = new Point(left, PlayerPlace.Bottom);
        LewisPlace.Location = new Point(left, MaxPlace.Bottom);
        FernandoPlace.Location = new Point(left, LewisPlace.Bottom);
        CharlePlace.Location = new Point(left, FernandoPlace.Bottom);
    }

    private void PlayerPlace2(int left)
    {
        MaxPlace.Location = new Point(left, (int)(Size.Height * 0.05));
        PlayerPlace.Location = new Point(left, MaxPlace.Bottom);
        LewisPlace.Location = new Point(left, PlayerPlace.Bottom);
        FernandoPlace.Location = new Point(left, LewisPlace.Bottom);
        CharlePlace.Location = new Point(left, FernandoPlace.Bottom);
    }

    private void PlayerPlace3(int left)
    {
        MaxPlace.Location = new Point(left, (int)(Size.Height * 0.05));
        LewisPlace.Location = new Point(left, MaxPlace.Bottom);
        PlayerPlace.Location = new Point(left, LewisPlace.Bottom);
        FernandoPlace.Location = new Point(left, PlayerPlace.Bottom);
        CharlePlace.Location = new Point(left, FernandoPlace.Bottom);
    }

    private void PlayerPlace4(int left)
    {
        MaxPlace.Location = new Point(left, (int)(Size.Height * 0.05));
        LewisPlace.Location = new Point(left, MaxPlace.Bottom);
        FernandoPlace.Location = new Point(left, LewisPlace.Bottom);
        PlayerPlace.Location = new Point(left, FernandoPlace.Bottom);
        CharlePlace.Location = new Point(left, PlayerPlace.Bottom);
    }

    private void PlayerPlace5(int left)
    {
        MaxPlace.Location = new Point(left, (int)(Size.Height * 0.05));
        LewisPlace.Location = new Point(left, MaxPlace.Bottom);
        FernandoPlace.Location = new Point(left, LewisPlace.Bottom);
        CharlePlace.Location = new Point(left, FernandoPlace.Bottom);
        PlayerPlace.Location = new Point(left, CharlePlace.Bottom);
    }
}