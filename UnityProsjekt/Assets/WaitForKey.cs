using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaitForKey : MonoBehaviour
{
	public GameObject keyPromptPanel; // The keyprompt popup panel

	private int waitKeyId = -1;
	private bool waitForKey = false;
	KeyCode[] validKeyCodes;

	void Start()
	{
		validKeyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
	}
	void Update()
	{
		// If true, wait for inputkey
		if (waitForKey)
		{
			// If escape, cancel
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				DisableKeyPrompt ();
			}
			else if (Input.anyKeyDown && keyPromptPanel.activeInHierarchy)
			{
				// Set key to pressed eky if it isn't none
				KeyCode key = GetPressedKey ();
				if (key != KeyCode.None)
				{
					SetNewKey (key);
				}
			}
		}
	}

	public void WaitForNewControlKey(int keyId)
	{
		// TODO Move to playermanager

		// Enable prompt
		keyPromptPanel.SetActive (true);
		waitForKey = true;
		// Set waitkeyid to id gotten from function (called from key button)
		waitKeyId = keyId;
	}

	void SetNewKey(KeyCode key)
	{
		// Set correct key bases on waitKeyId
		if (waitKeyId == 0)
		{
//			leftKey = key;
		}
		else if (waitKeyId == 1)
		{
//			rightKey = key;
		}

		// Update keys
		UpdateKeys ();
		// Disable keyprompt
		DisableKeyPrompt ();
	}

	void DisableKeyPrompt()
	{
		// Disable promptpanel, set waitforkey to false
		keyPromptPanel.SetActive (false);
		waitKeyId = -1;
		waitForKey = false;
	}

	void UpdateKeys()
	{
		// Set correct text
		//leftText.text = Utils.KeyToString (leftKey);
		//rightText.text = Utils.KeyToString (rightKey);

		// Assign player keys to settingkeys
		//player.leftKey = leftKey;
		//player.rightKey = rightKey;
	}
	
	KeyCode GetPressedKey()
	{
		// Return pressed key if key is in validkeys
		for (int i = 0; i < validKeyCodes.Length; i ++)
		{
			if (Input.GetKeyDown (validKeyCodes[i]))
			{
				return (KeyCode)validKeyCodes[i];
			}
		}
		return KeyCode.None;
	}
}