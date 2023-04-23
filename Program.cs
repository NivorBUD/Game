namespace Game;
public class Program
{
    public static RaceModel RModel = new();
    public static Race RForm = new();
    public static Menu MenuAndGarageForm = new();

    public static void Main()
    {
        System.Windows.Forms.Application.Run(MenuAndGarageForm);
    }
}