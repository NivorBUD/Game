
using System.Collections.Generic;
using System.Drawing;
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
    public string CarBrand { get; private set; }

    public Car(System.Drawing.Point location, string brand)
    {
        Location = location;
        CarBrand = brand;
        DRSMaxTime = 50;
        DRSOn = false;
        MaxVelocity = new Vector(8, 20);
        DRSMultiplier = 0.1;

        SpecificationsLevels = new();
        var allSpecifications = new Specification[] { Specification.Speed, Specification.DRSTime, 
            Specification.DRSBoost, Specification.Boost, Specification.Control};
        foreach (var e in allSpecifications)
            SpecificationsLevels[e] = 1;
    }
}