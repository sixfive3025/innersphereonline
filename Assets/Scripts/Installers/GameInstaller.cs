using System;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller<GameInstaller>
{
    [Inject]
    Settings _settings = null;

    private const string NAME_InnerSphereObj = "InnerSphereBuilder";
    private const string NAME_NetworkObj = "Network";

    public override void InstallBindings()
    {
        Container.BindAllInterfacesAndSelf<GameController>().To<GameController>().AsSingle();
        Container.Bind<ISONetworkManager>().FromGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.Bind<IInitializable>().To<ISONetworkManager>().FromGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.Bind<IDisposable>().To<ISONetworkManager>().FromGameObject().WithGameObjectName(NAME_NetworkObj).AsSingle();
        Container.BindAllInterfacesAndSelf<LocalPlayerManager>().To<LocalPlayerManager>().AsSingle();
        Container.BindAllInterfacesAndSelf<UIManager>().To<UIManager>().AsSingle();
        Container.BindAllInterfacesAndSelf<InnerSphereBuilder>().To<InnerSphereBuilder>().FromGameObject().WithGameObjectName(NAME_InnerSphereObj);

        Container.Bind<SignalDispatcher>().To<SignalDispatcher>().AsSingle();
        Container.BindSignal<Signals.FactionSelected>();
        Container.BindSignal<Signals.SystemFactionChanged>();
        Container.BindSignal<Signals.PlayerJoined>();
        Container.BindSignal<Signals.PlayerDeparted>();
        Container.BindSignal<Signals.FatalError>();

        Container.BindAllInterfacesAndSelf<CameraHandler>().To<CameraHandler>().AsSingle();
        Container.BindAllInterfacesAndSelf<MouseHandler>().To<MouseHandler>().AsSingle();

        Container.BindFactory<FactionPickerUI, FactionPickerUI.Factory>().FromPrefab(_settings.FactionPickerPrefab);
        Container.BindFactory<ShowCoordsUI, ShowCoordsUI.Factory>().FromPrefab(_settings.ShowCoordsPrefab);
        Container.BindFactory<PlayerHUDUI, PlayerHUDUI.Factory>().FromPrefab(_settings.PlayerHUDPrefab);
        Container.BindFactory<PlayerController, PlayerCardUI, PlayerCardUI.Factory>().FromPrefab(_settings.PlayerCardPrefab);
        Container.BindFactory<StarSystemController, SystemHUDUI, SystemHUDUI.Factory>().FromPrefab(_settings.SystemHUDPrefab);
        Container.BindFactory<string, ErrorModalUI, ErrorModalUI.Factory>().FromPrefab(_settings.ErrorModalPrefab);

        Container.BindCommand<MoveCameraToPositionCmd, Vector3>().To<CameraHandler>(x => x.StartGlide).AsSingle();
        Container.BindCommand<ShowClickCoordinatesCmd, double, double>().To<UIManager>(x => x.UpdateCoordsUI).AsSingle();
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
    }
}