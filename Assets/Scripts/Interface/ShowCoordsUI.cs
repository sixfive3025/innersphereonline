using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ShowCoordsUI : MonoBehaviour {

	public void UpdateCoordinates( double x, double y )
	{
		GetComponentInChildren<Text>().text = x + ", " + y;
	}

	public class Factory : Factory<ShowCoordsUI> {}
}
