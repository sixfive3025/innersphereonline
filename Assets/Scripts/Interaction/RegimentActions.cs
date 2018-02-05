using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RegimentActions : Interaction {

    private List<GameObject> _lineList = null;
    private HashSet<string> _jumpable = null;
    private UIManager _uiManager;

    [Inject]
	public void Construct( UIManager uiManager )
	{
		_uiManager = uiManager;

		_lineList = new List<GameObject>();
        _jumpable = new HashSet<string>();
	}

    public override void Deselect()
	{
        _uiManager.RegimentDeselected(GetComponent<RegimentChit>().Regiment);

        foreach ( GameObject obj in _lineList )
        {
            GameObject.Destroy(obj);
        }
	}

	public override void Select()
	{
        StarSystemController currentSystem = transform.parent.GetComponent<StarSystemController>();

        Collider[] hits = Physics.OverlapSphere(transform.parent.position, 150f);
        for( int hit = 0; hit < hits.Length; hit++ )
        {
            StarSystemController hitSystem = hits[hit].GetComponent<StarSystemController>();

            if (hitSystem == currentSystem) continue;

            if ( hitSystem )
            {
                if ( Vector3.Distance(transform.parent.position, hitSystem.transform.position) <= 150f )
                {
                    GameObject lineHolder = new GameObject();
                    _lineList.Add( lineHolder );
                    lineHolder.transform.parent = transform;
                    LineRenderer lineRenderer = lineHolder.AddComponent<LineRenderer>();
                    lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
                    lineRenderer.widthMultiplier = 1f;

                    if ( hitSystem.GetComponent<FactionController>().CurrentFaction != currentSystem.GetComponent<FactionController>().CurrentFaction )
                    {
                        lineRenderer.startColor = Color.red;
                        lineRenderer.endColor = Color.red;
                        lineRenderer.widthMultiplier = 2f;
                    }

                    lineRenderer.SetPosition(0, transform.parent.position);
                    lineRenderer.SetPosition(1, hitSystem.transform.position);

                    _jumpable.Add(hitSystem.StarName);
                }
            }
        }

        _uiManager.RegimentSelected(GetComponent<RegimentChit>().Regiment, _jumpable);
	}
}