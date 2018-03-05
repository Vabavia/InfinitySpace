using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetData
{
	public const int RATING_MAX = 10000;

	public int rating;
	public int x;
	public int y;


	public PlanetData(int rating, int x, int y) {
		this.rating = rating;
		this.x = x;
		this.y = y;
	}
}


