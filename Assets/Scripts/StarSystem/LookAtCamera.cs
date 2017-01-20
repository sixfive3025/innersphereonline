using UnityEngine;
using Zenject;

public class LookAtCamera : MonoBehaviour {

	private Vector3 _lastCameraPos;
	private Camera _camera;

	[Inject]
	public void Construct ( GameController _gameController, 
					[Inject(Id = "MainCamera")] Camera camera ) 
	{
		// Dedicated server CPU usage optimization
		// TODO: Seems like a bad dependency
		if ( _gameController.IsHeadless() )
			Destroy( this );
		
		_camera = camera;		
	}
	
	void Update () {
		// Don't do anything if the camera hasn't moved
		if ( _camera.transform.position == _lastCameraPos )
			return;
		else _lastCameraPos =_camera.transform.position;

		transform.LookAt(_camera.transform);
		transform.Rotate( 0, 180, 0);
	}
}
