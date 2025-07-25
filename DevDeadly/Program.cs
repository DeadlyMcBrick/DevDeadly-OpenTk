//Windows Screen
using DevDeadly;
using OpenTK.Windowing.Common;
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(1600, 1000, "DevDeadly Project"))
        {
            game.Run();
        }
    }
}
