using UnityEngine;
using DG.Tweening;

public class GridCell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer brickSprite;
    [SerializeField] private SpriteRenderer bombSprite;
    [SerializeField] private SpriteRenderer thickSprite;


    [HideInInspector]
    public bool willExplode;

    private int posX;
    private int posY;
    private bool isBrick = false;
    private bool hasBomb = false;

    private void Start()
    {
        willExplode = false;
    }

    public void ActivateBrick()
    {
        isBrick = true;
        brickSprite.gameObject.SetActive(true);
    }

    public void AddBomb()
    {
        hasBomb = true;
        var scale = bombSprite.gameObject.transform.localScale;
        bombSprite.gameObject.transform.localScale = Vector3.zero;
        bombSprite.gameObject.SetActive(true);
        bombSprite.gameObject.transform.DOScale(scale, 0.5f).SetEase(Ease.OutBack);
    }

    public void ExplodeBomb()
    {
        //effect
        bombSprite.gameObject.SetActive(false);
    }

    public void BreakBrick()
    {
        if (!willExplode)
            return;

        isBrick = false;

        brickSprite.gameObject.SetActive(false);
        thickSprite.gameObject.SetActive(true);
    }


    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

    public bool GetHasBomb()
    {
        return hasBomb;
    }

    public bool GetIsBrick()
    {
        return isBrick;
    }

}
