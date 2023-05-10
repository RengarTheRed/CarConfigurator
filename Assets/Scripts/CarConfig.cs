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
#region Variables

    //Variables
    //Dropdown Boxes & Money / Buy Button
    public TMP_Dropdown carSelector;
    public TMP_Dropdown carPaint;
    public TMP_Dropdown carEngine;
    public TMP_Dropdown carWheel;
    public Button btnBuy;
    public TMP_Text playerMoney;

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

    //Preview Variables
    public List<GameObject> carObjects;
    public Material[] mats;
    int activeCar = 0;

    //Player Variables
    float money = 2000f;

    #endregion

    #region Initialise

    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    //Start function setup boxes and adds listeners
    void Start()
    {
        //Preps UI elements; clears/sets dropdown boxes, load car and updates money texts
        ClearAllOptions();
        LoadData();
        UpdateOptions();

        UpdateModel(0);
        CheckCost(0);
        SetMoney();

        //Sets Listeners for UI
        carSelector.onValueChanged.AddListener(OnCarChanged);
        carSelector.onValueChanged.AddListener(CheckCost);

        carPaint.onValueChanged.AddListener(OnPaintChanged);
        carPaint.onValueChanged.AddListener(CheckCost);

        carEngine.onValueChanged.AddListener(OnEngineChanged);
        carEngine.onValueChanged.AddListener(CheckCost);

        carWheel.onValueChanged.AddListener(OnWheelChanged);
        carWheel.onValueChanged.AddListener(CheckCost);

        btnBuy.onClick.AddListener(OnBuyClick);
    }

    /// Functions for setting up the boxes
    void LoadData()
    {
        //Populate Paint Array
        Paint blue = new Paint("Blue", 0);
        Paint green = new Paint("Green", 15);
        Paint red = new Paint("Red", 25);
        paints.Add(blue);
        paints.Add(green);
        paints.Add(red);


        //Populate Engine Array
        Engine lowSpec = new Engine("Rusted", 50, 30, 12, 8);
        Engine midSpec = new Engine("Efficiency", 125, 45, 18, 12);
        Engine highSpec = new Engine("Performance", 250, 60, 25, 15);
        engines.Add(lowSpec);
        engines.Add(midSpec);
        engines.Add(highSpec);

        //Populate Wheel Array
        Wheel square = new Wheel("Square", 50, 8, 8, 30);
        Wheel oval = new Wheel("Oval", 125, 12, 12, 45);
        Wheel round = new Wheel("Round", 250, 15, 15, 60);
        wheels.Add(square);
        wheels.Add(oval);
        wheels.Add(round);



        //Populate Car Array
        Car corsa = new Car("Corsa", 150, 12, 30, 12, blue, lowSpec, square);
        Car focus = new Car("Ford Focus", 250, 18, 45, 18, blue, midSpec, oval);
        Car buggati = new Car("Bugatti", 500, 25, 60, 25, red, highSpec, round);
        cars.Add(corsa);
        cars.Add(focus);
        cars.Add(buggati);
    }

    //Function for clearing dropdown boxes
    void ClearAllOptions()
    {
        carSelector.ClearOptions();
        carPaint.ClearOptions();
        carEngine.ClearOptions();
        carWheel.ClearOptions();
    }

    //Functions for populating the selection dropdown boxes
    void UpdateOptions()
    {
        PopulateNames();
        PopulatePaint();
        PopulateEngines();
        PopulateWheels();
    }
    void PopulateNames()
    {
        List<string> carNames = new List<string>();
        foreach (Car car in cars)
        {
            carNames.Add(car.name);
        }
        carSelector.AddOptions(carNames);
    }

    void PopulatePaint()
    {
        List<string> paintColours = new List<string>();

        foreach (Paint paint in paints)
        {
            paintColours.Add(paint.name);
        }

        //Sets Dropdown
        carPaint.AddOptions(paintColours);
    }

    void PopulateEngines()
    {
        List<string> engineOptions = new List<string>();

        foreach (Engine en in engines)
        {
            engineOptions.Add(en.name);
        }

        //Sets Dropdown
        carEngine.AddOptions(engineOptions);
    }

    void PopulateWheels()
    {
        List<string> wheelOptions = new List<string>();

        foreach (Wheel wh in wheels)
        {
            wheelOptions.Add(wh.name);
        }

        //Sets Dropdown
        carWheel.AddOptions(wheelOptions);
    }


#endregion

#region UI Events

    /// Functions for when dropdown boxes values are changed
    /// Car, Paint etc
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
        GetCurrentCar().colour = paints[index];
        UpdateModelPaint(activeCar);
    }

    private void OnEngineChanged(int index)
    {
        GetCurrentCar().engine = engines[index];
    }

    private void OnWheelChanged(int index)
    {
        GetCurrentCar().wheel = wheels[index];
    }

    //Buy button checks if can afford and commits purchase
    private void OnBuyClick()
    {
        float tmpMoney = money - GetCurrentCar().CheckCost();
        if (tmpMoney >= 0)
        {
            money = tmpMoney;
            SetMoney();
        }
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
        int offset = (newCarIndex) * 3;
        carObjects[newCarIndex].GetComponentInChildren<Renderer>().sharedMaterial = mats[carPaint.value+offset];
        return true;
    }
    #endregion

#region Utilities for Checking Car & updating costs
    //Function for calculating cost of order
    void CheckCost(int index)
    {
        totalCost.SetText("Total Cost £" + GetCurrentCar().CheckCost());
        TopSpeedSlider.SetValueWithoutNotify(GetCurrentCar().CheckTopSpeed());
        AccelerationSlider.SetValueWithoutNotify(GetCurrentCar().CheckAcceleration());
        HandlingSlider.SetValueWithoutNotify(GetCurrentCar().CheckHandling());
    }

    //Sets the Money UI Text
    private void SetMoney()
    {
        playerMoney.SetText("£ " + money);

    }

    // Not 100% if this is more efficient but it looks cleaner than having
    // cars[carselector.value] many times over
    private Car GetCurrentCar()
    {
        return cars[carSelector.value];
    }
 #endregion
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
class Component
{
    public string name;
    public float cost;
    public float tSpeedOffset = 0;
    public float accelOffset = 0;
    public float handlingOffset = 0;
}

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
    public Engine(string n, float c, float tSpeed, float aOffset, float hOffset)
    {
        name = n;
        cost = c;
        tSpeedOffset = tSpeed;
        accelOffset = aOffset;
        handlingOffset = hOffset;
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