using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public Vector3 screenPoint;
    public GameObject canvas;
    public GameObject overlay;
    public GameObject IKTarget;
    public Camera cam;

    void Update(){
        
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(!canvas.activeInHierarchy){
                canvas.SetActive(true);
                overlay.SetActive(true);
		        Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                TimeManager.instance.Stop();
            }
            else{
                canvas.SetActive(false);
                overlay.SetActive(false);
                TimeManager.instance.Play();
		        Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if(IKTarget.activeInHierarchy){
            OnMouseDrag();
        }

    }

    public void Title(){
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }
    public void Resume(){
        canvas.SetActive(false);
        overlay.SetActive(false);
        TimeManager.instance.Play();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMouseDrag()
    {
        screenPoint = cam.WorldToScreenPoint(IKTarget.transform.position);

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint);
        IKTarget.transform.position = curPosition;

    }
}
