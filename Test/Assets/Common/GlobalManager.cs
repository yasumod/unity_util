using UnityEngine;

/// <summary>
/// シングルマネージャー
/// 特殊なスクリプトをシングルトン管理するためのマネージャー
/// </summary>
public class GlobalManager : MonoBehaviour
{
    /// <summary>
    /// ロックオブジェクト
    /// </summary>
    private static Object initLock = new Object();

    /// <summary>
    /// マネージャーキャッシュ
    /// </summary>
    private static SimpleCache<MonoBehaviour> cache = SimpleCache<MonoBehaviour>.Instance;

    /// <summary>
    /// インスタンスのキャッシュ設定
    /// </summary>
    protected virtual bool IsCacheInstans { get { return false; } }

    /// <summary>
    /// マネージャー取得
    /// </summary>
    /// <typeparam name="T">マネージャータイプ</typeparam>
    /// <returns>マネージャーインスタンス</returns>
    public static T GetManager<T>() where T : MonoBehaviour
    {
        // キャッシュ検索
        if (cache.IsCache<T>())
        {
            // マネージャーの階層を深くしないつくりのため、ダウンキャスト ※ジェネリック解決のためだけの階層は作らない
            return (T)cache.Get<T>();
        }

        // コンポーネント検索用オブジェクト取得 ※マネージャー用ゲームオブジェクトは今のところひとつに絞る想定なので、直接シングルトン化
        GameObject globalGameObject = Singleton<GameObject>.Instance;
        globalGameObject.name = "GlobalManagerObject";

        // マネージャー検索
        T ret = globalGameObject.GetComponent<T>();

        // マネージャーが見つからない場合、新規作成
        if (ret == null)
        {
            ret = GetManager<T>(globalGameObject);
        }

        // キャッシュ対象ならインスタンスをキャッシュ
        if (ret is GlobalManager &&  ((GlobalManager)(object)ret).IsCacheInstans)
        {
            cache.Add(ret);
        }

        return ret;
    }

    /// <summary>
    /// マネージャー初期化(スレッドセーフ)
    /// </summary>
    private static T GetManager<T>(GameObject globalGameObject) where T : MonoBehaviour
    { 
        T ret;

        // スレッドロック
        lock (initLock)
        {
            // 行き違いで誰かがつくってるなら何もしない
            if ((ret = globalGameObject.GetComponent<T>()) != null)
            {
                return ret;
            }

            // コントロールオブジェクト作成
            ret = globalGameObject.AddComponent<T>() as T;

            return ret;
        }
    }
}
