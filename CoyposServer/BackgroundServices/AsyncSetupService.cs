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
		SetupDefaultLanguage();
	}

	private void SetupDefaultLanguage()
	{
		var databaseLanguage = _dbContext.Settings.FirstOrDefault(_ => _.Key == "language");
		if (databaseLanguage is null)
		{
			LanguageHelpers.DefaultLanguage = "pl";
			return;
		}

		LanguageHelpers.DefaultLanguage = databaseLanguage.Value;
	}
}