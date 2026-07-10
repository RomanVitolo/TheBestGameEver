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

        public static async Task<ISession> HostAsync(int maxPlayers)
        {
            await InitializeAsync();

            // Private: not listed in queries or quick-join, but still joinable with the code.
            var options = new SessionOptions
            {
                MaxPlayers = maxPlayers,
                IsPrivate = true
            }.WithRelayNetwork();

            Current = await MultiplayerService.Instance.CreateSessionAsync(options);
            return Current;
        }

        public static async Task<ISession> JoinAsync(string joinCode)
        {
            await InitializeAsync();

            Current = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
            return Current;
        }

        public static async Task LeaveAsync()
        {
            if (Current == null) return;

            await Current.LeaveAsync();
            Current = null;
        }
    }
}
