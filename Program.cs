using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Principal;
using System.Threading.Tasks;
public class Program
{
    private const string unlockCode = "3576876"; // The code to unlock the lock

    // Array to track which digits have been found
    private static bool[] foundDigits = new bool[unlockCode.Length];

    static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("Press Enter to run the code hack (or Ctrl+C to quit)...");
            Console.ReadLine();
            Console.WriteLine("Attempting to unlock...");

            // Create tasks for rolling digits that are not found yet
            var rollTasks = new Task<int>[foundDigits.Length];
            for (int i = 0; i < foundDigits.Length; i++)
            {
                if (!foundDigits[i])
                {
                    rollTasks[i] = RollDigitAsync($"digit{i + 1}"); // Start rolling the digit asynchronously
                }
                else
                {
                    // Initialize with a completed task to avoid null entries
                    rollTasks[i] = Task.FromResult(0);
                }
            }

            try
            {
                // Wait for all tasks to complete and get their results
                int[] rolledDigits = await Task.WhenAll(rollTasks);

                string rolledCode = new string(Array.ConvertAll(rolledDigits, d => (char)(d + '0')));

                Console.WriteLine($"The required code is: {rolledCode}");

                // Check which digits were found in this iteration
                bool[] newFoundDigits = CheckUnlockStatus(rolledDigits);

                // Update the foundDigits array and print messages for newly found digits
                for (int i = 0; i < foundDigits.Length; i++)
                {
                    if (!foundDigits[i] && newFoundDigits[i])
                    {
                        foundDigits[i] = true;
                        Console.WriteLine($"Click! the {FormatOrdinal(i + 1)} number was found!");
                    }
                }

                // Check if all digits are unlocked
                if (IsCodeUnlocked(foundDigits))
                {
                    Console.WriteLine("The code " + unlockCode + " lock is unlocked!");

                }
                else
                {
                    Console.WriteLine("The code remains locked.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }

    static async Task<int> RollDigitAsync(string digitName) // Asynchronous method to roll a digit
    {
        Console.WriteLine($"Rolling the {digitName} digit...");
        await Task.Delay(Random.Shared.Next(500, 1500)); // Simulate some delay
        int digit = Random.Shared.Next(1, 10); // Generate a random digit between 1 and 9
        Console.WriteLine($"The {digitName} digit rolled: {digit}");
        return digit;
    }

    static bool[] CheckUnlockStatus(int[] rolledDigits)
    {
        bool[] newFoundDigits = new bool[foundDigits.Length];
        for (int i = 0; i < foundDigits.Length; i++)
        {
            if (!foundDigits[i] && rolledDigits[i].ToString() == unlockCode[i].ToString())
            {
                newFoundDigits[i] = true;
            }
        }
        return newFoundDigits;
    }

    static bool IsCodeUnlocked(bool[] foundDigits)
    {
        foreach (bool found in foundDigits)
        {
            if (!found)
            {
                return false;
            }
        }
        return true;
    }

    // Method to format an integer as its ordinal representation
    private static string FormatOrdinal(int number)
    {
        int mod100 = number % 100;
        switch (mod100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
            default:
                switch (number % 10)
                {
                    case 1:
                        return number + "st";
                    case 2:
                        return number + "nd";
                    case 3:
                        return number + "rd";
                    default:
                        return number + "th";
                }
        }
    }
}