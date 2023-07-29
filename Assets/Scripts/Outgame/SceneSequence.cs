using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Outgame
{
    // シーンのシーケンスを制御するクラス
    public class SceneSequence : MonoBehaviour
    {
        // シーンを識別するための列挙型
        enum SceneIdentifier
        {
            Title,       // タイトルシーンを表す識別子
            UserCreate,  // ユーザー作成シーンを表す識別子
        }

        // 現在のシーケンスを保持する変数
        SceneIdentifier _sequence;

        // Updateメソッドはフレームごとに呼び出されるUnityのコールバックメソッド
        private void Update()
        {
            // 現在のシーケンスによって処理を分岐する
            switch (_sequence)
            {
                // ここにシーケンスごとの処理を追加していく予定
            }
        }
    }
}
