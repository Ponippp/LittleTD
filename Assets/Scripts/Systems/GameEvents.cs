using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameEvents
{

    public event Action<int, int, Vector3, Tilemap> OnSetupNewAStarGrid;
    //===================================================================================================================
    public void SetupNewAStarGrid(int height, int width, Vector3 offset, Tilemap floor) { OnSetupNewAStarGrid?.Invoke(height, width, offset, floor); }

}
