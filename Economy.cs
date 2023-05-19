using System;
using System.Collections.Generic;

namespace Game;

public class Economy
{
    public int Balance;
    public RaceModel RModel;
    public Dictionary<Specification, int> Price;

    public Economy(int balance)
    {
        Balance = balance;
    }

    public void MakePrices()
    {
        Price = new()
        {
            [Specification.Speed] = (int)(100 * Math.Pow(2, RModel.PlayerCar.SpecificationsLevels[Specification.Speed] - 1)),
            [Specification.DRSTime] = (int)(100 * Math.Pow(2, RModel.PlayerCar.SpecificationsLevels[Specification.DRSTime] - 1)),
            [Specification.DRSBoost] = (int)(100 * Math.Pow(2, RModel.PlayerCar.SpecificationsLevels[Specification.DRSBoost] - 1)),
            [Specification.Boost] = (int)(100 * Math.Pow(2, RModel.PlayerCar.SpecificationsLevels[Specification.Boost] - 1)),
            [Specification.Control] = (int)(100 * Math.Pow(2, RModel.PlayerCar.SpecificationsLevels[Specification.Control] - 1))
        };
    }

    public bool CanBuy(Specification specification) => 
        Price[specification] <= Balance && RModel.PlayerCar.SpecificationsLevels[specification] < 5;

    public void Buy(Specification specification)
    {
        RModel.UpdateCar(specification);
        Balance -= Price[specification];
        Price[specification] *= 2;
        RModel.MenuAndGarage.UpdateButtons();
        RModel.MenuAndGarage.UpdateProgressBars();
        RModel.MenuAndGarage.UpdateBalanceInfo(specification);
    }
}