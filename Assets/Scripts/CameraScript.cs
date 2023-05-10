using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    //Auto Rotate Variables
    public Slider CameraSlider;
    float cameraSpeed = 1f, toApply =.05f;
    bool bAutoRotate = true;

    //Drag Camera Variables
    Vector3 ResetPoint;

    //OnStart bind Listener for CameraSlider
    private void Start()
    {
        CameraSlider.onValueChanged.AddListener(SliderChanged);
    }
    //Camera modifier in LateUpdate for good practice
    private void LateUpdate()
    {
        //Mouse Down Camera Pan
        if(Input.GetMouseButtonDown(1))
        {
            //AutoRotate Disable on Drag
            bAutoRotate = false;
            ResetPoint = Camera.main.gameObject.transform.localPosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Camera.main.gameObject.transform.localPosition = ResetPoint;
            bAutoRotate = true;
        }
        if (bAutoRotate)
        {
            transform.Rotate(0, toApply, 0);
        }
    }
    void SliderChanged(float val)
    {
        toApply = val * cameraSpeed;
    }
}
