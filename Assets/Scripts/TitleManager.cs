using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public Vector3 screenPoint;
    public Vector3 offset;
    public GameObject IKTarget;


    void Awake(){
        Cursor.lockState = CursorLockMode.Confined;
    }
    void Update(){
        OnMouseDrag();
    }
    public void StartGame(){
        SceneManager.LoadScene("Level", LoadSceneMode.Single);
    }
    public void QuitGame(){
        Application.Quit();
    }

    void OnMouseDrag()
    {
        screenPoint = Camera.main.WorldToScreenPoint(IKTarget.transform.position);

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        IKTarget.transform.position = curPosition;

    }

}
