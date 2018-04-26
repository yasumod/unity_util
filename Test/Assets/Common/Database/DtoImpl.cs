using System.Collections.Generic;

/// <summary>
/// DTOインターフェース
/// </summary>
public interface DtoImpl
{
    /// <summary>
    /// PKグレップ
    /// 引数辞書から、PKのデータだけ抜き出します。
    /// </summary>
    /// <typeparam name="VALUE_TYPE">バリュータイプ</typeparam>
    /// <param name="rowData">ローデータ(PK情報は削除)</param>
    /// <returns>グレップ結果</returns>
    Dictionary<string, VALUE_TYPE> GrepPk<VALUE_TYPE>(ref Dictionary<string, VALUE_TYPE> rowData);

    /// <summary>
    /// Dictionay設定
    /// </summary>
    /// <param name="dicData">行データ</param>
    /// <returns>行データ設定後のDTO(チェーン用)</returns>
    void SetDictionary(Dictionary<string, object> dicData);

    /// <summary>
    /// バインド情報取得
    /// </summary>
    /// <returns>バインド情報(バインドカラム/ホスト変数名)</returns>
    Dictionary<string, string> GetBindInfo();

    /// <summary>
    /// バインドリスト取得
    /// </summary>
    /// <param name="prefix">プレフィックス</param>
    /// <param name="bindList">バインドリスト</param>
    /// <returns>バインド情報(ホスト変数名/ホスト変数設定値)</returns>
    Dictionary<string, object> GetBindList(string prefix = "", Dictionary<string, object> bindList = null);
}
