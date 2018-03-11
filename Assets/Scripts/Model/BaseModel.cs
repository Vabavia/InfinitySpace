using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : MonoBehaviour {
	public delegate void ChangeHandler ();
	public event ChangeHandler Changed;

	protected void FireChange ()
	{
		if (Changed != null) {
			Changed ();
		}
	}
}
