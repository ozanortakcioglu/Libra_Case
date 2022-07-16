using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using System;
using DG.Tweening;

public enum PanelNames
{
    MainMenu,
    InGame,
    EndPanel,
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
    [Header("LEVEL END PANEL")]
    public TMPro.TMP_Text levelEndHeadText;
    public GameObject[] levelEndStars;


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

    #region Custom Events

    public void RestartLevel()
    {
        LevelsManager.Instance.InitializeLevel();
        OpenPanel(PanelNames.InGame, true);
        SoundManager.Instance.PlaySound(SoundTrigger.ButtonClick);

    }

    public void BackToMain()
    {
        FindObjectOfType<DemoController>().ClearLevel();
        OpenPanel(PanelNames.MainMenu, true);
        SoundManager.Instance.PlaySound(SoundTrigger.ButtonClick);

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

    public void SetBombCount(int count)
    {
        bombCount.text = "" + count;
    }

    #endregion


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

    public void SetupLevelEndPanel()
    {
        Debug.Log("Setting Up Level End Panel");

        int starCount = FindObjectOfType<DemoController>().GetLevelStarCount();

        for (int i = 0; i < levelEndStars.Length; i++)
        {
            levelEndStars[i].SetActive(true);
            levelEndStars[i].transform.localScale = Vector3.zero;
        }

        if (starCount == 0)
        {
            //lose
            levelEndHeadText.text = "GAMEOVER!";
            SoundManager.Instance.PlaySound(SoundTrigger.Lose);

        }
        else
        {
            levelEndHeadText.text = "CONGRATS!";
            SoundManager.Instance.PlaySound(SoundTrigger.Win);


            if (starCount > 3)
                starCount = 3;

            for (int i = 0; i < starCount; i++)
            {
                levelEndStars[i].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f * i);
            }

            var oldStarLevel = LevelsManager.Instance.GetCurrentLevelStar();
            if (oldStarLevel < starCount)
                LevelsManager.Instance.SetLevelStar(LevelsManager.Instance.GetCurrentActiveLevel(), starCount);

        }
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

            OpenPanel(panelName);

        }
        else
        {
            Debug.LogWarning("No value for key: " + panelName + " exists");
        }

    }


    public void OpenPanel(PanelNames[] names)
    {
        foreach (PanelNames panelName in names)
            OpenPanel(panelName);
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
            foreach (var item in UIPanelsDictionary[panelName].UIPanel.GetComponentsInChildren<TweenAnimation>())
            {
                item.Play();
            }

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

