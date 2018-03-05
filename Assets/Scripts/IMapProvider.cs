using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapProvider
{
	List<PlanetData> LoadPlanetsAtRect (int x, int y, int w, int h);
	PlanetData LoadPlanetNearRatingAtRect (int rating, int x, int y, int w, int h);
}

