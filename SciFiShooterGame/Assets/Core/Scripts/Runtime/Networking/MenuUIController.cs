using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Drives the menu scene: host a public session, join by code, or pick a session from the browser.
    /// Assign the Canvas widgets in the Inspector; nothing here builds UI itself.
    ///
    /// Only the host loads the combat scene, and only through NetworkManager.SceneManager. Clients follow
    /// automatically — a plain SceneManager.LoadScene here would move the host alone and strand every client.
    /// </summary>
    public class MenuUIController : MonoBehaviour
    {
        [Header("Session")]
        [SerializeField, Range(2, 4)] private int _maxPlayers = 4;

        [Tooltip("Query open lobbies automatically when the menu opens. Turn off if Unity Services isn't " +
                 "configured yet, so the browser doesn't show a sign-in error before you do anything.")]
        [SerializeField] private bool _refreshOnStart = true;

        [Header("Host")]
        [SerializeField] private Button _hostButton;
        [SerializeField] private TMP_InputField _sessionNameInput;
        [SerializeField] private TMP_Text _joinCodeLabel;

        [Header("Join by code")]
        [SerializeField] private Button _joinByCodeButton;
        [SerializeField] private TMP_InputField _joinCodeInput;

        [Header("Lobby browser")]
        [SerializeField] private TMP_Dropdown _lobbyDropdown;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _joinSelectedButton;

        [Header("Shared")]
        [SerializeField] private TMP_Text _statusLabel;

        // Parallel to the dropdown's options: index -> session id.
        private readonly List<string> _sessionIds = new();

        // A "leaving" op (host/join) is under way — lock the whole menu, we're on our way out.
        private bool _leaving;
        // A lobby query is under way — lock only the browser controls, never the host/join inputs.
        private bool _refreshing;

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinByCodeButton.onClick.AddListener(OnJoinByCodeClicked);
            _refreshButton.onClick.AddListener(OnRefreshClicked);
            _joinSelectedButton.onClick.AddListener(OnJoinSelectedClicked);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinByCodeButton.onClick.RemoveListener(OnJoinByCodeClicked);
            _refreshButton.onClick.RemoveListener(OnRefreshClicked);
            _joinSelectedButton.onClick.RemoveListener(OnJoinSelectedClicked);
        }

        private void Start()
        {
            RefreshInteractables();

            if (_refreshOnStart)
                _ = RefreshAsync();
        }

        private void OnHostClicked() => _ = HostAsync();
        private void OnJoinByCodeClicked() => _ = JoinByCodeAsync();
        private void OnRefreshClicked() => _ = RefreshAsync();
        private void OnJoinSelectedClicked() => _ = JoinSelectedAsync();

        private async Task HostAsync()
        {
            if (_leaving) return;
            SetLeaving(true);
            SetStatus("Creating session...");

            try
            {
                string sessionName = _sessionNameInput != null ? _sessionNameInput.text : null;
                ISession session = await SessionService.HostAsync(sessionName, _maxPlayers);

                SetJoinCode(session.Code);
                SetStatus($"Hosting. Join code: {session.Code}");

                // The session already started the host, so the server can drive the scene load. Stay locked:
                // the menu scene is about to unload.
                NetworkManager.Singleton.SceneManager.LoadScene(GameScenes.Combat, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                ReportFailure(exception);
                if (this != null) SetLeaving(false);
            }
        }

        private async Task JoinByCodeAsync()
        {
            string joinCode = _joinCodeInput != null ? _joinCodeInput.text.Trim() : string.Empty;

            if (string.IsNullOrEmpty(joinCode))
            {
                SetStatus("Enter a join code first.");
                return;
            }

            await JoinAsync(() => SessionService.JoinByCodeAsync(joinCode));
        }

        private async Task JoinSelectedAsync()
        {
            int index = _lobbyDropdown != null ? _lobbyDropdown.value : -1;

            if (index < 0 || index >= _sessionIds.Count)
            {
                SetStatus("Select a session from the list first.");
                return;
            }

            string sessionId = _sessionIds[index];
            await JoinAsync(() => SessionService.JoinByIdAsync(sessionId));
        }

        private async Task JoinAsync(Func<Task<ISession>> join)
        {
            if (_leaving) return;
            SetLeaving(true);
            SetStatus("Joining session...");

            try
            {
                await join();

                // No LoadScene here: NGO synchronizes this client into whatever scene the host is running.
                SetStatus("Joined. Loading...");
            }
            catch (Exception exception)
            {
                ReportFailure(exception);
                if (this != null) SetLeaving(false);
            }
        }

        private async Task RefreshAsync()
        {
            if (_leaving || _refreshing) return;
            _refreshing = true;
            RefreshInteractables();
            SetStatus("Refreshing lobbies...");

            try
            {
                IList<ISessionInfo> sessions = await SessionService.QueryAsync();
                PopulateLobbyList(sessions);
                SetStatus(sessions.Count == 0 ? "No open lobbies found." : $"Found {sessions.Count} lobby(s).");
            }
            catch (Exception exception)
            {
                ReportFailure(exception);
            }
            finally
            {
                // Always clears, even on a thrown or hung-then-cancelled query, so the browser never sticks.
                _refreshing = false;
                if (this != null) RefreshInteractables();
            }
        }

        private void PopulateLobbyList(IList<ISessionInfo> sessions)
        {
            _sessionIds.Clear();

            var options = new List<TMP_Dropdown.OptionData>();

            foreach (ISessionInfo session in sessions)
            {
                int taken = session.MaxPlayers - session.AvailableSlots;
                options.Add(new TMP_Dropdown.OptionData($"{session.Name}  ({taken}/{session.MaxPlayers})"));
                _sessionIds.Add(session.Id);
            }

            if (_lobbyDropdown != null)
            {
                _lobbyDropdown.ClearOptions();
                _lobbyDropdown.AddOptions(options);
                _lobbyDropdown.value = 0;
                _lobbyDropdown.RefreshShownValue();
            }

            RefreshInteractables();
        }

        private void SetLeaving(bool leaving)
        {
            _leaving = leaving;
            RefreshInteractables();
        }

        /// <summary>
        /// Single source of truth for interactability. A lobby refresh only disables the browser controls;
        /// hosting and joining-by-code stay usable, so a slow or misconfigured query never locks the menu.
        /// </summary>
        private void RefreshInteractables()
        {
            bool idle = !_leaving;

            SetInteractable(_hostButton, idle);
            SetInteractable(_joinByCodeButton, idle);
            SetInteractable(_sessionNameInput, idle);
            SetInteractable(_joinCodeInput, idle);
            SetInteractable(_lobbyDropdown, idle);
            SetInteractable(_refreshButton, idle && !_refreshing);
            SetInteractable(_joinSelectedButton, idle && !_refreshing && _sessionIds.Count > 0);
        }

        private static void SetInteractable(Selectable selectable, bool interactable)
        {
            if (selectable != null) selectable.interactable = interactable;
        }

        private void ReportFailure(Exception exception)
        {
            string reason = exception is SessionException sessionException
                ? sessionException.Message
                : exception.Message;

            SetStatus($"Failed: {reason}");
            Debug.LogException(exception, this);
        }

        private void SetStatus(string message)
        {
            if (_statusLabel != null) _statusLabel.text = message;
        }

        private void SetJoinCode(string joinCode)
        {
            if (_joinCodeLabel != null) _joinCodeLabel.text = $"Join code: {joinCode}";
        }
    }
}
