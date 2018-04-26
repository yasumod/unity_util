/// <summary>
/// シングルトン
/// </summary>
public class Singleton<T> where T : new()
{
    /// <summary>
    /// シングルトンオブジェクト
    /// </summary>
    private static T instance;

    /// <summary>
    /// 静的コンストラクタ
    /// </summary>
    static Singleton()
    {
        instance = new T();
    }

    /// <summary>
    /// インスタンス取得
    /// </summary>
    /// <returns>インスタンス</returns>
    public static T Instance
    {
        get { return instance; }
    }
}
