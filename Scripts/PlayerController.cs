using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRB;
    private SphereCollider playerCollider;

    private Vector3 dashVelocity = Vector3.zero;
    private bool isDashing = false;

    // Playerの速度
    private float playerSpeed;

    // Playerに加える力
    public float moveSpeed = 75f;
    // Playerの回転速度
    public float rotateSpeed = 100f;
    // 最高速度
    public float maxSpeed = 75f;
    public float speedThreshold = 75f;  // 判定基準速度

    // Starに乗った時に加えられるベクトルと力
    Vector3 star1Vector = new Vector3(0.31f, 0f, -0.95f);
    Vector3 star2Vector = new Vector3(-0.81f, 0f, 0.59f);
    Vector3 star3Vector = new Vector3(1f, 0f, 0f);
    Vector3 star4Vector = new Vector3(-0.81f, 0f, -0.59f);
    Vector3 star5Vector = new Vector3(0.15f, 0.5f, 0.8f);
    public float starSpeed = 150f;

    // Groundとの接地
    public LayerMask groundLayer;

    // Starとの接地
    public LayerMask star1Layer;
    public LayerMask star2Layer;
    public LayerMask star3Layer;
    public LayerMask star4Layer;
    public LayerMask star5Layer;

    void Start()
    {
        playerRB = this.GetComponent<Rigidbody>();
        playerCollider = GetComponent<SphereCollider>();

        Application.targetFrameRate = 60; // フレームレートの決定
    }

    private void Update()
    {
        // Playerの速度を計算
        float playerSpeed = Mathf.Abs(playerRB.velocity.magnitude);

        // rでリスタート
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }
    }

    void FixedUpdate()
    {
        // 最高速度以下でボタン押したらPlayerを前進
        if (playerSpeed < maxSpeed && IsGrounded())
        {
            // 地面の法線を取得（Raycast で足元の情報を取得）
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
            {
                // 入力方向（ローカル座標での前方向）
                Vector3 inputDir = transform.forward * Input.GetAxis("Vertical");

                // 地面の法線に対して、移動方向を投影
                Vector3 moveDir = Vector3.ProjectOnPlane(inputDir, hit.normal).normalized;

                // 投影された方向に力を加える
                playerRB.AddForce(moveDir * moveSpeed, ForceMode.Acceleration);
            }
        }

        if (isDashing)
        {
            playerRB.velocity = Vector3.zero; // 安定化のため一度リセット
            playerRB.AddForce(dashVelocity, ForceMode.VelocityChange);
            isDashing = false; // 一回だけ発動
        }

        // Starに乗ったら星
        if (Isstar1ed())
        {
            playerRB.velocity = star1Vector * starSpeed;
        }
        if (Isstar2ed())
        {
            playerRB.velocity = star2Vector * starSpeed;
        }
        if (Isstar3ed())
        {
            playerRB.velocity = star3Vector * starSpeed;
        }
        if (Isstar4ed())
        {
            playerRB.velocity = star4Vector * starSpeed;
        }
        if (Isstar5ed())
        {
            playerRB.velocity = star5Vector * starSpeed;
        }

        // 向き
        Vector3 velocity = playerRB.velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
        float verticalSpeed = Mathf.Abs(velocity.y);
        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            // 地上：手動でY回転
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(0f, horizontalInput * rotateSpeed * Time.deltaTime, 0f, Space.Self);

            //  傾きをゆっくり補正（自然に直立）
            Quaternion currentRot = transform.rotation;
            Vector3 currentEuler = currentRot.eulerAngles;
            Quaternion targetRot = Quaternion.Euler(0f, currentEuler.y, 0f);  // 傾きゼロ
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * 2f); // ←補間で滑らか
        }
        else if (horizontalSpeed >= speedThreshold)
        {
            // 空中で水平方向に高速移動
            Vector3 horizontalDir = new Vector3(velocity.x, velocity.y, velocity.z);
            if (horizontalDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(horizontalDir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
        }
        else if (verticalSpeed >= speedThreshold)
        {
            //  空中で垂直方向に高速移動
            Vector3 dir = velocity.normalized;
            Vector3 up = Vector3.up;

            if (Mathf.Abs(Vector3.Dot(dir, up)) > 0.99f)
                up = Vector3.forward;

            Quaternion targetRot = Quaternion.LookRotation(dir, up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
        else
        {
            // 空中で低速時：Y軸のみ回転し、X/Zの傾きを徐々に補正
            float horizontalInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                Quaternion yaw = Quaternion.Euler(0f, horizontalInput * rotateSpeed * Time.deltaTime, 0f);
                transform.rotation = yaw * transform.rotation;

                // 傾きを徐々に戻す（XZ軸補正）
                Vector3 euler = transform.rotation.eulerAngles;
                Quaternion targetRot = Quaternion.Euler(0f, euler.y, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }
        }
    }

    // PlayerとGroundの接地判定
    private bool IsGrounded()
    {
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);
        bool grounded = Physics.CheckSphere(playerRB.position, 1f, groundLayer);
        return grounded;
    }


    // PlayerとStarの接地判定
    private bool Isstar1ed()
    {
        // Playerの足の位置を位置ベクトルとして設定する。
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);

        // Playerが地面に足をついていればtrue、ついていなければfalseとなる変数upedを宣言する。
        bool star1ed = Physics.CheckSphere(playerRB.position, 1f, star1Layer);

        /*IsGrounded()はgroundedの
        値を返すメソッド*/
        return star1ed;
    }
    private bool Isstar2ed()
    {
        // Playerの足の位置を位置ベクトルとして設定する。
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);

        // Playerが地面に足をついていればtrue、ついていなければfalseとなる変数upedを宣言する。
        bool star2ed = Physics.CheckSphere(playerRB.position, 1f, star2Layer);

        /*IsGrounded()はgroundedの
        値を返すメソッド*/
        return star2ed;
    }
    private bool Isstar3ed()
    {
        // Playerの足の位置を位置ベクトルとして設定する。
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);

        // Playerが地面に足をついていればtrue、ついていなければfalseとなる変数upedを宣言する。
        bool star3ed = Physics.CheckSphere(playerRB.position, 1f, star3Layer);

        /*IsGrounded()はgroundedの
        値を返すメソッド*/
        return star3ed;
    }
    private bool Isstar4ed()
    {
        // Playerの足の位置を位置ベクトルとして設定する。
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);

        // Playerが地面に足をついていればtrue、ついていなければfalseとなる変数upedを宣言する。
        bool star4ed = Physics.CheckSphere(playerRB.position, 1f, star4Layer);

        /*IsGrounded()はgroundedの
        値を返すメソッド*/
        return star4ed;
    }
    private bool Isstar5ed()
    {
        // Playerの足の位置を位置ベクトルとして設定する。
        Vector3 playerBottom = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y, playerCollider.bounds.center.z);

        // Playerが地面に足をついていればtrue、ついていなければfalseとなる変数upedを宣言する。
        bool star5ed = Physics.CheckSphere(playerRB.position, 1f, star5Layer);

        /*IsGrounded()はgroundedの
        値を返すメソッド*/
        return star5ed;
    }

    // rでリスタートのメソッド
    public void RestartCurrentScene()
    {
        UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1.0f;
    }

    public void TriggerDash(Vector3 velocity)
    {
        dashVelocity = velocity;
        isDashing = true;
    }
}
