using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CarConfig : MonoBehaviour
{
    //Variables
    //Dropdown Boxes
    public TMP_Dropdown carSelector;
    public TMP_Dropdown carPaint;
    public TMP_Dropdown carEngine;
    public TMP_Dropdown carWheel;

    //Stat Panel
    public TMP_Text totalCost;
    public Slider TopSpeedSlider;
    public Slider AccelerationSlider;
    public Slider HandlingSlider;

    //Car list
    List<Car> cars = new List<Car>();
    List<Paint> paints = new List<Paint>();
    List<Engine> engines = new List<Engine>();
    List<Wheel> wheels = new List<Wheel>();

    //Ingame Variables
    public List<GameObject> carObjects;
    public Material[] mats;
    int activeCar = 0;

    #region Initialise
    //Start function setup boxes and adds listeners
    void Start()
    {
        //Preps boxes
        ClearAllOptions();
        LoadData();
        UpdateModel(0);
        CheckCost(0);

        //Sets Listeners
        carSelector.onValueChanged.AddListener(OnCarChanged);
        carSelector.onValueChanged.AddListener(CheckCost);

        carPaint.onValueChanged.AddListener(OnPaintChanged);
        carPaint.onValueChanged.AddListener(CheckCost);

        carEngine.onValueChanged.AddListener(OnEngineChanged);
        carEngine.onValueChanged.AddListener(CheckCost);

        carWheel.onValueChanged.AddListener(OnWheelChanged);
        carWheel.onValueChanged.AddListener(CheckCost);
    }
    /// Functions for setting up the boxes
    void LoadData()
    {
        //Populate Paint Array
        Paint blue = new Paint("Blue", 5);
        Paint green = new Paint("Green", 15);
        Paint red = new Paint("Red", 25);
        paints.Add(blue);
        paints.Add(green);
        paints.Add(red);

        //Populate Engine Array
        Engine lowSpec = new Engine("Rusted", 50, -2, -2);
        Engine midSpec = new Engine("Efficiency", 150, 0, 0);
        Engine highSpec = new Engine("Performance", 250, 3, 3);
        engines.Add(lowSpec);
        engines.Add(midSpec);
        engines.Add(highSpec);

        //Populate Wheel Array
        Wheel square = new Wheel("Square", 0, -5, -5, -10);
        Wheel oval = new Wheel("Oval", 20, -2, -1, -3);
        Wheel round = new Wheel("Round", 50, 2, 2, 5);
        wheels.Add(square);
        wheels.Add(oval);
        wheels.Add(round);



        //Populate Car Array
        Car buggati = new Car("Bugatti", 500, 10, 10, 5, red, highSpec, round);
        Car fordfocus = new Car("Ford Focus", 200, 5, 5, 10, blue, midSpec, oval);
        cars.Add(buggati);
        cars.Add(fordfocus);


        PopulateNames();
        PopulatePaint();
        PopulateEngines();
        PopulateWheels();
    }
    //Function for clearing dropdown boxes
    void ClearAllOptions()
    {
        carSelector.ClearOptions();
        carPaint.ClearOptions();
        carEngine.ClearOptions();
        carWheel.ClearOptions();
    }
    //Function for populating car selector
    void PopulateNames()
    {
        List<string> carNames = new List<string>();
        foreach (Car car in cars)
        {
            carNames.Add(car.name);
        }
        carSelector.AddOptions(carNames);
    }
    //Function for populating paint choice
    void PopulatePaint()
    {
        List<string> paintColours = new List<string>();

        foreach (Paint paint in paints)
        {
            paintColours.Add(paint.name);
        }

        carPaint.AddOptions(paintColours);
    }
    void PopulateEngines()
    {
        List<string> engineOptions = new List<string>();

        foreach (Engine en in engines)
        {
            engineOptions.Add(en.name);
        }

        carEngine.AddOptions(engineOptions);
    }
    void PopulateWheels()
    {
        List<string> wheelOptions = new List<string>();

        foreach (Wheel wh in wheels)
        {
            wheelOptions.Add(wh.name);
        }

        carWheel.AddOptions(wheelOptions);
    }

    #endregion
    #region DropDownChanged
    /// Functions for when dropdown boxes values are changed
    /// Name, Paint etc
    private void OnCarChanged(int index)
    {
        carPaint.SetValueWithoutNotify(paints.IndexOf(cars[index].colour));
        carEngine.SetValueWithoutNotify(engines.IndexOf(cars[index].engine));
        carWheel.SetValueWithoutNotify(wheels.IndexOf(cars[index].wheel));

        if(UpdateModel(index))
        {
            activeCar = index;
        }
    }
    private void OnPaintChanged(int index)
    {
        cars[carSelector.value].colour = paints[index];
        UpdateModelPaint(activeCar);
    }
    private void OnEngineChanged(int index)
    {
        cars[carSelector.value].engine = engines[index];
    }
    private void OnWheelChanged(int index)
    {
        cars[carSelector.value].wheel = wheels[index];
    }
    #endregion
    #region PreviewModel
    //Functions for updating the preview model
    //Chose to use booleans since it can then verify if success in performing action
    bool UpdateModel(int newCarIndex)
    {
        carObjects[activeCar].SetActive(false);
        carObjects[newCarIndex].SetActive(true);
        carPaint.SetValueWithoutNotify(paints.IndexOf(cars[newCarIndex].colour));
        carEngine.SetValueWithoutNotify(engines.IndexOf(cars[newCarIndex].engine));
        carWheel.SetValueWithoutNotify(wheels.IndexOf(cars[newCarIndex].wheel));


        UpdateModelPaint(newCarIndex);
        return true;
    }
    bool UpdateModelPaint(int newCarIndex)
    {
        carObjects[newCarIndex].GetComponentInChildren<Renderer>().sharedMaterial = mats[carPaint.value];
        return true;
    }
    #endregion
    //Function for calculating cost of order
    void CheckCost(int index)
    {
        totalCost.SetText("Total Cost �" + cars[carSelector.value].CheckCost());
        TopSpeedSlider.SetValueWithoutNotify(cars[carSelector.value].CheckTopSpeed());
        AccelerationSlider.SetValueWithoutNotify(cars[carSelector.value].CheckAcceleration());
        HandlingSlider.SetValueWithoutNotify(cars[carSelector.value].CheckHandling());
    }
}



//Classes used for the project since I required names and costs
class Car
{
    //Base Variables
    public string name;
    public int baseCost;
    public float totalCost;
    float baseTopSpeed;
    float baseAccelertation;
    float baseHandling;

    //Components
    public Paint colour;
    public Engine engine;
    public Wheel wheel;

    public Car(string n, int bC, float baseTSpeed, float baseAccel, float baseHandle, Paint c, Engine en, Wheel wh)
    {
        //Name & Cost
        name = n;
        baseCost = bC;

        //Stats
        baseTopSpeed = baseTSpeed;
        baseAccelertation = baseAccel;
        baseHandling = baseHandle;

        //Comps
        colour = c;
        engine = en;
        wheel = wh;

        //Update Cost
        CheckCost();
    }

    /*Return Car Stats
     * Each below is sum of base car + component offsets
    */
    public float CheckCost()
    {
        totalCost = baseCost + colour.cost + engine.cost + wheel.cost;

        return totalCost;
    }
    public float CheckTopSpeed()
    {
        float tSpeed = baseTopSpeed + colour.tSpeedOffset + engine.tSpeedOffset + wheel.tSpeedOffset;

        return tSpeed;
    }
    public float CheckAcceleration()
    {
        float acceleration = baseAccelertation + colour.accelOffset + engine.accelOffset + wheel.accelOffset;

        return acceleration;
    }
    public float CheckHandling()
    {
        float handling = baseHandling + colour.handlingOffset + engine.handlingOffset + wheel.handlingOffset;

        return handling;
    }
}

//Component Classes
class Paint : Component
{
    public Paint(string n, float c)
    {
        name = n;
        cost = c;
    }
}
class Engine : Component
{
    public Engine(string n, float c, float tSpeed, float aOffset)
    {
        name = n;
        cost = c;
        tSpeedOffset = tSpeed;
        accelOffset = aOffset;
    }
}
class Wheel : Component
{
    public Wheel(string n, float c, float tSpeed, float aOffset, float hOffset)
    {
        name = n;
        cost = c;
        tSpeedOffset = tSpeed;
        accelOffset = aOffset;
        handlingOffset = hOffset;
    }
}
class Component
{
    public string name;
    public float cost;
    public float tSpeedOffset = 0;
    public float accelOffset = 0;
    public float handlingOffset = 0;
}