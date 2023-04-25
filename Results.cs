using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Label PlayerPlace;
    private Label Income;
    private Label MaxPlace;
    private Label LewisPlace;
    private Label FernandoPlace;
    private Label CharlePlace;
    private Button RepeatRace;
    private Button ToGarageFromResults;
    private Button ToMenuFromResults;
    private List<Control> ResultControls;

    public void GoToResults()
    {
        MakeResults();
    }

    public void MakeResults()
    {
        Visible = true;
        UpdateResultLabels();
        foreach (var e in ResultControls)
            Controls.Add(e);
    }

    private void MakeResultControls()
    {
        MakeResultButtons();
        MakeResultLabels();
        MakeResultControlsList();
    }

    private void MakeResultControlsList()
    {
        ResultControls = new()
        {
            PlayerPlace,
            Income,
            MaxPlace,
            LewisPlace,
            FernandoPlace,
            CharlePlace,
            RepeatRace,
            ToGarageFromResults,
            ToMenuFromResults
        };
    }

    public void MakeResultButtons()
    {
        RepeatRace = new()
        {
            Size = new Size((int)(Size.Width * 0.2), (int)(Size.Width * 0.07)),
            Location = new Point((int)(Size.Width * 0.05), (int)(Size.Height - Size.Width * 0.1)),
            Text = "ПОВТОРИТЬ ЗАЕЗД",
            Font = new Font("Ariel", 30)
        };
        RepeatRace.Click += (s, e) => 
        {
            Visible = false;
            Controls.Clear();
            RaceModel.Start();
        };

        ToGarageFromResults = new()
        {
            Size = new Size((int)(Size.Width * 0.4), RepeatRace.Height),
            Location = new Point((int)(RepeatRace.Right + Size.Width * 0.05), RepeatRace.Top),
            Text = "В ГАРАЖ",
            Font = new Font("Ariel", 30)
        };
        ToGarageFromResults.Click += (s, e) =>
        {
            Controls.Clear();
            GoToGarage();
        };

        ToMenuFromResults = new()
        {
            Size = new Size((int)(Size.Width * 0.2), RepeatRace.Height),
            Location = new Point((int)(ToGarageFromResults.Right + Size.Width * 0.05), RepeatRace.Top),
            Text = "В МЕНЮ",
            Font = new Font("Ariel", 30)
        };
        ToMenuFromResults.Click += (s, e) =>
        {
            Controls.Clear();
            GoToMenu();
        };

    }

    private void MakeResultLabels()
    {
        PlayerPlace = new()
        {
            Text = "#" + RaceModel.StartPlace,
            Font = new Font("Ariel", 300),
            Size = new Size((int)(Size.Width * 0.3), (int)(Size.Width * 0.15)),
            Location = new Point((int)(Size.Width * 0.01), (int)(Size.Width * 0.01)),
            BackColor = Color.Transparent,
            ForeColor = Color.Red,
        };

        Income = new()
        {
            Text = "+" + RaceModel.Income + "$",
            Font = new Font("Ariel", 200),
            Size = new Size((int)(Size.Width * 0.43), (int)(Size.Width * 0.12)),
            Location = new Point(PlayerPlace.Left, (int)(PlayerPlace.Bottom + Size.Width * 0.05)),
            BackColor = Color.Transparent,
            ForeColor = Color.Red,
        };
    }

    public void UpdateResultLabels()
    {
        PlayerPlace.Text = "#" + RaceModel.StartPlace;
        Income.Text = "+" + RaceModel.Income + "$";
    }
}