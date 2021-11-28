using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {
    
    public RectTransform canvas = null;
    [SerializeField] private TMP_Text moneyText = null;

    public void UpdateMoneyAmount(float amount) {
        if (moneyText == null) {
            Debug.LogError("Player UI money text prefab not assigned");
            return;
        }
        moneyText.text = amount.ToString()+'$';
    }

    public void SetMoneyTextVisibility(bool visible) {
        moneyText.gameObject.SetActive(visible);
    }
    
}
