using System;
using System.Threading.Tasks;

namespace GamingCoPilot.Services
{
    /// <summary>
    /// Collects feedback from the user, specifically a helpfulness rating.
    /// </>
    public class FeedbackCollector
    {
        /// <summary>
        /// Asynchronously collects a helpfulness rating from the user via console input.
        /// </summary>
        /// <returns>
        /// An integer rating between 1 and 5 inclusive.
        /// The method will repeatedly prompt the user until a valid rating is entered.
        /// </returns>
        public async Task<int> CollectRatingAsync()
        {
            while (true)
            {
                Console.Write("Rate helpfulness 1-5: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int rating) && rating >= 1 && rating <= 5)
                {
                    return rating;
                }

                Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
            }
        }
    }
}