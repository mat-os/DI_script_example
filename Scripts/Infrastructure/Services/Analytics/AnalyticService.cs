using System;
using System.Text;
using Configs;
using Game.Scripts.Core.StateMachine;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.LevelStateMachin.States;
using Game.Scripts.Infrastructure.Services.Level;
using Game.Scripts.Utils.Debug;
using Io.AppMetrica;
using UnityEngine;

namespace Game.Scripts.Infrastructure.Services.Analytics
{
	public class AnalyticService : IDisposable
	{
		private DataService _dataService;
		private string _appMetricaKey;
		
		private float _levelStart;
		private LevelsRepository _levelsRepository;
		private LevelDataService _levelDataService;

		// Основные события уровня
		private const string LEVEL = "level";
		private const string LEVEL_START_EVENT = "level_start";
		private const string LEVEL_COMPLETE_EVENT = "level_complete";
		private const string LEVEL_FAIL_EVENT = "level_fail";
		private const string RETRY_LEVEL_EVENT = "level_retry";
		private const string STARS_EARNED = "stars_earned";
		private const string SCORE = "score";
		private const string ATTEMPT_NUMBER = "attempt_number";
		private const string RETRY_REASON = "retry_reason";
		
		private const string LEVEL_FAIL_EVENT_DETAILS = "level_fail_details";
		private const string LEVEL_FAIL_DETAILS = "fail_details";
		
		private const string LEVEL_RETRY_EVENT_DETAILS = "level_retry_details";
		private const string LEVEL_RETRY_DETAILS = "retry_details";

		// Экономика
		private const string CURRENCY_EARNED_EVENT = "currency_earned";
		private const string CURRENCY_EARNED_SOURCE = "currency_source";
		private const string CURRENCY_EARNED_AMOUNT = "currency_amount";
		
		private const string CURRENCY_SPENT_EVENT = "currency_spent";

		// Интерфейс и улучшения
		private const string UI_CLICK_EVENT = "ui_click";
		private const string UI_BUTTON_ID = "ui_id";
		
		private const string UPGRADE_USED_EVENT = "upgrade_used";
		
		private const string UPGRADE_BUY_EVENT = "upgrade_buy";
		private const string UPGRADE_TYPE_EVENT = "upgrade_type";
		private const string UPGRADE_BUY_LEVEL = "upgrade_level";

		// Поведение игроков
		private const string PLAYER_EXIT_LEVEL_EVENT = "exit_level";
		private const string EXIT_REASON = "exit_reason";
		
		private const string CUSTOMIZATION_NAME = "customization_name";
		private const string BUY_CUSTOMIZATION_EVENT = "buy_customization";

		private const string PLAYER_QUIT_GAME_EVENT = "player_quit_game";
		private const string DAILY_RETURN_EVENT = "daily_return";

		private const string DAYS_SINCE_REG = "days since reg";
		private const string TIME_SPENT = "time_spent";
		
		// События сессии
		//private const string SESSION_START_EVENT = "session_start";
		//private const string SESSION_END_EVENT = "session_end";
		public AnalyticService(DataService dataService, GameConfig gameConfig, LevelsRepository levelsRepository, LevelDataService levelDataService)
		{
			_levelDataService = levelDataService;
			_levelsRepository = levelsRepository;
			_dataService = dataService;
			_appMetricaKey = gameConfig.AppMetricaKey;
		}

		public void Initialize()
		{
			AppMetrica.Activate(new AppMetricaConfig(_appMetricaKey)
			{
				FirstActivationAsUpdate = _dataService.GameSettings.IsItFirstLaunch.Value == false
			});
			
			SubscribeEvents();
			
			CustomDebugLog.Log($"[ANAL] Initialize!", DebugColor.Green);
		}

		private void SubscribeEvents()
		{
		}

		// 🔥 Основные события уровня
		public void LogLevelStart(int levelIndex, int packIndex)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);

			String eventParameters = $"{{\"{LEVEL}\":\"{levelId}\", \"{DAYS_SINCE_REG}\":\"{GetDaysSinceRegistration()}\"}}";
			SendAppMetricaEvent(LEVEL_START_EVENT, eventParameters);

			CustomDebugLog.Log($"[ANAL] Log Level Start {levelId}, DAYS_SINCE_REG {GetDaysSinceRegistration()}" , DebugColor.Green);
			_levelStart = Time.time;
		}

		public void LogLevelComplete(int levelIndex, int packIndex, int starsEarned, int score)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);
			
			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{TIME_SPENT}\":\"{GetTimeSpent()}\", ")
				.Append($"\"{STARS_EARNED}\":\"{starsEarned}\", ")
				.Append($"\"{SCORE}\":\"{score}\", ")
				.Append($"\"{ATTEMPT_NUMBER}\":\"{GetCurrentAttemptNumber()}\", ")
				.Append($"\"{DAYS_SINCE_REG}\":\"{GetDaysSinceRegistration()}\"")
				.Append("}");
				
			SendAppMetricaEvent(LEVEL_COMPLETE_EVENT, sb.ToString());
			
			CustomDebugLog.Log($"[ANAL] Log Level Complete. Level: {levelId}, " +
			                   $"TIME_SPENT: {GetTimeSpent()}," +
			                   $" Attempt: {GetCurrentAttemptNumber()}," +
			                   $" DAYS_SINCE_REG: {GetDaysSinceRegistration()}" +
			                   $"STARS_EARNED { starsEarned} " +
			                   $"SCORE {score}" , DebugColor.Green);
			
			//String eventParameters = $"{{\"level\":\"{levelNumber}\", \"{TIME_SPENT}\":\"{Get_Time_Spent()}\", \"{DAYS_SINCE_REG}\":\"{GetDaysSinceRegistration()}\"}}";
		}

		public void LogLevelFail(int levelIndex, int packIndex)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);
				
			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{TIME_SPENT}\":\"{GetTimeSpent()}\", ")
				.Append($"\"{ATTEMPT_NUMBER}\":\"{GetCurrentAttemptNumber()}\", ")
				.Append($"\"{DAYS_SINCE_REG}\":\"{GetDaysSinceRegistration()}\"")
				.Append("}");
				
			SendAppMetricaEvent(LEVEL_FAIL_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] Log Level Fail. Level: {levelId}, TIME_SPENT: {GetTimeSpent()}, Attempt: {GetCurrentAttemptNumber()}, DAYS_SINCE_REG: {GetDaysSinceRegistration()}" , DebugColor.Green);

			var sbDetailed = new StringBuilder();
			var failDetailed = $"{levelId}_attempt_{GetCurrentAttemptNumber()}";
			sbDetailed.Append("{")
				.Append($"\"{LEVEL_FAIL_DETAILS}\":\"{failDetailed}\", ")
				.Append("}");
			SendAppMetricaEvent(LEVEL_FAIL_EVENT_DETAILS, sbDetailed.ToString());

			CustomDebugLog.Log($"[ANAL] Log Level Fail DETAILED. LEVEL_FAIL_DETAILS: {failDetailed}" , DebugColor.Green);
			//String eventParameters = $"{{\"level\":\"{levelNumber}\",  \"{TIME_SPENT}\":\"{GetTimeSpent()}\", \"{DAYS_SINCE_REG}\":\"{GetDaysSinceRegistration()}\"}}";
		}

		public void LogLevelRetry(int levelIndex, int packIndex, ERetryReason eRetryReason)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);

			var retryNumber = GetCurrentAttemptNumber() - 1;
			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{RETRY_REASON}\":\"{eRetryReason.ToString()}\", ")
				.Append($"\"{ATTEMPT_NUMBER}\":\"{retryNumber}\"")
				.Append("}");
			
			SendAppMetricaEvent(RETRY_LEVEL_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] RetryLevel Event. Level: {levelId}, Source: {eRetryReason}, Attempt: {GetCurrentAttemptNumber()}" , DebugColor.Green);
			
			var sbDetailed = new StringBuilder();
			var retryDetailed = $"{levelId}_attempt_{retryNumber}_{eRetryReason.ToString()}";
			sbDetailed.Append("{")
				.Append($"\"{LEVEL_RETRY_DETAILS}\":\"{retryDetailed}\", ")
				.Append("}");
			SendAppMetricaEvent(LEVEL_RETRY_EVENT_DETAILS, sbDetailed.ToString());
			CustomDebugLog.Log($"[ANAL] LEVEL_RETRY_EVENT_DETAILS Event. LEVEL_RETRY_DETAILS: {retryDetailed}" , DebugColor.Green);

			//String eventParameters = $"{{\"level\":\"{levelNumber}\", \"retry_source\":\"{retryReason}\", \"attempt_number\":\"{GetCurrentAttemptNumber()}\"}}";
		}

		// 🔥 Экономика
		public void LogCurrencyEarned(int levelIndex, int packIndex, ECurrencySource source, int amount)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);

			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{CURRENCY_EARNED_SOURCE}\":\"{source.ToString()}\", ")
				.Append($"\"{CURRENCY_EARNED_AMOUNT}\":\"{amount}\"")
				.Append("}");
			
			SendAppMetricaEvent(CURRENCY_EARNED_EVENT, sb.ToString());
			
			CustomDebugLog.Log($"[ANAL] Log Currency Earned. Level: {levelId}, Source: {source.ToString()}, Amount: {amount}" , DebugColor.Green);
		}

		public void LogBuyUpgrade(string upgradeType, int upgradeLevel)
		{
			var levelId = _levelDataService.GetGlobalLevelId(_dataService.Level.LevelIndex.Value, _dataService.Level.LevelPackIndex.Value);

			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{UPGRADE_TYPE_EVENT}\":\"{upgradeType}\", ")
				.Append($"\"{UPGRADE_BUY_LEVEL}\":\"{upgradeLevel}\"")
				.Append("}");
			
			SendAppMetricaEvent(UPGRADE_BUY_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] Log Buy Upgrade. Level: {levelId}, Upgrade Type: {upgradeType}, Upgrade Level: {upgradeLevel}" , DebugColor.Green);
		}

		// 🔥 Поведение игроков
		public void LogPlayerExitLevel(int levelIndex, int packIndex, EExitLevelReason exitReason)
		{
			var levelId = _levelDataService.GetGlobalLevelId(levelIndex, packIndex);

			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{EXIT_REASON}\":\"{exitReason.ToString()}\"")
				.Append("}");

			SendAppMetricaEvent(PLAYER_EXIT_LEVEL_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] Log Exit Level. Level: {levelId}, EXIT_REASON: {exitReason.ToString()}" , DebugColor.Green);
		}
		
		// 🔥 Интерфейс
		public void LogUIClick(string buttonId)
		{
			var levelId = _levelDataService.GetGlobalLevelId(_dataService.Level.LevelIndex.Value, _dataService.Level.LevelPackIndex.Value);
			
			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{LEVEL}\":\"{levelId}\", ")
				.Append($"\"{UI_BUTTON_ID}\":\"{buttonId}\"")
				.Append("}");
			
			SendAppMetricaEvent(UI_CLICK_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] LogUIClick. Level: {levelId}, UI_BUTTON_ID: {buttonId}" , DebugColor.Green);
		}
		public void LogPlayerBuyCustomizationItem(string itemName)
		{
			var sb = new StringBuilder();
			sb.Append("{")
				.Append($"\"{CUSTOMIZATION_NAME}\":\"{itemName}\", ")
				.Append("}");
			
			SendAppMetricaEvent(BUY_CUSTOMIZATION_EVENT, sb.ToString());
			CustomDebugLog.Log($"[ANAL] LogPlayerBuyCustomizationItem. CUSTOMIZATION_NAME: {itemName}" , DebugColor.Green);
		}
		/*public void LogDailyReturn(int returnStreak, int daysAbsent)
		{
			var eventParameters = $"{{\"return_streak\":\"{returnStreak}\", \"days_absent\":\"{daysAbsent}\"}}";
			SendAppMetricaEvent(DAILY_RETURN_EVENT, eventParameters);
		}*/

		private void SendAppMetricaEvent(string eventName, string properties)
		{
			if (string.IsNullOrWhiteSpace(eventName))
			{
				CustomDebugLog.LogError($"[ANAL] SendAppMetricaEvent : string.IsNullOrWhiteSpace(eventName)", DebugColor.Green);
				return;
			}
			AppMetrica.ReportEvent(eventName, properties);
			AppMetrica.SendEventsBuffer();
		}

		private int GetDaysSinceRegistration()
		{
			// Получаем строковое значение даты регистрации
			string registrationDateString = _dataService.GameSettings.RegistrationDate.Value;

			// Проверка на null или пустую строку
			if (string.IsNullOrEmpty(registrationDateString))
				throw new InvalidOperationException("RegistrationDate is not set or is empty.");

			// Попытка преобразования строки в DateTime
			if (!DateTime.TryParseExact(registrationDateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var registrationDate))
				throw new InvalidOperationException("Invalid RegistrationDate format. Expected format is yyyy-MM-dd.");

			// Вычисление разницы в днях
			return (DateTime.Now - registrationDate).Days;
		}

		private int GetTimeSpent()
		{
			var endTime = Time.time;
			return Mathf.RoundToInt(endTime - _levelStart);
		}

		private int GetCurrentAttemptNumber()
		{
			return _dataService.Level.AttemptNumber.Value;
		}
		public void Dispose()
		{
			
		}
	}

	public enum ERetryReason
	{
		AfterFail = 1,
		AfterWin = 2,
		FromSettings = 3,
	}
	public enum ECurrencySource
	{
		LevelCompleted = 1,
		LevelFailed = 2,
	}	
	public enum EExitLevelReason
	{
		SettingsMenu = 1,
		LevelComplete = 2,
		LevelFail = 3,
	}
}
