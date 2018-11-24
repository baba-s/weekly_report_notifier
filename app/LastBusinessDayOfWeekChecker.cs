using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// 週の最終営業日かどうかを判断するクラス
/// </summary>
internal static class LastBusinessDayOfWeekChecker
{
	//==============================================================================
	// クラス
	//==============================================================================
	/// <summary>
	/// syukujitsu_kyujitsu.csv から読み込んだレコードを管理するクラス
	/// </summary>
	private sealed class HolidayData
	{
		public string DateTime	{ get; set; }
		public string Name		{ get; set; }
	}

	/// <summary>
	/// syukujitsu_kyujitsu.csv から読み込んだテーブルを管理するクラス
	/// </summary>
	private sealed class HolidayTable : ClassMap<HolidayData>
	{
		private HolidayTable()
		{
			Map( c => c.DateTime	).Index( 0 );
			Map( c => c.Name		).Index( 1 );
		}
	}

	//==============================================================================
	// 変数
	//==============================================================================
	private static DateTime[] m_holidayList;	// 休日・祝日の日付を管理する配列

	//==============================================================================
	// 関数
	//==============================================================================
	/// <summary>
	/// <para>http://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html からダウンロードできる</para>
	/// <para>syukujitsu_kyujitsu.csv を読み込んで休日・祝日の日付を管理する配列を作成します</para>
	/// </summary>
	public static void Load( string path )
	{
		using ( var csv = new CsvReader( new StreamReader( path, Encoding.UTF8 ) ) )
		{
			var config = csv.Configuration;
			config.HasHeaderRecord = true;	// ヘッダ行は無視
			config.RegisterClassMap<HolidayTable>();

			m_holidayList = csv
				.GetRecords<HolidayData>()
				.Select( c => DateTime.Parse( c.DateTime ) )
				.Select( c => c.Date )
				.ToArray()
			;
		}
	}

	/// <summary>
	/// 指定された日付が週の最終営業日の場合 true を返します
	/// </summary>
	public static bool IsLast( DateTime dt )
	{
		// 時間は無視
		var now = dt.Date;

		// 指定された日付が属する週の金曜日を取得
		var lastDay	= now.AddDays( DayOfWeek.Friday - now.DayOfWeek );

		// 取得した金曜日が休日もしくは祝日の場合
		while
		(
			m_holidayList.Contains( lastDay.Date )	||
			lastDay.DayOfWeek == DayOfWeek.Saturday	||
			lastDay.DayOfWeek == DayOfWeek.Sunday
		)
		{
			// 前日が休日もしくは祝日かどうかを確認していく
			lastDay = lastDay.AddDays( -1 );
		}

		// 指定された日付が週の最終営業日かどうか
		var isLast = now == lastDay;

		return isLast;
	}
}