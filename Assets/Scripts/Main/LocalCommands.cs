using UnityEngine;
using Zenject;

public class LocalCommands {

}

public class MoveCameraToPositionCmd : Signal<MoveCameraToPositionCmd, Vector3> { }
public class ShowClickCoordinatesCmd : Signal<ShowClickCoordinatesCmd, double, double> { }
