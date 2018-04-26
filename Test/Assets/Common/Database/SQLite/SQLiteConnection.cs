using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SQLiteコネクション 
/// </summary>
public class SQLiteConnection : IDisposable
{
    /// <summary>
    /// タイムアウト時間 ※ディスク書き込み時間分ロックがかかるので、ありえない時間を設定
    /// </summary>
    private const int TIMEOUT = 10000;

    /// <summary>
    /// コネクション
    /// </summary>
    private IntPtr? connection = null;

    /// <summary>
    /// DBファイルパス
    /// </summary>
    private string path;

    /// <summary>
    /// DBクリア
    /// </summary>
    /// <param name="name">DB名</param>
    public static void ClearDb(string name)
    {
        // クリアDB
        if (!DebugManager.IsClearLocalDB)
        {
            return;
        }

        // すべてのDAOでループ
        foreach (DaoImpl dao in new List<DaoImpl>()
        {
            new AlreadyDataDao(),
            new FavoriteGroupDao(),
            new GroupDataDao(),
            new GroupUserDao(),
            new LikeDataDao(),
            new OfflineDataDao(),
            new TimelineDataDao(),
            new TimelineInfoDao(),
            new UserInfoDao(),
        })
        {
            // 該当のDBの場合、データクリア
            if (name.Equals(dao.GetDbName()))
            {
                dao.DeleteAll();
            }
        }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="name">DB名/param>
    public SQLiteConnection(string name)
    {
        // DBファイル名に変換
        name = name + ".db";

        // DBファイルパス生成
        path = System.IO.Path.Combine(Application.persistentDataPath, name);

        // DBファイルが見配置ならストリーミングアセッツからコピー(新規の場合のみ)
        if (!System.IO.File.Exists(path) || DebugManager.IsResetLocalDB)
        {
            System.IO.File.Copy(System.IO.Path.Combine(Application.streamingAssetsPath, name), path, true);
        }
    }

    /// <summary>
    /// オープンチェック
    /// </summary>
    /// <returns>true:オープン/false:クローズ</returns>
    public bool IsOpen()
    {
        return this.connection != null;
    }

    /// <summary>
    /// コネクションオープン
    /// </summary>
    /// <returns>コネクション</returns>
    public IntPtr Open()
    {
        // すでに開いている場合、使いまわす
        if (IsOpen())
        {
            return (IntPtr)connection;
        }

        // 新規オープン
        IntPtr con;
        if (SQLiteApi.sqlite3_open(path, out con) != SQLiteApi.SQLITE_OK)
        //if (SQLiteApi.sqlite3_open_v2(path, out con, 0x00000002 | 0x00020000, null) != SQLiteApi.SQLITE_OK)
            {
            throw new SQLiteException("Could not open database file: " + path);
        }

        // タイムアウト設定
        if (SQLiteApi.sqlite3_busy_timeout(con, TIMEOUT) != SQLiteApi.SQLITE_OK)
        {
            throw new SQLiteException("Could not Set timeout: " + path);
        }

        connection = con;
        return (IntPtr)connection;
    }

    /// <summary>
    /// クローズ
    /// </summary>
    public void Close()
    {
        // すでに閉じている場合何もしない
        if (!IsOpen())
        {
            return;
        }

        IntPtr con = (IntPtr)connection;
        connection = null;

        // クローズエラーは何もしない
        SQLiteApi.sqlite3_close(con);
    }

    /// <summary>
    /// ディスポーズ ※クローズ漏れのセーフティー
    /// </summary>
    public void Dispose()
    {
        Close();
    }
}
