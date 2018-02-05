using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RegimentChit : MonoBehaviour {

    private RegimentController _regiment;
    private Sprite _regimentInsignia = null;
    private Sprite _veterencyLabel = null;
    private Sprite _battalionLabel = null;
    private Sprite _brigadeInsignia = null;

    [Inject]
    void Construct( RegimentController regiment )
    {
        _regiment = regiment;
    }

    public RegimentController Regiment {
        get { return _regiment; }
    }

    void Start()
    {
        // This code assumes no more than two regiments per system
        RegimentController[] regList = _regiment.transform.parent.GetComponentsInChildren<RegimentController>();
        if ( regList.Length > 1 && regList[0] != _regiment )
            transform.Translate( new Vector3(22,0,0) );

        transform.Find("FactionBackground").GetComponent<Image>().color = _regiment.GetComponent<FactionController>().GetFactionColor();

        transform.Find("RegimentName").GetComponent<Text>().text = _regiment.RegimentName;

        _regimentInsignia = Resources.Load("Images/insignia/" + _regiment.RegimentInsignia, typeof(Sprite)) as Sprite;
		transform.Find("RegimentInsignia").GetComponent<Image>().sprite = _regimentInsignia;

        _veterencyLabel = Resources.Load("Textures/chits/" + _regiment.VeterencyLevel, typeof(Sprite)) as Sprite;
		transform.Find("VeterencyLevel").GetComponent<Image>().sprite = _veterencyLabel;

        _battalionLabel = Resources.Load("Textures/chits/" + _regiment.BattalionCount, typeof(Sprite)) as Sprite;
		transform.Find("RegimentStrength").GetComponent<Image>().sprite = _battalionLabel;

        _brigadeInsignia = Resources.Load("Images/insignia/" + _regiment.BrigadeInsignia, typeof(Sprite)) as Sprite;
		transform.Find("BrigadeInsignia").GetComponent<Image>().sprite = _brigadeInsignia;
    }

    public class Factory : Factory<RegimentController, RegimentChit> {}
}