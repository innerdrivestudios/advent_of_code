//Solution for https://adventofcode.com/2020/day/1 (Ctrl+Click in VS to follow link)

// Your input: a bunch of numbers for an "expense report"
string myInput = "1782\r\n1344\r\n1974\r\n1874\r\n1800\r\n1973\r\n1416\r\n1952\r\n1982\r\n1506\r\n1642\r\n1514\r\n1978\r\n1895\r\n1747\r\n1564\r\n1398\r\n1683\r\n1886\r\n1492\r\n1629\r\n1433\r\n295\r\n1793\r\n1740\r\n1852\r\n1697\r\n1471\r\n1361\r\n1751\r\n1426\r\n2004\r\n1763\r\n1663\r\n1742\r\n1666\r\n1733\r\n1880\r\n1600\r\n1723\r\n1478\r\n1912\r\n1820\r\n1615\r\n1875\r\n1547\r\n1554\r\n752\r\n1905\r\n1368\r\n954\r\n1425\r\n1391\r\n691\r\n1835\r\n744\r\n1850\r\n1713\r\n1995\r\n1926\r\n1817\r\n1774\r\n1986\r\n2010\r\n1427\r\n1609\r\n1927\r\n1362\r\n1420\r\n1722\r\n1590\r\n1925\r\n1617\r\n1434\r\n1826\r\n1636\r\n1687\r\n1946\r\n704\r\n1797\r\n1517\r\n1801\r\n1865\r\n1963\r\n1828\r\n1829\r\n1955\r\n1832\r\n1987\r\n1585\r\n1646\r\n1575\r\n1351\r\n1345\r\n1729\r\n1933\r\n1918\r\n1902\r\n1490\r\n1627\r\n1370\r\n1650\r\n1340\r\n1539\r\n1588\r\n1715\r\n1573\r\n1384\r\n1403\r\n1673\r\n1750\r\n1578\r\n1831\r\n1849\r\n1719\r\n1359\r\n2008\r\n1837\r\n1958\r\n480\r\n1388\r\n1770\r\n1999\r\n1066\r\n1730\r\n1541\r\n1802\r\n1962\r\n1891\r\n1816\r\n1505\r\n1665\r\n1551\r\n1954\r\n1378\r\n1998\r\n1612\r\n1544\r\n1953\r\n1502\r\n1888\r\n1655\r\n1614\r\n1903\r\n1675\r\n1498\r\n1653\r\n1769\r\n1863\r\n1607\r\n1945\r\n1651\r\n1558\r\n1777\r\n1460\r\n1711\r\n1677\r\n1988\r\n1441\r\n1821\r\n1867\r\n1656\r\n1731\r\n1885\r\n1482\r\n1439\r\n1990\r\n1809\r\n1794\r\n1951\r\n1858\r\n1969\r\n509\r\n1486\r\n1971\r\n1557\r\n1896\r\n1884\r\n1834\r\n1814\r\n1216\r\n1997\r\n1966\r\n1808\r\n1754\r\n1804\r\n1684\r\n2001\r\n1699\r\n1781\r\n1429\r\n1322\r\n1603\r\n1596\r\n1823\r\n1700\r\n1552\r\n1352\r\n1621\r\n1669\r\n";

long[] numbers = myInput
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
	.Select(long.Parse)
	.ToArray();

// Part 1 - Find the two numbers that add up to 2020 and return their product

long GetExpenseChecksumFor2(long[] pNumbers)
{

	for (int i = 0; i < pNumbers.Length - 1; i++)
	{
		long entryA = pNumbers[i];

		if (entryA >= 2020) continue; //prune for speed
		
		for (int j = i + 1; j < pNumbers.Length; j++)
		{
			long entryB = pNumbers[j];

            if (entryA + entryB == 2020)
			{
				return entryA * entryB;
			}
		}
	}

	return -1;
}

Console.WriteLine("Part 1 - Expense report checksum for 2 numbers: " + GetExpenseChecksumFor2(numbers));

// Part 2 - Find the three numbers that add up to 2020 and return their product

long GetExpenseChecksumFor3(long[] pNumbers)
{

	for (int i = 0; i < pNumbers.Length - 2; i++)
	{
		long entryA = pNumbers[i];

		if (entryA >= 2020) continue; //prune for speed

		for (int j = i + 1; j < pNumbers.Length - 1; j++)
		{
			long entryB = pNumbers[j];

			if (entryA + entryB >= 2020) continue; //prune for speed

			for (int k = j + 1; k < pNumbers.Length; k++)
			{
				long entryC = pNumbers[k];

				if (entryA + entryB + entryC == 2020)
				{
					return entryA * entryB * entryC;
				}

			}
		}
	}

	return -1;
}

Console.WriteLine("Part 2 - Expense report checksum for 3 numbers: " + GetExpenseChecksumFor3(numbers));


Console.ReadKey();

