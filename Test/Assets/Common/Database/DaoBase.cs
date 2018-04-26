using System;
using System.Collections.Generic;

/// <summary>
/// DAOベース
/// </summary>
/// <typeparam name="DTO_TYPE">DTOタイプ</typeparam>
public abstract class DaoBase<DTO_TYPE> : SQLiteDao,DaoImpl where DTO_TYPE : DtoBase<DTO_TYPE> , new()
{
    /// <summary>
    /// テーブル名
    /// </summary>
    /// <returns>テーブル名</returns>
    protected abstract string TblName();

    /// <summary>
    /// DTOファクトリ
    /// </summary>
    /// <returns>DAO</returns>
    protected DTO_TYPE DtoFactory()
    {
        return new DTO_TYPE();
    }

    /// <summary>
    /// レコード件数取得
    /// </summary>
    /// <returns>レコード件数</returns>
    public int Count()
    {
        return AppUtil.Obj2Int((ExecuteFetchOne(string.Format("SELECT COUNT(*) as TOTAL_COUNT FROM {0}", TblName()))["TOTAL_COUNT"]));
    }

    /// <summary>
    /// グループ削除
    /// </summary>
    /// <param name="groupId">グループID</param>
    public void DeleteGroup(int groupId)
    {
        Execute(string.Format("DELETE FROM {0} WHERE GROUP_ID=:GROUP_ID", TblName()), new Dictionary<string, object>() { { ":GROUP_ID", groupId } });
    }

    /// <summary>
    /// リプレース
    /// </summary>
    /// <param name="dto">リプレース対象データ</param>
    public virtual void Replace(DTO_TYPE dto)
    {
        // 新規登録
        if (Select(dto) == null)
        {
            Insert(dto);
        }
        else
        {
            Update(dto);
        }
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dto">登録対象データ</param>
    public virtual void Insert(DTO_TYPE dto)
    {
        Dictionary<string, string> bindInfo = dto.GetBindInfo();
        string methodType = "Insert";
        string query = GetQueryCache(methodType);

        // クエリがキャッシュになければ生成
        if (query == null)
        {
            string valueQuery = string.Join(",", CollectionUtil.Collection2Array<string>(bindInfo.Values));
            string clmQuery = string.Join(",", CollectionUtil.Collection2Array<string>(bindInfo.Keys));
            query = string.Format("INSERT INTO {0}({1}) VALUES({2})", TblName(), clmQuery, valueQuery);

            // 生成タイミングで再登録
            AddQueryCache(methodType, query);
        }

        Execute(query, dto.GetBindList());
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dtoList">登録対象データリスト</param>
    public virtual void Insert(List<DTO_TYPE> dtoList)
    {
        // 登録対象がなければ何もしない
        if (dtoList.Count < 1)
        {
            return;
        }

        // 先頭レコードをベースにカラムクエリを作製
        string clmQuery = string.Join(",", CollectionUtil.Collection2Array<string>(dtoList[0].GetBindInfo().Keys));

        // バリュークエリ生成
        List<string> valueQueryList = new List<string>();
        Dictionary<string, object> bindList = new Dictionary<string, object>();
        int recIndex = 0;
        foreach(DTO_TYPE dto in dtoList)
        {
            recIndex++;
            Dictionary<string, object> tmpBindList = dto.GetBindList(recIndex.ToString());
            dto.GetBindList(recIndex.ToString(), bindList);
            valueQueryList.Add(string.Format("({0})",string.Join(",", CollectionUtil.Collection2Array<string>(tmpBindList.Keys))));
        }

        string query = string.Format("INSERT INTO {0}({1}) VALUES {2}", TblName(), clmQuery, string.Join(",",valueQueryList.ToArray()));

        Execute(query, bindList);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto">更新対象データ</param>
    public virtual void Update(DTO_TYPE dto)
    {
        Dictionary<string, string> bindInfo = dto.GetBindInfo();
        Dictionary<string, string> pkBindInfo = dto.GrepPk(ref bindInfo);
        string methodType = "Update";
        string query = GetQueryCache(methodType);

        // クエリがキャッシュになければ生成
        if (query == null)
        {
            string updateQuery = string.Join(",", CreateQueryList(bindInfo));
            string whereQuery = string.Join(" AND ", CreateQueryList(pkBindInfo));
            query = string.Format("UPDATE {0} SET {1} WHERE {2}", TblName(), updateQuery, whereQuery);

            // 生成タイミングで再登録
            AddQueryCache(methodType, query);
        }

        Execute(query, dto.GetBindList());
    }

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="dto">削除対象データ</param>
    public virtual void Delete(DTO_TYPE dto)
    {
        Dictionary<string, string> bindInfo = dto.GetBindInfo();
        Dictionary<string, string> pkBindInfo = dto.GrepPk(ref bindInfo);

        // クエリ生成
        string whereQuery = string.Join(" AND ", CreateQueryList(pkBindInfo));
        string query = string.Format("DELETE FROM {0} WHERE {1}", this.TblName(), whereQuery);

        Execute(query,dto.GetBindList());
    }

    /// <summary>
    /// 全データ削除
    /// </summary>
    public virtual void DeleteAll()
    {
        Execute(string.Format("DELETE FROM {0}", this.TblName()));
    }

    /// <summary>
    /// 検索
    /// </summary>
    /// <param name="dto">検索対象データ</param>
    /// <returns>行DTO</returns>
    public DTO_TYPE Select(DTO_TYPE dto)
    {
        Dictionary<string, string> bindInfo = dto.GetBindInfo();
        Dictionary<string, string> pkBindInfo = dto.GrepPk(ref bindInfo);

        string methodType = "Select";
        string query = GetQueryCache(methodType);

        // クエリがキャッシュになければ生成
        if (query == null)
        {
            string whereQuery = string.Join(" AND ", CreateQueryList(pkBindInfo));
            query = string.Format("SELECT * FROM {0} WHERE {1} LIMIT 1", this.TblName(), whereQuery);

            // 生成タイミングで再登録
            AddQueryCache(methodType, query);
        }

        // データ取得
        Dictionary<string, object> row = this.ExecuteFetchOne(query, dto.GetBindList());

        // データが取得できなければnull返却
        if (row == null)
        {
            return null;
        }

        // メッセージパース用DTO
        DTO_TYPE retObj = this.DtoFactory();
        retObj.SetDictionary(row);

        return retObj;
    }

    /// <summary>
    /// 全データフェッチ(フェッチ終了までの間、該当のＤＢはロックされます)
    /// </summary>
    /// <returns>行DTO</returns>
    public IEnumerable<DTO_TYPE> Fetch(string query,Dictionary<string,object> bindList = null)
    {
        // メッセージパース用DTO
        DTO_TYPE dto = this.DtoFactory();

        // メッセージを全件取得し、1件づつ呼び元に返却
        foreach (Dictionary<string, object> rowData in ExecuteFetch(query, bindList))
        {
            // DTOにデータ設定
            dto.SetDictionary(rowData);

            // DTOに設定して返却
            yield return dto;
        }
    }

    /// <summary>
    /// 全データ取得
    /// </summary>
    /// <returns>行DTO</returns>
    public IEnumerable<DTO_TYPE> FetchAll()
    {
        // メッセージパース用DTO
        DTO_TYPE dto = this.DtoFactory();

        // メッセージを全件取得し、1件づつ呼び元に返却
        foreach (Dictionary<string, object> rowData in ExecuteFetch(string.Format("SELECT * FROM {0}", this.TblName())))
        {
            // DTOにデータ設定
            dto.SetDictionary(rowData);

            // DTOに設定して返却
            yield return dto;
        }
    }

    /// <summary>
    /// クエリリスト作成
    /// </summary>
    /// <param name="queryData">クエリデータ</param>
    /// <param name="queryDelmit">クエリデリミタ</param>
    /// <returns>クエリリスト</returns>
    private static string[] CreateQueryList(Dictionary<string, string> queryData, string queryDelmit = "=")
    {
        string[] queryList = new string[queryData.Count];
        int i = 0;

        // 対象分ループしながらクエリリストを作成
        foreach (KeyValuePair<string, string> clmData in queryData)
        {
            queryList[i++] = string.Format("{0} {1} {2}", clmData.Key, queryDelmit, clmData.Value);
        }

        return queryList;
    }

    #region 共有インターフェース ※共通呼び出しようのインターフェース/実処理はない

    /// <summary>
    /// リプレース
    /// </summary>
    /// <param name="dto">リプレース対象DTO</param>
    public void Replace(DtoImpl dto)
    {
        Replace((DTO_TYPE)dto);
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dto">登録対象DTO</param>
    public void Insert(DtoImpl dto)
    {
        Insert((DTO_TYPE)dto);
    }

    /// <summary>
    /// 登録
    /// </summary>
    /// <param name="dtoList">登録対象リスト</param>
    public void Insert(List<DtoImpl> dtoList)
    {
        Insert((List<DtoImpl>)dtoList);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="dto">更新対象DTO</param>
    public void Update(DtoImpl dto)
    {
        Update((DTO_TYPE)dto);
    }

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="dto">削除対象DTO</param>
    public void Delete(DtoImpl dto)
    {
        Delete((DTO_TYPE)dto);
    }

    /// <summary>
    /// 検索
    /// </summary>
    /// <param name="dto">検索条件DTO</param>
    /// <returns>検索結果</returns>
    DtoImpl DaoImpl.Select(DtoImpl dto)
    {
        return Select((DTO_TYPE)dto);
    }

    /// <summary>
    /// フェッチ
    /// </summary>
    /// <param name="query">クエリ</param>
    /// <param name="bindList">バインドリスト</param>
    /// <returns>行データDTO</returns>
    IEnumerable<DtoImpl> DaoImpl.Fetch(string query, Dictionary<string, object> bindList)
    {
        return (IEnumerable<DtoImpl>)Fetch(query, bindList);
    }

    /// <summary>
    /// フェッチ
    /// </summary>
    /// <returns>行データDTO</returns>
    IEnumerable<DtoImpl> DaoImpl.FetchAll()
    {
        return (IEnumerable<DtoImpl>)FetchAll();
    }
    #endregion
}
