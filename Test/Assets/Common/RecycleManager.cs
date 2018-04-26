using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// リサイクルマネージャー
/// </summary>
public class RecycleManager : MonoBehaviour
{
    /// <summary>
    /// リサイクルコンテナ
    /// </summary>
    [SerializableAttribute]
    public class RecycleContainer : RecycleImpl
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id;

        /// <summary>
        /// リサイクル数
        /// </summary>
        public int recycleCount;

        /// <summary>
        /// コンテナ
        /// </summary>
        public GameObject container;

        /// <summary>
        /// リサイクルIDゲッター
        /// </summary>
        public string RecycleId{ get { return id; } }
    }

    /// <summary>
    /// コンテナリスト
    /// </summary>
    [SerializeField]
    public List<RecycleContainer> containerList;

    /// <summary>
    /// コンテナインデックス
    /// </summary>
    private Dictionary<string, int> containerIndex;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        int index = 0;

        // コンテナインデックス生成
        containerIndex = new Dictionary<string, int>();
        foreach(RecycleContainer cur in containerList)
        {
            containerIndex.Add(cur.id, index++);
        }
    }

    /// <summary>
    /// リサイクルコンテナクリア
    /// </summary>
    /// <param name="prefab">プレイハブ</param>
    public void ClearRecyleContainer(GameObject prefab)
    {
        RecycleImpl impl = prefab.GetComponent<RecycleImpl>();

        // コンテナが見つからなければnull返却
        if (impl == null)
        {
            return;
        }

        // リサイクルコンテナクリア
        ClearRecyleContainer(impl.RecycleId);
    }

    /// <summary>
    /// リサイクルコンテナクリア
    /// </summary>
    /// <param name="id">ID</param>
    public void ClearRecyleContainer(string id)
    {
        RecycleContainer recycleContainer = GetRecycleContainer(id);

        // リサイクルコンテナがなければここまで
        if(recycleContainer == null)
        {
            return;
        }

        // リサイクルコンテナクリア
        foreach (Transform child in recycleContainer.container.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// リサイクル
    /// </summary>
    /// <param name="target"></param>
    public void Recycle(GameObject target)
    {
        RecycleContainer recycleContainer = GetRecycleContainer(target.GetComponent<RecycleImpl>());

        // コンテナ未定義なら何もしない
        if (recycleContainer == null)
        {
            return;
        }

        // リサイクルカウントを超えている場合は破棄
        if(recycleContainer.container.transform.childCount >= recycleContainer.recycleCount)
        {
            Destroy(target);
            return;
        }

        // 非アクティブに変更
        target.SetActive(false);

        // コンテナに追加
        target.transform.SetParent(recycleContainer.container.transform);
    }

    /// <summary>
    /// プレハブ実体化
    /// </summary>
    /// <param name="prefab">プレハブ</param>
    /// <returns>実体化したオブジェクト</returns>
    public GameObject Instantiate(GameObject prefab)
    {
        RecycleContainer recycleContainer = GetRecycleContainer(prefab.GetComponent<RecycleImpl>());

        // コンテナが見つからなければnull返却
        if(recycleContainer == null)
        {
            return CreateObject(prefab);
        }

        // コンテナがあれば返却
        foreach(Transform child in recycleContainer.container.transform)
        {
            // リサイクルからはずす
            child.SetParent(null);

            // アクティブ化
            child.gameObject.SetActive(true);

            // ゲームオブジェクトを返却
            return child.gameObject;
        }

        // コンテナが定義されていても、リサイクル対象がいない場合
        return CreateObject(prefab);
    }

    /// <summary>
    /// リサイクルコンテナ取得
    /// </summary>
    /// <param name="impl"></param>
    /// <returns></returns>
    private RecycleContainer GetRecycleContainer(RecycleImpl impl)
    {

        // リサイクルインターフェース未実装なら新規オブジェクト生成
        if (impl == null)
        {
            return null;
        }

        // コンテナを返却
        return GetRecycleContainer(impl.RecycleId);
    }

    /// <summary>
    /// リサイクルコンテナ取得
    /// </summary>
    /// <param name="impl"></param>
    /// <returns></returns>
    private RecycleContainer GetRecycleContainer(string id)
    {

        // コンテナ未定義
        if (!containerIndex.ContainsKey(id))
        {
            return null;
        }


        // コンテナを返却
        return containerList[containerIndex[id]];
    }

    /// <summary>
    /// オブジェクト生成
    /// </summary>
    /// <param name="prefab">プレハブ</param>
    /// <returns>生成したオブジェクト</returns>
    public GameObject CreateObject(GameObject prefab)
    {
        GameObject copy = GameObject.Instantiate(prefab) as GameObject;
        copy.transform.SetParent(null);

        // コンテナが合っても
        return copy;
    }
}