using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}
public class TempSJ : MonoBehaviour
{
    public GameObject tree;
    private Season currentSeason;
    private Animator treeAnimator;
    public AnimationClip springAnimation;
    public AnimationClip summerAnimation;
    public AnimationClip autumnAnimation;
    public AnimationClip winterAnimation;
    void Start()
    {
        currentSeason = Season.Spring;  // 시작 계절 봄  (대충 임시)
        treeAnimator = tree.GetComponent<Animator>();
        UpdateTreeAnimation();
    }
    void 대충어디선가날짜바꿔주는메서드가작동하다가한달이지나면()
    {
        ChangeSeason();
    }
    void ChangeSeason()
    {
        if (currentSeason == Season.Winter)
        {
            currentSeason = Season.Spring;
        }
        else
        {
            currentSeason += 1;
        }
        UpdateTreeAnimation();
    }
    void UpdateTreeAnimation()
    {
        switch (currentSeason)
        {
            case Season.Spring:
                treeAnimator.Play(springAnimation.name); // <- 임시임, 요 부분은 좀 더 디테일 수정이 필요함 (조수정 말고)
                // 두번째 줄에 대충 봄 나무 로직 ( 공통적인 나무 상호작용 로직 + 대충 봄 전용 나무 애니메이션 동작 메서드)
                // 또 뭐가 있지? 킹받네
                break;
            case Season.Summer:
                treeAnimator.Play(summerAnimation.name);
                break;
            case Season.Autumn:
                treeAnimator.Play(autumnAnimation.name);
                break;
            case Season.Winter:
                treeAnimator.Play(winterAnimation.name);
                break;
        }
    }
}