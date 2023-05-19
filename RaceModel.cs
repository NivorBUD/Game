using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game;

public class RaceModel
{
    public bool GameIsGo;
    public int SectorNumberActual;
    public int SectorNumberNext;
    public List<int> Track;
    public int ActualSectorId;
    public int NextSectorId;
    public Economy Economy;
    public Race RaceForm;
    public Car PlayerCar;
    public Menu MenuAndGarage;
    public int StartPlace;
    public int FinishPlace;
    public int Income;
    public bool IsFirstRace;
    public bool IsFirstWin;
    public Save Save;

    public RaceModel()
    {
        Save = new Save();
        LoadSave();
        Track = new List<int>();
        ActualSectorId = 0;
    }

    public void LoadSave()
    {
        PlayerCar = new Car(new Point(0, 0), "McLaren");
        IsFirstRace = int.Parse(Save.SaveInfo["IsFirstRace"]) == 1;
        IsFirstWin = int.Parse(Save.SaveInfo["IsFirstWin"]) == 1;
        Economy = new(int.Parse(Save.SaveInfo["Balance"]));
        StartPlace = int.Parse(Save.SaveInfo["StartPlace"]);
        PlayerCar.SpecificationsLevels[Specification.Speed] = int.Parse(Save.SaveInfo["Speed"]);
        PlayerCar.SpecificationsLevels[Specification.DRSTime] = int.Parse(Save.SaveInfo["DRSTime"]);
        PlayerCar.SpecificationsLevels[Specification.DRSBoost] = int.Parse(Save.SaveInfo["DRSBoost"]);
        PlayerCar.SpecificationsLevels[Specification.Boost] = int.Parse(Save.SaveInfo["Boost"]);
        PlayerCar.SpecificationsLevels[Specification.Control] = int.Parse(Save.SaveInfo["Control"]);
        Economy.RModel = this;
        Economy.MakePrices();
    }

    public void Start()
    {
        PlayerCar.Place = StartPlace;
        GameIsGo = false;
        ActualSectorId = 0;
        SectorNumberActual = 0;
        SectorNumberNext = 1;
        MakeTrack(10);
        NextSectorId = Track[1];
        PlayerCar.ToStartValues();
        RaceForm.Start();
    }

    public void StartRace()
    {
        GameIsGo = true;
        RaceForm.MiliSeconds = 0;
        RaceForm.Stopwatch.Start();
    }

    public void PauseRace()
    {
        GameIsGo = false;
    }

    public void FinishRace(int finishPlace)
    {
        FinishPlace = finishPlace;
        Income = (6 - finishPlace) * 300;
        Economy.Balance += Income;
        StartPlace = Math.Min(finishPlace, PlayerCar.Place);
        Save.RewriteSave(new int[9] { IsFirstRace ? 1 : 0, IsFirstWin ? 1 : 0,
                Economy.Balance,
                StartPlace,
                PlayerCar.SpecificationsLevels[Specification.Speed],
                PlayerCar.SpecificationsLevels[Specification.DRSTime],
                PlayerCar.SpecificationsLevels[Specification.DRSBoost],
                PlayerCar.SpecificationsLevels[Specification.Boost],
                PlayerCar.SpecificationsLevels[Specification.Control] });
        RaceForm.Finish();
    }

    public void DRS()
    {
        PlayerCar.DRSOn = !PlayerCar.DRSOn;
        if (PlayerCar.DRSUsedTime > PlayerCar.DRSMaxTime || !PlayerCar.OnRoad)
            PlayerCar.DRSOn = false;
    }

    public void MakeTrack(int trackLength)
    {
        var random = new Random();
        var prevgroup = -1;
        Track = new() { 0, 0, 0, 0, 0 };
        for (int i = 0; i < trackLength; i++)
        {
            var group = random.Next(0, 3);
            if (group == prevgroup)
            {
                i--;
                continue;
            }
            if (prevgroup != -1)
                Track.Add(prevgroup * 10 + group);
            else
                Track.Add(group);
            prevgroup = group;
            var c = random.Next(0, 4);
            for (int j = 0; j < c; j++)
                Track.Add(group * 11);
        }
        if (Track.Last() % 10 != 0)
            Track.Add(Track.Last() % 10 * 10);
        if (Track.Count % 2 != 0)
            Track.Add(0);
        Track.Add(100);
    }

    public void Move(Size PanelSize)
    {
        if (!PlayerCar.OnRoad) PlayerCar.DRSOn = false;
        var x = PlayerCar.Location.X;
        int k = (int)(PanelSize.Width * 3.0 / 1100);
        if (GameIsGo && PlayerCar.Velocity.Y != 0)
        {
            if (RaceForm.Controls.Contains(RaceForm.OvertakenBot))
            {
                if (RaceForm.OvertakenBot.Bottom >= RaceForm.Player.Top &&
                    RaceForm.OvertakenBot.Top <= RaceForm.Player.Bottom)
                {
                    var newX1 = (int)(x + PlayerCar.Velocity.X);
                    var newX2 = newX1 + RaceForm.Player.Width;
                    if ((RaceForm.OvertakenBot.Left < newX1 && RaceForm.OvertakenBot.Right > newX1) ||
                        (RaceForm.OvertakenBot.Left < newX2 && RaceForm.OvertakenBot.Right > newX2))
                    {
                        PlayerCar.Velocity.X = 0;
                        return;
                    }
                }
            }
            if ((x <= 0 && PlayerCar.Velocity.X < 0) || (RaceForm.Player.Right >= PanelSize.Width && PlayerCar.Velocity.X > 0))
                return;
            PlayerCar.Location.X += (int)(PlayerCar.Velocity.X * k * PlayerCar.Velocity.Y / PlayerCar.MaxVelocity.Y);
        }
    }

    public void ChangeSpeed(string keyWS, string keyAD)
    {
        var controlMultiplayer = 0.9 + (double)PlayerCar.SpecificationsLevels[Specification.Control] / 10;
        if (GameIsGo)
        {
            switch (keyWS)
            {
                case "W":
                    if (PlayerCar.Velocity.Y < PlayerCar.MaxVelocity.Y)
                        PlayerCar.Velocity.Y += 1 + PlayerCar.BoostMultiplayer;
                    if (!PlayerCar.OnRoad)
                    {
                        PlayerCar.Velocity.Y += (1 + PlayerCar.BoostMultiplayer) / 3;
                        if (PlayerCar.Velocity.Y > PlayerCar.MaxVelocity.Y / 3)
                            while (PlayerCar.Velocity.Y > PlayerCar.MaxVelocity.Y / 3)
                                PlayerCar.Velocity.Y -= 1;
                    }
                    break;
                case "S":
                    if (PlayerCar.Velocity.Y > -PlayerCar.MaxVelocity.Y)
                        PlayerCar.Velocity.Y -= (1 + PlayerCar.BoostMultiplayer);
                    if (!PlayerCar.OnRoad)
                        PlayerCar.Velocity.Y -= 1;
                    if (PlayerCar.Velocity.Y < 0)
                        PlayerCar.Velocity.Y = 0;
                    break;
                default:
                    if (!PlayerCar.OnRoad)
                        PlayerCar.Velocity.Y -= 1;
                    if (Math.Abs(PlayerCar.Velocity.Y) < 0.3)
                        PlayerCar.Velocity.Y = 0;
                    if (Math.Abs(PlayerCar.Velocity.Y) > 0)
                        if (PlayerCar.Velocity.Y < 0)
                            PlayerCar.Velocity.Y += 0.3;
                        else
                            PlayerCar.Velocity.Y -= 0.3;
                    if (PlayerCar.Velocity.Y < 0)
                        PlayerCar.Velocity.Y = 0;
                    break;
            }
            switch (keyAD)
            {
                case "A":
                    if (RaceForm.Contains(RaceForm.OvertakenBot))
                        if (RaceForm.OvertakenBot.Bottom >= RaceForm.Player.Top &&
                            RaceForm.OvertakenBot.Top <= RaceForm.Player.Bottom &&
                            RaceForm.OvertakenBot.Right - RaceForm.Player.Left < 15 &&
                            RaceForm.OvertakenBot.Right - RaceForm.Player.Left > -70)
                        {
                            PlayerCar.Velocity.X = 0;
                            break;
                        }
                    if (PlayerCar.Velocity.X > -PlayerCar.MaxVelocity.X)
                        PlayerCar.Velocity.X -= controlMultiplayer;
                    PlayerCar.Velocity.X = (int)PlayerCar.Velocity.X;
                    if (RaceForm.BotIsOvertaking)
                        if (PlayerCar.Velocity.X > 1)
                            PlayerCar.Velocity.X = 1;
                        else if (PlayerCar.Velocity.X < -1)
                            PlayerCar.Velocity.X = -1;
                    break;
                case "D":
                    if (RaceForm.Contains(RaceForm.OvertakenBot))
                        if (RaceForm.OvertakenBot.Bottom >= RaceForm.Player.Top &&
                            RaceForm.OvertakenBot.Top <= RaceForm.Player.Bottom &&
                            RaceForm.OvertakenBot.Left - RaceForm.Player.Right < 15 &&
                            RaceForm.OvertakenBot.Left - RaceForm.Player.Right > -70)
                        {
                            PlayerCar.Velocity.X = 0;
                            break;
                        }
                    if (PlayerCar.Velocity.X < PlayerCar.MaxVelocity.X)
                        PlayerCar.Velocity.X += controlMultiplayer;
                    PlayerCar.Velocity.X = (int)PlayerCar.Velocity.X;
                    if (RaceForm.BotIsOvertaking)
                        if (PlayerCar.Velocity.X > 1)
                            PlayerCar.Velocity.X = 1;
                        else if (PlayerCar.Velocity.X < -1)
                            PlayerCar.Velocity.X = -1;
                    break;
            }
        }
    }

    public void UpdateCar(Specification specification)
    {
        if (PlayerCar.SpecificationsLevels[specification] == 5) return;
        PlayerCar.SpecificationsLevels[specification] += 1;
        switch (specification)
        {
            case Specification.Speed:
                PlayerCar.MaxVelocity = new System.Windows.Vector(PlayerCar.MaxVelocity.X, PlayerCar.MaxVelocity.Y + 2);
                break;
            case Specification.DRSTime:
                PlayerCar.DRSMaxTime += 10;
                break;
            case Specification.DRSBoost:
                PlayerCar.DRSMultiplier += 0.1;
                break;
            case Specification.Boost:
                PlayerCar.BoostMultiplayer += 0.1;
                break;
        }
    }
}