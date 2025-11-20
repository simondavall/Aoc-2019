INPUTS=input.txt

.PHONY: test

test: Program.cs $(INPUTS)
	dotnet build -c Release && ./bin/Release/net10.0/Day13 $(INPUTS)
