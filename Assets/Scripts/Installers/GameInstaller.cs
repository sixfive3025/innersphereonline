using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [Inject]
    Settings _settings = null;

    private const string NAME_InnerSphereObj = "InnerSphereBuilder";
    private const string NAME_RegimentObj = "RegimentBuilder";
    private const string NAME_NetworkObj = "Network";

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        Container.BindInterfacesAndSelfTo<SystemRegistry>().AsSingle();
        Container.Bind<ISONetworkManager>().FromNewComponentOnNewGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.Bind<IInitializable>().To<ISONetworkManager>().FromNewComponentOnNewGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.Bind<IDisposable>().To<ISONetworkManager>().FromNewComponentOnNewGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.BindInterfacesAndSelfTo<LocalPlayerManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<UIManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<InnerSphereBuilder>().FromNewComponentOnNewGameObject().WithGameObjectName(NAME_InnerSphereObj).AsSingle();
        Container.BindInterfacesAndSelfTo<SharedCurveTracker>().AsSingle();

        Container.BindInterfacesAndSelfTo<RegimentBuilder>().FromNewComponentOnNewGameObject().WithGameObjectName(NAME_RegimentObj).AsSingle();
        Container.BindFactory<RegimentController, RegimentController.Factory>().FromComponentInNewPrefab(_settings.RegimentPrefab);
        Container.BindFactory<RegimentController, RegimentChit, RegimentChit.Factory>().FromComponentInNewPrefab(_settings.RegimentChitPrefab);

        Container.Bind<SignalDispatcher>().To<SignalDispatcher>().AsSingle();
        Container.DeclareSignal<Signals.FactionSelected>();
        Container.DeclareSignal<Signals.SystemFactionChanged>();
        Container.DeclareSignal<Signals.RegimentMoved>();
        Container.DeclareSignal<Signals.PlayerJoined>();
        Container.DeclareSignal<Signals.PlayerDeparted>();
        Container.DeclareSignal<Signals.FatalError>();
        Container.DeclareSignal<MoveCameraToPositionCmd>();
        Container.DeclareSignal<ShowClickCoordinatesCmd>();

        Container.BindInterfacesAndSelfTo<CameraHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<MouseHandler>().AsSingle();

        Container.BindFactory<FactionPickerUI, FactionPickerUI.Factory>().FromComponentInNewPrefab(_settings.FactionPickerPrefab);
        Container.BindFactory<ShowCoordsUI, ShowCoordsUI.Factory>().FromComponentInNewPrefab(_settings.ShowCoordsPrefab);
        Container.BindFactory<PlayerHUDUI, PlayerHUDUI.Factory>().FromComponentInNewPrefab(_settings.PlayerHUDPrefab);
        Container.BindFactory<PlayerController, PlayerCardUI, PlayerCardUI.Factory>().FromComponentInNewPrefab(_settings.PlayerCardPrefab);
        Container.BindFactory<StarSystemController, SystemHUDUI, SystemHUDUI.Factory>().FromComponentInNewPrefab(_settings.SystemHUDPrefab);
        Container.BindFactory<string, ErrorModalUI, ErrorModalUI.Factory>().FromComponentInNewPrefab(_settings.ErrorModalPrefab);

        Container.BindSignal<Vector3, MoveCameraToPositionCmd>().To<CameraHandler>(x => x.StartGlide).AsSingle();
        Container.BindSignal<double, double, ShowClickCoordinatesCmd>().To<UIManager>(x => x.UpdateCoordsUI).AsSingle();
    }

    [Serializable]
    public class Settings
    {
        public GameObject FactionPickerPrefab;
        public GameObject ShowCoordsPrefab;
        public GameObject StarSystemPrefab;

        public GameObject PlayerHUDPrefab;
        public GameObject PlayerCardPrefab;
        public GameObject SystemHUDPrefab;
        public GameObject ErrorModalPrefab;
        public GameObject RegimentPrefab;
        public GameObject RegimentChitPrefab;
    }
}