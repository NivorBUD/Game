using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Label PlayerResult;
    private Label Income;
    private Label MaxPlace;
    private Label LewisPlace;
    private Label FernandoPlace;
    private Label CharlePlace;
    private Label PlayerPlace;
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
        MakeResultTable();
        MakeResultControlsList();
    }

    private void MakeResultControlsList()
    {
        ResultControls = new()
        {
            PlayerResult,
            Income,
            MaxPlace,
            LewisPlace,
            FernandoPlace,
            CharlePlace,
            PlayerPlace,
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
            Font = new Font("Ariel", Size.Width > 2000 ? 30 : 23)
        };
        RepeatRace.Click += (s, e) => 
        {
            Visible = false;
            Controls.Clear();
            if (RaceModel.FinishPlace == 1)
            {
                GoToFinalStory();
                return;
            }
            RaceModel.Start();
        };

        ToGarageFromResults = new()
        {
            Size = new Size((int)(Size.Width * 0.4), RepeatRace.Height),
            Location = new Point((int)(RepeatRace.Right + Size.Width * 0.05), RepeatRace.Top),
            Text = "В ГАРАЖ",
            Font = new Font("Ariel", Size.Width > 2000 ? 30 : 23)
        };
        ToGarageFromResults.Click += (s, e) =>
        {
            Controls.Clear();
            if (RaceModel.FinishPlace == 1)
            {
                GoToFinalStory();
                return;
            }
            GoToGarage();
        };

        ToMenuFromResults = new()
        {
            Size = new Size((int)(Size.Width * 0.2), RepeatRace.Height),
            Location = new Point((int)(ToGarageFromResults.Right + Size.Width * 0.05), RepeatRace.Top),
            Text = "В МЕНЮ",
            Font = new Font("Ariel", Size.Width > 2000 ? 30 : 23)
        };
        ToMenuFromResults.Click += (s, e) =>
        {
            Controls.Clear();
            if (RaceModel.FinishPlace == 1)
            {
                GoToFinalStory();
                return;
            }
            GoToMenu();
        };
    }

    private void MakeResultLabels()
    {
        PlayerResult = new()
        {
            Text = "#" + RaceModel.FinishPlace,
            Font = new Font("Ariel", Size.Width > 2000 ? 300 : 200),
            Size = new Size((int)(Size.Width * 0.3), (int)(Size.Width * 0.15)),
            Location = new Point((int)(Size.Width * 0.01), (int)(Size.Width * 0.01)),
            BackColor = Color.Transparent,
            ForeColor = Color.Red,
        };

        Income = new()
        {
            Text = "+" + RaceModel.Income + "$",
            Font = new Font("Ariel", Size.Width > 2000 ? 200 : 120),
            Size = new Size((int)(Size.Width * 0.43), (int)(Size.Width * 0.12)),
            Location = new Point(PlayerResult.Left, (int)(PlayerResult.Bottom + Size.Width * 0.05)),
            BackColor = Color.Transparent,
            ForeColor = Color.Red,
        };
    }

    public void UpdateResultLabels()
    {
        PlayerResult.Text = "#" + RaceModel.StartPlace;
        Income.Text = "+" + RaceModel.Income + "$";
        SetTablePlaces();
    }

    private void MakeResultTable()
    {
        MaxPlace = new()
        {
            Text = "Max",
            Font = new Font("Ariel", Size.Width > 2000 ? 100 : 60),
            Size = new Size(Size.Width / 4, Size.Height / 10)
        };

        LewisPlace = new()
        {
            Text = "Lewis",
            Font = MaxPlace.Font,
            Size = MaxPlace.Size,
        };

        FernandoPlace = new()
        {
            Text = "Fernando",
            Font = MaxPlace.Font,
            Size = MaxPlace.Size
        };

        CharlePlace = new()
        {
            Text = "Charle",
            Font = MaxPlace.Font,
            Size = MaxPlace.Size
        };

        PlayerPlace = new()
        {
            Text = "You",
            Font = MaxPlace.Font,
            Size = MaxPlace.Size,
        };
    }

    private void SetTablePlaces()
    {
        var placeDict = new Dictionary<int, Point>();
        var leftEdge = Size.Width / 2;
        var height = (int)(MaxPlace.Height * 1.1);
        placeDict.Add(1, new Point(leftEdge, Size.Height / 20));
        placeDict.Add(2, new Point(leftEdge, placeDict[1].Y + height));
        placeDict.Add(3, new Point(leftEdge, placeDict[2].Y + height));
        placeDict.Add(4, new Point(leftEdge, placeDict[3].Y + height));
        placeDict.Add(5, new Point(leftEdge, placeDict[4].Y + height));

        MaxPlace.Location = RaceModel.FinishPlace > 1 ? placeDict[1] : placeDict[2];
        LewisPlace.Location = RaceModel.FinishPlace > 2 ? placeDict[2] : placeDict[3];
        FernandoPlace.Location = RaceModel.FinishPlace > 3 ? placeDict[3] : placeDict[4];
        CharlePlace.Location = RaceModel.FinishPlace > 4 ? placeDict[4] : placeDict[5];
        PlayerPlace.Location = placeDict[RaceModel.FinishPlace];
    }
}