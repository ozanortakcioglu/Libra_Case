using System.Collections;
using System.Collections.Generic;
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
    public GameObject levelContentUIPrefab;
    public Transform scrollViewContent;
    private List<MainMenuLevelUI> mainManuLevels;
    [Header("IN GAME PANEL ITEMS")]
    public TMPro.TMP_Text levelText;
    public TMPro.TMP_Text bombCount;


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

    private void Start()
    {
        SetupLevelContentUI();

        OpenPanel(PanelNames.MainMenu);
    }

    private void SetupLevelContentUI()
    {
        mainManuLevels = new List<MainMenuLevelUI>();
        var levelCount = LevelsManager.Instance.GetLevelCount();

        for (int i = 0; i < levelCount; i++)
        {
            var go = Instantiate(levelContentUIPrefab, scrollViewContent);
            go.GetComponent<MainMenuLevelUI>().InitializeUI(i);
            mainManuLevels.Add(go.GetComponent<MainMenuLevelUI>());
        }
    }

    #region On Panel Opened Actions

    public void OnMainMenuPanelOpened()
    {
        Debug.Log("Setting Up Main Menu Panel");

        var levelStars = LevelsManager.Instance.GetLevelStars();
        bool isLastActive = true;
        for (int i = 0; i < mainManuLevels.Count; i++)
        {
            mainManuLevels[i].SetupUI(levelStars[i], isLastActive);
            if (levelStars[i] > 0)
                isLastActive = true;
            else
                isLastActive = false;
        }
    }

    public void OnInGamePanelOpened()
    {
        Debug.Log("Setting Up Game Panel");

        levelText.text = "Level-" + (LevelsManager.Instance.GetCurrentActiveLevel() + 1);
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

