using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavyJones : MonoBehaviour
{
    public enum AreaZone
    {
        AreaOne,
        AreaTwo,
        AreaThree
    }

    public bool FireSoundActive, StepSoundActive, LaughSoundActive;
    [SerializeField] private GameObject Player, Bullet, Pistol, Fireball, SpawnZone1, SpawnZone2, SpawnZone3;
    [SerializeField] private Animator DavyAnimator;
    [SerializeField] private AudioClip[] HitSounds;
    [SerializeField] private AudioSource DavyAudioSource;
    private bool IsPlayerHitArea;
    public AreaZone DavyArea = AreaZone.AreaOne;
    public float Health = 100.0f;
    private float Reload = 1.0f;
    private bool RespawnDavy, IsLaughs;
    private int PlayerZone;
    private float DistanceToPlayer;

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "AreaOne":
                DavyArea = AreaZone.AreaOne;
                break;
            case "AreaTwo":
                DavyArea = AreaZone.AreaTwo;
                break;
            case "AreaThree":
                DavyArea = AreaZone.AreaThree;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            transform.position = SpawnZone1.transform.position;
        }
    }

    void FixedUpdate()
    {
        DistanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
        if (DistanceToPlayer < 0.5f)
            IsPlayerHitArea = true;
        else
            IsPlayerHitArea = false;
            
        Look(Player.transform.position, 10f);
        if (!IsLaughs)
        {
            if ((int)DavyArea == (int)Player.GetComponent<Control>().PlayerArea)
            {
                if (!IsPlayerHitArea)
                {
                    FireSoundActive = false;
                    StepSoundActive = true;
                    LaughSoundActive = false;
                    DavyAnimator.SetBool("IsMove", true);
                    DavyAnimator.SetBool("IsShoot", false);
                    DavyAnimator.SetBool("IsAttack", false);
                    DavyAnimator.SetBool("IsLaughs", false);
                    Move();
                }
                else
                {
                    FireSoundActive = false;
                    StepSoundActive = false;
                    LaughSoundActive = false;
                    DavyAnimator.SetBool("IsAttack", true);
                    DavyAnimator.SetBool("IsMove", false);
                    DavyAnimator.SetBool("IsShoot", false);
                    DavyAnimator.SetBool("IsLaughs", false);
                    Hit();
                }
            }
            else
            {
                StepSoundActive = false;
                LaughSoundActive = false;
                DavyAnimator.SetBool("IsAttack", false);
                DavyAnimator.SetBool("IsMove", false);
                DavyAnimator.SetBool("IsShoot", true);
                DavyAnimator.SetBool("IsLaughs", false);
                Reload -= Time.deltaTime;
                if (Reload < 0)
                {
                    FireSoundActive = true;
                    Shoot();
                }

            }
        }
        else
        {
            FireSoundActive = false;
            StepSoundActive = false;
            LaughSoundActive = true;
            DavyAnimator.SetBool("IsAttack", false);
            DavyAnimator.SetBool("IsMove", false);
            DavyAnimator.SetBool("IsShoot", false);
            DavyAnimator.SetBool("IsLaughs", true);
        }

        RespawnDavy = Player.GetComponent<Control>().TimeToRespawnDavyFlag;
        PlayerZone = (int)Player.GetComponent<Control>().PlayerArea;

        Respawn();

        if (Health < 0)
            Destroy(gameObject);

        //Что-то типа:
        /*
        if (Health == 70 || Health == 40 || Health == 10)
        {
            //Злобный крик и респавн на палубе
        }
        */
    }

    public void IsDavyHit(float damage)
    {
        Health -= damage;
        var rnd = UnityEngine.Random.Range(0, HitSounds.Length - 1);
        DavyAudioSource.PlayOneShot(HitSounds[rnd]);
    }

    void Respawn()
    {
        if (RespawnDavy)
        {
            IsLaughs = true;
            Invoke("SpawnDavy", 1.9f);
            RespawnDavy = Player.GetComponent<Control>().TimeToRespawnDavyFlag = false;
        }
    }
    
    void SpawnDavy()
    {
        IsLaughs = false;
        DavyAnimator.SetBool("IsLaughs", false);
        switch (PlayerZone)
        {
            case 0:
                transform.position = SpawnZone1.transform.position;
                break;
            case 1:
                transform.position = SpawnZone2.transform.position;
                break;
            case 2:
                transform.position = SpawnZone3.transform.position;
                break;
            default:
                break;
        }
    }

    void Look(Vector3 point, float speed)
    {
        var direction = (point - transform.position).normalized;
        direction.y = 0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), speed);
    }

    void Move()
    {
        transform.position = transform.position + transform.forward * 0.055f;
    }

    void Shoot()
    {
        //Звук выстрела
        Reload = 1.0f;
        //Pistol.transform.LookAt(Player.transform, Pistol.transform.forward);
        var muzzle = Pistol.transform.Find("Muzzle");
        var newBullet = Instantiate(Bullet);
        newBullet.transform.SetParent(transform.parent);
        var newFireball = Instantiate(Fireball);
        newFireball.transform.position = muzzle.transform.position;
        newFireball.transform.rotation = muzzle.transform.rotation;
        newBullet.transform.position = muzzle.transform.position;
        var newVect = Player.transform.Find("Main Camera").transform.position - muzzle.transform.position;
        //newBullet.transform.LookAt(Player.transform, newBullet.transform.up);
        newFireball.GetComponent<Rigidbody>().AddForce(newVect.normalized * -10);
        newBullet.GetComponent<Rigidbody>().AddForce(newVect.normalized * 2000);
    }

    void Hit()
    {
        //Звук удара саблей
    }
}
