using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class PlayerHUDUI : MonoBehaviour {

	private PlayerCardUI.Factory _playerCardFactory;

	private Dictionary<PlayerController, PlayerCardUI> _players = new Dictionary<PlayerController,PlayerCardUI>();

	[Inject]
	public void Construct(PlayerCardUI.Factory playerCardFactory) 
	{
		_playerCardFactory = playerCardFactory;
	}

	public void AddPlayer( PlayerController player )
	{
		PlayerCardUI playerCard = _playerCardFactory.Create(player);
		_players.Add(player, playerCard);

		// Make the card a child of the list and draw them in a vertical list
		playerCard.transform.SetParent( gameObject.transform );
		RectTransform playerTransform = playerCard.gameObject.GetComponent<RectTransform>();
		playerTransform.anchoredPosition = new Vector2( 30, -30 - ((_players.Count-1)*55) );	
	}

	public void RemovePlayer( PlayerController player )
	{
		Destroy(_players[player].gameObject);
		_players.Remove(player);

		// TODO: Redraw the list so a gap isn't left behind
	}

	public class Factory : Factory<PlayerHUDUI> {}
}
