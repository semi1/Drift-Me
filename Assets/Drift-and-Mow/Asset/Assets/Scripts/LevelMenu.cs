﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] Buttons;
    public VehicleLiist ListOfVehicles; // Add reference to the car list

    public void OpenLevel(int Leveid)
    {
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
        GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
        string carName = selectedCar.GetComponent<CarController>().CarName;

        // ✅ Only allow playing if the car is owned
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            string LevelName = "Level " + Leveid;
            SceneManager.LoadScene(LevelName);
        }
        else
        {
            Debug.Log("Car is not unlocked! Cannot start game.");
        }
    }
}
