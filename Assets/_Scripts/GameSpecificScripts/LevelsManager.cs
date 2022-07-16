using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    public static LevelsManager Instance;

    [SerializeField] private LevelInfo[] levels;

    private List<int> levelStars;
    private int currentLevelIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CheckAndFillLevelStars();
    }

    public void InitializeLevel(int level = -1)
    {
        if(level > -1)
        {
            currentLevelIndex = level;
        }
        FindObjectOfType<DemoController>().InitializeGamePlay(levels[currentLevelIndex]);
    }

    public int GetCurrentActiveLevel()
    {
        return currentLevelIndex;
    }

    public int GetLevelCount()
    {
        return levels.Length;
    }

    public LevelInfo GetLevelInfo(int level)
    {
        return levels[level];
    }

    public List<int> GetLevelStars()
    {
        CheckAndFillLevelStars();
        return levelStars;
    }

    public int GetCurrentLevelStar()
    {
        CheckAndFillLevelStars();
        return levelStars[currentLevelIndex];
    }

    public void SetLevelStar(int level, int star)
    {
        PlayerPrefs.SetInt("LevelStar" + level, star);
        PlayerPrefs.Save();
    }

    public int GetStarCountBeforeALevel(int level)
    {
        var temp = 0;
        for (int i = 0; i < level; i++)
        {
            temp += levelStars[i];
        }
        return temp;
    }

    private void CheckAndFillLevelStars()
    {
        levelStars = new List<int>();
        for (int i = 0; i < levels.Length; i++)
        {
            var star = PlayerPrefs.GetInt("LevelStar" + i, 0);
            levelStars.Add(star);
        }
    }
}
