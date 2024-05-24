using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketScript : MonoBehaviour
{
    private int objectCount = 0;  // Keeps track of the number of objects caught
    public float speed = 5.0f;  // Speed at which the basket moves

    void Update()
    {
        MoveBasket();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))  // Ensure the object has the tag "Collectible"
        {
            objectCount++;  // Increment the count of objects
            Debug.Log("Total objects: " + objectCount);  // Optionally log the current count
            Destroy(other.gameObject);  // Destroys the object upon entering the basket if it should act as a "kill zone"
        }
    }

    private void MoveBasket()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");  // Get left/right arrow key input
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);  // Create movement vector
        transform.Translate(movement * speed * Time.deltaTime, Space.World);  // Apply movement
    }
}
