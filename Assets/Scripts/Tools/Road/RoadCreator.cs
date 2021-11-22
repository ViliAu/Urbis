using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCreator : MonoBehaviour {

    [HideInInspector]
    public RoadData path;

    public void CreatePath() {
        path = new RoadData(transform.position);
    }
}