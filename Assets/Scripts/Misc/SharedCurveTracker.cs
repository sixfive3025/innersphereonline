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
        public bool Equals ( PointPair pair1, PointPair pair2 )
        {
            return (pair1.p1 == pair2.p1) && (pair1.p2 == pair2.p2);
        }
        
        public int GetHashCode( PointPair pair )
        {
            string hashstring = "";

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

