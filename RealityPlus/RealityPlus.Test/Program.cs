using RealityPlus.Models.Player;
using RealityPlus.Test.Clients;

internal sealed class Program
{
    private static UserClient User = new UserClient();

    private static void Main()
    {
        Console.WriteLine("Awaiting for the Game engine to Spin up");
        // Ensure the server has enough time to spin up
        Thread.Sleep(5000);

        Console.WriteLine("Starting Tests");
        MainAsync().GetAwaiter().GetResult();
        Console.WriteLine("Tests Completed");

    }

    private static async Task MainAsync()
    {
        Console.WriteLine("Testing Basic User functionality");
        var testUser = new UserDetailsWithPassword
        {
            UserName = "test123",
            FirstName = "CW",
            LastName = "testign1",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 55,
            Email = "cw@test.com",
            Password = "5678"
        };
        var playerId = await User.CreateUser(testUser);
        if (!playerId.HasValue)
        {
            throw new Exception("Create Player returned no Id");
        }
        var testPlayer = await User.GetUserDetails(playerId.Value);
        if (testPlayer == null)
        {
            throw new Exception("Saved Player didn't return from server");
        }
        EnuseValuesMatch(testUser.UserName, testPlayer.UserName);
        EnuseValuesMatch(testUser.FirstName, testPlayer.FirstName);
        EnuseValuesMatch(testUser.LastName, testPlayer.LastName);
        EnuseValuesMatch(testUser.Region, testPlayer.Region);
        EnuseValuesMatch(testUser.Rank, testPlayer.Rank);
        EnuseValuesMatch(testUser.Email, testPlayer.Email);

        Console.WriteLine("Basic User Passed");
    }



    private static void EnuseValuesMatch(object expectedValue, object actualValue)
    {
        if (expectedValue.ToString() != actualValue.ToString())
        {
            throw new Exception($"{actualValue} should be {expectedValue}");
        }
    }
}





