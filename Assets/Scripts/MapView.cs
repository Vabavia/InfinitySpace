using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
	public const int SCALE_MIN = 5;
	public const int SCALE_MAX = 10000;

	private const int SCALE_SPECIAL = 10;
	private const int SCALE_SPECIAL_SIZE = 9;

	private const int PLANETS_SPRITES_NUM = 100;

	public int planetsInSpecialScale = 20;

	public int lagStartScale = 2000;

	public float minPos = -5f;
	public float maxPos = 5f;
	
	public IMapProvider mapProvider;

	private int _spaceShipRating;
	public int spaceShipRating {
		set { 
			_spaceShipRating = value;
			QueueRedraw ();
		}
		get { return _spaceShipRating; }
	}

	public GameObject spaceShip;
	public GameObject planetPrefab;

	private int _scale = SCALE_MIN;

	public int scale {
		set { 
			_scale = Mathf.Min (SCALE_MAX, Mathf.Max (SCALE_MIN, value));
			QueueRedraw ();
		}
		get { return _scale; }
	}

	private int _x = 0;

	public int x {
		set {
			_x = value;
			QueueRedraw ();
		}
		get { return _x; }
	}

	private int _y = 0;

	public int y {
		set {
			_y = value;
			QueueRedraw ();
		}
		get { return _y; }
	}
		
	private bool redrawQueued = false;
	private IEnumerator drawSpecialCoroutine;

	private GameObject planetsContainer;

	void Start ()
	{
		planetsContainer = new GameObject ("PlanetsContainer");
		planetsContainer.transform.parent = transform;

		QueueRedraw ();
	}

	void QueueRedraw ()
	{
		if (!redrawQueued) {
			StartCoroutine (Redraw ());
		}
	}

	IEnumerator Redraw ()
	{
		redrawQueued = true;
		yield return null;

		Clear ();

		Debug.LogFormat ("Redraw x={0}, y={1}, scale={2}", _x, _y, _scale);

		if (_scale < SCALE_SPECIAL) {
			DrawAllPlanets ();
		} else {
			DrawSpecialPlanets ();
		}

		redrawQueued = false;
	}

	void Clear ()
	{
		foreach (Transform child in planetsContainer.transform) {
			Destroy (child.gameObject);
		}
	}

	void DrawAllPlanets ()
	{
		float cellSize = (maxPos - minPos) / (float)_scale;
		float planetScale = (float)SCALE_MIN / (float)_scale;

		var planets = mapProvider.LoadPlanetsAtRect (x, y, _scale, _scale);

		foreach (PlanetData planet in planets) {
			DrawPlanet (
				planet.rating,
				minPos + ((float)(planet.x - _x) + 0.5f) * cellSize,
				minPos + ((float)(planet.y - _y) + 0.5f) * cellSize,
				planetScale
			);
		}

		DrawSpaceShip (cellSize, planetScale, _scale % 2 == 0);
	}

	void DrawSpecialPlanets ()
	{
		float cellSize = (maxPos - minPos) / (float)SCALE_SPECIAL_SIZE;
		float planetScale = (float)SCALE_MIN / (float)SCALE_SPECIAL_SIZE;

		if (drawSpecialCoroutine != null) {
			StopCoroutine (drawSpecialCoroutine);
		}
		drawSpecialCoroutine = DrawSpecial (cellSize, planetScale);
		StartCoroutine (drawSpecialCoroutine);
			
		DrawSpaceShip (cellSize, planetScale, SCALE_SPECIAL_SIZE % 2 == 0);
	}


	IEnumerator DrawSpecial (float cellSize, float planetScale)
	{
		float cellsPerSector = (float)_scale / (float)SCALE_SPECIAL_SIZE;

		List<PlanetData> planets = new List<PlanetData> ();
		Dictionary<PlanetData, Vector2> planetsPos = new Dictionary<PlanetData, Vector2> ();
		int planetsNeed = planetsInSpecialScale;

		int center = Mathf.CeilToInt (SCALE_SPECIAL_SIZE * 0.5f); 
		int radiusMax = SCALE_SPECIAL_SIZE - center;

//		int n = 0;

		Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
		for (var radius = 1; radius <= radiusMax; ++radius) {
			foreach (Vector2Int dir in dirs) {
				var dirScaled = dir * radius;
				for (var pos = 1 - radius; pos <= radius; ++pos) {
					int i = center + pos * dir.y + dirScaled.x - 1;
					int j = center - pos * dir.x + dirScaled.y - 1;

					//Log Positions
//					Debug.Log ("Find PLANET For " + i + " " + j);
//					GameObject go = new GameObject ("!", typeof(TextMesh));
//					go.GetComponent<TextMesh> ().text = string.Format ("({0}) {1},{2}", n, i, j);
//					var text = go.GetComponent<TextMesh> ();
//					text.fontSize = 24;
//					text.characterSize = 0.1f;
//					text.alignment = TextAlignment.Center;
//					text.anchor = TextAnchor.MiddleCenter;
//					go.transform.parent = planetsContainer.transform;
//					go.transform.position = new Vector3 (minPos + ((float)i + 0.5f) * cellSize, minPos + ((float)j + 0.5f) * cellSize);
//					n++;

					int posX = Mathf.RoundToInt (cellsPerSector * i);
					int posY = Mathf.RoundToInt (cellsPerSector * j);
					var planet = mapProvider.LoadPlanetNearRatingAtRect (
						_spaceShipRating,
						             _x + posX,
						             _y + posY,
						             Mathf.RoundToInt (cellsPerSector * (i + 1)) - posX,
						             Mathf.RoundToInt (cellsPerSector * (j + 1)) - posY
					             );

					if (planet != null) {
						if (planet.rating == _spaceShipRating) {
							DrawPlanet (
								planet.rating,
								minPos + ((float)i + 0.5f) * cellSize,
								minPos + ((float)j + 0.5f) * cellSize,
								planetScale
							);
							planetsNeed--;
							if (planetsNeed == 0) {
								drawSpecialCoroutine = null;
								yield break;
							}
						} else {
							planets.Add (planet);
							planetsPos.Add (planet, new Vector2 (minPos + ((float)i + 0.5f) * cellSize, minPos + ((float)j + 0.5f) * cellSize));
						}
					}

					if (_scale > lagStartScale) {
						yield return null;
					}
				}
			}
		}

		planets.Sort (delegate(PlanetData a, PlanetData b) {
			return Mathf.Abs (_spaceShipRating - a.rating) - Mathf.Abs (_spaceShipRating - b.rating);
		});

		foreach (PlanetData planet in planets) {
			DrawPlanet (
				planet.rating,
				planetsPos [planet].x,
				planetsPos [planet].y,
				planetScale
			);
			planetsNeed--;
			if (planetsNeed == 0) {
				drawSpecialCoroutine = null;
				yield break;
			}
		}
	}

	void DrawPlanet (int rating, float x, float y, float planetScale)
	{
		int planetType = Mathf.CeilToInt (PLANETS_SPRITES_NUM * (float)((float)(rating + 1) / (float)(PlanetData.RATING_MAX + 1)));

		GameObject planetGO = Instantiate (planetPrefab, planetsContainer.transform);
		planetGO.name = "Planet " + planetType;

		Sprite planetSprite = Resources.Load<Sprite> ("planet_" + planetType.ToString ("D3"));
		planetGO.GetComponentInChildren<SpriteRenderer> ().sprite = planetSprite;

		planetGO.GetComponentInChildren<TextMesh> ().text = rating.ToString ("D5");

		planetGO.transform.position = new Vector3 (x, y, 0);
		planetGO.transform.localScale = planetGO.transform.localScale * planetScale;
	}

	void DrawSpaceShip (float cellSize, float planetScale, bool evenScale)
	{
		spaceShip.transform.localScale = Vector3.one * planetScale;
		if (evenScale) {
			spaceShip.transform.position = new Vector3 (cellSize * 0.5f, cellSize * 0.5f);
		} else {
			spaceShip.transform.position = Vector3.zero;
		}
	}
}
