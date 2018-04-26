
using System.Collections.Generic;

/// <summary>
/// DTOクラスベース
/// </summary>
/// <typeparam name="DTOTYPE">DTOクラスジェネリクス</typeparam>
abstract public class DtoBase<DTOTYPE> : DtoImpl where DTOTYPE : DtoBase<DTOTYPE>,new()
{
    /// <summary>
    /// 行データ
    /// </summary>
    protected Dictionary<string, object> rowData;

    /// <summary>
    /// デフォルトローデータ取得
    /// </summary>
    /// <returns>デフォルトローデータ</returns>
    protected abstract Dictionary<string, object> GetDefaultRowData();

    /// <summary>
    /// PKリスト取得
    /// </summary>
    /// <returns>PKリスト</returns>
    protected abstract List<string> GetPKList();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DtoBase()
    {
        // 空の行データ作成 ※カラムがここで確定
        this.rowData = new Dictionary<string, object>(this.GetDefaultRowData());
    }

    /// <summary>
    /// シャローコピー (meta定義メンバーのみコピーします)
    /// </summary>
    /// <returns>コピー結果</returns>
    public DTOTYPE ShallowCopy()
    {
        DTOTYPE instance = new DTOTYPE();
        instance.SetDictionary(GetRowData());
        return instance;
    }

    /// <summary>
    /// ローデータ取得
    /// </summary>
    /// <returns>ローデータ</returns>
    private Dictionary<string, object> GetRowData()
    {
        return this.rowData;
    }

    /// <summary>
    /// PKグレップ
    /// 引数辞書から、PKのデータだけ抜き出します。
    /// </summary>
    /// <typeparam name="VALUE_TYPE">バリュータイプ</typeparam>
    /// <param name="rowData">ローデータ(PK情報は削除)</param>
    /// <returns>グレップ結果</returns>
    public Dictionary<string, VALUE_TYPE> GrepPk<VALUE_TYPE>(ref Dictionary<string, VALUE_TYPE> rowData)
    {
        Dictionary<string, VALUE_TYPE> pkRow = new Dictionary<string, VALUE_TYPE>();

        foreach (string pkClm in this.GetPKList())
        {
            // PK情報を抜き出し
            pkRow.Add(pkClm, rowData[pkClm]);

            // PK情報削除
            rowData.Remove(pkClm);
        }

        return pkRow;
    }

    /// <summary>
    /// Dictionay設定
    /// </summary>
    /// <typeparam name="DTO">DTOタイプ</typeparam>
    /// <param name="dto">行データ</param>
    public void SetDictionary<DTO>(DtoBase<DTO> dto) where DTO : DtoBase<DTO>, new()
    {
        // 設定データがなければnullを返却
        if (dto == null)
        {
            return;
        }

        SetDictionary(dto.rowData);
    }

    /// <summary>
    /// Dictionay設定
    /// </summary>
    /// <param name="dicData">行データ</param>
    public void SetDictionary(Dictionary<string, object> dicData)
    {
        // 設定データがなければnullを返却
        if (dicData == null)
        {
            return;
        }

        foreach (KeyValuePair<string, object> clmData in dicData)
        {
            this.SetValue<object>(clmData.Key,clmData.Value);
        }
    }

    /// <summary>
    /// バインド情報取得
    /// </summary>
    /// <returns>バインド情報(バインドカラム/ホスト変数名)</returns>
    public Dictionary<string, string> GetBindInfo()
    {
        Dictionary<string, string> bindInfo = new Dictionary<string, string>();

        // すべてのキーをエンコードにかける
        foreach (KeyValuePair<string, object> clmData in this.GetDefaultRowData())
        {
            bindInfo.Add(clmData.Key, ":" + clmData.Key);
        }

        return bindInfo;
    }

    /// <summary>
    /// バインドリスト取得
    /// </summary>
    /// <param name="prefix">プレフィックス</param>
    /// <param name="bindList">バインドリスト</param>
    /// <returns>バインド情報(ホスト変数名/ホスト変数設定値)</returns>
    public Dictionary<string, object> GetBindList(string prefix = "", Dictionary<string, object> bindList = null)
    {
        if (bindList == null)
        {
            bindList = new Dictionary<string, object>();
        }

        foreach (KeyValuePair<string, object> clmData in this.GetDefaultRowData())
        {
            bindList.Add(string.Format(":{0}{1}",prefix,clmData.Key), GetValue<object>(clmData.Key));
        }

        return bindList;
    }

    /// <summary>
    /// バリュー取得
    /// </summary>
    /// <typeparam name="VALUE_TYPE">バリュータイプ</typeparam>
    /// <param name="key">キー</param>
    /// <returns>バリュー</returns>
    protected VALUE_TYPE GetValue<VALUE_TYPE>(string key)
    {
        // キーチェック
        if (!this.CheckKey(key))
        {
            return default(VALUE_TYPE);
        }

        // ★数値系コンバートロジック ※後付で対応したので、初回ゲット時にコンバート★
        //  SQLITE 可変長int (データに応じて長さが変わる)
        //  SQLITEからの取得はすべてlong(int64(
        //  取得した後Dtoの定義(Getterの型指定)に応じてコンバート処理

        // Int
        if (typeof(VALUE_TYPE) == typeof(int) && !(rowData[key] is int))
        {
            rowData[key] = AppUtil.Obj2Int(rowData[key]);
        }

        // Long
        if (typeof(VALUE_TYPE) == typeof(long) && !(rowData[key] is long))
        {
            rowData[key] = AppUtil.Obj2Long(rowData[key]);
        }

        // 指定カラムのデータを返却
        return (VALUE_TYPE)rowData[key];
    }

    /// <summary>
    /// バリュー設定
    /// </summary>
    /// <typeparam name="VALUE_TYPE">バリュータイプ</typeparam>
    /// <param name="key">キー</param>
    /// <param name="value">バリュー</param>
    protected void SetValue<VALUE_TYPE>(string key,VALUE_TYPE value)
    {
        // 指定カラムのデータを設定
        this.rowData[key] = value;
    }

    /// <summary>
    /// キーチェック
    /// </summary>
    /// <param name="key">キー</param>
    private bool CheckKey(string key)
    {
        // 無効なキー指定はエラー
        return this.rowData.ContainsKey(key);
    }

    /// <summary>
    /// シリアライズ前処理
    /// </summary>
    protected virtual void OnCustomBeforeSerialize()
    {
    }

    /// <summary>
    /// デシリアライズ後処理
    /// </summary>
    protected virtual void OnCustomAfterDeserialize()
    {
    }
}
