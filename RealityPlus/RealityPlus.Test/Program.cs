using RealityPlus.Models.Player;
using RealityPlus.Test.Clients;
using System.Text.RegularExpressions;

internal sealed class Program
{
    private static UserClient User = new UserClient();
    private static SessionClient SessionClient = new SessionClient();
    private static MatchClient MatchClient = new MatchClient();


    private const string LowLoggedInUser = "lowUser";
    private static string MediumLoggedInUser = "mediumUser";
    private static string HighLoggedInUser = "highUser";
    private static string MatchingDiffRegionUser = "differntRegion";

    private static Guid MediumUserSession;

    private static void Main()
    {
        Console.WriteLine("Awaiting for the Game engine to Spin up");
        // Ensure the server has enough time to spin up
        Thread.Sleep(1000);

        Console.WriteLine("Starting Tests");
        MainAsync().GetAwaiter().GetResult();
        Console.WriteLine("Tests Completed");


        Console.WriteLine("Press return to close");
        Console.ReadLine();
    }

    private static async Task MainAsync()
    {
        await TestBasicUserAndSession();
        await RampUpUsers();
        await TestQuickMatch();
        await TestRankMatch();
    }

    private static async Task TestQuickMatch()
    {
        Console.WriteLine("");
        Console.WriteLine("Quick Matches");
        Console.WriteLine("");
        var sessionId = await SessionClient.Login("test123", "5678");
        if (!sessionId.HasValue)
        {
            throw new Exception("Player failed to log in with the correct password");
        }
        var matches = await MatchClient.Match(sessionId.Value, false);
        EnuseValuesMatch(4, matches.Count());
        foreach (var match in matches)
        {
            Console.WriteLine(match);
        }
        Console.WriteLine("");
        await SessionClient.Logout(sessionId.Value);
    }

    private static async Task TestRankMatch()
    {
        Console.WriteLine("");
        Console.WriteLine("Ranked Matches (Closest logged in)");
        Console.WriteLine("");
        var sessionId = await SessionClient.Login("test123", "5678");
        if (!sessionId.HasValue)
        {
            throw new Exception("Player failed to log in with the correct password");
        }
        var matches = await MatchClient.Match(sessionId.Value, true);
        EnuseValuesMatch(1, matches.Count());
        EnuseValuesMatch(MediumLoggedInUser, matches.First());
        foreach (var match in matches)
        {
            Console.WriteLine(match);
        }
        Console.WriteLine("");
        Console.WriteLine("Ranked Matches (Closest logged out in)");
        await SessionClient.Logout(MediumUserSession);
        matches = await MatchClient.Match(sessionId.Value, true);
        EnuseValuesMatch(2, matches.Count());
 
        //EnuseValuesMatch(MediumLoggedInUser, matches.First());
        foreach (var match in matches)
        {
            Console.WriteLine(match);
        }
        if (!matches.Contains(LowLoggedInUser) || !matches.Contains(HighLoggedInUser))
        {
            throw new Exception("The Users for wide match are not what is expected");
        }
        Console.WriteLine("");
        await SessionClient.Logout(sessionId.Value);
    }


    private static async Task RampUpUsers()
    {
        var lowUserNotLoggedIn = new UserDetailsWithPassword
        {
            UserName = "low1",
            FirstName = "low",
            LastName = "1",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 15,
            Email = "low@1.com",
            Password = "testPass"
        };
        await User.CreateUser(lowUserNotLoggedIn);
        var lowUserLoggedIn = new UserDetailsWithPassword
        {
            UserName = LowLoggedInUser,
            FirstName = "low",
            LastName = "2",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 17,
            Email = "low@1.com",
            Password = "testPass"
        };
        await User.CreateUser(lowUserLoggedIn);
        await SessionClient.Login(LowLoggedInUser, "testPass");

        var medUserNotLoggedIn = new UserDetailsWithPassword
        {
            UserName = "med1",
            FirstName = "med",
            LastName = "1",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 35,
            Email = "med@1.com",
            Password = "testPass"
        };

        await User.CreateUser(medUserNotLoggedIn);
        var medUserLoggedIn = new UserDetailsWithPassword
        {
            UserName = MediumLoggedInUser,
            FirstName = "med",
            LastName = "2",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 35,
            Email = "med@1.com",
            Password = "testPass"
        };
        await User.CreateUser(medUserLoggedIn);
        MediumUserSession = (await SessionClient.Login(MediumLoggedInUser, "testPass")).GetValueOrDefault();

        var highUserNotLoggedIn = new UserDetailsWithPassword
        {
            UserName = "high1",
            FirstName = "high",
            LastName = "1",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 54,
            Email = "high@1.com",
            Password = "testPass"
        };

        await User.CreateUser(highUserNotLoggedIn);
        var highUserLoggedIn = new UserDetailsWithPassword
        {
            UserName = HighLoggedInUser,
            FirstName = "high",
            LastName = "2",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 52,
            Email = "high1.com",
            Password = "testPass"
        };
        await User.CreateUser(highUserLoggedIn);
        await SessionClient.Login(HighLoggedInUser, "testPass");

        var diffRegionUser = new UserDetailsWithPassword
        {
            UserName = MatchingDiffRegionUser,
            FirstName = "diff",
            LastName = "user",
            Region = RealityPlus.Models.Common.ERegion.NorthAmerica,
            Rank = 52,
            Email = "diff.user@test.com",
            Password = "testPass"
        };
        await User.CreateUser(diffRegionUser);
        await SessionClient.Login(MatchingDiffRegionUser, "testPass");
    }

    private static async Task TestBasicUserAndSession()
    {
        Console.WriteLine("Testing Basic User functionality");
        var testUser = new UserDetailsWithPassword
        {
            UserName = "test123",
            FirstName = "CW",
            LastName = "testign1",
            Region = RealityPlus.Models.Common.ERegion.Europe,
            Rank = 33,
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

        Console.WriteLine("Testing SessionClient Functionality");
        var sessionId = await SessionClient.Login("test123", "5678");
        if (!sessionId.HasValue)
        {
            throw new Exception("Player failed to log in with the correct password");
        }
        var loggedInUser = await SessionClient.GetUserDetails(sessionId.Value);
        if (loggedInUser == null)
        {
            throw new Exception("No Details returned for the login user");
        }
        EnuseValuesMatch(testUser.UserName, loggedInUser.UserName);
        EnuseValuesMatch(testUser.FirstName, loggedInUser.FirstName);
        EnuseValuesMatch(testUser.LastName, loggedInUser.LastName);
        EnuseValuesMatch(testUser.Region, loggedInUser.Region);
        EnuseValuesMatch(testUser.Rank, loggedInUser.Rank);
        EnuseValuesMatch(testUser.Email, loggedInUser.Email);

        try
        {
            var _ = await SessionClient.Login("test123", "NoThePassword");
        } catch (Exception ex)
        {
            EnuseValuesMatch("User already logged in", ex.Message);
        }

        await SessionClient.Logout(sessionId.Value);
        try
        {
            var _ = await SessionClient.Login("test123", "TryAgain");
        }
        catch (Exception ex)
        {
            EnuseValuesMatch("user or password incorrect", ex.Message);
        }

        Console.WriteLine("Tested SessionClient Functionality");
    }

    private static void EnuseValuesMatch(object expectedValue, object actualValue)
    {
        if (expectedValue.ToString().Trim() != actualValue.ToString().Trim())
        {
            throw new Exception($"'{actualValue}' should be '{expectedValue}'");
        }
    }
}





