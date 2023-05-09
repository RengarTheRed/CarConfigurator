using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Slider CameraSlider;

    float cameraSpeed = .1f, toApply =.05f;
    bool bAutoRotate = true;
    private void Start()
    {
        CameraSlider.onValueChanged.AddListener(SliderChanged);
    }
    private void Update()
    {
        if(bAutoRotate)
        {
            transform.Rotate(0, toApply, 0);
        }
    }
    void SliderChanged(float val)
    {
        toApply = val * cameraSpeed;
    }
}
