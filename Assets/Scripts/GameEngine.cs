using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
	[Range (0f, 1.0f)]
	public float fillPlanets = 0.3f;

	public MapModel mapModel;
	public ShipModel shipModel;

	public MapView mapView;
	public UIView uiView;

	private MapProvider mapProvider;

	public GameController gameController;

	void Awake ()
	{
		Debug.Log ("GameEngine Start");

		mapProvider = new MapProvider (fillPlanets);
		shipModel.rating = Random.Range (0, 10000);

		mapView.mapProvider = mapProvider;
		mapView.mapModel = mapModel;
		mapView.shipModel = shipModel;

		uiView.mapModel = mapModel;
		uiView.shipModel = shipModel;

		gameController.mapView = mapView;
		gameController.mapModel = mapModel;
		gameController.shipModel = shipModel;

		Debug.Log ("GameEngine End");
	}
}
