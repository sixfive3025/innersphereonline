using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    public GameInstaller.Settings GameInstaller;
    public GameController.Settings GameController;
    public UIManager.Settings UIManager;
    public ISONetworkManager.Settings NetworkManager;
    public StarSystemController.Settings StarSystemController;
    public InnerSphereBuilder.Settings InnerSphereBuilder;
    public CameraHandler.Settings CameraHandler;

    public override void InstallBindings()
    {
        Container.BindInstance(GameInstaller);
        Container.BindInstance(GameController);
        Container.BindInstance(UIManager);
        Container.BindInstance(NetworkManager);
        Container.BindInstance(StarSystemController);
        Container.BindInstance(InnerSphereBuilder);
        Container.BindInstance(CameraHandler);
    }
}