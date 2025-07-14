DateTimeOffset now = DateTimeOffset.Now;
Console.WriteLine("Current time with is: " + now.ToString("hh:mm:ss tt zzz", System.Globalization.CultureInfo.InvariantCulture));
