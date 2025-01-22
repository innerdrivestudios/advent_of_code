// Solution for https://adventofcode.com/2017/day/2 (Ctrl+Click in VS to follow link)

// Your input: lines with sequences of numbers, e.g. like this:

// 5 1 9 5
// 7 5 3
// 2 4 6 8

// But then more :) :
string myInput = "414\t382\t1515\t319\t83\t1327\t116\t391\t101\t749\t1388\t1046\t1427\t105\t1341\t1590\r\n960\t930\t192\t147\t932\t621\t1139\t198\t865\t820\t597\t165\t232\t417\t19\t183\r\n3379\t987\t190\t3844\t1245\t1503\t3151\t3349\t2844\t4033\t175\t3625\t3565\t179\t3938\t184\r\n116\t51\t32\t155\t102\t92\t65\t42\t48\t91\t74\t69\t52\t89\t20\t143\r\n221\t781\t819\t121\t821\t839\t95\t117\t626\t127\t559\t803\t779\t543\t44\t369\r\n199\t2556\t93\t1101\t122\t124\t2714\t625\t2432\t1839\t2700\t2636\t118\t2306\t1616\t2799\r\n56\t804\t52\t881\t1409\t47\t246\t1368\t1371\t583\t49\t1352\t976\t400\t1276\t1240\r\n1189\t73\t148\t1089\t93\t76\t3205\t3440\t3627\t92\t853\t95\t3314\t3551\t2929\t3626\r\n702\t169\t492\t167\t712\t488\t357\t414\t187\t278\t87\t150\t19\t818\t178\t686\r\n140\t2220\t1961\t1014\t2204\t2173\t1513\t2225\t443\t123\t148\t580\t833\t1473\t137\t245\r\n662\t213\t1234\t199\t1353\t1358\t1408\t235\t917\t1395\t1347\t194\t565\t179\t768\t650\r\n119\t137\t1908\t1324\t1085\t661\t1557\t1661\t1828\t1865\t432\t110\t658\t821\t1740\t145\r\n1594\t222\t4140\t963\t209\t2782\t180\t2591\t4390\t244\t4328\t3748\t4535\t192\t157\t3817\r\n334\t275\t395\t128\t347\t118\t353\t281\t430\t156\t312\t386\t160\t194\t63\t141\r\n146\t1116\t153\t815\t2212\t2070\t599\t3018\t2640\t47\t125\t2292\t165\t2348\t2694\t184\r\n1704\t2194\t1753\t146\t2063\t1668\t1280\t615\t163\t190\t2269\t1856\t150\t158\t2250\t2459\r\n";

// Who said LINQ is unreadable??
int[][] numbers = myInput
    //Get all the separate strings (with numbers separated by tabs) by splitting on \r\n
    .Split ("\r\n", StringSplitOptions.RemoveEmptyEntries)	
	//Split each of those lines on a \t char and convert the results to an array of integers
	.Select (x => x.Split ("\t", StringSplitOptions.RemoveEmptyEntries).Select (int.Parse).ToArray())
	//Convert all those integers arrays to an array
	.ToArray ();

// Your task - part 1: calculate the sum of the deltas between the min and max of each line and add all of them

Console.WriteLine("Part 1 - Checksum: " + numbers.Select (x => x.Max() - x.Min()).Sum());
Console.WriteLine ("Part 2 - Alternative checksum: " + GetAlternativeChecksum(numbers));
Console.ReadKey();

int GetAlternativeChecksum(int[][] pNumbers)
{
    int total = 0;

    //get a line with numbers
    foreach (int[] numberList in pNumbers)
    {
        //compare all the numbers in the list back and forth
        for (int j = 0; j < numberList.Length; j++)
        {
            for (int k = 0; k < numberList.Length; k++)
            {
                if (j == k) continue;

                //use int rounding to figure out if numbers divide evenly, 
                //the next calculations will only work if there are no digits after the comma
                int factor = numberList[j] / numberList[k];
                if (factor * numberList[k] == numberList[j]) total += factor;
            }
        }
    }

    return total;
}




