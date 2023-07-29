using Cysharp.Threading.Tasks;
using MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Outgame
{
    // クエスト結果表示ビュークラス
    public class UIQuestResultView : UIStackableView
    {
        // ビューのルートオブジェクト
        [SerializeField] GameObject _root;

        // 報酬のプレハブオブジェクト
        [SerializeField] GameObject _rewardPrefab;

        // クエストID
        int _questId = 0;

        // AwakeCallメソッドのオーバーライド
        protected override void AwakeCall()
        {
            // ビューIDを設定
            ViewId = ViewID.QuestResult;
            // ポップアップUIがないことを設定
            _hasPopUI = false;

            // ビューの作成を実行
            CreateView();
        }

        // 報酬オブジェクトの文字列を取得するメソッド
        string GetRewardObjectString(APIResponceQuestReward reward)
        {
            // リターン用の変数
            string ret = "";

            // 報酬の種類に応じて文字列を設定
            switch ((RewardItemType)reward.type)
            {
                case RewardItemType.None: break;
                case RewardItemType.Card: ret = MasterData.GetLocalizedText(MasterData.GetCard(int.Parse(reward.param[0])).Name); break;
                case RewardItemType.Money: ret = string.Format("{0}Money", int.Parse(reward.param[0])); break;
                case RewardItemType.Item: ret = string.Format("{0}{1}つ", MasterData.GetLocalizedText(MasterData.GetItem(int.Parse(reward.param[0])).Name), int.Parse(reward.param[1])); break;

                    //TODO: イベントポイント
                    //case RewardItemType.EventPoint: ret = string.Format("{0}ポイント", int.Parse(reward.param[0])); break;
            }
            return ret;
        }

        // ビューの作成メソッド
        void CreateView()
        {
            // シーケンスからクエスト結果パッケージを取得
            var package = SequenceBridge.GetSequencePackage<QuestPackage>("Quest");

            // 報酬の表示を行う
            foreach (var reward in package?.QuestResult?.rewards)
            {
                Debug.Log(reward);
                // 報酬のタイプが0の場合はスキップ
                if (reward.type == 0) continue;

                // 報酬のオブジェクトをインスタンス化して表示する
                var rewardObj = GameObject.Instantiate(_rewardPrefab, _root.transform);
                var text = rewardObj.GetComponent<TextMeshProUGUI>();

                // 報酬の文字列を取得して表示する
                text.text = string.Format("{0}を手に入れた", GetRewardObjectString(reward));
            }

            // シーケンスの削除
            SequenceBridge.DeleteSequence("Quest");
        }

        // GoHomeメソッド
        public void GoHome()
        {
            // ホーム画面に遷移する
            UIManager.NextView(ViewID.Home);
        }
    }
}