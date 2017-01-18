using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MouseHandler : ITickable {

	private List<Interactive> _selections = new List<Interactive>();
	private Plane _plane = new Plane(Vector3.up, Vector3.zero);
	private Vector3 _v3Center = new Vector3(0.5f,0.5f,0.0f);
	
	readonly Camera _camera;
	readonly MoveCameraToPositionCmd _moveCameraCmd;
	readonly ShowClickCoordinatesCmd _showCoordsCmd;

	// Allow mouse interaction to be enabled and disabled
	public bool MouseInputEnabled = false;

	public MouseHandler( [Inject(Id = "MainCamera")] Camera camera,
						 MoveCameraToPositionCmd moveCameraCmd,
						 ShowClickCoordinatesCmd showCoordsCmd )
	{
		_camera = camera;
		_moveCameraCmd = moveCameraCmd;
		_showCoordsCmd = showCoordsCmd;
	}

	public void Tick () 
	{
		if ( !MouseInputEnabled ) return;

		var mouseRay = _camera.ScreenPointToRay( Input.mousePosition );

		if( Input.GetMouseButtonDown(0) )
		{
			float mDist;
			if ( _plane.Raycast(mouseRay, out mDist) )
			{
				Vector3 m3Hit = mouseRay.GetPoint (mDist);
				_showCoordsCmd.Execute( System.Math.Round(m3Hit.x/3,2), System.Math.Round(m3Hit.z/3,2) );
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			Ray cameraRay = _camera.ViewportPointToRay(_v3Center);
			float cDist;
			float mDist;
			if (_plane.Raycast(cameraRay, out cDist) &&
				_plane.Raycast(mouseRay, out mDist))
			{
				Vector3 v3Hit   = cameraRay.GetPoint (cDist);
				Vector3 m3Hit	= mouseRay.GetPoint (mDist);
				
				Vector3 v3Delta = (v3Hit - m3Hit) * -1;
				
				_moveCameraCmd.Execute( _camera.transform.position + v3Delta);
			}
			return;
		}
		else if (!Input.GetMouseButtonDown(0))
			return;

		var es = UnityEngine.EventSystems.EventSystem.current;
		if (es != null && es.IsPointerOverGameObject())
			return;

		if (_selections.Count > 0) {
			foreach (var sel in _selections)
			{
				if (sel != null) sel.Deselect();
			}
			_selections.Clear();
		}

		RaycastHit hit;
		if ( !Physics.Raycast(mouseRay, out hit) )
			return;

		var interact = hit.transform.GetComponent<Interactive>();
		if (interact == null)
			return;

		_selections.Add(interact);
		interact.Select();
	}
	
}
