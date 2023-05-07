using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SimplesDev.TarzanSimulator.Components;

namespace SimplesDev.TarzanSimulator.Managers
{
    public class GameManager : MonoBehaviour
    {

        public List<GameObject> platforms;
        public List<GameObject> vines;
        public Transform character;
        public PlayerController playerController;

        public static GameManager Instance;

        private void Awake() => Instance ??= this;

        private void Start()
        {
            character = GameObject.FindGameObjectWithTag("Player").transform;
            playerController = character.GetComponent<PlayerController>();
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            platforms = new List<GameObject>(GameObject.FindGameObjectsWithTag("Platform"));
            vines = new List<GameObject>(GameObject.FindGameObjectsWithTag("Vine"));
        }

        private void FixedUpdate()
        {
            if (!playerController.IsAlive())
                return;
            DestroyPassedPlatforms();
            DestroyPassedVines();
        }
        public void DestroyPassedPlatforms()
        {
            platforms = new List<GameObject>(GameObject.FindGameObjectsWithTag("Platform"));
            for (int i = 0; i < platforms.Count; i++)
            {
                if (platforms[i].transform.position.x < character.position.x - 50)
                {
                    if (!playerController.IsAlive())
                        return;
                    Destroy(platforms[i]);
                }
            }
        }
        public void DestroyPassedVines()
        {
            vines = new List<GameObject>(GameObject.FindGameObjectsWithTag("Vine"));
            for (int i = 0; i < vines.Count; i++)
            {
                if (vines[i].transform.position.x < character.position.x - 50)
                {
                    if (!playerController.IsAlive())
                        return;
                    Destroy(vines[i]);
                }
            }
        }
        public void RetryGame()
        {
            clearLevel();
            SceneManager.LoadScene("LevelOne");
        }

        public void clearLevel()
        {
            platforms.Clear();
            vines.Clear();
        }

    }
}
