//Windows Screen
using DevDeadly;
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(900, 800, "DevDeadlyGPS"))
        {
            game.Run();
        }
    }
}
