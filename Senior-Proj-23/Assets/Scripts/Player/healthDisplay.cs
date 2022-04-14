using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class healthDisplay : MonoBehaviour
{
    public static healthDisplay instance;
    public TextMeshProUGUI text;
    public float displayhealth;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = this;
        }
    }
    void Update(){
        displayhealth = playerHealth.instance.health;
        ChangeHealth();
    }

    public void ChangeHealth(){
        text.text = displayhealth.ToString();
    }
}
