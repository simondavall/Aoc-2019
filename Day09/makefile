INPUTS=sample.txt sample2.txt sample3.txt input.txt

.PHONY: test

test: Program.cs $(INPUTS)
	dotnet build -c Release && ./bin/Release/net10.0/Day09 $(INPUTS)
