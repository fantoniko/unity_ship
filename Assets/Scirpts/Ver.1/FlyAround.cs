using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Управление летающим самолетом
public class FlyAround : MonoBehaviour
{
    // Цель следования
    [HideInInspector]
    public GameObject target;

    [SerializeField]
    private float acceleration;

    [SerializeField]
    public float minVelocity;

    [SerializeField]
    public float maxVelocity;

    [SerializeField]
    private float angularVelocity;

    [SerializeField]
    private float reqDistanceToTarget;

    public float reqDistanceToShip;

    // transform корабля для вычислений
    private Transform shipTransform;
    // Угловая скорость в градусах
    private float phi;
    // Коэффициент смещения позиции для преследования
    private float T;

    private float timer;
    private Rigidbody rb;

    private float animationTimer = 1.0f;
    private bool isTimerReached = false;

    void Start()
    {
        // 30-секундный таймер полета
        timer = 30.0f;
        rb = GetComponent<Rigidbody>();
        shipTransform = transform.parent;
        // Убрать родителя для избавления от зависимых координат
        transform.SetParent(null);
        // Угловая скорость в градусах
        phi = angularVelocity * 180 / Mathf.PI;
    }

    void Update()
    {
        // Секундный таймер до окончания анимации взлета
        if (isTimerReached)
        {
            UpdateRotationAndAcceleration();
            UpdateTimer();
        }
        else
        {
            animationTimer -= Time.deltaTime;
            if (animationTimer <= 0)
            {
                isTimerReached = true;
                // Вернуть объекту прежний размер
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                // Инициализация позиции
                transform.position = shipTransform.position + shipTransform.forward * 5.5f;
                // Инициализация начальной скорости
                rb.velocity = transform.forward * minVelocity;
            }     
        }
    }

    void UpdateRotationAndAcceleration()
    {
        // Вычисление расстояний до корабля и цели
        //var distanceToShip = Vector3.Distance(transform.position, shipTransform.position);
        var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // Эксперименты с динамическим T
        //if (distanceToShip < reqDistanceToShip)
        //{
        //    //T = (distanceToShip / reqDistanceToShip) * reqDistanceToShip - reqDistanceToShip / 2;
        //    T += (distanceToTarget / reqDistanceToTarget) * reqDistanceToTarget;
        //}

        // Подобранное T при заданном расстоянии до цели
        T = reqDistanceToTarget / -3.0f;

        // Вычисление угла между текущим и требуемым направлением
        var estimatedPos = target.transform.position + target.transform.forward * T;
        var estimatedVec = estimatedPos - transform.position;
        var angle = Vector3.Angle(estimatedVec, transform.forward);

        // Проверка поворота на превышение максимальной угловой скорости
        var rot = Mathf.Abs(angle) <= phi ? angle : phi * Mathf.Sign(angle);

        // Поворот самолета и вектора скорости по его направлению
        transform.Rotate(new Vector3(0, -rot, 0));
        rb.velocity = transform.forward * rb.velocity.magnitude;

        // Управление ускорением в зависимости от расстояния до цели и текущей скорости
        if (distanceToTarget > reqDistanceToTarget)
        {
            if (rb.velocity.magnitude < maxVelocity)
            {
                // Уменьшение величины ускорения в зависимости от расстояния до цели (аккуратный подлет к требуемой позиции)
                var razn = distanceToTarget - reqDistanceToTarget;
                if (razn < reqDistanceToTarget)
                    rb.AddForce(transform.forward * (razn / reqDistanceToTarget) * acceleration);
                else
                    rb.AddForce(transform.forward * acceleration);
            }
        }
        else
        {
            if (rb.velocity.magnitude > minVelocity)
                rb.AddForce(transform.forward * acceleration * -1.0f);
        }
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        // Уничтожение объекта по истечению таймера
        if (timer <= 0.0f)
            Destroy(gameObject);
    }
}