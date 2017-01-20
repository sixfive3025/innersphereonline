using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SystemHUDUI : MonoBehaviour {

	private SignalDispatcher _signalDispatcher;
	private StarSystemController _targetStarSystem;
	private LocalPlayerManager _localPlayerManager;

	[Inject]
	public void Construct(StarSystemController targetStarSystem, SignalDispatcher signalDispatcher, LocalPlayerManager localPlayerManager) 
	{
		_targetStarSystem = targetStarSystem;
		_signalDispatcher = signalDispatcher;
		_localPlayerManager = localPlayerManager;

		GetComponentsInChildren<Text>()[0].text = _targetStarSystem.StarName;
	
		StarSystemData sysData = _targetStarSystem.GetComponent<StarSystemData>();
		GetComponentsInChildren<Text>()[1].text = sysData.OrigX + ", " + sysData.OrigY;
		GetComponentsInChildren<Text>()[2].text = "Harvard Class " + sysData.StarClass.Split('h')[1].ToUpper();
		if ( sysData.IsDataKnown )
		{
			GetComponentsInChildren<Text>()[3].text = "Planets: " + ( sysData.PlanetsUnknown ? "More than " : "" ) + sysData.PlanetsKnown.ToString();
			GetComponentsInChildren<Text>()[4].text = "Gravity: " + sysData.Gravity;
			GetComponentsInChildren<Text>()[5].text = "Atmosphere: " + sysData.Atmosphere;
			GetComponentsInChildren<Text>()[6].text = "Water: " + 	sysData.Water + "%";
			GetComponentsInChildren<Text>()[7].text = "Climate: " + sysData.Climate;
			GetComponentsInChildren<Text>()[8].text = "Terrain: " + sysData.Terrain;
			GetComponentsInChildren<Text>()[9].text = "Development: " + sysData.Development;
			GetComponentsInChildren<Text>()[10].text = "Flag: " + sysData.HasFlag;
		}
		else 
		{
			for ( int x = 3; x <= 10; x++)
				GetComponentsInChildren<Text>()[x].text = "";
		}

		GetComponentInChildren<Button>().onClick.AddListener( () => CaptureSystem() );
	}

	public void CaptureSystem()
	{
		_signalDispatcher.DispatchSystemFactionChanged(_targetStarSystem, _localPlayerManager.PlayerFactionString);
	}

	public class Factory : Factory<StarSystemController, SystemHUDUI> {}
}
