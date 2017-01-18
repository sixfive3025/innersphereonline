using UnityEngine;
using Zenject;

public static class Signals {

	public class FactionSelected : Signal<FactionSelected,string> {}
	public class SystemFactionChanged : Signal<SystemFactionChanged,GameObject,string> {}
	public class PlayerJoined : Signal<PlayerJoined,PlayerController> {}
	public class PlayerDeparted : Signal<PlayerDeparted,PlayerController> {}
}

public class SignalDispatcher {

	private Signals.FactionSelected _factionSelectedSignal;
	private Signals.SystemFactionChanged _systemFactionChangedSignal;
	private Signals.PlayerJoined _playerJoinedSignal;
	private Signals.PlayerDeparted _playerDepartedSignal;

	public SignalDispatcher(Signals.FactionSelected factionSelectedSignal,
							Signals.SystemFactionChanged systemFactionChangedSignal,
							Signals.PlayerJoined playerJoinedSignal,
							Signals.PlayerDeparted playerDepartedSignal) 
	{
		_factionSelectedSignal = factionSelectedSignal;
		_systemFactionChangedSignal = systemFactionChangedSignal;
		_playerJoinedSignal = playerJoinedSignal;
		_playerDepartedSignal = playerDepartedSignal;
	}

	public void DispatchFactionSelected( string chosenFaction )
	{
		_factionSelectedSignal.Fire(chosenFaction);
	}

	public void DispatchSystemFactionChanged( StarSystemController system, string newFaction )
	{
		_systemFactionChangedSignal.Fire(system.gameObject, newFaction);
	}

	public void DispatchPlayerJoined( PlayerController player )
	{
		_playerJoinedSignal.Fire(player);
	}

	public void DispatchPlayerDeparted( PlayerController player )
	{
		_playerDepartedSignal.Fire(player);
	}
}
