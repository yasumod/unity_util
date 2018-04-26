using System;
using System.Collections.Generic;

/// <summary>
/// SQLiteコネクションマネージャー
/// </summary>
public static class SQLiteConnectionManager
{
    /// <summary>
    /// コネクションリスト
    /// </summary>
    private static Dictionary<string, SQLiteConnection> connectionList = new Dictionary<string, SQLiteConnection>();

    /// <summary>
    /// トランザクションリスト
    /// </summary>
    private static Dictionary<string, bool> tranList = new Dictionary<string, bool>();

    /// <summary>
    /// 静的コンストラクタ
    /// </summary>
    static SQLiteConnectionManager()
    {
        // トランザクション & コネクションの閉じ漏れが終了トリガーを仕込んでおく
        GlobalManager.GetManager<EventManager>().AddExitAction(() =>
        {
            AllClose();
        });
    }

    /// <summary>
    /// コネクションオープン
    /// </summary>
    /// <param name="dbName">DB名</param>
    /// <returns>コネクション</returns>
    public static IntPtr ConnectionOpen(string dbName)
    {
        // 初回のみコネクション生成
        if (!connectionList.ContainsKey(dbName))
        {
            connectionList.Add(dbName, new SQLiteConnection(dbName));
            SQLiteConnection.ClearDb(dbName);
        }

        // コネクションを開く
        return connectionList[dbName].Open();
    }

    /// <summary>
    /// コネクションクローズ
    /// </summary>
    /// <param name="dbName">DB名</param>
    public static void ConnectionClose(string dbName)
    {
        // トランザクション中は、閉じない
        if (IsTran(dbName))
        {
            return;
        }

        // コネクションが有効なら閉じる
        if (connectionList.ContainsKey(dbName))
        {
            connectionList[dbName].Close();
        }
    }

    /// <summary>
    /// トランザクション判定
    /// </summary>
    /// <param name="dbName">DB名</param>
    /// <returns>true:トランザクション内/false:トランザクション外</returns>
    public static bool IsTran(SQLiteDao dao)
    {
        return tranList.ContainsKey(dao.GetDbName());
    }

    /// <summary>
    /// トランザクション判定
    /// </summary>
    /// <param name="dbName">DB名</param>
    /// <returns>true:トランザクション内/false:トランザクション外</returns>
    public static bool IsTran(string dbName)
    {
        return tranList.ContainsKey(dbName);
    }

    /// <summary>
    /// トランザクションDAO生成
    /// </summary>
    /// <typeparam name="DAO_TYPE">DAOタイプ</typeparam>
    /// <returns>トランザクションDAO</returns>
    public static DAO_TYPE TransactionDaoFactory<DAO_TYPE>() where DAO_TYPE : SQLiteDao, new()
    {
        DAO_TYPE dao = new DAO_TYPE();
        Begin(dao);
        return dao;
    }

    /// <summary>
    /// トランザクション開始
    /// </summary>
    /// <param name="dao">DAO</param>
    public static void Begin(SQLiteDao dao)
    {
        string dbName = dao.GetDbName();

        //　すでに開始済みなら何もしない
        if (IsTran(dbName))
        {
            return;
        }

        // トランザクションに追加して、BEGIN発行
        tranList.Add(dbName, true);
        dao.Execute("BEGIN");
    }

    /// <summary>
    /// コミット
    /// </summary>
    public static void Commit()
    {
        Exception exp = null;
        try
        {
            TransactionEnd("COMMIT");
        }
        catch (Exception e)
        {
            // エラー発生時はエクセプションを退避して、ロールバック
            exp = e;
            RollBack();
        }

        // エラーがあればスロー
        if (exp != null)
        {
            throw exp;
        }
    }

    /// <summary>
    ///　ロールバック
    /// </summary>
    public static void RollBack()
    {
        // ロールバック実施
        TransactionEnd("ROLLBACK", true);
    }

    /// <summary>
    /// トランザクション終了
    /// </summary>
    /// <param name="query">終了クエリ</param>
    /// <param name="isErrSkip">エラースキップフラグ</param>
    private static void TransactionEnd(string query, bool isErrSkip = false)
    {
        // トランザクション終了
        foreach (string key in tranList.Keys)
        {
            SQLiteDao.Execute(ConnectionOpen(key), query, null, isErrSkip);
        }

        // トランザクションリスト初期化
        tranList.Clear();
    }

    /// <summary>
    /// コネクションオールクローズ
    /// </summary>
    private static void AllClose()
    {
        // ひとまずロールバック
        RollBack();

        // コネクションディスポーズ
        foreach (string closeTarget in connectionList.Keys)
        {
            ConnectionClose(closeTarget);
        }

        // 不要なデータ開放
        connectionList.Clear();
    }
}
