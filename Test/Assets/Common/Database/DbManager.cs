using System;

/// <summary>
/// DBマネージャー<br>
/// ・必要に応じてDAO本体のインスタンスキャッシュ(ジェネリクスは使わずインスタンス毎)
/// ・DBバージョンアップ
/// </summary>
public class DbManager
{
    /// <summary>
    /// DBバージョン 現在のＤＢバージョンを格納DBを変更する場合ここのバージョンをあげてください
    /// </summary>
    private const int DB_VERSION = 1;

    /// <summary>
    /// DBバージョンアップ
    /// </summary>
    public static void UpdateVersion()
    {
        int curVersion = ConfigDao.SelectDbVersion();

        // 1バージョンずつアップしていく
        for (++curVersion; curVersion <= DB_VERSION; curVersion++)
        {
            // バージョンアップ実施 ※呼び出し回数が少ないので、実装コストを取ってジェネリクス利用
            DbVersionBase verInst = (DbVersionBase)Activator.CreateInstance(Type.GetType(String.Format("DbVersion{0}",curVersion)));
            verInst.VersionUp(curVersion);
        }
    }
}
