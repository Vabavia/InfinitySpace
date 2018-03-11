using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour
{
	private bool redrawQueued = false;

	protected void QueueRedraw ()
	{
		if (!redrawQueued) {
			StartCoroutine (RedrawCoroutine ());
		}
	}

	private IEnumerator RedrawCoroutine ()
	{
		redrawQueued = true;
		yield return null;

		Redraw ();

		redrawQueued = false;
	}

	protected virtual void Redraw ()
	{
	}
}
