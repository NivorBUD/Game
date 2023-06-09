﻿using System.Collections.Generic;
using System.Windows;

namespace Game;

public class Car
{
    public Vector MaxVelocity;
    public System.Drawing.Point Location;
    public Vector Velocity;
    public bool DRSOn;
    public int DRSMaxTime;
    public int DRSUsedTime;
    public double DRSMultiplier;
    public double BoostMultiplayer;
    public Dictionary<Specification, int> SpecificationsLevels;
    public bool OnRoad;
    public int Place;
    public string CarBrand { get; private set; }

    public Car(System.Drawing.Point location, string brand)
    {
        Place = 5;
        Location = location;
        CarBrand = brand;
        DRSMaxTime = 50;
        DRSOn = false;
        MaxVelocity = new Vector(12, 20);
        DRSMultiplier = 1.1;
        OnRoad = true;
        SpecificationsLevels = new();
    }

    public void ToStartValues()
    {
        DRSUsedTime = 0;
        DRSOn = false;
        Velocity = new Vector(0, 0);
    }
}