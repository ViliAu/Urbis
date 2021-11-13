using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour {
    private static StaticCoroutine sc = null;
    public static StaticCoroutine Instance {
        get {
            if (sc == null) {
                sc = new GameObject("StaticKorska").AddComponent<StaticCoroutine>();
            }
            return sc;
        }
    }
}
