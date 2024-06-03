using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateImage : MonoBehaviour
{
    public float rotationSpeed = 10f; // Скорость вращения

    void Update()
    {
        // Вращаем объект по оси Z на указанную скорость
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}