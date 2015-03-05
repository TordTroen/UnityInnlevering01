using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelToolButton : MonoBehaviour, IPointerClickHandler
{
	public int id;
	private LevelGenerator lvlGen;
	public Image img;

	void Awake()
	{
		lvlGen = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LevelGenerator>();
		img = GetComponent<Image>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		lvlGen.SetBrick (this);
	}
}