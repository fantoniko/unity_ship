using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAround2 : MonoBehaviour
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

    private float T;
    private float phi;

    private float timer;
    private Rigidbody rb;

    [HideInInspector]
    public List<GameObject> waypoints;
    [HideInInspector]
    public bool isFirst;
    [HideInInspector]
    public float radius;
    private int curWayPoint;

    private const float sqrt2 = 1.4142f;

    void Start()
    {
        // 30-секундный таймер полета
        timer = 30.0f;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * minVelocity;
        transform.position = transform.parent.position + Vector3.forward * reqDistanceToShip;
        shipTransform = transform.parent;
        // Убрать родителя для избавления от зависимых координат
        transform.SetParent(null);
        // Угловая скорость в градусах
        phi = angularVelocity * 180 / Mathf.PI;
        // Текущая контрольная точка
        curWayPoint = 0;

        if (isFirst)
            target = waypoints[0];
    }

    void Update()
    {
        // Первый в списке активных самолетов идет через контрольные точки,
        // остальные следуют друг за другом
        if (isFirst)
            UpdateWaypointsPassing();
        else
            UpdateTracking();
        UpdateTimer();
    }

    // Функция поведения ведущего самолета
    void UpdateWaypointsPassing()
    {
        var distanceToShip = Vector3.Distance(transform.position, shipTransform.position);
        var distanceToTarget = Vector3.Distance(transform.position, waypoints[curWayPoint].transform.position);

        // Проверка на прохождение контрольной точки
        if (distanceToTarget < 0.5)
        {
            curWayPoint++;
            if (curWayPoint > waypoints.Count - 1)
                curWayPoint = 0;
            // После прохождения точки, целью преследования самолета назначается следующая
            target = waypoints[curWayPoint];
        }

        // T, рассчитанное так, чтобы ведущий самолет описывал дугу круга,
        // проходя между двумя контрольными точками
        T = distanceToTarget / (radius * sqrt2) * - radius;

        // Вычисление угла между текущим и требуемым направлением
        var estimatedPos = target.transform.position + target.transform.forward * T;
        var estimatedVec = estimatedPos - transform.position;

        // Проверка поворота на превышение максимальной угловой скорости
        var rot = Vector3.Angle(transform.forward, estimatedVec);

        // Поворот самолета и вектора скорости по его направлению
        transform.Rotate(new Vector3(0, -rot, 0));
        rb.velocity = transform.forward * rb.velocity.magnitude;

        // Управление ускорением 
        // Ведущий самолет идет на средней скорости между минимальной и максимальной
        var avg = (minVelocity + maxVelocity) / 2.0f;
        if (rb.velocity.magnitude < avg)
        {
            if (rb.velocity.magnitude < maxVelocity)
                rb.AddForce(transform.forward * acceleration);
        }
        else
        {
            if (rb.velocity.magnitude > minVelocity)
                rb.AddForce(transform.forward * acceleration * -1.0f);
        }
    }

    // Функция поведения ведомых самолетов
    void UpdateTracking()
    {
        // Вычисление расстояний до корабля и цели
        //var distanceToShip = Vector3.Distance(transform.position, shipTransform.position);
        var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // Эксперименты с динамическим T
        //if (distanceToShip < reqDistanceToShip)
        //{
        //    T = (distanceToShip / reqDistanceToShip) * reqDistanceToShip - reqDistanceToShip;
        //    //T += (distanceToTarget / reqDistanceToTarget) * reqDistanceToTarget;
        //}

        // Подобранное T при заданном расстоянии до цели
        T = reqDistanceToTarget / -2.0f;

        // Вычисление угла между текущим и требуемым направлением
        var estimatedPos = target.transform.position + target.transform.forward * T;
        var estimatedVec = estimatedPos - transform.position;
        var angle = Vector3.Angle(transform.forward, estimatedVec);

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