using MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outgame
{
    // イベントに関連するヘルパークラス
    public class EventHelper
    {
        // 開催中のイベントのIDを取得するメソッド
        static public List<int> GetAllOpenedEvent()
        {
            // 開催中のイベントIDを保持するリスト
            List<int> ret = new List<int>();

            // MasterDataクラスに定義されている全てのイベントに対してループを実行
            foreach (var evt in MasterData.Events)
            {
                // イベントが開催中(IsEventOpenメソッドで確認)であれば、そのイベントIDをリストに追加
                if (IsEventOpen(evt.Id))
                {
                    ret.Add(evt.Id);
                }
            }

            // 開催中のイベントIDのリストを返す
            return ret;
        }

        // 指定したイベントIDのイベントが開催中かどうかを判定するメソッド
        static public bool IsEventOpen(int eventId)
        {
            // イベントIDに対応するイベント情報をMasterDataクラスから取得
            var evt = MasterData.GetEvent(eventId);

            // 現在の日時を取得
            DateTime now = DateTime.Now;

            // イベントが開始日時よりも後かつ終了日時よりも前であれば、イベントは開催中と判定
            return evt.StartAt <= now && now <= evt.EndAt;
        }

        // 指定したイベントIDのイベントがプレイ可能かどうかを判定するメソッド
        static public bool IsEventGamePlayable(int eventId)
        {
            // イベントIDに対応するイベント情報をMasterDataクラスから取得
            var evt = MasterData.GetEvent(eventId);

            // 現在の日時を取得
            DateTime now = DateTime.Now;

            // イベントが開始日時よりも後かつゲーム終了日時よりも前であれば、イベントはプレイ可能と判定
            return evt.StartAt <= now && now <= evt.GameEndAt;
        }
    }
}