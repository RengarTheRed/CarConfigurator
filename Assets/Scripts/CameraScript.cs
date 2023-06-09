using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    //Auto Rotate Variables
    public Slider cameraSlider;
    public List<GameObject> cameras;
    public TMP_Dropdown cameraSelector;
    private int activeCamera = 1;
    private float cameraSpeed = 1f;
    private float toApply =.05f;
    
    //OnStart bind Listener for CameraSlider & Camera Dropdown
    private void Start()
    {
        cameraSlider.onValueChanged.AddListener(SliderChanged);
        LoadCameraOptions();
        cameraSelector.onValueChanged.AddListener(ChangeActiveCamera);
    }

    //Function that Sets up the dropdown box with camera options
    private void LoadCameraOptions()
    {
        List<string> cameraNames = new List<string>();
        foreach(GameObject cam in cameras)
        {
            cameraNames.Add(cam.name);
        }
        cameraSelector.ClearOptions();
        cameraSelector.AddOptions(cameraNames);
    }
    
    private void Update()
    {
        //Listens for keyboard input to cycle cameras
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
    
    //Function for changing the active camera, disables current before activating new,
    //accessed by selector & keyboard event
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
