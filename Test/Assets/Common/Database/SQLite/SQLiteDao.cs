using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using melmo;

/// <summary>
/// SQLiteDAO
/// </summary>
public abstract class SQLiteDao 
{

    /// <summary>
    /// 無効なポインター
    /// </summary>
    private static readonly IntPtr disablePtr = new IntPtr(-1);

    /// <summary>
    /// クエリキャッシュ ※アプリ終了まで持ち続ける
    /// </summary>
    private static readonly SimpleCache<string> queryCache = new SimpleCache<string>()
    {
        CacheLimit = AppSetting.QUERY_ONM_CACHE_COUNT
    };

    /// <summary>
    /// DB名プロパティ
    /// </summary>
    /// <returns>DB名</returns>
    protected abstract string DbName();

    /// <summary>
    /// DAOファクトリ
    /// </summary>
    /// <returns>DAO</returns>
    public static DAO_TYPE DaoFactory<DAO_TYPE>() where DAO_TYPE : SQLiteDao, new()
    {
        return new DAO_TYPE();
    }

    /// <summary>
    /// Datetime文字列取得
    /// </summary>
    /// <returns>DateTime文字列</returns>
    public static string GetDatetimeStr(DateTime? target = null)
    {
        return DispUtil.GetDatetimeStr(target);
    }

    /// <summary>
    /// SQLログ出力
    /// </summary>
    /// <param name="msg">メッセージ</param>
    private static void SQLLog(string msg)
    {
        if (DebugManager.IsSqlLog)
        {
            UnityEngine.Debug.Log("[SQL] " + msg);
        }
    }

    /// <summary>
    /// DB名取得
    /// </summary>
    /// <returns>DB名</returns>
    public string GetDbName()
    {
        return DbName();
    }

    /// <summary>
    /// クエリキャッシュクリア
    /// </summary>
    private static void ClearQueryCache()
    {
        queryCache.Del();
    }

    /// <summary>
    /// クエリキャッシュ追加
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="query">クエリ</param>
    protected void AddQueryCache(string key, string query)
    {
        queryCache.Add(CreateQueryCacheKey(key));
    }

    /// <summary>
    /// クエリキャッシュ取得
    /// </summary>
    /// <param name="key"></param>
    /// <returns>クエリ</returns>
    protected string GetQueryCache(string key)
    {
        string cacheKey = CreateQueryCacheKey(key);

        // キャッシュされてなければnull返却
        if (!queryCache.IsCache(cacheKey))
        {
            return null;
        }
        
        return queryCache.Get(cacheKey);
    }

    /// <summary>
    /// キャッシュキー作成
    /// </summary>
    /// <param name="seed">シード</param>
    /// <returns>キャッシュキー</returns>
    private string CreateQueryCacheKey(string seed)
    {
        // 継承先クラス名 + シード
        return String.Format("{0}:::{1}", GetType().ToString(), seed);
    }

    /// <summary>
    /// コネクションオープン
    /// </summary>
    /// <returns>コネクション</returns>
    private IntPtr Open()
    {
        return SQLiteConnectionManager.ConnectionOpen(DbName());
    }

    /// <summary>
    /// コネクションクローズ
    /// </summary>
    private void Close()
    {
        SQLiteConnectionManager.ConnectionClose(DbName());
    }

    /// <summary>
    /// クエリ遅延実行
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <param name="isSkipExp">エラースキップフラグ</param>
    public void Execute(string query, Dictionary<string, object> bindList = null, bool isSkipExp = false)
    {
        // コネクションオープン
        IntPtr con = System.IntPtr.Zero;

        try
        {
            con = Open();
            Execute(con, query, bindList);
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// クエリ遅延実行
    /// </summary>
    /// <param name="con">コネクション</param>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <param name="isSkipExp">エラースキップフラグ</param>
    public static void Execute(IntPtr con, string query, Dictionary<string, object> bindList = null, bool isSkipExp = false)
    {
        // ステートメントオープン
        IntPtr stmt = OpenStmt(con, query);

        // バインド
        BindParam(stmt, bindList);

        try
        {
            // 実行
            if (SQLiteApi.sqlite3_step(stmt) != SQLiteApi.SQLITE_DONE)
            {
                if (!isSkipExp)
                {
                    throw new SQLiteException("Could not execute SQL statement." + query, con);
                }
            }
        }
        finally
        {
            CloseStmt(stmt);
        }
    }

    /// <summary>
    /// クエリ逐次実行
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="isSkipExp">エラースキップフラグ</param>
    public void Exec(string query,bool isSkipExp = false)
    {
        IntPtr con = System.IntPtr.Zero;

        try
        {
            con = Open();
            Exec(con, query, isSkipExp);
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// クエリ逐次実行
    /// </summary>
    /// <param name="con">コネクション</param>
    /// <param name="query">クエリ</param>
    /// <param name="isSkipExp">エラースキップフラグ</param>
    public static void Exec(IntPtr con, string query,bool isSkipExp = false)
    {
        IntPtr stmt;

        if (SQLiteApi.sqlite3_exec(con,query,IntPtr.Zero, IntPtr.Zero,out stmt) != SQLiteApi.SQLITE_OK)
        {
            if (!isSkipExp)
            {
                throw new SQLiteException("Could not exec SQL statement.",con);
            }
        }
    }

    /// <summary>
    /// 1行フェッチ
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <returns>行データ</returns>
    public Dictionary<string, object> ExecuteFetchOne(string query, Dictionary<string, object> bindList = null)
    {
        Dictionary<string, object> ret = null;

        // ステップ実行で全データを取得
        foreach (Dictionary<string, object> row in ExecuteStep(query, bindList))
        {
            ret = row;
        }

        // 最終行を返却
        return ret;
    }

    /// <summary>
    /// フェッチ実行
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <returns>行データ</returns>
    public IEnumerable<Dictionary<string, object>> ExecuteFetch(string query, Dictionary<string, object> bindList = null)
    {
        int offset = 0;
        string offsetKey = ":__OFFSET";

        // オフセット取得クエリを追加
        query = String.Format("SELECT * FROM ({0}) LIMIT 1 OFFSET {1}", query, offsetKey);
        
        // バインドリストが未設定なら作成
        if (bindList == null)
        {
            bindList = new Dictionary<string, object>();
        }

        // オフセット用バインドリストを設定
        bindList.Add(offsetKey, 0);

        // オフセットをずらしながらステップ実行
        foreach (Dictionary<string, object> row in ExecuteStep(query, bindList, (stmt, bind) => { bind[offsetKey] = ++offset;BindParam(stmt, bind); }))
        {
            yield return row;
        }
    }

    /// <summary>
    /// ステップ実行
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <param name="fetchPostAction">フェッチ後処理</param>
    /// <returns>行データ</returns>
    private IEnumerable<Dictionary<string, object>> ExecuteStep(string query, Dictionary<string, object> bindList = null, Action<IntPtr, Dictionary<string, object>> fetchPostAction = null )
    {
        // ポストアクション未指定なら、空アクションを設定
        if (fetchPostAction == null)
        {
            fetchPostAction = (ptr, bind) => { };
        }

        // コネクションオープン
        IntPtr con = System.IntPtr.Zero;

        try
        {
            con = Open();

            // ステートメントオープン
            IntPtr stmt = OpenStmt(con, query);

            // パラメーターバインド
            BindParam(stmt, bindList);

            try
            {
                // カラムインデックス作成
                int columnCount = SQLiteApi.sqlite3_column_count(stmt);
                Dictionary<int, string> clmIndex = new Dictionary<int, string>();
                for (int i = 0; i < columnCount; i++)
                {
                    clmIndex.Add(i, Marshal.PtrToStringAnsi(SQLiteApi.sqlite3_column_name(stmt, i)));
                }

                // 1行ずつデータを取得
                Dictionary<string, object> dataRow = new Dictionary<string, object>();
                while (SQLiteApi.sqlite3_step(stmt) == SQLiteApi.SQLITE_ROW)
                {
                    // すべてのカラムのデータを取得し、カラム名をキーにした連想配列に設定
                    for (int i = 0; i < columnCount; i++)
                    {
                        dataRow[clmIndex[i]] = GetValue(stmt, i);
                    }

                    // データ返却
                    yield return dataRow;

                    // ポストアクション
                    fetchPostAction(stmt, bindList);
                }
            }
            finally
            {
                CloseStmt(stmt);
            }
        }
        finally
        {
            Close();
        }
    }

    /// <summary>
    /// バリュー取得
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="clmIndex">カラムインデックス</param>
    /// <returns>バリュー</returns>
    private static object GetValue(IntPtr stmt, int clmIndex)
    {
        // タイプごとに指定されたカラムのデータを抜き出す
        switch (SQLiteApi.sqlite3_column_type(stmt, clmIndex))
        {
            case SQLiteApi.SQLITE_INTEGER:
                return SQLiteApi.sqlite3_column_int64(stmt, clmIndex);

            case SQLiteApi.SQLITE_TEXT:
                IntPtr text = SQLiteApi.sqlite3_column_text(stmt, clmIndex);
                return Marshal.PtrToStringAnsi(text);

            case SQLiteApi.SQLITE_FLOAT:
                return SQLiteApi.sqlite3_column_double(stmt, clmIndex);

            case SQLiteApi.SQLITE_BLOB:
                IntPtr blob = SQLiteApi.sqlite3_column_blob(stmt, clmIndex);
                int size = SQLiteApi.sqlite3_column_bytes(stmt, clmIndex);
                byte[] data = new byte[size];
                Marshal.Copy(blob, data, 0, size);
                return data;
        }

        return null;
    }

    /// <summary>
    /// オープンステートメント
    /// </summary>
    /// <param name="con">DBコネクション</param>
    /// <param name="query">クエリ</param>
    /// <returns>ステートメント</returns>
    private static IntPtr OpenStmt(IntPtr con, string query)
    {
        IntPtr stmtHandle;

        SQLLog(query);

        if (SQLiteApi.sqlite3_prepare_v2(con, query, query.Length, out stmtHandle, IntPtr.Zero) != SQLiteApi.SQLITE_OK)
        {
            throw new SQLiteException("Prepare Error " + query,con);
        }

        return stmtHandle;
    }

    /// <summary>
    /// クローズステートメント
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    private static void CloseStmt(IntPtr? stmt)
    {
        // 無効なステートメントは何もしない
        if (stmt == null)
        {
            return;
        }

        // ステートメントクローズのエラーは無視
        SQLiteApi.sqlite3_finalize((IntPtr)stmt);
    }

    /// <summary>
    /// リセットステートメント
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    private static void ResetStmt(IntPtr? stmt)
    {

        // 無効なステートメントは何もしない
        if (stmt == null)
        {
            return;
        }

        // ステートメントリセット
        if (SQLiteApi.sqlite3_reset((IntPtr)stmt) != SQLiteApi.SQLITE_OK)
        {
            throw new SQLiteException("Stmt Reset Error");
        }
    }

    /// <summary>
    /// パラメーターバインド
    /// </summary>
    /// <param name="stmHandle">ステートメント</param>
    /// <param name="bindList">バインドリスト</param>
    private static void BindParam(IntPtr stmHandle, Dictionary<string, object> bindList)
    {
        int paramNo = 0;
        List<string> bindLog = new List<string>();

        // リセット
        ResetStmt(stmHandle);

        // バインド対象がない場合何もしない
        if (bindList == null)
        {
            return;
        }

        // バインド対象を全件ループ
        foreach (KeyValuePair<string, object> bindData in bindList)
        {
        	// パラメーターNoを算出
            paramNo = SQLiteApi.sqlite3_bind_parameter_index(stmHandle, bindData.Key);

            if (bindData.Value != null)
            {
                bindLog.Add(bindData.Key + " value= " + bindData.Value.ToString());
            }

            // string
            if (bindData.Value is string)
            {
                string strValue = bindData.Value.ToString();
                ExecBind(()=>SQLiteApi.sqlite3_bind_text16(stmHandle, paramNo, strValue, System.Text.Encoding.Unicode.GetByteCount(strValue), disablePtr));
                continue;
            }

            // int
            if (bindData.Value is int)
            {
                ExecBind(() => SQLiteApi.sqlite3_bind_int(stmHandle, paramNo, ((int)bindData.Value)));
                continue;
            }

            // long
            if (bindData.Value is long)
            {
                ExecBind(() => SQLiteApi.sqlite3_bind_int64(stmHandle, paramNo, ((long)bindData.Value)));
                continue;
            }

            // double
            if (bindData.Value is double)
            {
                ExecBind(() => SQLiteApi.sqlite3_bind_double(stmHandle, paramNo, ((double)bindData.Value)));
                continue;
            }

            // blob
            if (bindData.Value is byte[])
            {
                byte[] byteValue = (byte[])bindData.Value;
                ExecBind(() => SQLiteApi.sqlite3_bind_blob(stmHandle, paramNo, byteValue, byteValue.Length, disablePtr));
                continue;
            }
        }

        SQLLog("BIND : " + string.Join(" # ", CollectionUtil.Collection2Array<string>(bindLog)));
    }

    /// <summary>
    /// バインド実行
    /// </summary>
    /// <param name="bindAction">バインドアクション</param>
    private static void ExecBind(Func<int> bindAction)
    {
        // 後々エラー処理が入れれるよう枠だけ用意
        bindAction();
    }
}
