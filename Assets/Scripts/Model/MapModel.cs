using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModel : BaseModel
{
	public const int SCALE_MIN = 5;
	public const int SCALE_MAX = 10000;

	private int _scale = SCALE_MIN;
	public int scale {
		set { 
			_scale = Mathf.Min (SCALE_MAX, Mathf.Max (SCALE_MIN, value));
			FireChange ();
		}
		get { return _scale; }
	}

}
