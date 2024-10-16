using System.Collections.Generic;
using UnityEngine;

namespace ShootRelated
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private BulletController _bulletPrefab;
        [SerializeField] private int _initialPoolSize = 20;
    
        private List<BulletController> _freeBullets = new();
        private List<BulletController> _usedBullets = new();

        private void Start()
        {
            for (int i = 0; i < _initialPoolSize; i++) CreateNewBullet();
        }

        private void CreateNewBullet()
        {
            BulletController newBullet = Instantiate(_bulletPrefab, transform);
            newBullet.Initialize(_ => ReturnBullet(newBullet));
            newBullet.gameObject.SetActive(false);
            _freeBullets.Add(newBullet);
        }

        public BulletController GetBullet()
        {
            BulletController bullet;

            if (_freeBullets.Count > 0)
            {
                bullet = _freeBullets[0];
                _freeBullets.RemoveAt(0);
            }
            else
            {
                bullet = Instantiate(_bulletPrefab, transform);
                bullet.Initialize(_ => ReturnBullet(bullet));
            }

            _usedBullets.Add(bullet);
            return bullet;
        }

        public void ReturnBullet(BulletController bullet)
        {
            if (_usedBullets.Contains(bullet))
            {
                _usedBullets.Remove(bullet);
                _freeBullets.Add(bullet);
                bullet.gameObject.SetActive(false);
            }
        }
    }
}