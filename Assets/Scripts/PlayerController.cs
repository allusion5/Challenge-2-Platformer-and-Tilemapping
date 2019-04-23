using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer marioSprite;

    public float speed;
    public float jumpForce;
    public float invincibilityFlashSpeed;
    private bool isGrounded = true;
    private bool isDead;
    private bool isSuper = false;
    private Rigidbody2D Mario;
    private CapsuleCollider2D MarioCollider;
    private AudioManager audioManager;

    public static PlayerController instance;

    public Text livesText;
    public Text coinText;
    public Text winText;
    public Text timerText;
    public GameObject UI;
    private int livesCount;
    private int coinCount;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        FindObjectOfType<AudioManager>().Play("background1.1");

        Mario = GetComponent<Rigidbody2D>();
        MarioCollider = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        marioSprite = GetComponent<SpriteRenderer>();
        audioManager = GetComponent<AudioManager>();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(UI);

        winText.text = " ";

        livesCount = 3;
        coinCount = 0;
        SetCoinCount();
        SetLivesCount();
        
    }

    void Update()
    {   
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Mario.position.y < -10)
        {
            isGrounded = true;
            isDead = false;

            if (Mario.position.y < -30 && livesCount > 0)
            {
                MarioCollider.isTrigger = false;
                Mario.AddForce(Vector2.zero);
                marioSprite.flipX = false;
                Mario.MovePosition(new Vector2 (-10.85f, -2.495f));

                if (coinCount < 4)
                {
                    FindObjectOfType<AudioManager>().Play("background1.1");
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("background1.2");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        void SetAnimState()
        {
            if (isGrounded)
            {
                if (Mario.velocity.x != 0)
                {
                    anim.SetInteger("State", 1);
                }
                else
                {
                    anim.SetInteger("State", 0);
                }
            }

            else
            {
                anim.SetInteger("State", 2);
            }

        }
        void SetFacing()
        {
            if (Mario.velocity.x > 0)
            {
                marioSprite.flipX = false;
            }
            if (Mario.velocity.x < 0)
            {
                marioSprite.flipX = true;
            }
        }

        SetFacing();
        SetAnimState();

        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector2 movement = new Vector2(moveHorizontal, 0);

        Mario.AddForce(movement * speed);
    }
    //Jump and boolean for Is Grounded for animation state jump
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true;
        }

        if (isGrounded == true && !isDead)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (isSuper)
                {
                    FindObjectOfType<AudioManager>().Play("superJump");
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("marioJump");
                }
                Mario.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isGrounded = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup") && !isDead)
        {
            FindObjectOfType<AudioManager>().Play("powerupCollect");
            collision.gameObject.SetActive(false);
            anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SuperMario");
            MarioCollider.size = new Vector2(1, 2);
            isSuper = true;
        }

        if (collision.gameObject.CompareTag("Wall") && Mario.velocity.y == 0 && !isGrounded)
        {
            FindObjectOfType<AudioManager>().Play("bump");
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isSuper)
            {
                anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Mario");
                FindObjectOfType<AudioManager>().Play("powerDown");
                StartCoroutine(InvincibilityFrames(invincibilityFlashSpeed));
            

                IEnumerator InvincibilityFrames(float x)
                {
                    Physics2D.IgnoreLayerCollision(2, 8, true);
                    for (int i=0; i <10; i++)
                    {
                        marioSprite.enabled = false;
                        yield return new WaitForSecondsRealtime(x);
                        marioSprite.enabled = true;
                        yield return new WaitForSecondsRealtime(x);
                    }
                    Physics2D.IgnoreLayerCollision(2, 8, false);
                }
                MarioCollider.size = new Vector2(0.8125f, 1);
                isSuper = false;
            }
            else
            {
                isDead = true;
                livesCount--;
                SetLivesCount();

                if (coinCount < 4)
                {
                    FindObjectOfType<AudioManager>().Pause("background1.1");
                }
                else
                {
                    FindObjectOfType<AudioManager>().Pause("background1.2");
                }
                if (isDead)
                {
                    FindObjectOfType<AudioManager>().Play("marioDeath");
                    Mario.velocity = Vector2.zero;
                    anim.SetInteger("State", 3);
                    MarioCollider.isTrigger = true;
                    Mario.velocity = new Vector2(0, 6);
                    collision.gameObject.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin") && !isDead)
        {
            FindObjectOfType<AudioManager>().Play("pickupCoin");
            collision.gameObject.SetActive(false);
            coinCount++;
            SetCoinCount();

            if (coinCount == 4)
            {
                FindObjectOfType<AudioManager>().Pause("background1.1");
                FindObjectOfType<AudioManager>().Play("stageClear");
                Time.timeScale = 0;

                IEnumerator ChangeScene()
                {
                    yield return new WaitForSecondsRealtime(5.5f);
                    Mario.velocity = Vector2.zero;
                    isGrounded = false;
                    marioSprite.flipX = false;
                    anim.SetInteger("State", 2);
                    Mario.transform.position = (new Vector2(-10.85f, 10));
                    livesCount = 3;
                    SetLivesCount();
                    SceneManager.LoadScene(sceneBuildIndex: 1, LoadSceneMode.Single);
                    FindObjectOfType<AudioManager>().Play("background1.2");
                    Time.timeScale = 1;
                }

                StartCoroutine(ChangeScene());
            }

            if (coinCount == 8)
            {
                FindObjectOfType<AudioManager>().Pause("background1.2");
                FindObjectOfType<AudioManager>().Play("worldClear");
                Time.timeScale = 0;
                winText.text = "WORLD CLEAR";
            }
        }
    }

    void OnCollisonExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }

    private void LateUpdate()
    {
        if (Mario.position.y < -20 && livesCount < 1)
        {
            Mario.transform.position = (new Vector2(0, -10));
            Mario.constraints = RigidbodyConstraints2D.FreezePosition;
            FindObjectOfType<AudioManager>().Play("gameOver");
            winText.text = "GAME OVER";
        }
    }
    void SetCoinCount()
    {
        coinText.text = "x0" + coinCount.ToString();
    }
    void SetLivesCount()
    {
        livesText.text = "0" + livesCount.ToString();
    }
}