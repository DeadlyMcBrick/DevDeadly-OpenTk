//Windows Screen
using DevDeadly;
using OpenTK.Windowing.Common;
class Program
{
    static void Main(string[] args)
    {
        //using var player = new AudioPlayer("wrld.wav");

        //Console.WriteLine("Reproduciendo audio...");
        //player.Play();

        using (Game game = new Game(1280, 800, "DevDeadly Project"))
        {
            game.Run();
        }
    } 
}
