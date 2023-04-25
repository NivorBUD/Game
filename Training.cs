using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Game;

public partial class Menu : Form
{
    private Button ToGarageFromTraining;
    private int TrainingTalkNumber;
    private List<string> TrainingTalks;
    private List<Control> TrainingControlsList;

    private void GoToTraining()
    {
        MakeTraining();
    }

    private void MakeTraining()
    {
        BackgroundImage = ResizeImage(Program.RForm.bitmaps["Training.png"], Size);
        TrainingTalkNumber = 0;
        Manager.Location = new Point(Size.Width - Manager.Width, (int)(Size.Height - Manager.Height * 1.1) * 4 / 5);
        Talk.Location = new Point(0, Manager.Top);
        Talk.Text = TrainingTalks[TrainingTalkNumber];
        MouseClick += (o, e) =>
        {
            if (TrainingTalkNumber >= TrainingTalks.Count - 1)
            {
                ToGarageFromTraining.Visible = true;
                return;
            }
            TrainingTalkNumber++;
            Talk.Text = TrainingTalks[TrainingTalkNumber];
        };
        foreach (var e in TrainingControlsList)
            Controls.Add(e);
        ToGarageFromTraining.Visible = false;
    }

    private void MakeTrainingControls()
    {
        TrainingTalkNumber = 0;
        MakeTrainingTalks();
        MakeTrainingButton();
        MakeTrainingControlsList();
    }

    private void MakeTrainingControlsList()
    {
        TrainingControlsList = new()
        {
            ToGarageFromTraining,
            Manager, //из Story
            Talk     //из Story
        };
    }

    private void MakeTrainingButton()
    {
        ToGarageFromTraining = new()
        {
            Size = new Size((int)(Size.Width * 0.4), (int)(Size.Width * 0.07)),
            Text = "В гараж",
            Font = new Font("Ariel", 40),
            BackColor = Color.Transparent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
        };
        ToGarageFromTraining.Location = new Point((Talk.Width - ToGarageFromTraining.Width) / 2, Manager.Bottom);
        ToGarageFromTraining.Click += (s, e) =>
        {
            Controls.Clear();
            GoToGarage();
        };
        ToGarageFromTraining.Paint += Button_Paint;
        ToGarageFromTraining.FlatAppearance.MouseOverBackColor = Color.Transparent;
        ToGarageFromTraining.FlatAppearance.MouseDownBackColor = Color.Transparent;
        ToGarageFromTraining.MouseEnter += (s, e) => ToGarageFromTraining.FlatAppearance.MouseOverBackColor = Color.FromArgb(120, Color.Black);
        ToGarageFromTraining.FlatAppearance.BorderSize = 0;
    }

    private void MakeTrainingTalks()
    {
        TrainingTalks = new()
        {
            "Решил вспомнить основы управления машиной и её улучшения?" +
            "Хорошо, давай напомню тебе быстренько основы.",

            "Основное управление происходит с помощью клавиш W, A, S, D." +
            "Это было очевидно, тут ничего нового, только тебе надо учесть один факт," +
            "в нашей машине не предусмотрено движение назад, как и задняя передача, только вперед.",

            "И так, дальше. Следующее, про что хотелось бы тебе рассказать - DRS." +
            "Drag Racing System - если интересно, то его суть заключается в изменении угла атаки заднего антикрыла, что дает хороший прирост скорости.",

            "Ну ладно, как этим управлять? В заезде внизу справа у тебя будет индикатор DRS, который будет показывать," +
            "включен он у тебя или нет, а также полоска, которая показывает, сколько времени использования DRS у тебя осталось.",

            "Да, у этой системы есть свои ограничения на время использования, которое можно увеличивать, все там же, в гараже. " +
            "Кстати, забыл тебе сказать про включение, оно происходит на кнопку E, как и выключение.",

            "Кстати, про гараж, в нем ты можешь улучшать скорость, время использования DRS, ускорение при DRS, " +
            "ускорение самого болида и управление. Всё это стоит денег, у тебя будет информация о стоимости каждого улучшения.",

            "Изначально у тебя будет 1000$, это от меня, не благодари.",

            "И так, в целом, ты готов, пора внести бару обновлений в болид и начать заезд.",
        };
    }
}