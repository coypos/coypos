using System.Globalization;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CoyposServer.BackgroundServices;

public class AsyncSetupService : BackgroundService
{
	private readonly DatabaseContext _dbContext;
	
	public AsyncSetupService(DatabaseContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var lang = SetupDefaultLanguage();
		Log.Msg($"Language set to: {new CultureInfo(lang).DisplayName} ({lang})");
	}

	private string SetupDefaultLanguage()
	{
		var databaseLanguage = _dbContext.Settings.FirstOrDefault(_ => _.Key == "language");
		LanguageHelpers.DefaultLanguage = databaseLanguage is null ? "pl" : databaseLanguage.Value;
		return LanguageHelpers.DefaultLanguage;
	}
}