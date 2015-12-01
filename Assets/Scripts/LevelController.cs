using UnityEngine;
using System.Collections;

public class LevelController : GenericSingleton<LevelController>
{
    public PathManager _path;
    public PathEventManager _eventManger;
    public LegionMarchController _legionCtrl;
    public GameObject _heroObj;
    public Canvas _uiCanvas;

    private void Start()
    {
        _path.EnableEdit(false);
        //PathSurfaceWalker walker = PathSurfaceWalker.AttachPathSurfaceWalker(Instantiate(_enemyPrefab), _path, 0, 1f, 5f, 0f, 1f);
        //walker.MoveOnPathSurface(0, 1f, 0f);
        ManualLoadEvents();

        _legionCtrl.Init();
        _legionCtrl.StartControlHero();
        _legionCtrl.StartControlCrowd();
        _legionCtrl._crowd.GetComponent<CrowdManager>().SpawnCitizen(_legionCtrl._crowd.transform.forward);
    }

    public void ManualLoadEvents()
    {
        var event0 = TutorialEvent.CreateNewEvent(0, 0.4f, 1, 0f);
        event0._copNodeIdx = 1;
        event0._copTngOffset = 0.9f;
        _eventManger._events.Add(event0);

        var event1 = CopSquadEvent.CreateNewEvent(3, 0.5f, 5, 0.5f, 3f);
        event1._copNum = 2;
        event1._attackInteval = 3f;
        event1._attackTimerOffset = 1f;
        event1._speed = 3.0f;
        _eventManger._events.Add(event1);

        var event2 = CopSquadEvent.CreateNewEvent(7, 0.5f, 9, 1f, 5f);
        event2._copNum = 3;
        event2._attackInteval = 2.5f;
        event2._attackTimerOffset = 1f;
        event2._speed = 5.0f;
        _eventManger._events.Add(event2);

        var event3 = CopMotorcadeEvent.CreateNewEvent(11, 0.5f, 13, 1f, 5f);
        event3._copNum = 4;
        event3._attackInteval = 2f;
        event3._attackTimerOffset = 1f;
        event3._speed = 5.0f;
        _eventManger._events.Add(event3);

        var event4 = CopMotorcadeEvent.CreateNewEvent(13, 0.5f, 15, 1f, 5f);
        event4._copNum = 5;
        event4._attackInteval = 1f;
        event4._attackTimerOffset = 1f;
        event4._speed = 5.0f;
        _eventManger._events.Add(event4);

        var event5 = TankEvent.CreateNewEvent(17, 0.5f, 19, 1f, 2f);
        event5._speed = 2.0f;
        _eventManger._events.Add(event5);

        AscendingEvent eventFinal = AscendingEvent.CreateNewEvent(20, 0.0f);
        eventFinal._ascendingCtrl._param = 3f;
        eventFinal._ascendingCtrl._startDist = 3f;
        eventFinal._ascendingCtrl._endDist = 0.1f;
        eventFinal._maxSpeed = 5f;
        eventFinal._acc = 0.5f;
        _eventManger._events.Add(eventFinal);

        //test end
        //AscendingEvent eventFinal = AscendingEvent.CreateNewEvent(1, 0.5f);
        //eventFinal._ascendingCtrl._param = 3f;
        //eventFinal._ascendingCtrl._startDist = 3f;
        //eventFinal._ascendingCtrl._endDist = 0.1f;
        //eventFinal._maxSpeed = 5f;
        //eventFinal._acc = 0.5f;
        //_eventManger._events.Add(eventFinal);
    }
}
