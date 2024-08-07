using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplierGo, scoreGo;
    private int score,multiplier,streak;
    public static ScoreController instance;

    void Awake()
    { instance = this; }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        streak = 0;
        multiplier = 1;

        UpdateMultiplierText();
        UpdateScoreText();
    }

  
    public int EnemyDefeated(int points)
    {
        StreakUp();
        AddPoints(points);
        return points;
    }

    private void MultiplierDown()
    {
        if (multiplier > 1)
        {
            multiplier--;
            UpdateMultiplierText();
            Conductor.instance.MultDown();
        }
    }

    private void AddPoints(int points)
    {
        score += points*multiplier;
        UpdateScoreText();
    }

    private void RemovePoints(int points)
    {
        score -= points;
        if (score < 0)
        {
            score = 0;
        }
        UpdateScoreText();
    }

    private void StreakUp() 
    {
        streak++;
        if (streak%4 ==0) 
        {
            MultiplierUp();
        }
        print("Streak is" + streak);
    }

    private void MultiplierUp()
    {
        if (multiplier <Conductor.instance.MultClips.Length+1)
        {
            multiplier++;
            UpdateMultiplierText();
            Conductor.instance.PlayNextMultip();
        }
    }

    public void StreakDown()
    {
        streak = 0;
        MultiplierDown();
        print("Streak down");
    }

    public int PlayerHit(int points)
    {
        StreakDown();
        RemovePoints(points);
        return points;
    }
    private void UpdateMultiplierText() { multiplierGo.SetText(multiplier.ToString());}

    private void UpdateScoreText() { scoreGo.SetText(score.ToString()); }
}
