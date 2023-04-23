using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game;

public class RaceModel
{
    private bool GameIsGo;
    private int SectorNumberActual;
    private int SectorNumberNext;
    public List<int> Track;
    public int ActualSectorId;
    public int NextSectorId;
    public Economy Economy;
    public Race RaceForm;
    public Car PlayerCar;
    public Menu MenuAndGarage;
    public int StartPlace;
    public int Income;

    public RaceModel()
    {
        PlayerCar = new Car(new Point(50, 50), "McLaren");
        Track = new List<int>();
        ActualSectorId = 0;
        Economy = new(10000);
        Economy.RModel = this;
    }

    public void Start()
    {
        GameIsGo = false;
        ActualSectorId = 0;
        SectorNumberActual = 0;
        SectorNumberNext = 1;
        MakeTrack(5);
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

    public void StopRace()
    {
        throw new NotImplementedException();
    }

    public void FinishRace(int miliseconds, int finishPlace)
    {
        Income = (6 - finishPlace) * 200;
        Economy.Balance += Income;
        StartPlace = finishPlace;
        RaceForm.Finish();
    }

    public void DRS()
    {
        PlayerCar.DRSOn = !PlayerCar.DRSOn;
        if (PlayerCar.DRSUsedTime > PlayerCar.DRSMaxTime)
            PlayerCar.DRSOn = false;
    }

    public void MakeTrack(int trackLength)
    {
        var random = new Random();
        var prevgroup = -1;
        Track = new();
        Track.Add(1234);
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
            var c = random.Next(0, 3);
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
        var x = PlayerCar.Location.X;
        int k = (int)(PanelSize.Width * 3.0 / 1100);
        if (GameIsGo && PlayerCar.Velocity.Y != 0)
        {
            if ((x <= 0 && PlayerCar.Velocity.X < 0) || (x >= PanelSize.Width && PlayerCar.Velocity.X > 0)) return;
            PlayerCar.Location.X += (int)(PlayerCar.Velocity.X * k);
        }
    }

    public void ChangeSpeed(string keyWS, string keyAD)
    {
        var controlMultiplayer = 0.8 + (double)PlayerCar.SpecificationsLevels[Specification.Control] / 10;
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
                    break;
            }
            switch (keyAD)
            {
                case "A":
                    if (PlayerCar.Velocity.X > -PlayerCar.MaxVelocity.X)
                        PlayerCar.Velocity.X -= controlMultiplayer;
                    break;
                case "D":
                    if (PlayerCar.Velocity.X < PlayerCar.MaxVelocity.X)
                        PlayerCar.Velocity.X += controlMultiplayer;
                    break;
            }
        }
    }

    public void ChangeSector()
    {
        if (SectorNumberNext + 1 == Track.Count)
        {
            NextSectorId = -1;
            ActualSectorId = Track[SectorNumberActual + 1];
        }
        else
        {
            SectorNumberActual = SectorNumberNext;
            ActualSectorId = Track[SectorNumberActual];
            SectorNumberNext += 1;
            NextSectorId = Track[SectorNumberNext];
        }
    }

    public void UpdateCar(Specification specification)
    {
        PlayerCar.SpecificationsLevels[specification] += 1;
        switch (specification)
        {
            case Specification.Speed:
                PlayerCar.MaxVelocity.Y += 2;
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