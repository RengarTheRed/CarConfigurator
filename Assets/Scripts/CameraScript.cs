using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    //Auto Rotate Variables
    public Slider cameraSlider;
    public List<GameObject> cameras;
    private int activeCamera = 1;
    private float cameraSpeed = 1f;
    private float toApply =.05f;

    /*Drag Camera Variables
    Vector3 ResetPoint;
    */

    //OnStart bind Listener for CameraSlider
    private void Start()
    {
        cameraSlider.onValueChanged.AddListener(SliderChanged);
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (activeCamera == cameras.Count-1)
            {
                ChangeActiveCamera(0);
            }
            else
            {
                ChangeActiveCamera(activeCamera+1);
            }
        }
    }
    
    //Function for changing the active camera, disables current before activating new
    public void ChangeActiveCamera(int newIndex)
    {
        cameras[activeCamera].SetActive(false);
        activeCamera = newIndex;
        cameras[activeCamera].SetActive(true);
    }
    //Camera modifier in LateUpdate for good practice
    private void LateUpdate()
    {
        transform.Rotate(0, toApply, 0);
    }

    void SliderChanged(float val)
    {
        toApply = val * cameraSpeed;
    }
}
