using System;
using System.Windows.Forms;

namespace Game;
public partial class Race : Form
{
    private static Timer Timer;
    private static Timer SpeedTimer;
    private static Timer UpdateTimer;

    private void InitializeTimers()
    {
        Timer = new Timer
        {
            Interval = 1
        };
        Timer.Tick += Timer_Tick;

        SpeedTimer = new Timer
        {
            Interval = 100,
        };
        SpeedTimer.Tick += SpeedTimer_Tick;

        UpdateTimer = new Timer
        {
            Interval = 10,
        };
        UpdateTimer.Tick += UpdateTimer_Tick;

        DRSTimer = new Timer
        {
            Interval = 20,
        };
        DRSTimer.Tick += DRSTimer_Tick;
    }

    private void DRSTimer_Tick(object sender, EventArgs e)
    {
        if (RaceModel.PlayerCar.DRSOn) RaceModel.PlayerCar.DRSUsedTime++;
        if (RaceModel.PlayerCar.DRSOn && RaceModel.PlayerCar.DRSUsedTime >= RaceModel.PlayerCar.DRSMaxTime)
            RaceModel.DRS();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        RaceTimeBox.Text = GetTime();
        RaceModel.Move(Size);
        if (RaceModel.GameIsGo && RaceModel.SectorNumberActual < 6)
            MoveBotsFromStart();
        DrawCar();

        if (LightN >= 6 && RaceModel.ActualSectorId != 1234)
            Light.Image = ResizeImage(bitmaps["Light0.png"], Light.Size);
    }

    public void LightTimer_Tick(object sender, EventArgs e)
    {
        var timer = (Timer)sender;
        LightN++;
        if (LightN <= 5)
            Light.Image = ResizeImage(bitmaps["Light" + LightN + ".png"], Light.Size);
        if (LightN == 6)
        {
            Light.Image = ResizeImage(bitmaps["Light0.png"], Light.Size);
            RaceModel.StartRace();
            timer.Stop();
        }
    }

    private void SpeedTimer_Tick(object sender, EventArgs e)
    {
        RaceModel.ChangeSpeed(KeyWS.KeyCode.ToString(), KeyAD.KeyCode.ToString());
        RaceModel.PlayerCar.OnRoad = CheckRoad();
        if (!RaceModel.PlayerCar.OnRoad)
            TimeOffTheRoad++;
    }

    private void UpdateTimer_Tick(object sender, EventArgs e)
    {
        ChangeBackground();
        ChangeDRSStatus();
    }

    private void StopTimers()
    {
        Timer.Stop();
        SpeedTimer.Stop();
        UpdateTimer.Stop();
        DRSTimer.Stop();
    }

    private void StartTimers()
    {
        Timer.Start();
        SpeedTimer.Start();
        UpdateTimer.Start();
        DRSTimer.Start();
    }
}