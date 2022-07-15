using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenAnimation : MonoBehaviour
{

    public float time;
    public Ease ease = Ease.Linear;
    [Space(10)]
    public int loopCount;
    public LoopType loopType;
    [Space(10)]
    public bool isRelative;
    [Space(10)]
    public bool isScale;
    public Vector3 scale;
    public bool hasInit;
    public Vector3 initScale;
    [Space(10)]
    public bool isMove;
    public Vector3 move;
    public bool isLocalMove;
    [Space(10)]
    public bool isRotate;
    public Vector3 rotate;
    public bool isLocalRotate;


    public void Play()
    {
        if (isMove)
        {
            if (isLocalMove)
            {
                transform.DOLocalMove(move, time).SetRelative(isRelative).SetLoops(loopCount, loopType).SetEase(ease);
            }
            else
            {
                transform.DOMove(move, time).SetRelative(isRelative).SetLoops(loopCount, loopType).SetEase(ease);
            }
        }
        if (isRotate)
        {
            if (isLocalRotate)
            {
                transform.DOLocalRotate(rotate, time).SetRelative(isRelative).SetLoops(loopCount, loopType).SetEase(ease);
            }
            else
            {
                transform.DORotate(rotate, time).SetRelative(isRelative).SetLoops(loopCount, loopType).SetEase(ease);
            }
        }
        if (isScale)
        {
            if (hasInit)
                transform.localScale = initScale;
            transform.DOScale(scale, time).SetRelative(isRelative).SetLoops(loopCount, loopType).SetEase(ease);
        }
    }
}
