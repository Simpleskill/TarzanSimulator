using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public CharController charController;
    public Button retryButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        retryButton.gameObject.SetActive(!charController.isAlive);
    }

    public void Retry()
    {
        SceneManager.LoadScene("LevelOne");
    }
}
