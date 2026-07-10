using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Wraps the Multiplayer Services Sessions API, which bundles Lobby and Relay behind a join code.
    ///
    /// Creating or joining a session starts NetworkManager for us — the SDK's NetworkManagerSession calls
    /// StartHost/StartClient internally. Never call those yourself here, or NGO starts twice.
    /// </summary>
    public static class SessionService
    {
        public static ISession Current { get; private set; }

        public static bool IsHost => Current != null && Current.IsHost;

        /// <summary>Anonymous sign-in. Requires the project to be linked to a Unity Cloud org.</summary>
        public static async Task InitializeAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public static async Task<ISession> HostAsync(string sessionName, int maxPlayers)
        {
            await InitializeAsync();

            // Public so it shows up in the lobby browser. Still joinable by code as well as by id.
            var options = new SessionOptions
            {
                Name = string.IsNullOrWhiteSpace(sessionName) ? "Session" : sessionName.Trim(),
                MaxPlayers = maxPlayers,
                IsPrivate = false
            }.WithRelayNetwork();

            Current = await MultiplayerService.Instance.CreateSessionAsync(options);
            return Current;
        }

        public static async Task<ISession> JoinByCodeAsync(string joinCode)
        {
            await InitializeAsync();

            Current = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            return Current;
        }

        public static async Task<ISession> JoinByIdAsync(string sessionId)
        {
            await InitializeAsync();

            Current = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
            return Current;
        }

        /// <summary>Lists public, joinable sessions for the lobby browser.</summary>
        public static async Task<IList<ISessionInfo>> QueryAsync()
        {
            await InitializeAsync();

            var options = new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }

        public static async Task LeaveAsync()
        {
            if (Current == null) return;

            await Current.LeaveAsync();
            Current = null;
        }
    }
}
