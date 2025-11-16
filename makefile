.PHONY: test

test:
	cd Aoc dotnet build -c Release -o ./Aoc/bin/Release/net10.0 - && ./bin/Release/net10.0/Aoc
