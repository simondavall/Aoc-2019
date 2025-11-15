# Aoc-2019
Advent of Code 2019 written using C# (.Net 10.0)

### To run all 25 days: ###
1. Install .Net 10.0 sdk/runtime.
2. Navigate to the project root (where you will find aoc.sh)
3. Execute aoc.sh 
```bash
./aoc.sh
```
This should build all days' projects in Rerlease mode, and execute them.
The results will be displayed in the terminal window, with executiojn timings for each.

### To run an individual day: ###
1. Install .Net 10.0 sdk/runtime.
2. Navigate to the day (e.g. /Day01)
3. Build the project with: 
```bash
dotnet build -c Release
```
4. The execute with: 
```bash
./bin/Release/net10.0/Day01 <filename>
```
Replace \<filename\> with either sample.txt or input.txt
