using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSmoke : MonoBehaviour
{
    // ��������� ����������
    float particleEmissionRate = 0;

    // ����������
    PlayerControl playerControl;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmissionModule;

    // Awake ���������� ��� �������� ���������� ��������.
    void Awake()
    {
        // �������� ���������� ������
        playerControl = GetComponentInParent<PlayerControl>();

        // �������� ������� ������
        particleSystemSmoke = GetComponent<ParticleSystem>();

        // �������� ��������� �������
        particleSystemEmissionModule = particleSystemSmoke.emission;

        // ���������� ������� �������
        particleSystemEmissionModule.rateOverTime = 0;
    }

    // Update ���������� ���� ��� �� ����
    void Update()
    {
        // ��������� ���������� ������ �� ��������
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmissionModule.rateOverTime = particleEmissionRate;

        if (playerControl != null && playerControl.IsTurning() && playerControl.currentSpeed > 0)
        {
            // ���� ������ �������������� ������, �� ����� ��������� ��� � ����������� �� �����
            particleEmissionRate = 20; // �������� �� ������ ��� ��������
        }
        else
        {
            particleEmissionRate = 0;
        }
    }
}
