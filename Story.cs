using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Label Talk;
    private PictureBox Manager;
    private Button ToGarageFromStory;
    private Button ToTrainingFromStory;
    private List<Control> StoryControlsList;
    private int StoryTalkNumber;
    private List<string> StoryTalks;

    private void GoToStory()
    {
        MakeStory();
    }

    private void MakeStory()
    {
        StoryTalkNumber = 0;
        Manager.Location = new Point(Size.Width - Manager.Width, (int)(Size.Height - Manager.Height * 1.1) * 4 / 5);
        Talk.Location = new Point(0, Manager.Top);
        Talk.Text = StoryTalks[StoryTalkNumber];
        MouseClick += (o, e) =>
        {
            if (StoryTalkNumber >= StoryTalks.Count - 1)
            {
                ToGarageFromStory.Visible = true;
                ToTrainingFromStory.Visible = true;
                return;
            }
            StoryTalkNumber++;
            Talk.Text = StoryTalks[StoryTalkNumber];
        };
        BackgroundImage = ResizeImage(Program.RForm.bitmaps["Story.png"], Size);
        foreach (var e in StoryControlsList) 
            Controls.Add(e);
        ToGarageFromStory.Visible = false;
        ToTrainingFromStory.Visible = false;
    }

    private void MakeStoryControls()
    {
        StoryTalkNumber = 0;
        MakeStoryTalks();
        MakeManagerAndTalk();
        MakeStoryButtons();
        MakeStoryControlsList();
    }

    private void MakeManagerAndTalk()
    {
        Manager = new()
        {
            Size = new Size(Size.Width / 8, (int)(Size.Width / 8 * 1.082)),
            BackColor = Color.Transparent,
        };
        Manager.Image = ResizeImage(Program.RForm.bitmaps["Manager.png"], Manager.Size);
        Manager.Location = new Point(Size.Width - Manager.Width, (int)(Size.Height - Manager.Height * 1.1) * 4 / 5);

        Talk = new()
        {
            Text = StoryTalks[0],
            Font = new Font("Ariel", 50),
            Location = new Point(0, Manager.Top),
            Size = new Size(Size.Width - Manager.Width, Manager.Height),
            BackColor = Color.FromArgb(100, Color.Black),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            FlatStyle = FlatStyle.Flat,
            
        };
    }

    private void MakeStoryButtons()
    {
        MakeToGarageFromStoryButton();
        MakeToTrainingFromStoryButton();
    }

    private void MakeToTrainingFromStoryButton()
    {
        ToTrainingFromStory = new()
        {
            Size = new Size((int)(Size.Width * 0.35), (int)(Size.Width * 0.07)),
            Location = new Point(ToGarageFromStory.Right + Size.Width / 20, Manager.Bottom),
            Text = "А ну, напомни в двух словах, как\nдолжен управлять этим пепелацом.",
            Font = new Font("Ariel", 50),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };
        ToTrainingFromStory.Click += (s, e) =>
        {
            Controls.Clear();
            GoToTraining();
        };
        ToTrainingFromStory.Paint += Button_Paint;
        ToTrainingFromStory.FlatAppearance.MouseOverBackColor = Color.Transparent;
        ToTrainingFromStory.FlatAppearance.MouseDownBackColor = Color.Transparent;
        ToTrainingFromStory.MouseEnter += (s, e) => ToTrainingFromStory.FlatAppearance.MouseOverBackColor = Color.FromArgb(120, Color.Black);
        ToTrainingFromStory.FlatAppearance.BorderSize = 0;
    }

    private void MakeToGarageFromStoryButton()
    {
        ToGarageFromStory = new()
        {
            Size = new Size((int)(Size.Width * 0.35), (int)(Size.Width * 0.07)),
            Location = new Point(Size.Width / 7, Manager.Bottom),
            Text = "В целом, я и так помню,\nкак управлять болидом,\nпошли в гараж и гонять!",
            Font = new Font("Ariel", 50),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };
        ToGarageFromStory.Click += (s, e) =>
        {
            Controls.Clear();
            GoToGarage();
        };
        ToGarageFromStory.Paint += Button_Paint;
        ToGarageFromStory.FlatAppearance.MouseOverBackColor = Color.Transparent;
        ToGarageFromStory.FlatAppearance.MouseDownBackColor = Color.Transparent;
        ToGarageFromStory.MouseEnter += (s, e) => ToGarageFromStory.FlatAppearance.MouseOverBackColor = Color.FromArgb(120, Color.Black);
        ToGarageFromStory.FlatAppearance.BorderSize = 0;
    }

    private void Button_Paint(object sender, PaintEventArgs e)
    {
        var obj = sender as Button;
        float fontSize = NewFontSize(e.Graphics, obj.Size, obj.Font, obj.Text);
        Font f = new("Times New Roman", fontSize, FontStyle.Regular);
        obj.Font = f;
    }

    private void MakeStoryControlsList()
    {
        StoryControlsList = new()
        {
            Talk,
            Manager,
            ToGarageFromStory,
            ToTrainingFromStory
        };
    }

    private void MakeStoryTalks()
    {
        StoryTalks = new()
        {
            "Привет, впервые в паддоке?",

            "Можешь не отвечать, вижу, что ты тут впервые. ",

            "Хммм, ...",

            "Ты, получается, тот новичок, который пришел к нам, в формулу 1, из младших серий. \n" + 
            "Надеюсь, что ты готов к соревнованиям, ведь у меня есть для тебя контракт с одной командой.",

            "У McLaren сейчас дела не очень, их команда не может занять нормальные места в гонках, машина не очень удалась.\n" +
            "Они единственные рискнули на контракт с молодым пилотом, то бишь, с тобой, ведь у других всё в порядке.",

            "Тебе придется посоревноваться с сильными командами, у этой команды на тебя большие надежды.",

            "Мдаааа, раньше команда британской автомобильной компании была успешной, чего стоили заезды Айртона Сенны...",

            "Ну ладно, не зачем нам прошлое ворошить, пора творить будущее и завоёвывать новые награды, подиумы и отличные заезды.",

            "Новичок, готов ли ты пойти в гараж за улучшениями и начать свой первый заезд или " +
            "мне сначала напомнить тебе про основные элементы управления болидом и про его улучшения?",
        };
    }
}