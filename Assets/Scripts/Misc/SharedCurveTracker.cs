using System.Collections.Generic;
using UnityEngine;

public class SharedCurveTracker
{
    public Dictionary< PointPair,List<Vector2> > CurveLookup = null;

    public SharedCurveTracker()
    {
        CurveLookup = new Dictionary< PointPair,List<Vector2> >( new PointPairComparer() );
    }

    public class PointPair
    {
        public Vector2 p1;
        public Vector2 p2;
    }

    private class PointPairComparer : IEqualityComparer<PointPair>
    {
        private PointPair clipPair( PointPair pair )
        {
            PointPair clipped = new PointPair();
            clipped.p1.x = Mathf.Floor( pair.p1.x );
            clipped.p1.y = Mathf.Floor( pair.p1.y );
            clipped.p2.x = Mathf.Floor( pair.p2.x );
            clipped.p2.y = Mathf.Floor( pair.p2.y );

            return pair;
        }

        public bool Equals ( PointPair iPair1, PointPair iPair2 )
        {
            PointPair pair1 = clipPair(iPair1);
            PointPair pair2 = clipPair(iPair2);

            return ((pair1.p1 == pair2.p1) && (pair1.p2 == pair2.p2)) || 
                    ((pair1.p2 == pair2.p1) && (pair1.p1 == pair2.p2));
        }
        
        public int GetHashCode( PointPair iPair )
        {
            string hashstring = "";
            PointPair pair = clipPair(iPair);

            // Generate the same hash code even if the points are in a different order
            if ( pair.p1.x > pair.p2.x )
            {
                hashstring = pair.p1.x + "," + pair.p1.y + "-" + pair.p2.x + "," + pair.p2.y;
            }
            else if ( pair.p1.x < pair.p2.x )
            {
                hashstring = pair.p2.x + "," + pair.p2.y + "-" + pair.p1.x + "," + pair.p1.y;
            }
            else
            {
                if ( pair.p1.y >= pair.p2.y )
                {
                    hashstring = pair.p1.x + "," + pair.p1.y + "-" + pair.p2.x + "," + pair.p2.y;
                }
                else if ( pair.p1.y < pair.p2.y )
                {
                    hashstring = pair.p2.x + "," + pair.p2.y + "-" + pair.p1.x + "," + pair.p1.y;
                }
            }

            return hashstring.GetHashCode();
        }
    }
}

/*
// JUST TESTIN'

			List<Vector2> testList = new List<Vector2>();

			SharedCurveTracker.PointPair testPair = new SharedCurveTracker.PointPair();
			testPair.p1 = new Vector2( 5f, 10f );
			testPair.p2 = new Vector2( 5f, 40f );
			_curveTracker.CurveLookup[testPair] = testList;

			SharedCurveTracker.PointPair validatePair = new SharedCurveTracker.PointPair();
			validatePair.p2 = new Vector2( 5f, 10f );
			validatePair.p1 = new Vector2( 5f, 40f );

			try
			{
				List<Vector2> testResultList = _curveTracker.CurveLookup[validatePair];
				Debug.Log( "FOUND!");
			}
			catch ( KeyNotFoundException ) 
			{
				Debug.Log( "DID NOT FIND! ");
			}
			// ///////////
 */

 /*
 
 SharedCurveTracker.PointPair pair = new SharedCurveTracker.PointPair();
				pair.p1 = calcPoints[1];
				pair.p2 = calcPoints[2];

				try
				{
					List<Vector2> previouslyGenerated = _curveTracker.CurveLookup[pair];

					if ( previouslyGenerated != null )
					{
						if (previouslyGenerated[0] != lastPoint)
						{
							previouslyGenerated.Reverse();
							Debug.Log("Reversed");
							if (previouslyGenerated[0] != lastPoint) 
								Debug.Log("Ugh");
						}
						latePoints.AddRange( previouslyGenerated );
						lastPoint = previouslyGenerated[previouslyGenerated.Count-1];
						Debug.Log( "MATCH!" );
						continue;
					}
				}
				catch ( KeyNotFoundException ) {}
 
  */