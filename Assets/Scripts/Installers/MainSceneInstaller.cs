using Player;
using ShootRelated;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private CarController _playerCarController;
        [SerializeField] private BulletPool _bulletPool;
    
        public override void InstallBindings()
        {
            Container
                .Bind<CarController>()
                .FromInstance(_playerCarController)
                .AsSingle();
        
            Container
                .Bind<BulletPool>()
                .FromInstance(_bulletPool)
                .AsSingle(); //TODO Change to factory
        }
    }
}