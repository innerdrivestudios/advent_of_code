record Resource (string name, ulong amount);
record ProductionRule (List<Resource> input, Resource output);