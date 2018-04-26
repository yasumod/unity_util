using System.Collections.Generic;

/// <summary>
/// DAOインターフェース
/// </summary>
public interface DaoImpl
{
    /// <summary>
    /// DB名取得
    /// </summary>
    /// <returns>DB名</returns>
    string GetDbName();

    /// <summary>
    /// リプレース
    /// </summary>
    /// <param name="dto">リプレース対象データ</param>
    void Replace(DtoImpl dto);

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dto">登録対象データ</param>
    void Insert(DtoImpl dto);

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dtoList">登録対象データリスト</param>
    void Insert(List<DtoImpl> dtoList);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto">更新対象データ</param>
    void Update(DtoImpl dto);

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="dto">削除対象データ</param>
    void Delete(DtoImpl dto);

    /// <summary>
    /// 全データ削除
    /// </summary>
    /// <returns>行DTO</returns>
    void DeleteAll();

    /// <summary>
    /// 検索
    /// </summary>
    /// <param name="dto">検索対象データ</param>
    /// <returns>検索結果</returns>
    DtoImpl Select(DtoImpl dto);

    /// <summary>
    /// 全データフェッチ(フェッチ終了までの間、該当のＤＢはロックされます)
    /// </summary>
    /// <returns>行DTO</returns>
    IEnumerable<DtoImpl> Fetch(string query,Dictionary<string,object> bindList = null);

    /// <summary>
    /// 全データ取得
    /// </summary>
    /// <returns>行DTO</returns>
    IEnumerable<DtoImpl> FetchAll();
}
