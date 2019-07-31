using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Управление списком активных самолетов
public class AircraftLaunch : MonoBehaviour
{
    // Префаб самолета со скриптом полета
    [SerializeField]
    private GameObject aircraftSource;
    // Спутник корабля
    [SerializeField]
    private GameObject satellite;
    // Список для хранения активных самолетов 
    private List<GameObject> aircrafts;

    void Start()
    {
        aircrafts = new List<GameObject>();
        // Инициализация радиуса полета для спутника
        satellite.GetComponent<CircularMovement>().radius = aircraftSource.GetComponent<FlyAround>().reqDistanceToShip;
        // Вычисление средней скорости самолета
        var averageAircraftVelocity = (aircraftSource.GetComponent<FlyAround>().maxVelocity + aircraftSource.GetComponent<FlyAround>().minVelocity) / 2.0f;
        // Инициализация скорости спутника
        satellite.GetComponent<CircularMovement>().velocity = averageAircraftVelocity;
    }

    void Update()
    {
        UpdateStatusAndRemove();
        LaunchNewAircraft();
    }

    // Проверка ссылки на первый самолет (удаляется собственным скриптом по истечении таймера)
    // и переназначение цели следования для следовавшего за ним самолета
    void UpdateStatusAndRemove()
    {
        if (aircrafts.Count != 0 && aircrafts[0] == null)
        {
            // При посадке первого самолета в списке, целью следования второго назначается спутник
            if (aircrafts.Count > 1)
                aircrafts[1].GetComponent<FlyAround>().target = satellite;

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
            // Сделать невидимым до завершения анимации
            newAircraft.transform.localScale = new Vector3(0f, 0f, 0f);

            if (aircrafts.Count == 0)
                // Если самолет первый в списке, целью следования назначается спутник
                newAircraft.GetComponent<FlyAround>().target = satellite;
            else
                // Иначе целью назначается последний самолет в списке
                newAircraft.GetComponent<FlyAround>().target = aircrafts[aircrafts.Count - 1];

            // Добавление в список
            aircrafts.Add(newAircraft);
        }
    }
}
