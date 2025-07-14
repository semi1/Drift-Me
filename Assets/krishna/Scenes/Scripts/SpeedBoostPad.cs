using UnityEngine;

public class SpeedBoostPad : MonoBehaviour
{
    public float boostMultiplier = 20f;
    public float boostDuration = 30f;

    private void OnEnterCollision(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
               Debug.Log("Speed boost collided ");

            Controller car = collision.GetComponent<Controller>();
            if (car != null)
            {
                Debug.Log("Speed boost called ");
               // car.ActivateSpeedBoost(boostMultiplier, boostDuration);
            }
        }
    }
}
