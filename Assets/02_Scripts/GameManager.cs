using UnityEngine;

public class GameManager : MonoBehaviour {

    public Zone zone;

    public void Start() {
        zone = new Zone(10);
        GetComponentInChildren<ZoneInstantiation>().InstantiateZone(zone);
    }

}
