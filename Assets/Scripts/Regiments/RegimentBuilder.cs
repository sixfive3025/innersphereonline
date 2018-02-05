using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class RegimentBuilder : NetworkBehaviour {

    private RegimentController.Factory _regimentFactory;
    private const string NAME_HouseOOBSuffix = "houseoob"; 

    [Inject]
    public void Construct( RegimentController.Factory regimentFactory )
    {
        _regimentFactory = regimentFactory;
    }

    public void Build( bool buildOnServer )
    {			
        foreach(Faction fa in Enum.GetValues(typeof(Faction))) // For each house
        {
            if (fa == Faction.None) continue;

            TextAsset regimentAsset = Resources.Load( fa + NAME_HouseOOBSuffix ) as TextAsset;
            if ( regimentAsset == null )
            {
                Debug.Log( "WARNING: No regiments found: " + fa.ToString() + NAME_HouseOOBSuffix);
                continue;
            }
		    string[] regimentList = regimentAsset.text.Split('\n');

            for ( int reg = 0; reg < regimentList.Length; reg++ ) // Load all the regiments
            {
                try
                {
                    string[] regimentLine = regimentList[reg].Split(',');
                    string regimentName = regimentLine[0].Trim();
                    int regimentCount = (Convert.ToInt32(regimentLine[1]));
                    string veterency = regimentLine[2].Trim();
                    string initialLocation = regimentLine[3].Trim();
                    string brigade = regimentLine[4].Trim();
                    string regimentInsignia = "";
                    string brigadeInsignia = "";

                    if ( regimentLine.Length > 5 )
                    {
                        regimentInsignia = regimentLine[5].Trim();
                        brigadeInsignia = regimentLine[6].Trim();
                    }

                    RegimentController newRegiment = _regimentFactory.Create();
                    newRegiment.Setup(regimentName, regimentCount, veterency, initialLocation, brigade, regimentInsignia, brigadeInsignia);
                    newRegiment.GetComponent<FactionController>().CurrentFaction = fa;

                    if ( buildOnServer ) NetworkServer.Spawn(newRegiment.gameObject);
                }
                catch ( FormatException ex ) 
                { 
                    Debug.LogError("Could not load regiment" + reg + " (" + ex.Message + ")" );
                    continue;
                }
                catch ( IndexOutOfRangeException ex )
                {
                    if ( reg != regimentList.Length-1) // Mark keeps adding blank lines at the end
                    {
                        Debug.LogError("Could not load regiment" + reg + " (" + ex.Message + ")" );
                        continue;
                    }
                }
            }
        }
    }
}