using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModel : BaseModel
{
	private int _rating;

	public int rating {
		set { 
			_rating = value;
			FireChange ();
		}
		get { return _rating; }
	}

	private int _x = 0;

	public int x {
		set {
			_x = value;
			FireChange ();
		}
		get { return _x; }
	}

	private int _y = 0;

	public int y {
		set {
			_y = value;
			FireChange ();
		}
		get { return _y; }
	}
}
