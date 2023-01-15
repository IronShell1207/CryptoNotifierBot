using TelegramBot.Models;

namespace TelegramBot.Helpers
{
    public class AppSettingsHelper
    {
        #region Private Fields

        /// <summary>
        /// Статичный экземпляр класса <see cref="LocalSettingsHelper"/>.
        /// </summary>
        private static readonly Lazy<AppSettingsHelper> _instance = new((() => new AppSettingsHelper()));

        private SettingsModel _settings;

        private string _settingsFolderName = @"\Tcryptobot\";

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Статичный экземпляр класса <see cref="LocalSettingsHelper"/>.
        /// </summary>
        public static AppSettingsHelper Instance => AppSettingsHelper._instance.Value;

        /// <summary>
        /// Настройки бота.
        /// </summary>
        public SettingsModel Settings
        {
            get
            {
                if (_settings != null) return _settings;
                _settings = File.Exists(_settsPath) ? JsonHelper.LoadSettings<SettingsModel>(_settsPath) : new SettingsModel();
                return _settings;
            }
        }

        #endregion Public Properties

        #region Private Properties

        private string _settsPath
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _settingsFolderName;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path + "config.json";
            }
        }

        #endregion Private Properties
    }
}