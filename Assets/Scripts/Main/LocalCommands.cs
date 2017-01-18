using UnityEngine;
using Zenject;

public class LocalCommands {

}

public class MoveCameraToPositionCmd : Command<Vector3> { }
public class ShowClickCoordinatesCmd : Command<double, double> { }
