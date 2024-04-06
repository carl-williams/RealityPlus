using RealityPlus.Server.DI;
using RealityPlus.Server.Server;

internal sealed class Program
{

    private static void Main()
    {
        Console.WriteLine("Starting Game Server...");
        using (var server = DependancyInjectrion.GetRequiredService<Listener>())
        {
            server.Start();
            Console.WriteLine("Started Game Server...");

            while (true)
            {
                Thread.Sleep(1);
            }
        };
    }
}



