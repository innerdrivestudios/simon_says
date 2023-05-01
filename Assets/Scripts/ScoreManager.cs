using UnityEngine;
using UnityEngine.Events;

/**
 * A central place to manage (high) scores in our game.
 * The score is based on how many patterns the player completes (of course).
 * 
 * In the future I might add a difficulty multiplier into the mix but for now I left it out.
 */
[DisallowMultipleComponent]
public class ScoreManager : MonoBehaviour
{
	private int score = 0;
	private int highscore = 0;

	public UnityEvent<int> onScoreChanged;
	public UnityEvent<int> onHighScoreLoaded;
	public UnityEvent<int> onHighScoreChanged;

	private void Awake()
	{
		LoadHighScore();
	}

	//Called by GameManager.onGameStart
	public void ResetScore()
	{
		SetScore(0);
	}

	//Called by GameManager.OnPatternRepeat
	public void PatternRepeated()
	{
		SetScore(score + 1);
	}

	//Called by GameManager.OnGameEnd
	public void OnGameEnd()
	{
		if (score > highscore)
		{
			highscore = score;
			onHighScoreChanged.Invoke(highscore);
			SaveHighScore();
		}
	}

	private void SetScore (int pScore)
	{
		score = pScore;
		onScoreChanged.Invoke(pScore);
	}

	private void LoadHighScore()
	{
		highscore = PlayerPrefs.GetInt("highscore", 0);
		onHighScoreLoaded.Invoke(highscore);
	}

	private void SaveHighScore()
	{
		PlayerPrefs.SetInt("highscore", highscore);
	}
}


/*
private float multiplier = 1.5f;
private float totalScore = 0;

public UnityEvent<float> onMultiplierChanged;
public UnityEvent<float> onTotalScoreChanged;

private void UpdateMultiplier()
{
	multiplier = 1.5f;
	onMultiplierChanged.Invoke(multiplier);

	UpdateTotal();
}

private void UpdateTotal() 
{
	totalScore = multiplier * score;

	onTotalScoreChanged.Invoke(totalScore);
}
*/