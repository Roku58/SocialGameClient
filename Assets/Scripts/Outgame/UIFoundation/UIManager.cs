using Outgame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// UIの管理を行うクラス。
/// </summary>
public class UIManager
{
    static UIManager _instance = new UIManager(); // UIManager クラスのシングルトンインスタンスを生成
    // public static UIManager Instance => _instance;
    private UIManager() { } // プライベートなコンストラクタでインスタンス生成を制限

    RectTransform _root; // キャンバスのRectTransformを保持
    UIStackableView _current = null; // 現在表示されているUIのスタックビューを保持
    Stack<UIInformationBase> _uiSceneHistory = new Stack<UIInformationBase>(); // UIシーンの履歴を保持するスタック
    Stack<UIStackableView> _uiStack = new Stack<UIStackableView>(); // UIのスタックを保持するスタック

    Dictionary<ViewID, GameObject> _sceneCache = new Dictionary<ViewID, GameObject>(); // シーンをキャッシュするための辞書

    /// <summary>
    /// シングルトンインスタンスをセットアップします。指定されたビューIDのシーンをロードします。
    /// </summary>
    public static void Setup(ViewID entry)
    {
        _instance._uiStack.Clear(); // UIスタックをクリア
        var rootCanvas = GameObject.FindObjectOfType<Canvas>(); // シーン内からCanvasコンポーネントを検索
        _instance._root = rootCanvas.GetComponent<RectTransform>(); // キャンバスのRectTransformを取得
        _instance.LoadScene(entry, null); // 指定されたビューIDでシーンをロード
    }

    /// <summary>
    /// UIを一つ前の状態に戻します。戻れた場合はtrueを返します。
    /// </summary>
    public static bool Back()
    {
        if (!_instance._current) return false; // 現在のUIスタックビューがない場合、戻れない

        _instance._current.Exit(); // 現在のUIスタックビューのExitメソッドを呼び出してUIを終了

        if (_instance._uiStack.Count > 0)
        {
            GameObject.Destroy(_instance._current.gameObject); // 現在のUIを破棄
            _instance._current = _instance._uiStack.Pop(); // UIスタックから前のスタックビューをポップ
            return true; // 戻った場合はtrueを返す
        }
        else
        {
            if (_instance._uiSceneHistory.Count >= 2)
            {
                _instance._uiSceneHistory.Pop(); // 今いたページのページ履歴を消す
                var info = _instance._uiSceneHistory.Pop();
                NextView(info.ViewID, info); // その前のページに戻る
            }
            else
            {
                NextView(ViewID.Home); // ホーム画面に戻る
            }
        }

        return false; // 戻れない場合はfalseを返す
    }

    void LoadSceneStack(ViewID next, UIInformationBase info)
    {
        LoadScene(next, info, true); // シーンをスタックしてロード
    }

    void LoadScene(ViewID next, UIInformationBase info, bool isStack = false)
    {
        GameObject sceneOrigin = null;
        if (_sceneCache.ContainsKey(next))
        {
            sceneOrigin = _sceneCache[next]; // シーンがキャッシュにあればキャッシュから読み込む
        }
        else
        {
            // シーンがキャッシュにない場合はアドレスブルから読み込む
            sceneOrigin = Addressables.LoadAssetAsync<GameObject>(string.Format("Assets/Scenes/Game/UI/{0}.prefab", next.ToString())).WaitForCompletion();
        }

        if (sceneOrigin == default)
        {
            Debug.LogError($"{next.ToString()}: シーンの読み込みに失敗");
            return;
        }

        var scene = GameObject.Instantiate(sceneOrigin, _root); // シーンを生成し、キャンバスの下に配置
        var view = scene.GetComponent<UIStackableView>(); // 生成されたシーンからUIStackableViewコンポーネントを取得
        if (view == default)
        {
            Debug.LogError($"{next.ToString()}: シーン管理スクリプトの読み込みに失敗");
            return;
        }

        CreateUIParts(next); // 指定されたビューIDに応じてUIのパーツを生成

        view.Enter(); // UIStackableViewのEnterメソッドを呼び出してUIを表示

        if (!isStack && _current)
        {
            GameObject.Destroy(_current.gameObject); // スタックしないケースの場合でも、スタックして後で壊すほうがいい
        }

        _current = view; // 現在のUIスタックビューを更新
        _current.SetInformation(info); // UIStackableViewの情報を設定

        Addressables.Release(sceneOrigin); // アドレスブルアセットを解放
    }

    /// <summary>
    /// 指定されたシーンオブジェクトを生成し、UIを表示します。
    /// </summary>
    public static void NextView(GameObject origin, UIInformationBase info = null)
    {
        var scene = GameObject.Instantiate(origin, _instance._root); // 指定されたシーンオブジェクトを生成し、キャンバスの下に配置
        var view = scene.GetComponent<UIStackableView>(); // 生成されたシーンからUIStackableViewコンポーネントを取得
        if (view == default)
        {
            Debug.LogError($"シーン管理スクリプトの読み込みに失敗");
            return;
        }

        _instance._current?.Exit(); // 現在のUIスタックビューのExitメソッドを呼び出してUIを終了
        GameObject.Destroy(_instance._current.gameObject); // 現在のUIを破棄

        _instance._current = view; // 現在のUIスタックビューを更新
        _instance._current?.Enter(); // UIStackableViewのEnterメソッドを呼び出してUIを表示
    }

    /// <summary>
    /// 指定されたビューIDに対応するシーンをロードして表示します。
    /// </summary>
    public static void NextView(ViewID next, UIInformationBase info = null)
    {
        _instance._current?.Exit(); // 現在のUIスタックビューのExitメソッドを呼び出してUIを終了

        if (info != null)
        {
            info.ViewID = next; // 情報に指定されたビューIDを設定
            _instance._uiSceneHistory.Push(info); // 情報をUIシーンの履歴に追加
        }
        else
        {
            _instance._uiSceneHistory.Push(new UIInformationBase() { ViewID = next }); // ビューIDだけの情報をUIシーンの履歴に追加
        }
        _instance.LoadScene(next, info); // 指定されたビューIDのシーンをロード
    }

    /// <summary>
    /// 指定されたビューIDに対応するシーンをスタックしてロードして表示します。
    /// </summary>
    public static void StackView(ViewID next, UIInformationBase info = null)
    {
        _instance._uiStack.Push(_instance._current); // 現在のUIスタックビューをスタックにプッシュ
        _instance.LoadSceneStack(next, info); // 指定されたビューIDのシーンをスタックしてロード
    }

    void CreateUIParts(ViewID next)
    {
        switch (next)
        {
            case ViewID.Home:
                if (UIStatusBar.IsNull)
                {
                    var stOrigin = Addressables.LoadAssetAsync<GameObject>("Assets/Scenes/Game/UI/Status.prefab").WaitForCompletion(); // アドレスブルからStatus.prefabを読み込み
                    GameObject.Instantiate(stOrigin, _root); // Status.prefabを生成してキャンバスの下に配置
                    Addressables.Release(stOrigin); // アドレスブルアセットを解放
                }
                break;
        }
    }
}
