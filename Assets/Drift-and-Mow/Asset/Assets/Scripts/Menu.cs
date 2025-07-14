using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject OptionPanel;
    public GameObject MainMenuPanel;
    public GameObject LevelPanel;
    public VehicleLiist ListOfVehicles; // Add reference to the car list

    public void PlayButtonClicked()
    {
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
        GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
        string carName = selectedCar.GetComponent<CarController>().CarName;

        // ✅ Only allow playing if the car is owned
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            SceneManager.LoadScene(2);
            OnClickLetsPlayBtn();
        }
        else
        {
            Debug.Log("Car is not unlocked! Cannot start game.");
        }
    }
    public void OptionBtnClicked()
    {
        OptionPanel.SetActive(true);
        MainMenuPanel.SetActive(false); 
    }
    public void OnClickLetsPlayBtn()
    {
        //Title.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
    public void QuitBtnClicked()
    {
        Application.Quit();
    }
    public void GarageBtnClicked()
    {
        SceneManager.LoadScene("SelectCar");
    }
    public void LevelsBtnClicked()
    {
        LevelPanel.SetActive(true );   
        OptionPanel.SetActive(false );
        MainMenuPanel.SetActive(false);
    }
    public void BackBtnClicked()
    {
        OptionPanel.SetActive(false);
        MainMenuPanel.SetActive(true );
    }
    public void CloseBtnClicked()
    {
        LevelPanel.SetActive(false );
        OptionPanel.SetActive(true );
    }
}
