using MySuperShop.Interfaces;

namespace MySuperShop.Services
{
    /// <summary>
    /// Генератор UTC времени 
    /// </summary>
    public class UtcCurrentTime : ICurrentTime
	{

		/// <summary>
		/// Получить текущее UTC время
		/// </summary>
		/// <returns>Текущее UTC время</returns>
		public DateTime getCurrentTime()
		{
			return DateTime.UtcNow;
		}
	}
}
