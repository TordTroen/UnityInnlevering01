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
		string divider = " - ";
		if (playerId == 1 || playerId == 3)
		{
			divider = "\n";
		}
		scoreTexts[playerId].text = string.Format ("<color=#22df71>{0}{1}</color><color=#26a2df>{2}</color>", player.curHealth, divider, player.score.ToString ());
	}
}