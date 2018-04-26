using UnityEngine;

/// <summary>
/// 銃
/// </summary>
public class Gun : MonoBehaviour
{
    /// <summary>
    /// 弾
    /// </summary>
    [SerializeField]
    private Bullet bullet;

    /// <summary>
    /// リサイクルマネージャー
    /// </summary>
    [SerializeField]
    private RecycleManager recycleMgr;

    /// <summary>
    /// リコイル値
    /// </summary>
    [SerializeField]
    private float recoilValue = 4;

    /// <summary>
    /// リコイル値
    /// </summary>
    [SerializeField]
    private float recoilSeedMin = -0.01f;

    /// <summary>
    /// リコイル値
    /// </summary>
    [SerializeField]
    private float recoilSeedMax = 0.01f;

    /// <summary>
    /// 弾速
    /// </summary>
    [SerializeField]
    private float fireSpeed = 100;

    /// <summary>
    /// 連射速度
    /// </summary>
    [SerializeField]
    private float fireRate = 200;

    /// <summary>
    /// 連射速度(毎秒)
    /// </summary>
    private float fireRatePerSec;

    /// <summary>
    /// 発射秒数
    /// </summary>
    private float fireSec;

    /// <summary>
    /// Start
    /// </summary>
    void Start ()
    {
        // ファイヤーレート関連データ初期化
        fireSec = -1;
        fireRatePerSec = 60 / fireRate;
        
	}

    /// <summary>
    /// FixedUpdate
    /// </summary>
    void FixedUpdate ()
    {
        if (Input.GetKey("d"))
        {
            recycleMgr.ClearRecyleContainer(bullet.gameObject);
        }

        if (CheckFire(Input.GetKey("space")))
        {
            Fire();
        }
	}

    /// <summary>
    /// 発射可否判定
    /// </summary>
    /// <param name="isFireBtn">ファイヤボタン押下状況</param>
    /// <returns>true:発射可能/false:発射不可能</returns>
    private bool CheckFire(bool isFireBtn)
    {
        // ボタンが押されてなければ発射しない
        if (!isFireBtn)
        {
            fireSec = -1;
            return false;
        }

        // 初回は問答無用に発射
        if (fireSec == -1)
        {
            fireSec = 0;
            return true;
        }

        // 2回目以降はファイヤーレートを元に発射可否を判断する

        // 発射秒数加算
        fireSec += Time.deltaTime;

        // ファイアレートに達していなければスキップ
        if (fireSec < fireRatePerSec)
        {
            return false;
        }

        // 一発分秒数を減産
        fireSec -= fireRatePerSec;

        return true;
    }

    /// <summary>
    /// 発射
    /// </summary>
    protected virtual void Fire()
    {
        // 弾丸作成
        GameObject newBullet = recycleMgr.Instantiate(bullet.gameObject) as GameObject;

        // ポジション情報設定
        newBullet.transform.SetParent(transform);
        newBullet.transform.forward = transform.forward;
        newBullet.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
        newBullet.transform.rotation = bullet.transform.rotation;

        // 発射位置を設定
        Bullet bulletInfo = newBullet.GetComponent<Bullet>();
        bulletInfo.firePosition = transform.position;
        bulletInfo.gun = this;

        // 向きに対して、リコイルを反映し弾側で弾を飛ばす
        newBullet.GetComponent<Rigidbody>().velocity = (transform.forward + RecoilVector()) * fireSpeed;
    }

    /// <summary>
    /// 発射終了
    /// </summary>
    public void ExitFire(GameObject bullet)
    {
        // 使い終わった弾はリサイクルにまわす
        recycleMgr.Recycle(bullet);
    }

    /// <summary>
    /// リコイル取得
    /// </summary>
    /// <returns>リコイル情報</returns>
    private Vector3 RecoilVector()
    {
        // 銃の特性範囲でランダムでリコイルする
        return new Vector3
        (
            Random.RandomRange(recoilSeedMin, recoilSeedMax),
            Random.RandomRange(recoilSeedMin, recoilSeedMax),
            Random.RandomRange(recoilSeedMin, recoilSeedMax)
        ) * recoilValue;
    }
}
    