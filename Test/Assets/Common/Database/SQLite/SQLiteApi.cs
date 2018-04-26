using System;
using System.Runtime.InteropServices;

/// <summary>
/// SQLiteAPI ※DLLとのリンクファイル
/// </summary>
public class SQLiteApi
{
    /// <summary>
    /// SQLステータス : OK
    /// </summary>
    public const int SQLITE_OK = 0;

    /// <summary>
    /// SQLステータス : 行
    /// </summary>
    public const int SQLITE_ROW = 100;

    /// <summary>
    /// SQLステータス : 終了
    /// </summary>
    public const int SQLITE_DONE = 101;

    /// <summary>
    /// SQL : int
    /// </summary>
    public const int SQLITE_INTEGER = 1;

    /// <summary>
    /// SQL : float
    /// </summary>
    public const int SQLITE_FLOAT = 2;

    /// <summary>
    /// SQL : text
    /// </summary>
    public const int SQLITE_TEXT = 3;

    /// <summary>
    /// SQL : blob
    /// </summary>
    public const int SQLITE_BLOB = 4;

    /// <summary>
    /// SQL : null
    /// </summary>
    public const int SQLITE_NULL = 5;

    public const int SQLITE_OPEN_TRANSIENT_DB = 400;

    /// <summary>
    /// コネクションオープン
    /// </summary>
    /// <param name="filename">ファイル名</param>
    /// <param name="db">コネクションポインター</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2")]
    public static extern int sqlite3_open_v2(string filename, out IntPtr db,int opt, string vfs);

    /// <summary>
    /// タイムアウト設定
    /// </summary>
    /// <param name="db">コネクションポインター</param>
    /// <param name="ms">タイムアウトミリセック</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout")]
    public static extern int sqlite3_busy_timeout(IntPtr db, int ms);

    /// <summary>
    /// コネクションオープン
    /// </summary>
    /// <param name="filename">ファイル名</param>
    /// <param name="db">コネクションポインター</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_open")]
    public static extern int sqlite3_open(string filename, out IntPtr db);

    /// <summary>
    /// コネクションクローズ
    /// </summary>
    /// <param name="db">コネクションポインター</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_close")]
    public static extern int sqlite3_close(IntPtr db);

    /// <summary>
    /// コネクションクローズ
    /// </summary>
    /// <param name="db">コネクションポインター</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_close_v2")]
    public static extern int sqlite3_close_v2(IntPtr db);

    /// <summary>
    /// プリペア
    /// </summary>
    /// <param name="db">コネクションポインター</param>
    /// <param name="zSql">クエリ</param>
    /// <param name="nByte">クエリサイズ</param>
    /// <param name="stmt">ステートメント</param>
    /// <param name="pzTail">クエリ中断ポインター</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2")]
    public static extern int sqlite3_prepare_v2(IntPtr db, string zSql, int nByte, out IntPtr stmt, IntPtr pzTail);

    /// <summary>
    /// ステップ実行
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_step")]
    public static extern int sqlite3_step(IntPtr stmt);

    /// <summary>
    /// SQL直実行
    /// </summary>
    /// <param name="db">コネクションポインター</param>
    /// <param name="zSql">クエリ</param>
    /// <param name="callback">コールバック</param>
    /// <param name="args">バインド情報</param>
    /// <param name="errorMessage">エラーメッセージポインタ</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_exec")]
    public static extern int sqlite3_exec(IntPtr db, string sql, IntPtr callback, IntPtr args, out IntPtr errorMessage);

    /// <summary>
    /// ステートメント破棄
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_finalize")]
    public static extern int sqlite3_finalize(IntPtr stmt);

    /// <summary>
    /// カラム数取得
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <returns>カラム数</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_count")]
    public static extern int sqlite3_column_count(IntPtr stmt);

    /// <summary>
    /// カラム名取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>絡む姪ポインタ</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_name")]
    public static extern IntPtr sqlite3_column_name(IntPtr stmt, int iCol);

    /// <summary>
    /// カラムタイプ取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>カラムタイプ</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_type")]
    public static extern int sqlite3_column_type(IntPtr stmt, int iCol);

    /// <summary>
    /// 設定値取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>設定値</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64")]
    public static extern long sqlite3_column_int64(IntPtr stmt, int iCol);

    /// <summary>
    /// 設定値取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>設定値</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_text")]
    public static extern IntPtr sqlite3_column_text(IntPtr stmt, int iCol);

    /// <summary>
    /// 設定値取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>設定値</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_double")]
    public static extern double sqlite3_column_double(IntPtr stmt, int iCol);

    /// <summary>
    /// 設定値取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>設定値</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob")]
    public static extern IntPtr sqlite3_column_blob(IntPtr stmt, int iCol);

    /// <summary>
    /// 設定値取得
    /// </summary>
    /// <param name="stmt">ステートメントポインタ</param>
    /// <param name="iCol">カラムインデックス</param>
    /// <returns>設定値</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes")]
    public static extern int sqlite3_column_bytes(IntPtr stmt, int iCol);

    /// <summary>
    /// ステートメント初期化
    /// </summary>
    /// <param name="stmHandle">ステートメントポインタ</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_reset(IntPtr stmt);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <param name="val">blobポインタ</param>
    /// <param name="n">blobサイズ</param>
    /// <param name="free">開放コールバック</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_blob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <param name="val">バインドバリュー</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_double(IntPtr stmt, int index, double val);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <param name="val">バインドバリュー</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_int(IntPtr stmt, int index, int val);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <param name="val">バインドバリュー</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_int64(IntPtr stmt, int index, long val);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_null(IntPtr stmt, int index);

    /// <summary>
    /// バインド
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="index">バインドインデックス</param>
    /// <param name="val">バインドバリュー</param>
    /// <param name="n">テキストサイズ</param>
    /// <param name="free">コールバック</param>
    /// <returns>処理結果</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl,CharSet = CharSet.Unicode)]
    public static extern int sqlite3_bind_text16(IntPtr stmt,int index,[MarshalAs(UnmanagedType.LPWStr)] string val,int n,IntPtr free);

    /// <summary>
    /// バインドインデックス取得
    /// </summary>
    /// <param name="stmt">ステートメント</param>
    /// <param name="name">ホスト変数名</param>
    /// <returns>バインドインデックス</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_bind_parameter_index(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

    /// <summary>
    /// エラーコード取得
    /// </summary>
    /// <param name="db">コネクションポインタ</param>
    /// <returns>エラーコード</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_errcode", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sqlite3_errcode(IntPtr db);

    /// <summary>
    /// エラーメッセージ取得
    /// </summary>
    /// <param name="db">コネクションポインタ</param>
    /// <returns>エラーメッセージ</returns>
    [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg")]
    public static extern IntPtr sqlite3_errmsg(IntPtr db);
}
