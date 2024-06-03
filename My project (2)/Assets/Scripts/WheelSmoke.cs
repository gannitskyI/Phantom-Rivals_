using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSmoke : MonoBehaviour
{
    // Локальные переменные
    float particleEmissionRate = 0;

    // Компоненты
    PlayerControl playerControl;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;

    // Awake вызывается при загрузке экземпляра сценария.
    void Awake()
    {
        // Получить контроллер игрока
        playerControl = GetComponentInParent<PlayerControl>();

        // Получить систему частиц
        particleSystemSmoke = GetComponent<ParticleSystem>();

        // Получить компонент эмиссии
        particleSystemEmissionModule = particleSystemSmoke.emission;

        // Установить нулевую эмиссию
        particleSystemEmissionModule.rateOverTime = 0;
    }

    // Update вызывается один раз за кадр
    void Update()
    {
        // Уменьшить количество частиц со временем
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (playerControl != null && playerControl.IsTurning() && playerControl.currentSpeed > 0)
        {
            // Если машина поворачивается вперед, то будем испускать дым в зависимости от этого
            particleEmissionRate = 20; // Измените на нужное вам значение
        }
        else
        {
            particleEmissionRate = 0;
        }
    }
}
