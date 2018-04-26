using UnityEngine;

/// <summary>
/// 銃弾
/// </summary>
public class Bullet : MonoBehaviour, RecycleImpl
{

    /// <summary>
    /// リサイクルIDプロパティ
    /// </summary>
    public string RecycleId
    {
        get
        {
            return "bullet";
        }
    }

    /// <summary>
    /// 発射銃
    /// </summary>
    public Gun gun;

    /// <summary>
    /// 発射位置
    /// </summary>
    public Vector3 firePosition;
	
    /// <summary>
    /// Update
    /// </summary>
	void Update ()
    {

        // 弾が生きている間は何もしない
		if(transform.position.y > 0)
        {
            return;
        }

        // 弾が死んだ場合、終了関数呼び出し
        gun.ExitFire(gameObject);
    }
}
