using UnityEngine;
using Zenject;

public class LookAtCamera : MonoBehaviour {

	private Vector3 _lastCameraPos;

	[Inject]
	public void Construct ( GameController _gameController ) {
		// Dedicated server CPU usage optimization
		// TODO: Seems like a bad dependency
		if ( _gameController.IsHeadless() )
			Destroy( this );
	}
	
	void Update () {
		if ( !Camera.main ) return;

		// Don't do anything if the camera hasn't moved
		if ( Camera.main.transform.position == _lastCameraPos )
			return;
		else _lastCameraPos = Camera.main.transform.position;

		transform.LookAt(Camera.main.transform);
		transform.Rotate( 0, 180, 0);
	}
}
