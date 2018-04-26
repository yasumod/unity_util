/// <summary>
/// SQLiteエクセプション
/// </summary>
public class SQLiteException : System.Exception
{
    /// <summary>
    /// 追加メッセージ
    /// </summary>
    private string addMessage = "";

    /// <summary>
    /// メッセージプロパティ
    /// </summary>
    public override string Message
    {
        get
        {
            // ベースのメッセージに付加情報を設定して返却
            return base.Message + addMessage;
        }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="db">DBコネクション</param>
    public SQLiteException(string message, System.IntPtr? db = null) : base(message)
    {
        // コネクション未設定なら何もしない
        if (db == null)
        {
            return;
        }

        // コネクションが有効なら付加情報を追加する
        System.IntPtr dbCon = (System.IntPtr)db;
        addMessage = string.Format("# errcode = {0} errmsg = {1}",SQLiteApi.sqlite3_errcode(dbCon), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(SQLiteApi.sqlite3_errmsg(dbCon)));
    }
}
