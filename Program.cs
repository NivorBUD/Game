namespace Game;
public class Program
{
    public static RaceModel RModel = new();
    public static Race RForm = new();
    public static MenuAndGarage MenuAndGarageForm = new();

    public static void Main()
    {
        System.Windows.Forms.Application.Run(MenuAndGarageForm);
    }
}