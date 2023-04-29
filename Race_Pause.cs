using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game;
public partial class Race : Form
{
    private Button Continue;
    private Button Exit;

    private void GoToPause()
    {
        MakePause();
    }

    private void MakePause()
    {
        RaceModel.PauseRace();
        Controls.Add(Continue);
        Controls.Add(Exit);
        Continue.BringToFront();
        Exit.BringToFront();
    }

    private void MakePauseButtons()
    {
        Continue = new()
        {
            Size = new Size(Size.Width / 4, Size.Height / 9),
            Text = "Продолжить",
            Font = new Font("Ariel", 50),
            BackColor = Color.FromArgb(90, Color.Black),
            ForeColor = Color.Black,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        Continue.Location = new Point((Size.Width - Continue.Width) / 2, Size.Height / 2 - Continue.Height);
        Continue.Click += (s, e) =>
        {
            Controls.Remove(Continue);
            Controls.Remove(Exit);
            RaceModel.GameIsGo = true;
        };

        Exit = new()
        {
            Size = new Size(Size.Width / 9, Size.Height / 9),
            Text = "Выход",
            Font = new Font("Ariel", 50),
            BackColor = Color.FromArgb(90, Color.Black),
            ForeColor = Color.Black,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        Exit.Location = new Point((Size.Width - Exit.Width) / 2, Size.Height / 2 + Exit.Height);
        Exit.Click += (s, e) =>
        {
            Visible = false;
            RaceModel.MenuAndGarage.GoToMenu();
        };
    }
}