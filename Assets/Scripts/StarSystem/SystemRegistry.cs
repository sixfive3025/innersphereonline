using System.Collections.Generic;
using UnityEngine;

class SystemRegistry
{
    private Dictionary<string,GameObject> _systemLookup;

    public SystemRegistry()
    {
        _systemLookup = new Dictionary<string,GameObject>();
    }

    public void Register( string systemName, GameObject systemObj )
    {
        _systemLookup[systemName] = systemObj;
    }

    public GameObject Lookup( string systemName )
    {
        GameObject foundSystem = null;

        try
        {
            foundSystem = _systemLookup[systemName];
        }
        catch ( KeyNotFoundException )
        {
            Debug.LogError("Hayden's Mistake: Cannot find system: " + systemName);
        }

        return foundSystem;
    }
}