﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public KeyCode jump { get; set; }
    public KeyCode forward { get; set; }
    public KeyCode backward { get; set; }
    public KeyCode right { get; set; }
    public KeyCode left { get; set; }
    public KeyCode crouch { get; set; }
    public KeyCode interact { get; set; }
    public KeyCode missions { get; set; }
    public bool menuopen;

    public double TextSize { get; set; } = 14;

    public float volume;

    

    public static GameManager instance;
    public PlayerManager playerManager;

    void Awake()
    {
        MakeSingleton();
        AssignKeys();
        try 
        {
            playerManager.player = GameObject.Find("Pol");
        } catch{}
    }

    void AssignKeys()
    {
        jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKey", "Space"));
        forward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forwardKey", "W"));
        backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardKey", "S"));
        right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
        left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A"));
        crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("CrouchKey", "LeftControl"));
        interact = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("InteractKey", "F"));
        missions = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("MissionsKey", "Tab"));
    }

    void Start()
    {
        menuopen = false;
        TextSize = PlayerPrefs.GetFloat("TextSize", 14);
#if UNITY_EDITOR
        // Todas las referencias a objetos singleton deben hacerse como muy pronto desde el Start(),
        // porque los Awake() se ejecutan en orden aleatorio.
        if (SceneManager.GetActiveScene().buildIndex != 0) {
            LocalizationManager.instance.LoadLocalizedText("Trial_level_Default.json");
            LocalizationManagerMenuPausa.instance.LoadLocalizedText("Menu_pausa_Default.json");
        }
#endif
        SceneManager.sceneLoaded += OnSceneLoaded;
        Time.timeScale = 1;
        volume = PlayerPrefs.GetFloat("volume", 100);
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().buildIndex != 0) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
#endif
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (scene.buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            LocalizationManager.instance.LoadLocalizedText("Menu_Default.json");

        } else {
            LocalizationManager.instance.LoadLocalizedText("Trial_level_Default.json");
            LocalizationManagerMenuPausa.instance.LoadLocalizedText("Menu_pausa_Default.json");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerManager.player = GameObject.Find("Pol");
        }
        /*else if (scene.buildIndex == 1)
        {

            LocalizationManager.instance.LoadLocalizedText("Trial_level_Default.json");
            LocalizationManagerMenuPausa.instance.LoadLocalizedText("Menu_pausa_Default.json");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController = FindObjectOfType<PlayerController>();

        } else if (scene.buildIndex == 2)
        {

            LocalizationManager.instance.LoadLocalizedText("Trial_level_Default.json");
            LocalizationManagerMenuPausa.instance.LoadLocalizedText("Menu_pausa_Default.json");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController = FindObjectOfType<PlayerController>();

        }
        else if (scene.buildIndex == 3)
        {

            LocalizationManager.instance.LoadLocalizedText("Trial_level_Default.json");
            LocalizationManagerMenuPausa.instance.LoadLocalizedText("Menu_pausa_Default.json");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerController = FindObjectOfType<PlayerController>();

        }*/

    }

    protected void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
