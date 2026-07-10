using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Drives the menu scene. Assign the Canvas widgets in the Inspector; nothing here builds UI itself.
    ///
    /// Only the host loads the combat scene, and only through NetworkManager.SceneManager. Clients follow
    /// automatically — calling UnityEngine.SceneManagement.SceneManager.LoadScene here would move the host
    /// alone and leave every client sitting in the menu.
    /// </summary>
    public class MenuUIController : MonoBehaviour
    {
        [Header("Session")]
        [SerializeField, Range(2, 4)] private int _maxPlayers = 4;

        [Header("Buttons")]
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;

        [Header("Text")]
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private TMP_Text _joinCodeLabel;
        [SerializeField] private TMP_Text _statusLabel;

        private bool _busy;

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
        }

        private void OnHostClicked() => _ = HostAsync();
        private void OnJoinClicked() => _ = JoinAsync();

        private async System.Threading.Tasks.Task HostAsync()
        {
            if (!BeginRequest("Creating session...")) return;

            try
            {
                ISession session = await SessionService.HostAsync(_maxPlayers);

                SetJoinCode(session.Code);
                SetStatus($"Hosting. Join code: {session.Code}");

                // The session has already started the host, so the server can drive the scene load.
                NetworkManager.Singleton.SceneManager.LoadScene(GameScenes.Combat, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                Fail(exception);
            }
        }

        private async System.Threading.Tasks.Task JoinAsync()
        {
            string joinCode = _joinCodeInput != null ? _joinCodeInput.text.Trim() : string.Empty;

            if (string.IsNullOrEmpty(joinCode))
            {
                SetStatus("Enter a join code first.");
                return;
            }

            if (!BeginRequest("Joining session...")) return;

            try
            {
                await SessionService.JoinAsync(joinCode);

                // No LoadScene here: NGO synchronizes this client into whatever scene the host is running.
                SetStatus("Joined. Loading...");
            }
            catch (Exception exception)
            {
                Fail(exception);
            }
        }

        private bool BeginRequest(string status)
        {
            if (_busy) return false;

            _busy = true;
            SetInteractable(false);
            SetStatus(status);
            return true;
        }

        private void Fail(Exception exception)
        {
            // The menu scene is gone on success, so we only ever get here while it still exists.
            _busy = false;
            SetInteractable(true);

            string reason = exception is SessionException sessionException
                ? sessionException.Message
                : exception.Message;

            SetStatus($"Failed: {reason}");
            Debug.LogException(exception, this);
        }

        private void SetInteractable(bool interactable)
        {
            _hostButton.interactable = interactable;
            _joinButton.interactable = interactable;

            if (_joinCodeInput != null) _joinCodeInput.interactable = interactable;
        }

        private void SetStatus(string message)
        {
            if (_statusLabel != null) _statusLabel.text = message;
        }

        private void SetJoinCode(string joinCode)
        {
            if (_joinCodeLabel != null) _joinCodeLabel.text = joinCode;
        }
    }
}
