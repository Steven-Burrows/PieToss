using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Diagnostics;

namespace PiToss;

class Program
{
    static void Main()
    {
        //
        // Estimates pi by repeatedly simulating fair coin toss sequences and
        // averaging the heads-to-tosses ratio at the first point where heads becomes the majority.
        // 
        long giveup = (long)10; // Discard any streaks this long - Looks like a big number ? Bigger it is the more accurate the results


        long seqCounter = 0;
        decimal seqSum = 0;
        long maxStreak = 0;
        long totalTosses = 0;
        long totalTossesAll = 0;
        long resultSet = 0;

        long heads = 0;
        long tosses = 0;
        long toss = 0;

        // Create a list to store 1st Match for each decimal place from 2 to 8
        List<MatchResult> results = new List<MatchResult>();

        //Count the number of instances of the program running
        var processName = Process.GetCurrentProcess().ProcessName;
        var instanceCount = Process.GetProcessesByName(processName).Length;
        var resultSetStart = 10 * (instanceCount - 1);


        DateTime startTime = DateTime.Now;
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var outputPath = Path.Combine(desktopPath, $"pi-results.csv");
        if (!File.Exists(outputPath))
        {
            File.AppendAllText(outputPath, "Result Set,DP,Estimate,Sequences,Tosses,MaxStreak" + Environment.NewLine);
        }

        var logPath = Path.Combine(desktopPath, $"pi-log.csv");
        if (!File.Exists(logPath))
        {
            File.AppendAllText(logPath, "Result Set,Sequence,Estimate,Log,Tosses,MaxStreak" + Environment.NewLine);
        }
        var logFreq = 1000;


        Random RG = new Random();

        while (resultSet < 10)
        {
            resultSet++;
            var resultDisplay = resultSetStart + resultSet;
            // Initialize results for each decimal place from 2 to 8
            results = new List<MatchResult>();
            for (int dp = 2; dp <= 8; dp++)
            {
                results.Add(new MatchResult(dp));
            }

            seqCounter = 0;
            seqSum = 0;
            maxStreak = 0;
            totalTosses = 0;

            heads = 0;
            tosses = 0;
            toss = 0;

            // loop while there is not a matched result
            while (results.Any(r => !r.Done))
            {
                //toss = RandomNumberGenerator.GetInt32(2);
                toss = RG.Next(2);
                tosses++;
                if (toss == 0)
                {
                    heads++;

                    // Sequence ends when heads is more than half of tosses or giveup is reached
                    if ((2 * heads > tosses) || tosses >= giveup)
                    {
                        totalTosses += tosses;
                        totalTossesAll += tosses;
                        if (tosses < giveup)
                        {
                            seqCounter++;
                            seqSum += (decimal)heads / tosses;
                            maxStreak = Math.Max(maxStreak, tosses);
                            var estimate = 4 * seqSum / seqCounter;

                            // Check for new matches for each decimal place and update results
                            foreach (var result in results.Where(r => !r.Done && r.TryMatch(estimate)))
                            {
                                result.Sequences = seqCounter;
                                result.Tosses = totalTosses;
                                result.MaxStreak = maxStreak;
                                result.PiEstimate = estimate;

                                Console.WriteLine($"\nResult {resultDisplay} - DP {result.DecimalPlaces} - Sequences {seqCounter:N0} - Max Streak {maxStreak:N0} - Total Coin Tosses {totalTosses:N0}\n");
                                File.AppendAllText(outputPath, $"{resultDisplay},{result.DecimalPlaces},{estimate},{seqCounter},{totalTosses},{maxStreak}" + Environment.NewLine);
                                //Console.Beep(1000, 50);
                            }

                            if (logFreq > 0 && seqCounter % logFreq == 0)
                            {
                                Console.WriteLine($"Results {resultDisplay} - Sequences {seqCounter:N0}  pi {(estimate):N10} ({maxStreak:N0}) TPS {Math.Log10(totalTossesAll / ((DateTime.Now - startTime).TotalSeconds)):N1}");
                                File.AppendAllText(logPath, $"{resultDisplay},{seqCounter},{estimate},{Math.Log((double)estimate, Math.PI)},{totalTosses},{maxStreak}" + Environment.NewLine);
                            }
                        }
                        else
                        {
                            // Discard the sequence
                        }

                        heads = 0;
                        tosses = 0;
                    }
                }
            }
        }
        Console.WriteLine($"\nTotal Coin Tosses {totalTossesAll:N0} in {(DateTime.Now - startTime).TotalSeconds:N1} seconds\n");
        //Console.Beep(2000, 200);
    }
}

class MatchResult
{
    public int DecimalPlaces { get; set; }
    public decimal Target { get; set; }
    public long Sequences { get; set; }
    public long Tosses { get; set; }
    public long MaxStreak { get; set; }
    public decimal PiEstimate { get; set; }

    public MatchResult(int decimalPlaces)
    {
        DecimalPlaces = decimalPlaces;
        Target = Math.Round(3.1415926535897932384M, decimalPlaces, MidpointRounding.ToZero);
        Sequences = 0;
        Tosses = 0;
        MaxStreak = 0;
    }

    public bool Done
    {
        get
        {
            return Sequences != 0;
        }
    }

    public bool TryMatch(decimal piEstimate)
    {
        if (Math.Round(piEstimate, DecimalPlaces, MidpointRounding.ToZero) == Target)
        {
            return true;
        }
        return false;
    }
}