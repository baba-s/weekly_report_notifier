using CommandLine;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

/*
 * # 使用例
 *
 * app.exe ^
 * --holidayFilePath "C:\syukujitsu_kyujitsu.csv" ^
 * --incomingWebhooksUrl "【Incoming Webhooks の URL】" ^
 * --channel "#general" ^
 * --text "週報を提出してください" ^
 * --date "2018/11/22"
 */
internal static class Program
{
	//==============================================================================
	// クラス
	//==============================================================================
	/// <summary>
	/// コマンドライン引数を管理するクラス
	/// </summary>
	private sealed class Options
	{
		[Option( "holidayFilePath", Required = true, HelpText = "http://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html からダウンロードできる syukujitsu_kyujitsu.csv のファイルパス" )]
		public string HolidayFilePath { get; set; }
		[Option( "incomingWebhooksUrl", Required = true, HelpText = "通知を投げる先の Slack の Incoming Webhooks URL" )]
		public string IncomingWebhooksUrl { get; set; }
		[Option( "channel", Required = true, HelpText = "通知を投げる先の Slack のチャンネル名" )]
		public string Channel { get; set; }
		[Option( "text", Required = true, HelpText = "Slack に投げるテキスト" )]
		public string Text { get; set; }
		[Option( "date", HelpText = "日付（yyyy/MM/dd） デフォルト値は本日" )]
		public string Date { get; set; }
	}

	/// <summary>
	/// Slack に送信するデータを管理するクラス
	/// </summary>
	private sealed class Payload
	{
		public string channel	;
		public string text		;
	}

	//==============================================================================
	// 関数
	//==============================================================================
	/// <summary>
	/// エントリポイント
	/// </summary>
	private static void Main( string[] args )
	{
		// コマンドライン引数を解析
		Parser.Default
			.ParseArguments<Options>( args )
			.WithParsed( OnParsed )
		;
	}

	/// <summary>
	/// コマンドライン引数の解析が完了したら呼び出される
	/// </summary>
	private static void OnParsed( Options options )
	{
		// 指定された日付を DateTime に変換
		// 日付が指定されていない場合は本日を使用
		var date	= options.Date;
		var now		= string.IsNullOrWhiteSpace( date )
			? DateTime.Now.Date
			: DateTime.Parse( options.Date )
		;

		// 指定された日付が週の最終営業日かどうかを確認
		LastBusinessDayOfWeekChecker.Load( options.HolidayFilePath );
		var isLast = LastBusinessDayOfWeekChecker.IsLast( now );

		if ( !isLast )
		{
			Console.WriteLine( "週の最終営業日ではありません" );
			return;
		}

		Console.WriteLine( "週の最終営業日なので Slack に通知を投げます" );

		// 週の最終営業日の場合は Slack に通知を投げる
		var payload = new Payload
		{
			channel	= options.Channel,
			text	= options.Text,
		};

		var json	= JsonConvert.SerializeObject( payload );
		var client	= new WebClient { Encoding = Encoding.UTF8 };
		client.Headers.Add( HttpRequestHeader.ContentType, "application/json;charset=UTF-8" );
		client.UploadString( options.IncomingWebhooksUrl, json );
	}
}