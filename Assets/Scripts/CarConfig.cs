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
    //Dropdown Boxes & Money / Buy & Quit buttons
    public TMP_Dropdown CarSelector;
    public TMP_Dropdown CarPaint;
    public TMP_Dropdown CarEngine;
    public TMP_Dropdown CarWheel;
    public TMP_Text PlayerMoney;
    public Button BtnBuy;
    public Button BtnQuit;

    //Stat Panel
    public TMP_Text TotalCost;
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

    //Audio Variables
    public List<AudioClip> audioClips;

    #endregion

    #region Initialise

    //Awake set frame limit to 60 as higher is unnecessary for this
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
        CheckCar(0);
        SetMoney();

        //Sets Listeners for Dropdown Boxes
        CarSelector.onValueChanged.AddListener(OnCarChanged);
        CarSelector.onValueChanged.AddListener(CheckCar);

        CarPaint.onValueChanged.AddListener(OnPaintChanged);
        CarPaint.onValueChanged.AddListener(CheckCar);

        CarEngine.onValueChanged.AddListener(OnEngineChanged);
        CarEngine.onValueChanged.AddListener(CheckCar);

        CarWheel.onValueChanged.AddListener(OnWheelChanged);
        CarWheel.onValueChanged.AddListener(CheckCar);

        //Listeners for Buttons
        BtnBuy.onClick.AddListener(OnBuyClick);
        BtnQuit.onClick.AddListener(QuitButton);
    }

    //Update for checking if esc clicked to quit
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            QuitButton();
        }
    }

    // Functions for setting up the boxes
    // If using external file replace with StreamWriter/Reader and learn how to obfuscate
    void LoadData()
    {
        //Populate Paint Array
        Paint blue = new Paint("Blue", 0);
        Paint green = new Paint("Green", 0);
        Paint red = new Paint("Red", 0);
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
        CarSelector.ClearOptions();
        CarPaint.ClearOptions();
        CarEngine.ClearOptions();
        CarWheel.ClearOptions();
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
        CarSelector.AddOptions(carNames);
    }

    void PopulatePaint()
    {
        List<string> paintColours = new List<string>();

        foreach (Paint paint in paints)
        {
            paintColours.Add(paint.name);
        }

        //Sets Dropdown
        CarPaint.AddOptions(paintColours);
    }

    void PopulateEngines()
    {
        List<string> engineOptions = new List<string>();

        foreach (Engine en in engines)
        {
            engineOptions.Add(en.name);
        }

        //Sets Dropdown
        CarEngine.AddOptions(engineOptions);
    }

    void PopulateWheels()
    {
        List<string> wheelOptions = new List<string>();

        foreach (Wheel wh in wheels)
        {
            wheelOptions.Add(wh.name);
        }

        //Sets Dropdown
        CarWheel.AddOptions(wheelOptions);
    }

#endregion

#region UI Events

    // Functions for when dropdown boxes values are changed
    // Car, Paint etc
    private void OnCarChanged(int index)
    {
        CarPaint.SetValueWithoutNotify(paints.IndexOf(cars[index].colour));
        CarEngine.SetValueWithoutNotify(engines.IndexOf(cars[index].engine));
        CarWheel.SetValueWithoutNotify(wheels.IndexOf(cars[index].wheel));

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

    private void QuitButton()
    {
        Application.Quit();
    }

    //Buy button checks if can afford and commits purchase
    private void OnBuyClick()
    {
        float tmpMoney = money - GetCurrentCar().CheckCost();
        if (tmpMoney >= 0)
        {
            money = tmpMoney;
            SetMoney();
            PlaySound(0);
        }
        else
        {
            PlaySound(1);
        }
    }

    private void PlaySound(int toPlay)
    {
        AudioSource.PlayClipAtPoint(audioClips[toPlay], carObjects[0].gameObject.transform.position, .4f);
    }

#endregion

#region PreviewModel

    //Functions for updating the preview model
    //Chose to use booleans since it can then verify if success in performing action
    bool UpdateModel(int newCarIndex)
    {
        carObjects[activeCar].SetActive(false);
        carObjects[newCarIndex].SetActive(true);
        CarPaint.SetValueWithoutNotify(paints.IndexOf(cars[newCarIndex].colour));
        CarEngine.SetValueWithoutNotify(engines.IndexOf(cars[newCarIndex].engine));
        CarWheel.SetValueWithoutNotify(wheels.IndexOf(cars[newCarIndex].wheel));


        UpdateModelPaint(newCarIndex);
        return true;
    }

    bool UpdateModelPaint(int newCarIndex)
    {
        int offset = (newCarIndex) * 3;
        carObjects[newCarIndex].GetComponentInChildren<Renderer>().sharedMaterial = mats[CarPaint.value+offset];
        return true;
    }

#endregion

#region Utilities for Checking Car & updating costs

    //Function for calculating cost of order and stats (int input since listener requires it)
    void CheckCar(int index)
    {
        TotalCost.SetText("Total Cost £" + GetCurrentCar().CheckCost());
        TopSpeedSlider.SetValueWithoutNotify(GetCurrentCar().CheckTopSpeed());
        AccelerationSlider.SetValueWithoutNotify(GetCurrentCar().CheckAcceleration());
        HandlingSlider.SetValueWithoutNotify(GetCurrentCar().CheckHandling());
    }

    //Sets the Money UI Text
    private void SetMoney()
    {
        PlayerMoney.SetText("£ " + money);

    }

    // Not 100% if this is more efficient but it looks cleaner than having
    // cars[CarSelector.value] many times over
    private Car GetCurrentCar()
    {
        return cars[CarSelector.value];
    }

 #endregion

}

#region Car/Component Classes
//Classes used for the project
//Car has all components but could easily make variants that have more/less
//Components have attributes offsets
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
    
    //Return Car Stats
    //Each below is sum of base car + component offsets
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

#endregion