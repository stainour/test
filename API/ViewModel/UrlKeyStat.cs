namespace API.ViewModel
{
    public class UrlKeyStat
    {
        /// <summary>
        /// Количество посещений
        /// </summary>
        public long HitCount { get; set; }

        /// <summary>
        /// Сокращённая ссылка
        /// </summary>
        public string Url { get; set; }
    }
}