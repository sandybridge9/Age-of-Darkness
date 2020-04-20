using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private GameObject mainMenu;
    private GameObject settingsMenu;
    private GameObject aboutMenu;

    void Awake()
    {
        mainMenu = transform.Find("MainMenu").gameObject;
        settingsMenu = transform.Find("SettingsMenu").gameObject;
        aboutMenu = transform.Find("AboutMenu").gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        //List<Button> buttons = mainMenu.GetComponents<Button>().ToList();
        //foreach (var b in buttons)
        //{
        //    b.animationTriggers.normalTrigger.
        //}
        settingsMenu.SetActive(true);
        ResetCertainMenuButtons(settingsMenu);
    }

    public void OnAboutButtonClick()
    {
        mainMenu.SetActive(false);
        aboutMenu.SetActive(true);
        ResetCertainMenuButtons(aboutMenu);
    }

    public void OnSettingsBackButtonClick()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        ResetCertainMenuButtons(mainMenu);
    }

    public void OnAboutBackButtonClick()
    {
        aboutMenu.SetActive(false);
        mainMenu.SetActive(true);
        ResetCertainMenuButtons(mainMenu);
    }

    private void ResetCertainMenuButtons(GameObject menu)
    {
        List<Button> buttons = menu.GetComponentsInChildren<Button>().ToList();
        foreach (var b in buttons)
        {
            //b.enabled = true;
            //b.enabled = false;
            //b.enabled = true;
            Debug.Log("reseting " +b);
        }
    }
}
