# PieToss

`PieToss` is a `.NET 10` console app that estimates `π` using repeated fair coin-toss sequences.

For each sequence, tosses continue until heads becomes a strict majority (`2 * heads > tosses`) or an optional cutoff (`giveup`) is reached. The program accumulates the ratio `heads / tosses` at each completed sequence and uses:

`piEstimate = 4 * average(heads / tosses)`

It records when the estimate first matches `π` for decimal places `2` through `8` (with truncation-style rounding via `MidpointRounding.ToZero`), and writes results/logs to CSV files on the desktop.

## Features

- Monte Carlo-style `π` estimation via coin toss simulation.
- Tracks first match for each decimal place `2..8` per result set.
- Writes match results to `pi-results.csv`.
- Writes periodic progress data to `pi-log.csv`.
- Supports multiple parallel instances with result set offsets.
- Optional debug output when Caps Lock is on.

## Requirements

- .NET SDK `10`
- Windows desktop environment (current output path targets the Desktop folder)

## Run

From the project folder:

`dotnet run -- [options]`

### Command-line options

- `--giveup=<long>`
  - Maximum tosses allowed in a sequence before discarding it.
  - `0` disables discard-by-limit.
  - Default: `0`

- `--logFreq=<int>`
  - Log progress every N completed sequences.
  - `0` disables periodic progress logging.
  - Default: `1000`

- `--sets=<int>`
  - Number of result sets to generate.
  - Default: `10`

### Example

`dotnet run -- --giveup=1000000000 --logFreq=500 --sets=25`

## Output files

Files are written to the user desktop:

- `pi-results.csv`
  - Columns: `Result Set,DP,Estimate,Sequences,Tosses,MaxStreak`

- `pi-log.csv`
  - Columns: `Result Set,Sequence,Estimate,Log,Tosses,MaxStreak`

## Notes

- `Random` is used for speed rather than cryptographic randomness.
- Very high decimal-place matching can take a long time.
- Heavy console logging impacts performance significantly.

## Project structure

- `Program.cs` — simulation engine, matching logic, and CSV output.

## License

No license is currently specified in this repository.
