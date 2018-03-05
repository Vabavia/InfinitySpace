using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{

	[Range (0f, 1.0f)]
	public float fillPlanets = 0.3f;

	public MapView mapView;

	public Text shipRating;
	public Text scaleText;
	public Slider scaleSlider;
	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;

	private MapProvider mapProvider;

	void Start ()
	{
		mapProvider = new MapProvider (fillPlanets);
		mapView.mapProvider = mapProvider;
		mapView.spaceShipRating = Random.Range (0, 10000);
		shipRating.text = "Ship " + mapView.spaceShipRating.ToString("D5");

		scaleSlider.onValueChanged.AddListener (delegate {
			SetScale (scaleSlider.value);
		});
		upButton.onClick.AddListener (delegate {
			mapView.y++;
		});
		downButton.onClick.AddListener (delegate {
			mapView.y--;
		});
		leftButton.onClick.AddListener (delegate {
			mapView.x--;
		});
		rightButton.onClick.AddListener (delegate {
			mapView.x++;
		});

		UpdateScaleText ();
	}

	public void SetScale (float scaleFloat)
	{
//		_scale = SCALE_MIN + Mathf.FloorToInt (scaleFloat * (SCALE_MAX - SCALE_MIN));
		//Log Slider
		mapView.scale = MapView.SCALE_MIN - 1 + Mathf.RoundToInt (Mathf.Pow (10, scaleFloat * Mathf.Log10 (MapView.SCALE_MAX - MapView.SCALE_MIN + 1)));
		Debug.Log ("Scale " + mapView.scale);

		UpdateScaleText ();
	}

	void UpdateScaleText() {
		scaleText.text = "Scale " + mapView.scale.ToString ("D5");
	}

	public void Update ()
	{
		if (Input.GetKeyDown (KeyCode.W)) {
			mapView.y++;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			mapView.x--;
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			mapView.y--;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			mapView.x++;
		}
	}
}
