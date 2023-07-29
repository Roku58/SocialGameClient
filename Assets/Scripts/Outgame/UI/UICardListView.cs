using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Outgame
{
    // ガチャ管理ビュークラス
    public class UICardListView : UIStackableView
    {
        // ListViewの参照
        [SerializeField] ListView _listView;

        // UIStackableViewクラスのAwakeCallメソッドのオーバーライド
        protected override void AwakeCall()
        {
            // ViewIDを設定
            ViewId = ViewID.CardList;
            // ポップアップUIがあることを設定
            _hasPopUI = true;
        }

        // Startメソッド
        private void Start()
        {
            // ListViewのセットアップを実行
            _listView.Setup();
            // アクティブにする
            Active();
        }

        // Backメソッド
        public void Back()
        {
            // UIManagerのBackメソッドを実行して戻る
            UIManager.Back();
        }
    }
}