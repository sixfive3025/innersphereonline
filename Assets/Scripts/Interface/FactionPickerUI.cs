using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactionPickerUI : MonoBehaviour {

	private SignalDispatcher _signalDispatcher;
	private Button[] _buttons;
	private string _faction = "";

	[Inject]
	public void Construct(SignalDispatcher signalDispatcher) 
	{
		_signalDispatcher = signalDispatcher;

		_buttons = GetComponentsInChildren<Button>();
		foreach ( Button btn in _buttons )
		{
			if ( btn.name == "JoinGameButton" )
				btn.onClick.AddListener( () => JoinGame());
			else btn.onClick.AddListener( () => ChooseFaction( btn.name ));
		}
	}

	public void ChooseFaction( string factionChosen )
	{
		_faction = factionChosen;
	}

	public void JoinGame()
	{
		InputField playerNameField = GetComponentInChildren<InputField>();
		if ( _faction != "")
		{
			_signalDispatcher.DispatchFactionSelected(_faction, playerNameField.text);
		}
	}

	public class Factory : Factory<FactionPickerUI> {}
}
