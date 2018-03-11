using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	public MapView mapView { get; set; }

	public MapModel mapModel { get; set; }

	public ShipModel shipModel { get; set; }


	public Slider scaleSlider;
	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;


	void Start ()
	{
		scaleSlider.onValueChanged.AddListener (delegate {
			SetScale (scaleSlider.value);
		});
		upButton.onClick.AddListener (delegate {
			shipModel.y++;
		});
		downButton.onClick.AddListener (delegate {
			shipModel.y--;
		});
		leftButton.onClick.AddListener (delegate {
			shipModel.x--;
		});
		rightButton.onClick.AddListener (delegate {
			shipModel.x++;
		});
	}

	public void SetScale (float scaleFloat)
	{
		//_scale = SCALE_MIN + Mathf.FloorToInt (scaleFloat * (SCALE_MAX - SCALE_MIN));
		//Log Slider
		mapModel.scale = MapModel.SCALE_MIN - 1 + Mathf.RoundToInt (
			Mathf.Pow (10, scaleFloat * Mathf.Log10 (MapModel.SCALE_MAX - MapModel.SCALE_MIN + 1))
		);
		Debug.Log ("Scale " + mapModel.scale);
	}

	public void Update ()
	{
		if (Input.GetKeyDown (KeyCode.W)) {
			shipModel.y++;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			shipModel.x--;
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			shipModel.y--;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			shipModel.x++;
		}
	}

}
