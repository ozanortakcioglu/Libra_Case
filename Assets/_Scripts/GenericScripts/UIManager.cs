using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using System;

public enum PanelNames
{
    MainMenu,
    InGame
}

[System.Serializable]
public class UIPanels : SerializableDictionaryBase<PanelNames, UIPanelAndSetup> { }

[System.Serializable]
public class UIPanelAndSetup
{
    public GameObject UIPanel;
    public UnityEvent UIPanelSetup;

}

public class UIManager : MonoBehaviour
{
    public UIPanels UIPanelsDictionary;

    [Header("MAIN MENU PANEL ITEMS")]
    [Header("IN GAME PANEL ITEMS")]


    public static UIManager Instance;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    #region On Panel Opened Actions

    public void OnMainMenuPanelOpened()
    {
        Debug.Log("Setting Up Main Menu Panel");

    }

    public void OnInGamePanelOpened()
    {
        Debug.Log("Setting Up Game Panel");
    }

    #endregion


    #region Panel Functions

    public void OpenPanel(string panel)
    {
        PanelNames panelName;
        if (Enum.TryParse<PanelNames>(panel, out panelName))
            OpenPanel(panelName);
        else
            Debug.LogWarning("Did not find panel: " + panel);
    }

    public void OpenPanel(PanelNames panelName, bool closeOtherPanels)
    {
        UIPanelAndSetup panelToOpen;
        if (UIPanelsDictionary.TryGetValue(panelName, out panelToOpen))
        {

            if (closeOtherPanels)
            {
                CloseAllPanels();
            }

            panelToOpen.UIPanel.SetActive(true);

            if (panelToOpen.UIPanelSetup != null)
            {
                panelToOpen.UIPanelSetup.Invoke();
            }

        }
        else
        {
            Debug.LogWarning("No value for key: " + panelName + " exists");
        }

    }


    public void OpenPanel(PanelNames[] names, bool closeOtherPanels)
    {
        if (closeOtherPanels)
            CloseAllPanels();

        foreach (PanelNames panelName in names)
            OpenPanel(panelName, false);
    }

    public void OpenPanel(PanelNames name, bool closeOtherPanels, float delay)
    {
        if (closeOtherPanels)
            CloseAllPanels();

        StartCoroutine(AddDelay(delay, () => { OpenPanel(name, closeOtherPanels); }));
    }

    public void OpenPanel(PanelNames panelName)
    {
        UIPanelAndSetup panelToOpen;
        if (UIPanelsDictionary.TryGetValue(panelName, out panelToOpen))
        {
            panelToOpen.UIPanel.SetActive(true);
            panelToOpen.UIPanelSetup?.Invoke();
        }
        else
        {
            Debug.LogWarning("No value for key: " + panelName + " exists");
        }

    }

    public void ClosePanel(string panel)
    {
        PanelNames panelName;
        if (!Enum.TryParse<PanelNames>(panel, out panelName))
        {
            Debug.LogWarning("No enum for string: " + panel);
            return;
        }

        UIPanelAndSetup currentPanel;
        if (UIPanelsDictionary.TryGetValue(panelName, out currentPanel))
            currentPanel.UIPanel.SetActive(false);
    }

    public void ClosePanel(PanelNames panelName)
    {
        UIPanelAndSetup currentPanel;
        if (UIPanelsDictionary.TryGetValue(panelName, out currentPanel))
            currentPanel.UIPanel.SetActive(false);
    }


    void CloseAllPanels()
    {
        foreach (PanelNames panelName in UIPanelsDictionary.Keys)
            ClosePanel(panelName);
    }

    IEnumerator AddDelay(float xSeconds, UnityAction Action)
    {
        yield return new WaitForSecondsRealtime(xSeconds);
        Action();
    }

    #endregion

}

