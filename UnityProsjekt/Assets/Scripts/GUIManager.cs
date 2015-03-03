using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public Text[] scoreTexts;

	public static GUIManager instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		for (int i = 0; i < scoreTexts.Length; i ++)
		{
			scoreTexts[i].gameObject.SetActive (i < (int)GameManager.instance.playerMode);
		}
	}

	public void UpdatePlayerStats(int playerId)
	{
		Paddle player = PlayerManager.instance.allPaddles[playerId];
		scoreTexts[playerId].text = string.Format ("Lives - {0} Score - {1}", player.curHealth, player.score.ToString ());
	}
}