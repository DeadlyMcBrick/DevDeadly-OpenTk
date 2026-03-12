//Windows Screen
using DevDeadly;
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(1200, 800, "DevDeadly Project"))
        {
            game.Run();
        }
    }
}
