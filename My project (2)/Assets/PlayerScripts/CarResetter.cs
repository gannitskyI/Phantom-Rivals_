using System.Collections.Generic;
using UnityEngine;

public class CarResetter : MonoBehaviour
{

    private Vector2 initialPosition;
    private Quaternion initialRotation;
    private PlayerControl playerControl;
    private void Awake()
    {
        // Сохраняем начальное положение и вращение машины при запуске игры
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
     // Метод для сброса машины на начальное место
    public void ResetCar()
    { 
        // Возвращаем машину на начальное положение и вращение
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Если у машины есть Rigidbody2D, сбрасываем её скорость
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        float secondsToDisable = 3f;
        playerControl = FindObjectOfType<PlayerControl>();
        playerControl.DisableControlForSeconds(secondsToDisable);
    }
}