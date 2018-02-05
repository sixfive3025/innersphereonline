using UnityEngine;
using System.Collections.Generic;

public class Splinulator {

    //Use the transforms of GameObjects in 3d space as your points or define array with desired points
	private Vector2[] _points = null;
	
	//Store points on the Catmull curve so we can visualize them
	private List<Vector2> _newPoints = null;
	
	//How many points you want on the curve
	public float AmountOfPoints = 10.0f;
	
	//set from 0-1
	public float Alpha = 0.5f;
	
	/////////////////////////////
	
	public List<Vector2> CatmulRom( Vector2[] points )
	{
        _points = points;
        _newPoints = new List<Vector2>();

		_newPoints.Clear();

		Vector2 p0 = points[0];
		Vector2 p1 = points[1];
		Vector2 p2 = points[2];
		Vector2 p3 = points[3];

		float t0 = 0.0f;
		float t1 = GetT(t0, p0, p1);
		float t2 = GetT(t1, p1, p2);
		float t3 = GetT(t2, p2, p3);

		for(float t=t1; t<t2; t+=((t2-t1)/AmountOfPoints))
		{
		    Vector2 A1 = (t1-t)/(t1-t0)*p0 + (t-t0)/(t1-t0)*p1;
		    Vector2 A2 = (t2-t)/(t2-t1)*p1 + (t-t1)/(t2-t1)*p2;
		    Vector2 A3 = (t3-t)/(t3-t2)*p2 + (t-t2)/(t3-t2)*p3;
		    
		    Vector2 B1 = (t2-t)/(t2-t0)*A1 + (t-t0)/(t2-t0)*A2;
		    Vector2 B2 = (t3-t)/(t3-t1)*A2 + (t-t1)/(t3-t1)*A3;
		    
		    Vector2 C = (t2-t)/(t2-t1)*B1 + (t-t1)/(t2-t1)*B2;
		    
		    _newPoints.Add(C);
		}

        return _newPoints;
	}

	float GetT(float t, Vector2 p0, Vector2 p1)
	{
	    float a = Mathf.Pow((p1.x-p0.x), 2.0f) + Mathf.Pow((p1.y-p0.y), 2.0f);
	    float b = Mathf.Pow(a, 0.5f);
	    float c = Mathf.Pow(b, Alpha);
	   
	    return (c + t);
	}
	
}