﻿using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AwakeManager : MonoBehaviour
{
    public GameObject Torotate;
    public Transform turntable;
    public float RotateSpeed;
    public VehicleLiist ListOfVehicles;
    public int VehiclePointer = 0;

    public TMP_Text Currencytext;
    public TMP_Text CarInfo;
    public GameObject BuyBtn;
    //public GameObject StartBtnBtn;
    public GameObject Player;

    private const string CurrencyKey = "Currency";

    private void Awake()
    {
        // Ensure first car is unlocked by default
        string firstCarName = ListOfVehicles.Vehicals[0].GetComponent<CarController>().CarName;
        if (PlayerPrefs.GetInt(firstCarName, 0) != 1)
        {
            PlayerPrefs.SetInt(firstCarName, 1); // Unlock first car
            PlayerPrefs.Save();
        }

        // Load previously selected car (or fallback to first owned)
        VehiclePointer = PlayerPrefs.GetInt("Pointer", 0);
        if (!IsCarOwned(VehiclePointer))
        {
            VehiclePointer = GetFirstOwnedCarIndex();
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
        }

        InstantiateSelectedCar();
        GetCarInfo();
    }

    private void FixedUpdate()
    {
        if (Player != null)
        {
            Player.transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime);
        }
    }

    void InstantiateSelectedCar()
    {
        GameObject carPrefab = ListOfVehicles.Vehicals[VehiclePointer];
        Player = Instantiate(carPrefab, turntable.position, Quaternion.identity);
        Player.tag = "Player";
    }

    void DestroyCurrentCar()
    {
        GameObject oldCar = GameObject.FindGameObjectWithTag("Player");
        if (oldCar != null)
            Destroy(oldCar);
    }

    public void NextBTN()
    {
        if (VehiclePointer < ListOfVehicles.Vehicals.Length - 1)
        {
            DestroyCurrentCar();
            VehiclePointer++;
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
            InstantiateSelectedCar();
            GetCarInfo();
        }
    }

    public void PreviousBTN()
    {
        if (VehiclePointer > 0)
        {
            DestroyCurrentCar();
            VehiclePointer--;
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
            InstantiateSelectedCar();
            GetCarInfo();
        }
    }

    public void GetCarInfo()
    {
        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);
        Currencytext.text = " $ : " + currency;

        CarController car = GetCurrentCar();
        string carName = car.CarName;

        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            CarInfo.text = " ";
            BuyBtn.SetActive(false);
            //StartBtnBtn.SetActive(true);
        }
        else
        {
            CarInfo.text = $"{carName} - ${car.CarPrice}";
            BuyBtn.SetActive(true);
            //StartBtnBtn.SetActive(false);

            BuyBtn.GetComponent<Button>().interactable = (currency >= car.CarPrice);
        }
    }

    public void BuyBUtton()
    {
        CarController car = GetCurrentCar();
        int price = car.CarPrice;
        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);

        if (currency >= price)
        {
            PlayerPrefs.SetInt(car.CarName, 1); // Set to owned
            PlayerPrefs.SetInt(CurrencyKey, currency - price);
            PlayerPrefs.Save();

            Debug.Log($"{car.CarName} purchased!");
            GetCarInfo();
        }
        else
        {
            Debug.Log("Not enough currency.");
        }
    }

    public void AwakeGameBtn()
    {
        string carName = GetCurrentCar().CarName;
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
            SceneManager.LoadScene("Level1");
        }
        else
        {
            Debug.Log("Car not owned!");
        }
    }

    CarController GetCurrentCar()
    {
        return ListOfVehicles.Vehicals[VehiclePointer].GetComponent<CarController>();
    }

    bool IsCarOwned(int index)
    {
        string carName = ListOfVehicles.Vehicals[index].GetComponent<CarController>().CarName;
        return PlayerPrefs.GetInt(carName, 0) == 1;
    }

    int GetFirstOwnedCarIndex()
    {
        for (int i = 0; i < ListOfVehicles.Vehicals.Length; i++)
        {
            if (IsCarOwned(i)) return i;
        }
        return 0;
    }

    public void OnClickBackBtn()
    {
        SceneManager.LoadScene(0);
    }
}
