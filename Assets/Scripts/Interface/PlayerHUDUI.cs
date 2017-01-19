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
		playerCard.transform.SetParent( gameObject.transform );

		_positionAllCards();
	}

	public void RemovePlayer( PlayerController player )
	{
		Destroy(_players[player].gameObject);
		_players.Remove(player);

		_positionAllCards();
	}

	private void _positionAllCards()
	{
		int count = 0;
		foreach ( KeyValuePair<PlayerController,PlayerCardUI> player in _players )
		{
			RectTransform playerTransform = player.Value.gameObject.GetComponent<RectTransform>();
			playerTransform.anchoredPosition = new Vector2( 30, -30 - ((count)*55) );
			count++;
		}
	}

	public class Factory : Factory<PlayerHUDUI> {}
}
