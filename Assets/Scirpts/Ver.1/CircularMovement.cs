using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Скрипт, управляющий спутником корабля, вращающимся вокруг корябля и 
// направляющим первый самолет в кортеже
public class CircularMovement : MonoBehaviour
{
    // Владелец спутника
    [SerializeField]
    private GameObject master;
    // Скорость спутника, рассчитанная внешне,
    // как средняя между мин. и макс. скоростями самолета
    [HideInInspector]
    public float velocity;
    // Радиус орбиты
    [HideInInspector]
    public float radius;

    private float timeCounter = 0;
    private float stepAngle;
    private float velocityCoef;

    void Start()
    {
        // 1 радиан в градусах
        stepAngle = 180 / Mathf.PI;
        // Коэффициент скорости для спутника
        velocityCoef = velocity / radius;
    }

    void Update()
    {
        // Вращение вокруг центра владельца
        timeCounter += Time.deltaTime * velocityCoef;
        float x = Mathf.Cos(timeCounter);
        float y = 0;
        float z = Mathf.Sin(timeCounter);
        transform.Rotate(new Vector3(0, -stepAngle * Time.deltaTime * velocityCoef, 0));
        transform.position = master.transform.position + new Vector3(x, y, z) * radius;
    }
}
