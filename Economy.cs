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
        Price = new()
        {
            [Specification.Speed] = 100,
            [Specification.DRSTime] = 100,
            [Specification.DRSBoost] = 100,
            [Specification.Boost] = 100,
            [Specification.Control] = 100
        };
    }

    public bool CanBuy(Specification specification) => Price[specification] <= Balance;

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