using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class healthManager : MonoBehaviour
{
    public static healthManager instance;
    public TextMeshProUGUI text;
    private float hp;
    // Start is called before the first frame update
    void Start()
    {
        
        if(instance == null){
            instance = this;
        }
    }

    public void ChangeScore(){
        hp = gameObject.GetComponent<playerHealth>().health;
        text.text = "HP " + hp.ToString();
    }
}
