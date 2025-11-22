using System;
using System.Globalization;
using System.Threading;

// I am very sorry for using AI, but I am horible C# programmer

namespace BetterCalc
{
    enum Mode
    {
        Quit = 0,
        Add = 1,
        Subtract,
        Multiply,
        Divide,
        SquareArea,
        RectangleArea,
        TriangleArea,
        TrapezoidArea,
        CircleArea,
        CircleCircumference,
        Density,
        Speed,
        Force,
        Pressure,
        Work,
        KineticEnergy,
        PotentialEnergy,
        PercenageConcentration,
        MolareConcentration,
        MolareMass,
        MolareVolume,
        KilometersToMiles,
        MetersToFeet,
        MilesToKilometers,
        FeetToMeters,
        FarenheitToCelsius,
        CelsiusToFarenheit,
        RandomNum,
        Credits
    }

    static class Messages
    {
        public const string Version = "2.2.0";
        public const string Author = "Wilq1PL";

        public static string[] MenuItems =
        {
            "Wyjście",
            "Dodawanie",
            "Odejmowanie",
            "Mnożenie",
            "Dzielenie",
            "Pole kwadratu",
            "Pole prostokąta",
            "Pole trójkąta",
            "Pole trapezu",
            "Pole koła",
            "Obwód koła",
            "Gęstość",
            "Prędkość",
            "Siła",
            "Ciśnienie",
            "Praca",
            "Energia kinetyczna",
            "Energia potencjalna",
            "Stężenie procentowe",
            "Stężenie molowe",
            "Masa molowa",
            "Objętość molowa",
            "Kilometry na mile",
            "Metry na stopy",
            "Mile na kilometry",
            "Stopy na metry",
            "Farenheity na Celsjusze",
            "Celsjusze na Farenheity",
            "Losowa liczba (tylko pełne liczby)",
            "Podziękowania"
        };

        public static string PromptChoice(int max) => $"\nWybierz tryb (0-{max}):";
        public const string InvalidChoice = "Nieprawidłowy wybór.";
        public const string EmptyInput = "Wejście nie może być puste. Spróbuj ponownie.";
        public const string InvalidNumber = "Nieprawidłowe dane: oczekiwana liczba lub ułamek. Spróbuj ponownie.";
        public const string InvalidFractionFormat = "Nieprawidłowy format ułamka. Użyj a/b lub liczba i ułamek mieszany '1 3/4'.";
        public const string DenominatorZero = "Błąd: mianownik nie może być zero.";
        public const string MustBePositive = "Wartość musi być większa od zera. Spróbuj ponownie.";
    }

    class Program
    {
        // Stałe i ustawienia
        const double Pi = Math.PI;
        const double G = 9.81; // przyspieszenie ziemskie
        const int ResultPrecision = 6; // liczba cyfr znaczących przy wyświetlaniu wyników

        // Wspólny generator losowych liczb (unikamy wielokrotnego seedowania)
        static readonly Random Rand = new Random();

        static void Main(string[] args)
        {
            try
            {
                RunSession();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Wystąpił nieoczekiwany błąd: " + ex.Message);
            }
        }

        static void RunSession()
        {
            Console.WriteLine($"Better-Calc v{Messages.Version} by {Messages.Author}");

            while (true)
            {
                DisplayMenu();
                int max = Messages.MenuItems.Length - 1;
                Console.WriteLine(Messages.PromptChoice(max));
                string choiceRaw = Console.ReadLine()?.Trim() ?? "";

                if (!int.TryParse(choiceRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int choiceNum)
                    || choiceNum < 0 || choiceNum > max)
                {
                    Console.WriteLine(Messages.InvalidChoice);
                    continue;
                }

                if (choiceNum == (int)Mode.Quit)
                {
                    Console.WriteLine("Koniec programu.");
                    return;
                }

                Mode mode = (Mode)choiceNum;
                try
                {
                    double result = HandleMode(mode);
                    // jeśli handler zwróci NaN, to oznacza, że już wypisał komunikat (np. błąd)
                    if (!double.IsNaN(result))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Wynik: " + FormatResult(result));
                    }
                }
                catch (OperationCanceledException)
                {
                    // Handler zgłosił rezygnację - powrót do menu
                    Console.WriteLine();
                    Console.WriteLine("Anulowano. Powrót do menu.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Błąd wykonania: " + ex.Message);
                }

                Console.WriteLine(); // odstęp przed kolejnym przebiegiem
                Thread.Sleep(2000); // Bez pauzy wynik od razu przesuwa się w górę przez co trudno go zauważyć 
            }
        }

        static void DisplayMenu()
        {
            for (int i = 0; i < Messages.MenuItems.Length; i++)
            {
                Console.WriteLine($"{i}. {Messages.MenuItems[i]}");
            }
        }

        static double HandleMode(Mode mode)
        {
            switch (mode)
            {
                // Podstawowe
                case Mode.Add:
                case Mode.Subtract:
                case Mode.Multiply:
                case Mode.Divide:
                    return HandleArithmetic(mode);
                // Geometryczne
                case Mode.SquareArea:
                    {
                        double side = ReadPositiveNumber("Podaj długość boku kwadrat:");
                        return side * side;
                    }
                case Mode.RectangleArea:
                    {
                        double w = ReadPositiveNumber("Podaj szerokość prostokąta:");
                        double h = ReadPositiveNumber("Podaj wysokość prostokąta:");
                        return w * h;
                    }
                case Mode.TriangleArea:
                    {
                        double b = ReadPositiveNumber("Podaj długość podstawy trójkąta:");
                        double h = ReadPositiveNumber("Podaj wysokość trójkąta  :");
                        return 0.5 * b * h;
                    }
                case Mode.TrapezoidArea:
                    {
                        double a = ReadPositiveNumber("Podaj długość pierwszej podstawy trapezu:");
                        double b = ReadPositiveNumber("Podaj długość drugiej podstawy trapezu:");
                        double h = ReadPositiveNumber("Podaj wysokość trapezu:");
                        return 0.5 * (a + b) * h;
                    }
                case Mode.CircleArea:
                    {
                        double r = ReadPositiveNumber("Podaj promień koła:");
                        return Math.PI * r * r;
                    }
                case Mode.CircleCircumference:
                    {
                        double r = ReadPositiveNumber("Podaj promień koła:");
                        return 2 * Math.PI * r;
                    }

                // Fizykochemiczne
                case Mode.Density:
                    {
                        double mass = ReadPositiveNumber("Podaj masę (kg):");
                        double volume = ReadPositiveNumber("Podaj objętość (m³):");
                        return mass / volume;
                    }
                case Mode.Speed:
                    {
                        double distance = ReadPositiveNumber("Podaj odległość (m):");
                        double time = ReadPositiveNumber("Podaj czas (s):", allowZero: true);
                        if (time == 0) throw new InvalidOperationException("Czas nie może wynosić zero dla obliczenia prędkości.");
                        return distance / time;
                    }
                case Mode.Force:
                    {
                        double mass = ReadPositiveNumber("Podaj masę (kg):");
                        double accel = ReadNumber("Podaj przyspieszenie (m/s²):");
                        return mass * accel;
                    }
                case Mode.Pressure:
                    {
                        double force = ReadNumber("Podaj siłę (N):");
                        double area = ReadPositiveNumber("Podaj powierzchnię (m²):");
                        return force / area;
                    }
                case Mode.Work:
                    {
                        double force = ReadNumber("Podaj siłę (N):");
                        double distance = ReadNumber("Podaj odległość (m):");
                        return force * distance;
                    }
                case Mode.KineticEnergy:
                    {
                        double mass = ReadPositiveNumber("Podaj masę (kg):");
                        double v = ReadNumber("Podaj prędkość (m/s):");
                        return 0.5 * mass * v * v;
                    }
                case Mode.PotentialEnergy:
                    {
                        double mass = ReadPositiveNumber("Podaj masę (kg):");
                        double height = ReadNumber("Podaj wysokość (m):");
                        return mass * G * height;
                    }
                case Mode.PercenageConcentration:
                    {
                        double solute = ReadPositiveNumber("Podaj masę substancji rozpuszczonej (g):");
                        double solution = ReadPositiveNumber("Podaj masę roztworu (g):");
                        if (solution < solute) throw new InvalidOperationException("Masa roztworu nie może być mniejsza od masy substancji rozpuszczonej.");
                        return (solute / solution) * 100.0;
                    }
                case Mode.MolareConcentration:
                    {
                        double moles = ReadPositiveNumber("Podaj liczbę moli substancji (mol):");
                        double volume = ReadPositiveNumber("Podaj objętość roztworu (L):");
                        return moles / volume;
                    }
                case Mode.MolareMass:
                    {
                        double mass = ReadPositiveNumber("Podaj masę substancji (g):");
                        double moles = ReadPositiveNumber("Podaj liczbę moli substancji (mol):");
                        return mass / moles;
                    }
                case Mode.MolareVolume:
                    {
                        double volume = ReadPositiveNumber("Podaj objętość gazu (L):");
                        double moles = ReadPositiveNumber("Podaj liczbę moli gazu (mol):");
                        return volume / moles;
                    }

                // Konwersje jednostek
                case Mode.KilometersToMiles:
                    {
                        double km = ReadPositiveNumber("Podaj odległość w kilometrach:");
                        return km * 0.621371;
                    }
                case Mode.MetersToFeet:
                    {
                        double meters = ReadPositiveNumber("Podaj odległość w metrach:");
                        return meters * 3.28084;
                    }
                case Mode.MilesToKilometers:
                    {
                        double miles = ReadPositiveNumber("Podaj odległość w milach:");
                        return miles / 0.621371;
                    }
                case Mode.FeetToMeters:
                    {
                        double feet = ReadPositiveNumber("Podaj odległość w stopach:");
                        return feet / 3.28084;
                    }
                case Mode.FarenheitToCelsius:
                    {
                        double f = ReadNumber("Podaj temperaturę w stopniach Farenheita:");
                        return (f - 32) * 5.0 / 9.0;
                    }
                case Mode.CelsiusToFarenheit:
                    {
                        double c = ReadNumber("Podaj temperaturę w stopniach Celsjusza:");
                        return (c * 9.0 / 5.0) + 32;
                    }
                // Inne
                case Mode.RandomNum:
                    {
                        double min = ReadNumber("Podaj dolną granicę zakresu:");
                        double max = ReadNumber("Podaj górną granicę zakresu:");
                        if (max < min) throw new InvalidOperationException("Górna granica nie może być mniejsza od dolnej.");

                        // convert to integer bounds
                        int minInt = (int)Math.Ceiling(min);
                        int maxInt = (int)Math.Floor(max);

                        if (maxInt < minInt)
                        {
                            throw new InvalidOperationException("Po zaokrągleniu do liczb całkowitych nie ma dostępnych wartości w podanym przedziale.");
                        }

                        int r = Rand.Next(minInt, maxInt + 1); // max is exclusive, so +1 to include maxInt
                        return (double)r;
                    }

                // Podziękowania
                case Mode.Credits:
                    {
                        Console.WriteLine(
                            $"Better-Calc v + {Messages.Version}" +
                            $"\nProgramowanie: {Messages.Author} z małą pomocą GitHub Copilot\n"
                            );
                        Console.WriteLine(
                            "Dziękuję za korzystanie z Better-Calc! Mam nadzieję, że już nigdy nie użyjesz swojego zwykłego kalkulatora.\n" +
                            "Jeśli masz sugestie lub chcesz zgłosić błąd, odwiedź repozytorium na GitHub."
                            );
                        Console.WriteLine("Jeszcze raz , dziękuję <3");
                        Console.ReadLine(); // Pauza, aby użytkownik mógł przeczytać podziękowania
                        return double.NaN;
                    }
                default:
                    Console.WriteLine(Messages.InvalidChoice);
                    return double.NaN;
            }
        }

        static double HandleArithmetic(Mode mode)
        {
            double a = ReadNumber("Podaj pierwszą liczbę (np. 3.5, 7/8 lub 1 3/4):");
            double b = ReadNumber("Podaj drugą liczbę (np. 2, 1/3 lub 0):");

            return mode switch
            {
                Mode.Add => a + b,
                Mode.Subtract => a - b,
                Mode.Multiply => a * b,
                Mode.Divide => b != 0 ? a / b : throw new InvalidOperationException("Nie można dzielić przez zero."),
                _ => double.NaN
            };
        }

        // Formatowanie wyniku z określoną precyzją (liczba cyfr znaczących)
        static string FormatResult(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value)) return value.ToString(CultureInfo.InvariantCulture);
            // "G{n}" używa n jako liczby cyfr znaczących
            return value.ToString($"G{ResultPrecision}", CultureInfo.InvariantCulture);
        }

        // Wczytuje liczbę; akceptuje:
        // - liczby całkowite/zmiennoprzecinkowe (separator '.' lub ',')
        // - ułamki w postaci "a/b"
        // - ułamki mieszane w postaci "I N/D" (np. "1 3/4")
        // Pętla powtarza się aż do poprawnego wprowadzenia.
        static double ReadNumber(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string? input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine(Messages.EmptyInput);
                    continue;
                }

                // Normalize whitespace
                input = System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");

                // Try mixed number: "I N/D" or "-I N/D"
                if (TryParseMixedNumber(input, out double mixed))
                {
                    return mixed;
                }

                // Try simple fraction "a/b"
                if (TryParseFraction(input, out double frac))
                {
                    return frac;
                }

                // Try decimal/integer with '.' or ',' as separator
                string normalized = input.Replace(',', '.');
                if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                {
                    return value;
                }

                Console.WriteLine(Messages.InvalidNumber);
            }
        }

        static bool TryParseFraction(string input, out double value)
        {
            value = 0;
            if (!input.Contains("/")) return false;

            string[] parts = input.Split('/');
            if (parts.Length != 2)
            {
                Console.WriteLine(Messages.InvalidFractionFormat);
                return false;
            }

            string nS = parts[0].Trim().Replace(',', '.');
            string dS = parts[1].Trim().Replace(',', '.');

            if (!double.TryParse(nS, NumberStyles.Float, CultureInfo.InvariantCulture, out double numerator) ||
                !double.TryParse(dS, NumberStyles.Float, CultureInfo.InvariantCulture, out double denominator))
            {
                Console.WriteLine(Messages.InvalidNumber);
                return false;
            }

            if (denominator == 0)
            {
                Console.WriteLine(Messages.DenominatorZero);
                return false;
            }

            value = numerator / denominator;
            return true;
        }

        static bool TryParseMixedNumber(string input, out double value)
        {
            value = 0;
            // pattern: optional sign, integer part, space, fraction part (a/b)
            // e.g. "-1 3/4" or "1 3/4"
            int spaceIndex = input.IndexOf(' ');
            if (spaceIndex <= 0) return false;

            string intPart = input.Substring(0, spaceIndex).Trim();
            string fracPart = input.Substring(spaceIndex + 1).Trim();

            // fractional part must contain '/'
            if (!fracPart.Contains("/")) return false;

            // parse integer part
            string intNormalized = intPart.Replace(',', '.');
            if (!double.TryParse(intNormalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double integer))
            {
                return false;
            }

            if (!TryParseFraction(fracPart, out double fracValue))
            {
                return false;
            }

            // preserve sign of integer part
            double sign = Math.Sign(integer);
            integer = Math.Abs(integer);

            value = sign * (integer + Math.Abs(fracValue));
            return true;
        }

        // Wczytuje liczbę większą od zera (używa ReadNumber)
        static double ReadPositiveNumber(string prompt, bool allowZero = false)
        {
            while (true)
            {
                double v = ReadNumber(prompt);
                if (allowZero)
                {
                    if (v < 0)
                    {
                        Console.WriteLine(Messages.MustBePositive);
                        continue;
                    }
                    return v;
                }
                else
                {
                    if (v <= 0)
                    {
                        Console.WriteLine(Messages.MustBePositive);
                        continue;
                    }
                    return v;
                }
            }
        }
    }
}
