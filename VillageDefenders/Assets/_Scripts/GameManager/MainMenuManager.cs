﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    #region FIELDS

    private GameObject mainMenu;
    private GameObject settingsMenu;
    private GameObject aboutMenu;

    #endregion

    #region PROPERTIES

    public AudioMixer mixer;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        mainMenu = transform.Find("MainMenu").gameObject;
        settingsMenu = transform.Find("SettingsMenu").gameObject;
        aboutMenu = transform.Find("AboutMenu").gameObject;
    }

    #endregion

    #region Methods

    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void OnQuitButtonClick()
    {
        //TODO "Are you sure? Y/N"
        Application.Quit(0);
    }

    public void OnSettingsButtonClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OnAboutButtonClick()
    {
        mainMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    public void OnSettingsBackButtonClick()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnAboutBackButtonClick()
    {
        aboutMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Set720pResolution()
    {
        Screen.SetResolution(1280, 720, true);
    }

    public void Set900pResolution()
    {
        Screen.SetResolution(1600, 900, true);
    }

    public void Set1080pResolution()
    {
        Screen.SetResolution(1920, 1080, true);
    }

    public void Set1440pResolution()
    {
        Screen.SetResolution(2560, 1440, true);
    }

    public void SetVolume(float volume)
    {
        mixer.SetFloat("Volume", volume);
    }

    #endregion
}
