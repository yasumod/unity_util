using System.Collections.Generic;

/// <summary>
/// キャッシュ
/// ※念のため横断的なシングルトンで実装(不要なら継承削除)
/// </summary>
/// <typeparam name="CACHE_TYPE">キャッシュ対象</typeparam>
public class SimpleCache<CACHE_TYPE> : Singleton<SimpleCache<CACHE_TYPE>>,System.IDisposable
{
    /// <summary>
    /// インスタンスキャッシュテーブル
    /// </summary>
    private Dictionary<string, CACHE_TYPE> cacheTbl = new Dictionary<string, CACHE_TYPE>();

    /// <summary>
    /// キャッシュプライオリティ
    /// </summary>
    public List<string> cachePriority = new List<string>();

    /// <summary>
    /// キャッシュリミット
    /// </summary>
    private int cacheLimit = -1;

    /// <summary>
    /// ディスポーズデリゲート
    /// </summary>
    private System.Action<CACHE_TYPE> disposeDelegate = null;

    /// <summary>
    /// ディスポーズデリゲート
    /// </summary>
    public System.Action<CACHE_TYPE> DisposeDelegate
    {
        set { disposeDelegate = value; }
    }

    /// <summary>
    /// キャッシュリミットプロパティ
    /// </summary>
    public int CacheLimit
    {
        set { cacheLimit = value; }
    }

    /// <summary>
    /// キャッシュ存在チェック
    /// </summary>
    /// <typeparam name="T">キャッシュ対象</typeparam>
    /// <returns>true:キャッシュ登録あり/false:キャッシュ登録なし</returns>
    public bool IsCache<T>()
    {
        return IsCache(typeof(T).ToString());
    }

    /// <summary>
    /// キャッシュ存在チェック
    /// </summary>
    /// <param name="cacheKey">キャッシュキー</param>
    /// <returns>true:キャッシュ登録あり/false:キャッシュ登録なし</returns>
    public bool IsCache(string cacheKey)
    {
        return cacheTbl.ContainsKey(cacheKey);
    }

    /// <summary>
    /// キャッシュ取得
    /// </summary>
    /// <typeparam name="T">キャッシュ対象</typeparam>
    /// <returns>キャッシュ</returns>
    public CACHE_TYPE Get<T>()
    {
        return Get(typeof(T).ToString());
    }

    /// <summary>
    /// キャッシュ取得
    /// </summary>
    /// <param name="cacheKey">キャッシュキー</param>
    /// <returns>キャッシュ</returns>
    public CACHE_TYPE Get(string cacheKey)
    {
        try
        {
            return (CACHE_TYPE)cacheTbl[cacheKey];
        }
        finally
        {
            // プライオリティ入れ替え
            cachePriority.Remove(cacheKey);
            cachePriority.Add(cacheKey);
        }
    }

    /// <summary>
    /// キャッシュ追加
    /// </summary>
    /// <typeparam name="T">キャッシュ対象</typeparam>
    /// <param name="value">追加対象</param>
    public void Add<T>(T value) where T : CACHE_TYPE
    {
        Add(typeof(T).ToString(), value);
    }

    /// <summary>
    /// キャッシュ追加
    /// </summary>
    /// <param name="cacheKey">キャッシュキー</param>
    /// <param name="value">追加対象</param>
    public void Add(string cacheKey, CACHE_TYPE value)
    {
        // すでにデータが入ってる場合いったんクリア
        Del(cacheKey);

        // 追加
        cachePriority.Add(cacheKey);
        cacheTbl.Add(cacheKey, value);

        // 古いキャッシュの削除

        // 削除件数算出
        int delCount = cacheTbl.Count - cacheLimit;

        // リミット未指定 or リミット範囲内なら何もしない
        if (cacheLimit < 1 || delCount < 1)
        {
            return;
        }

        // キャッシュ削除
        for(;delCount > 0;delCount --)
        {
            // 削除実行
            Del(cachePriority[delCount-1]);
        }
    }

    /// <summary>
    /// キャッシュ削除
    /// </summary>
    /// <typeparam name="T">キャッシュ対象</typeparam>
    public void Del<T>() where T : CACHE_TYPE
    {
        Del(typeof(T).ToString());
    }

    /// <summary>
    /// キャッシュ削除
    /// </summary>
    /// <param name="cacheKey">キャッシュキー</param>
    public void Del(string cacheKey)
    {
        cachePriority.Remove(cacheKey);

        // 空うちならここまで
        if (!cacheTbl.ContainsKey(cacheKey))
        {
            return;
        }

        // デリゲート設定済みなら呼び出し
        if (disposeDelegate != null)
        {
            disposeDelegate(cacheTbl[cacheKey]);
        }

        // キャッシュ削除
        cacheTbl.Remove(cacheKey);
    }

    /// <summary>
    /// キャッシュ全削除
    /// </summary>
    public void Del()
    {
        foreach(string key in cacheTbl.Keys)
        {
            Del(key);
        }
    }

    /// <summary>
    /// インスタンス破棄
    /// </summary>
    public void Dispose()
    {
        Del();
    }
}
