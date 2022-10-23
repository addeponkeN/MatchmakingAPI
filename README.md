# MatchmakingAPI


<?xml version="1.0"?>
<doc>
    <assembly>
        <name>Rovio.Matchmaking.Api</name>
    </assembly>
    <members>
        <member name="T:Rovio.Matchmaking.Api.Controllers.GameServicesController">
            <summary>
            Handles out tokens to clients
            </summary>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.GameServicesController.RegisterGame(Rovio.Matchmaking.Models.ServerModel)">
            <summary>
            Returns a token that is needed for the Matchmaking API calls.
            </summary>
            <param name="server">ServerModel containing a </param>
            <returns>A model containing the Game Service Id and the token that is required to make any Matchmaking API calls</returns>
        </member>
        <member name="T:Rovio.Matchmaking.Api.Controllers.MatchmakingController">
            <summary>
            Handles the Matchmaking API calls.
            A token is required to make any calls.
            A token can be retrieved from the <see cref="T:Rovio.Matchmaking.Api.Controllers.GameServicesController"/>.
            <remarks>Note to self: A token may not be required if used internally!</remarks>
            </summary>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.GetTest">
            <summary>
            Test
            </summary>
            <returns>API result</returns>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.AddPlayer(System.Guid,Rovio.Matchmaking.Models.PlayerModel)">
            <summary>
            Add a single player to the matchmaking queue
            </summary>
            <param name="token">Server token</param>
            <param name="player">Player to add</param>
            <returns>API result</returns>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.AddPlayers(System.Guid,Rovio.Matchmaking.Models.PlayerGroupModel)">
            <summary>
            Add multiple players to the matchmaking queue
            </summary>
            <param name="token">Server token</param>
            <param name="group">Players to add</param>
            <returns>API result</returns>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.GetReadySessions(System.Guid,Rovio.Utility.Continents)">
            <summary>
            Get all ready sessions.
            </summary>
            <param name="token">Server token</param>
            <param name="continent">The continent to get sessions from</param>
            <returns>List of all ready sessions</returns>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.GetReadyOngoingSessions(System.Guid,Rovio.Utility.Continents)">
            <summary>
            Get all ongoing sessions. 
            </summary>
            <param name="token">Server token</param>
            <param name="continent">The continent of the server</param>
            <returns>Collection of all ready ongoing sessions</returns>
        </member>
        <member name="M:Rovio.Matchmaking.Api.Controllers.MatchmakingController.AddOngoingSession(System.Guid,Rovio.Matchmaking.Models.OngoingSessionsModel)">
            <summary>
            Add an ongoing session to the matchmaker.
            </summary>
            <param name="token">Server token</param>
            <param name="session">Session to add</param>
            <returns>API result</returns>
        </member>
        <member name="F:Rovio.Matchmaking.Api.Services.MatchmakingService._updateFrequency">
            <summary>
            In milliseconds
            </summary>
        </member>
    </members>
</doc>
