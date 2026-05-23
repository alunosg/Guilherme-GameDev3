using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rig;
    public Collider col;
    public float speed = 10;
    public float jumpforce = 10;
    public float turningSpeedX = 1;
    public float turningSpeedY = 1;
    public float minRotX = -30;
    public float maxRotX = 75;
    public LayerMask floorlayer;
    public Transform camTarget;

    private Vector2 moveInput;

    public Transform aimImage;
    public GameObject bulletPrefab;

    public Transform shootTransform;
    public GameObject shootFX;

    public AudioSource audioSource;
    public AudioClip attackSFX, hitSFX, deathSFX;

    public Image hpImage;
    public float hp = 5;
    private float maxHp;

    public float deathDuration = 2;
    private bool locked = false;

    private void Start()
    {
        maxHp = hp;
    }

    public void GetHit(float damage)
    {
        if(hp > 0)
        {
            hp -= damage;
            hpImage.fillAmount = Mathf.Max(0, hp / maxHp);

            if (hp <= 0)
            {
                audioSource.PlayOneShot(deathSFX);
                locked = true;
                moveInput = Vector3.zero;
                rig.linearVelocity = Vector3.zero;
                Invoke(nameof(Reload), deathDuration);
            }
            else
            {
                audioSource.PlayOneShot(hitSFX);
            }
        }
    }

    private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!locked && context.started)
        {
            audioSource.PlayOneShot(attackSFX);

            Instantiate(shootFX, shootTransform);

            Rigidbody b = Instantiate(bulletPrefab,
                camTarget.position + camTarget.forward * 1,
                camTarget.rotation).GetComponent<Bullet>().rig;

            b.linearVelocity = rig.linearVelocity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!locked) moveInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (locked) return;

        Vector3 angles = new(0, transform.eulerAngles.y, 0);
        angles.y += context.ReadValue<Vector2>().x * turningSpeedX;
        Quaternion rot = Quaternion.Euler(angles);
        rig.MoveRotation(rot);

        float camX = camTarget.localEulerAngles.x;
        camX -= context.ReadValue<Vector2>().y * turningSpeedY;

        if (camX > 180f) camX -= 360f;
        else if (camX < -180f) camX += 360f;

        camX = Mathf.Clamp(camX, minRotX, maxRotX);
        camTarget.localEulerAngles = new(camX, 0f, 0f);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!locked && context.started)
        {
            Vector3 p1 = new(col.bounds.max.x, col.bounds.max.y, col.bounds.max.z);
            Vector3 p2 = new(col.bounds.min.x, col.bounds.max.y, col.bounds.min.z);
            Vector3 p3 = new(col.bounds.min.x, col.bounds.max.y, col.bounds.max.z);
            Vector3 p4 = new(col.bounds.max.x, col.bounds.max.y, col.bounds.min.z);
            Vector3 p5 = new(col.bounds.center.x, col.bounds.max.y, col.bounds.center.z);

            if (Physics.Raycast(p1, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p2, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p3, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p4, Vector3.down, col.bounds.size.y * 1.1f, floorlayer) ||
                Physics.Raycast(p5, Vector3.down, col.bounds.size.y * 1.1f, floorlayer))
            {
                rig.linearVelocity = new(rig.linearVelocity.x, jumpforce, rig.linearVelocity.z);
            }
        }
    }

    private void FixedUpdate()
    {
        if (locked) return;

        Vector3 vX = moveInput.x * speed * transform.right;
        Vector3 vY = rig.linearVelocity.y * Vector3.up;
        Vector3 vZ = moveInput.y * speed * transform.forward;
        rig.linearVelocity = vX + vY + vZ;
    }
}