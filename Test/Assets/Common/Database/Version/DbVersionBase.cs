/// <summary>
/// DBバージョンベース
/// </summary>
abstract public class DbVersionBase
{
    /// <summary>
    /// バージョンアップ実行
    /// </summary>
    abstract protected void ExecVersionUp();

    /// <summary>
    /// バージョンアップ実行
    /// </summary>
    /// <param name="version">バージョン</param>
    public void VersionUp(int version)
    {
        // バージョン単位の更新処理
        ExecVersionUp();

        // バージョン値を上げる ※連続呼び出しも回数が限定されるので、毎度インスタンス生成
        ConfigDao.UpdateDbVersion(version);
    }
}
