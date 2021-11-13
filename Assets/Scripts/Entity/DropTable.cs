#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic; 
 using UnityEngine;

public class DropTable : MonoBehaviour {

    public Droppable[] prefabs;
    private int totalWeight = 0;
    
    void Awake() {
        SetupRoll();
    }
    //Setup droppables
    public void SetupRoll(){
        totalWeight = 0;
        for (int i = 0; i < prefabs.Length; i++)
            totalWeight += prefabs[i].weight;
        for (int i = 0; i < prefabs.Length; i++){
            prefabs[i].absoluteChance = ((float)prefabs[i].weight / (float)totalWeight * 100).ToString("F2") + "%";
        }
    }

    public GameObject RollDrop(){
        SetupRoll();
        int roll = Random.Range(1, totalWeight+1);
        int currWeight = 0;
        for (int i = 0; i < prefabs.Length; i++){
            if(roll > currWeight && roll <= prefabs[i].weight + currWeight) {
                if (prefabs[i] != null)
                    return (prefabs[i].prefabToDrop);
                else
                    RollDrop();
            }
            else{
                currWeight += prefabs[i].weight;
                continue;
            }
        }
        return null;
    }

    // List element class
    [System.Serializable]
    public class Droppable {
        [Tooltip("The prefab/droptable to drop")]
        public GameObject prefabToDrop;
        [Tooltip("The chance of the prefab dropping out of combined waeights of this table")]
        [Range(1, 1000)]public int weight = 1;
        [InspectorName("Data")]
        public string absoluteChance;
    }
}
