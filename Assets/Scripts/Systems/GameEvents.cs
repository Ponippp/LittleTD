using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameEvents
{

    //===================================================================================================================
    //passing height, width, offset, and floor to an unknown function
    //event data type is used for observer patterns. the event is the thing that subscribers subscribe to
    //Action is more of a general thing you use when you want to pass in a fucntion as a param
    public event Action<int, int, Vector3, Tilemap> OnSetupNewAStarGrid;
    //Invoke() manually calls on actions. 
    //You call a func like foo(); and you call an action like foo.Invoke()
    public void SetupNewAStarGrid(int height, int width, Vector3 offset, Tilemap floor) { OnSetupNewAStarGrid?.Invoke(height, width, offset, floor); }
    //===================================================================================================================
    public event Action OnTowerGridUpdated;
    public void TowerGridUpdated() { OnTowerGridUpdated?.Invoke(); }
    //===================================================================================================================
    public event Action<Tower> OnTowerSelected; //need tower as a param to get range from specific tower instance
    public void TowerSelected(Tower tower) 
    { 
        // Debug.Log($"[GameEvents] TowerSelected called with tower: {tower?.name}");
        OnTowerSelected?.Invoke(tower); 
    }
    //===================================================================================================================
}
