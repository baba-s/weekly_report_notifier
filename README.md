# 週の最終営業日に Slack に通知を投げるツール

週の最終営業日に指定されたテキストを Slack に通知するツールです  
例えば、週報の提出をアナウンスするために使用できます  
Jenkins などで使用することを想定しています  

## 使用例

```bat
app.exe ^
--holidayFilePath "C:\syukujitsu_kyujitsu.csv" ^
--incomingWebhooksUrl "【Incoming Webhooks の URL】" ^
--channel "#general" ^
--text "週報を提出してください" ^
--date "2018/11/22"
```

指定された date が週の最終営業日の場合、Slack に通知を投げます  
Jenkins で毎日 17:00 に上記のバッチを実行することで  
Slack のメンバーに週報の提出をアナウンスできます  

## 引数

|引数|内容|
|:--|:--|
|--holidayFilePath|http://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html からダウンロードできる<br />syukujitsu_kyujitsu.csv のファイルパス|
|--incomingWebhooksUrl|通知を投げる先の Slack の Incoming Webhooks URL|
|--channel|通知を投げる先の Slack のチャンネル名|
|--text|Slack に投げるテキスト|
|--date|日付（yyyy/MM/dd） デフォルト値は本日|