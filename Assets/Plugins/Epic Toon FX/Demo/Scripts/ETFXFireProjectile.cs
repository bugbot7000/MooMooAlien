using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using NaughtyAttributes;

namespace EpicToonFX
{
    public class ETFXFireProjectile : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] projectiles;
        [Header("Missile spawns at attached game object")]
        public Transform spawnPosition;
        [HideInInspector]
        public int currentProjectile = 0;
        public float speed = 500;
        public float fireSpeed,fireTimer;
        public float MaxAmmo, CurrentAmmo;
        public Vector3 ProjectileScale;
        private AudioSource Sound;
        public AudioClip emptySound, reloadSound;
        private Animator anim;
        void Start()
        {
            CurrentAmmo = MaxAmmo;
            anim = GetComponent<Animator>();
            Sound = GetComponent<AudioSource>();
        }

        RaycastHit hit;

        public void CheckAndFire()
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireSpeed)
            {
                if (CurrentAmmo > 0)
                {

                    FireBullet();
                    Debug.Log("Shot Bullet No. " + CurrentAmmo);
                    fireTimer = 0;
                }
                else
                {
                    anim.Play("Gun Empty");
                    fireTimer = 0;
                    Sound.PlayOneShot(emptySound);
                }
            }
        }

        public void ReloadGun()
        {
            CurrentAmmo = MaxAmmo;
            anim.Play("Gun Reload");
            Sound.PlayOneShot(reloadSound);
            fireTimer = 0;
        }
        [Button("Fire Gun)")]
        private void FireBullet()
        {
            GameObject projectile =
                Instantiate(projectiles[currentProjectile], spawnPosition.position,
                    Quaternion.identity) as GameObject; //Spawns the selected projectile
            projectile.GetComponent<ETFXProjectileScript>().CustomScale = ProjectileScale;
            projectile.transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x,
                transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
         //   projectile.transform.localRotation = Quaternion.LookRotation(transform.position);
           // projectile.transform.LookAt(transform.forward); //Sets the projectiles rotation to look at the point clicked
           // Debug.Break();
            projectile.GetComponent<Rigidbody>()
                .AddForce(projectile.transform.forward *
                          speed); //Set the speed of the projectile by applying force to the rigidbody
            CurrentAmmo--;
        }
    }
}