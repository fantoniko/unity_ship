using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftLaunch2 : MonoBehaviour
{
    [SerializeField]
    private GameObject aircraftSource;
    [SerializeField]
    private GameObject waypointSource;
    [SerializeField]
    private float radius;

    // Список для хранения контрольных точек
    private List<GameObject> waypoints;
    // Список для хранения активных самолетов
    private List<GameObject> aircrafts;

    void Start()
    {
        aircrafts = new List<GameObject>();
        SetWaypoints();
    }

    // Расстановка контрольных точек вокруг корабля, 
    // указывающих направление движения по оси Z и образующих цикл
    void SetWaypoints()
    {
        waypoints = new List<GameObject>();

        var waypointNorth = Instantiate(waypointSource, transform) as GameObject;
        waypointNorth.transform.position = new Vector3(0, 0, radius);
        waypointNorth.transform.Rotate(new Vector3(0, -90, 0));
        waypointNorth.transform.SetParent(null);
        waypoints.Add(waypointNorth);

        var waypointWest = Instantiate(waypointSource, transform) as GameObject;
        waypointWest.transform.position = new Vector3(-radius, 0, 0);
        waypointWest.transform.Rotate(new Vector3(0, 180, 0));
        waypointWest.transform.SetParent(null);
        waypoints.Add(waypointWest);

        var waypointSouth = Instantiate(waypointSource, transform) as GameObject;
        waypointSouth.transform.position = new Vector3(0, 0, -radius);
        waypointSouth.transform.Rotate(new Vector3(0, 90, 0));
        waypointSouth.transform.SetParent(null);
        waypoints.Add(waypointSouth);

        var waypointEast = Instantiate(waypointSource, transform) as GameObject;
        waypointEast.transform.position = new Vector3(radius, 0, 0);
        waypointEast.transform.SetParent(null);
        waypoints.Add(waypointEast);
    }

    void Update()
    {
        UpdateWayPoints();
        UpdateStatusAndRemove();
        LaunchNewAircraft();
    }

    // Обновление позиций контрольных точек
    // (сохранение позиции относительно центра корабля, но векторы направлений не изменяются)
    void UpdateWayPoints()
    {
        waypoints[0].transform.position = transform.position + new Vector3(0, 0, radius);
        waypoints[1].transform.position = transform.position + new Vector3(-radius, 0, 0);
        waypoints[2].transform.position = transform.position + new Vector3(0, 0, -radius);
        waypoints[3].transform.position = transform.position + new Vector3(radius, 0, 0);
    }

    // Обновление статуса самолетов и их удаление по истечении внутреннего таймера
    void UpdateStatusAndRemove()
    {
        if (aircrafts.Count != 0 && aircrafts[0] == null)
        {
            // Следующий за удаляемым самолетом становится ведущим
            // (следует по контрольным точкам)
            if (aircrafts.Count > 1)
            {
                aircrafts[1].GetComponent<FlyAround2>().isFirst = true;
                aircrafts[1].GetComponent<FlyAround2>().target = waypoints[0];
            }
            
            aircrafts.RemoveAt(0);
        }
    }

    // Запуск нового самолета
    void LaunchNewAircraft()
    {
        // Проверка нажатия клавиши и наличия места в списке
        if (Input.GetKeyDown(KeyCode.H) && aircrafts.Count < 5)
        {
            // Создание нового объекта самолета
            var newAircraft = Instantiate(aircraftSource, transform) as GameObject;
            // Передача списка контрольных точек новому самолету
            newAircraft.GetComponent<FlyAround2>().waypoints = waypoints;
            newAircraft.GetComponent<FlyAround2>().radius = radius;

            if (aircrafts.Count == 0)
                // Если самолет первый в списке, то он следует по контрольным точкам
                newAircraft.GetComponent<FlyAround2>().isFirst = true;
            else
                // Иначе целью назначается последний самолет в списке
                newAircraft.GetComponent<FlyAround2>().target = aircrafts[aircrafts.Count - 1];

            aircrafts.Add(newAircraft);
        }
    }
}
