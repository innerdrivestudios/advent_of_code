// Solution for https://adventofcode.com/2019/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a long list of pixel data 

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("").Trim();

// ** Part 1: Figure out how many screens there are, find the screen with the fewest amount of 0 digits
// then multiply the amount of 1 and 2 digits...

// Note that none of the requirements actually require us to treat the input as integers

int pixelCount = myInput.Length;
int screenWidth = 25;	
int screenHeight = 6;
int pixelCountPerScreen = screenWidth * screenHeight;
int screenCount = pixelCount / pixelCountPerScreen;

string[] pixelDataPerScreen = new string[screenCount];
int zeroPixelCountMin = int.MaxValue;
int screenWithLeastZeroPixels = -1;

for (int i = 0; i < screenCount; i++)
{
	// This could definitely be more optimized
	pixelDataPerScreen[i] = myInput.Substring(i * pixelCountPerScreen, pixelCountPerScreen);

	int zeroPixelCount = pixelDataPerScreen[i].Count(x => x == '0');
	if (zeroPixelCount < zeroPixelCountMin)
	{
		zeroPixelCountMin = zeroPixelCount;
		screenWithLeastZeroPixels = i;
	}
}

// This could also be more optimized ;)
int part1Answer =
	pixelDataPerScreen[screenWithLeastZeroPixels].Count(x => x == '1') *
	pixelDataPerScreen[screenWithLeastZeroPixels].Count(x => x == '2');
 
Console.WriteLine("Part 1:" + part1Answer);

// ** Part 2: Let's decode the image...

Console.WriteLine();
Console.WriteLine("Part 2:\n");

Grid<char> screen = new Grid<char>(screenWidth, screenHeight);

// now for every pixel...
for (int pixelIndex = 0; pixelIndex < pixelCountPerScreen; pixelIndex++)
{
	int x = pixelIndex % screenWidth;
	int y = pixelIndex / screenWidth;

	//for every layer 
	for (int screenIndex = 0 ; screenIndex < screenCount; screenIndex++)
	{
		char pixelData = pixelDataPerScreen[screenIndex][pixelIndex];
		if (pixelData == '2') continue;
		screen[x, y] = pixelData == '1' ? 'O' : ' ';
		break;
	}
}

screen.Print();