using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public string scene;
    public Image image; 

    public bool alphaIncrease;

    void Awake(){
        image.color = new Vector4(image.color.r,image.color.g, image.color.b, 1);
    }
    
    public void Update(){
        if (!alphaIncrease){
            image.color = new Vector4(image.color.r,image.color.g, image.color.b, Mathf.MoveTowards(image.color.a, 0, Time.deltaTime*2));
        }
        else if (alphaIncrease){
            image.color = new Vector4(image.color.r,image.color.g, image.color.b, Mathf.MoveTowards(image.color.a, 1, Time.deltaTime*2));
            if(image.color.a >= 1){
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
        }
    }

	private void OnTriggerEnter(Collider c) {
		alphaIncrease = true;
	}

}
