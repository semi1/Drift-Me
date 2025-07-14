using UnityEngine;

public class End : MonoBehaviour
{
     private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
       
          if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.ShowLevelComplete();
        }
    }
}
