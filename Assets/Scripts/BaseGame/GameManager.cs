using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AvatarController;
using UtilsComplements;

namespace BaseGame
{
    public class GameManager : MonoBehaviour, ISingleton<GameManager>
    {
        private PlayerController _playerInstance;
        private List<IResetable> Resetables = new List<IResetable>();
        [Range(1, 3)] private int _level = 1;

        public ISingleton<GameManager> Instance => this;
        public PlayerController PlayerInstance => _playerInstance;
        public static PlayerController Player
        {
            get
            {
                if (!ISingleton<GameManager>.TryGetInstance(out var manager))
                    return null;
                return manager.PlayerInstance;
            }
        }
        public static int Level
        {
            get
            {
                if (!ISingleton<GameManager>.TryGetInstance(out var manager))
                    return 0;
                return manager._level;
            }
            set
            {
                if (!ISingleton<GameManager>.TryGetInstance(out var manager))
                    return;
                manager._level = Math.Clamp(value, 1, 3);
            }
        }

        private void Awake()
        {
            Instance.Instantiate();
            _level = 1;
        }

        private void OnDestroy() => Instance.RemoveInstance();

        public static GameManager GetGameManager()
        {
            if (ISingleton<GameManager>.TryGetInstance(out var manager))
                return manager;

            //It should set itself as the singleton, so this part the code will only triggered once
            var go = new GameObject("GameManager");
            return go.AddComponent<GameManager>();
        }

        public static void AddResetable(IResetable resetable)
        {
            var manager = GetGameManager();

            if (!manager.Resetables.Contains(resetable))
                manager.Resetables.Add(resetable);
        }

        public static void RemoveResetable(IResetable resetable)
        {
            var manager = GetGameManager();

            if (manager.Resetables.Contains(resetable))
                manager.Resetables.Remove(resetable);
        }

        public static void ResetGame()
        {
            if (Checkpoint.CurrentCheckpoint == null)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                GetGameManager().ResetFromCheckpoint();
        }

        public static void DeadSequence()
        {
            DeadManager.ActivateDead();
        }

        public void ResetFromCheckpoint()
        {
            foreach (var item in Resetables)
            {
                item.Reset();
            }

            Checkpoint current = Checkpoint.CurrentCheckpoint;

            Vector3 pos = current.GetSpawnPosition();
            _playerInstance.RequestTeleport(pos, current.transform.forward);
            current.ReproduceLastEvents();
        }

        public void SetPlayerInstance(PlayerController player)
        {
            if (_playerInstance == null)
            {
                _playerInstance = player;
                _playerInstance.DataContainer.DisablePowers();
            }
            else
            {
                Destroy(player.gameObject);
                Debug.Log("More than one player detected, deleted the copy but not implemented " +
                          "if meant to be a dummy in another scene");
            }
        }
    }
}