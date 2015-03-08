using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelToolButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler // For the leveleditors editing buttons
{
	public Image img;
	[HideInInspector]public int id;
	private LevelGenerator lvlGen;

	void Awake()
	{
		lvlGen = GameObject.FindGameObjectWithTag ("GameController").GetComponent<LevelGenerator>();
		img = GetComponent<Image>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		PaintBrick ();
	}

	// Allows holding to paint the bricks
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (Input.GetMouseButton (0))
		{
			PaintBrick ();
		}
	}

	void PaintBrick()
	{
		// Sets the button to correct brick
		lvlGen.SetBrick (this);
	}
}