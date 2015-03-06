using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelToolButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
	public int id;
	private LevelGenerator lvlGen;
	public Image img;

	void Awake()
	{
		lvlGen = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LevelGenerator>();
		img = GetComponent<Image>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		PaintBrick ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Input.GetMouseButton (0))
		{
			PaintBrick ();
		}
	}

	void PaintBrick()
	{
		lvlGen.SetBrick (this);
	}
}