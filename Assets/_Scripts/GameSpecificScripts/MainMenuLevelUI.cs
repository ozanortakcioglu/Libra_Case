using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuLevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject lockedButton;
    [SerializeField] private Image[] stars;
    [SerializeField] private Slider lockedSlider;
    [SerializeField] private TMP_Text sliderText;

    private int level;
    private bool hasSlider = false;

    public void InitializeUI(int _level)
    {
        level = _level;
        levelText.text = "Level-" + (level + 1);
        playButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            LevelsManager.Instance.InitializeLevel(level);
            UIManager.Instance.OpenPanel(PanelNames.InGame, true);
        });


        if (((float)level + 1) % 5 == 0)
        {
            hasSlider = true;
            lockedSlider.minValue = 0;
            int max = level * 2;
            lockedSlider.maxValue = max;
        }
        else
            hasSlider = false;
    }

    public void SetupUI(int starCount, bool isLastActive)
    {
        for (int i = 0; i < starCount; i++)
        {
            stars[i].color = Color.white;
        }

        if (hasSlider)
        {
            var max = lockedSlider.maxValue;
            var starCountBeforeThisLevel = LevelsManager.Instance.GetStarCountBeforeALevel(level);

            if (starCountBeforeThisLevel < max || !isLastActive)
            {
                lockedSlider.gameObject.SetActive(true);
                lockedSlider.value = starCountBeforeThisLevel;
                if (starCountBeforeThisLevel > max)
                    starCountBeforeThisLevel = (int)max;
                sliderText.text = starCountBeforeThisLevel + "/" + max;

                lockedButton.SetActive(true);
                playButton.SetActive(false);
            }
            else
            {
                lockedSlider.gameObject.SetActive(false);
                lockedButton.SetActive(false);
                playButton.SetActive(true);
            }
        }
        else
        {
            lockedSlider.gameObject.SetActive(false);
            if (isLastActive)
            {
                lockedButton.SetActive(false);
                playButton.SetActive(true);
            }
            else
            {
                playButton.SetActive(false);
                lockedButton.SetActive(true);
            }
        }
    }
}
