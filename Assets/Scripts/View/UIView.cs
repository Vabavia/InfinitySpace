using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIView : BaseView
{
	public MapModel mapModel { get; set; }

	public ShipModel shipModel { get; set; }

	public Text shipRatingText;
	public Text shipPositionText;

	public Text scaleText;

	void Start ()
	{
		Debug.Log ("UIView Start");

		mapModel.Changed += QueueRedraw;
		shipModel.Changed += QueueRedraw;

		QueueRedraw ();
	}

	protected override void Redraw ()
	{
		shipRatingText.text = "Ship " + shipModel.rating.ToString ("D5");
		shipPositionText.text = string.Format ("x={0}\ny={1}", shipModel.x.ToString (), shipModel.y.ToString ()); 
		scaleText.text = "Scale " + mapModel.scale.ToString ("D5");
	}
}
