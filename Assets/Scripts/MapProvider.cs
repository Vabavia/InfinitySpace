using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapProvider: IMapProvider
{
	private const int SECTOR_SIZE = 100;

	private float fillPlanets;

	private List<Vector2Int> coordsHelper;

	public MapProvider (float fillPlanets)
	{
		this.fillPlanets = fillPlanets;

		coordsHelper = new List<Vector2Int> ();
		for (var j = 0; j < SECTOR_SIZE; j++) {
			for (var i = 0; i < SECTOR_SIZE; i++) {
				coordsHelper.Add (new Vector2Int (i, j));
			}
		}
	}

	public List<PlanetData> LoadPlanetsAtRect (int x, int y, int w, int h)
	{
//		Debug.LogFormat ("Load Planets {0} {1} {2} {3}", x, y, w, h);

		List<PlanetData> planets = new List<PlanetData> ();

		int sectorStartX = GetSector(x);
		int sectorStartY = GetSector(y);
		int sectorEndX = GetSector(x + w);
		int sectorEndY = GetSector(y + h);

		for (var i = sectorStartX; i <= sectorEndX; ++i) {
			for (var j = sectorStartY; j <= sectorEndY; ++j) {
				int selectFromX = i == sectorStartX ? CropInSector (x) : 0;
				int selectFromY = j == sectorStartY ? CropInSector (y) : 0;
				int selectToX = i == sectorEndX ? CropInSector (x + w) : SECTOR_SIZE;
				int selectToY = j == sectorEndY ? CropInSector (y + h) : SECTOR_SIZE;

//				Debug.LogFormat ("Select From Sector ({0}, {1}) {2} {3} {4} {5}", i, j, selectFromX, selectFromY, selectToX, selectToY);
				var sectorPlanets = LoadSectorPlanets (i, j, selectFromX, selectFromY, selectToX, selectToY);

				planets.AddRange (sectorPlanets);
			}
		}

		return planets;
	}

	private int GetSector(int x) {
		return Mathf.FloorToInt ((float)x / (float)SECTOR_SIZE);
	}

	private int CropInSector (int x)
	{
		return ((x % SECTOR_SIZE) + SECTOR_SIZE) % SECTOR_SIZE;
	}

	public PlanetData LoadPlanetNearRatingAtRect (int rating, int x, int y, int w, int h)
	{
		int sectorStartX = GetSector(x);
		int sectorStartY = GetSector(y);
		int sectorEndX = GetSector(x + w);
		int sectorEndY = GetSector(y + h);

		List<PlanetData> planets = new List<PlanetData> ();

		for (var i = sectorStartX; i <= sectorEndX; ++i) {
			for (var j = sectorStartY; j <= sectorEndY; ++j) {
				int selectFromX = i == sectorStartX ? CropInSector (x) : 0;
				int selectFromY = j == sectorStartY ? CropInSector (y) : 0;
				int selectToX = i == sectorEndX ? CropInSector (x + w) : SECTOR_SIZE;
				int selectToY = j == sectorEndY ? CropInSector (y + h) : SECTOR_SIZE;

				var sectorPlanets = LoadSectorPlanets (i, j, selectFromX, selectFromY, selectToX, selectToY, rating);
				if (sectorPlanets.Count == 1 && sectorPlanets[0].rating == rating) {
					return sectorPlanets [0];
				}

				planets.AddRange (sectorPlanets);
			}
		}

		PlanetData nearestRatingPlanet = null;
		int ratingDiff = PlanetData.RATING_MAX;
		foreach (PlanetData planet in planets) {
			int diff = Mathf.Abs (rating - (int)rating);
			if (diff == 0) {
				return planet;
			}
			else if (diff < ratingDiff) {
				nearestRatingPlanet = planet;
			}
		}

		return nearestRatingPlanet;
	}

	private List<PlanetData> LoadSectorPlanets (int x, int y, int selectFromX, int selectFromY, int selectToX, int selectToY, int? nearRating = null)
	{
		List<PlanetData> planets = new List<PlanetData> ();

		System.Random randomSeed = new System.Random (x);
		System.Random random = new System.Random (randomSeed.Next () * y);

		int minPlanets = Mathf.CeilToInt (fillPlanets * SECTOR_SIZE * SECTOR_SIZE);
		int planetsCount = minPlanets;

//		Debug.LogFormat ("Sector ({0},{1}) {2}", x, y, planetsCount);

		List<Vector2Int> coords = new List<Vector2Int> (coordsHelper);

		int ratingDiff = PlanetData.RATING_MAX;
		PlanetData nearRatingPlanet = null;

		for (var i = 0; i < planetsCount; ++i) {
			int coordsIndex = random.Next (0, coords.Count);
			Vector2Int coord = coords [coordsIndex];
			coords.RemoveAt (coordsIndex);

			int rating = random.Next (0, PlanetData.RATING_MAX);

			if (coord.x >= selectFromX && coord.x < selectToX
			    && coord.y >= selectFromY && coord.y < selectToY) {

				PlanetData planet = new PlanetData (rating, x * SECTOR_SIZE + coord.x, y * SECTOR_SIZE + coord.y);
				if (nearRating != null) {
					int diff = Mathf.Abs (rating - (int)nearRating);
					if (diff < ratingDiff) {
						ratingDiff = diff;
						nearRatingPlanet = planet;
					}
				} else {
					planets.Add (planet);
				}
			}
		}
			
		if (nearRating != null) {
			if (nearRatingPlanet != null) {
				planets.Add (nearRatingPlanet);
			}
		}

		return planets;
	}
}

