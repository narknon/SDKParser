using SDKParser;

string[] array = File.ReadAllLines(args[0]);

Console.WriteLine("Is this file for classes or for structs? C/S");
bool isClasses = Console.ReadLine().ToUpper() == "C" ? true : false;

// Project name
Console.WriteLine("What do you want your project to be called?");
var name = Console.ReadLine();
var nameFormated = name?.ToUpper().Trim().Replace(" ", "").Replace("_API", "") + "_API";

Dictionary<int, int> dictionary = new Dictionary<int, int>();

if (isClasses)
{
	// Dictionary for start and end offsets (classes)
	int key = 0;
	for (int i = 0; i < array.Length; i++)
	{
		string text = array[i];
		if (text.StartsWith("// Class"))
		{
			Console.WriteLine("Found start index at " + i);
			key = i;
		}
		else if (text.StartsWith("};"))
		{
			Console.WriteLine("Found end index at " + i);
			dictionary.Add(key, i);
		}
	}
}
else
{
	// Dictionary for start and end offsets (structs)
	int key = 0;
	for (int i = 0; i < array.Length; i++)
	{
		string text = array[i];
		if (text.StartsWith("// Enum") || text.StartsWith("// ScriptStruct"))
		{
			Console.WriteLine("Found start index at " + i);
			key = i;
		}
		else if (text.StartsWith("};"))
		{
			Console.WriteLine("Found end index at " + i);
			try
			{
				dictionary.Add(key, i);
			}
			catch { }
		}
	}
}

// Create directories
Directory.CreateDirectory($"Classes\\{name}\\Public\\");
Directory.CreateDirectory($"Classes\\{name}\\Private\\");


// Parse/add individual data
foreach (KeyValuePair<int, int> item in dictionary)
{
	string[] array2 = new string[item.Value - item.Key];
	List<string> list = new List<string>();
	List<string> cppList = new List<string>();

	Array.Copy(array, item.Key, array2, 0, item.Value - item.Key);

	bool isStruct = array2[2].StartsWith("struct");
	int index = isClasses ? 2 : isStruct ? 2 : 1;
	// Top .cpp file
	cppList.Add($"#include \"{array2[index].Split(' ')[1].Split(' ')[0][1..] + ".h"}\"");

	// Top .h file
	list.Add("#pragma once");
	list.Add(string.Empty);
	list.Add("#include \"CoreMinimal.h\"");
	list.Add(string.Empty);
	list.Add(isClasses ? "UCLASS(Blueprintable)" : isStruct ? "USTRUCT(BlueprintType)" : "UENUM(BlueprintType)");
	if (isClasses) // Checks if its class
	{
		list.Add(array2[2].Replace("struct", "class").Replace(" : ", " : public ").Replace("{", ""));
		list.Add("{");
		list.Add(string.Empty);
		list.Add("\tGENERATED_BODY()");
		list.Add(string.Empty);
		list.Add("public:");
	}
	else if (isStruct) // Checks if its struct
	{
		list.Add(array2[2].Replace(" : ", " : public ").Replace("{", ""));
		list.Add("{");
		list.Add(string.Empty);
		list.Add("\tGENERATED_BODY()");
		list.Add(string.Empty);
		list.Add("public:");
	}
	else // Anything else equals enum
	{	
		list.Add(array2[1].Replace("{", ""));
		list.Add("{");
	}

	if (index == 2) list.Add(string.Empty);

	// Parse and add data
	for (int j = 3; j < array2.Length; j++)
	{
		if (!string.IsNullOrWhiteSpace(array2[j]))
		{
			if (!array2[j].Contains("pad_"))
			{
				if (array2[j].Contains("// Function"))
                {
					list.Add("\tUFUNCTION(Blueprintable, BlueprintCallable)");
					cppList.Add(string.Empty);
					cppList.Add($"{array2[j].Trim().Split(' ')[0]} {array2[index].Split(' ')[1]}::{array2[j].Replace("struct ", string.Empty).Replace("enum class ", string.Empty).Trim().SubstringAfter(" ").SubstringBefore(";")}");
					cppList.Add("{");
					cppList.Add("\treturn;");
					cppList.Add("};");
				}
				else if (index == 2) list.Add("\tUPROPERTY(EditAnywhere, BlueprintReadWrite, meta=(AllowPrivateAccess=true))");
				if (array2[j].Contains(": 1"))
					list.Add("\t" + array2[j].Replace("char", "uint8").SubstringBefore("//").TrimEnd());
				else list.Add((index == 2 ? "\t" : string.Empty) + array2[j].Replace("struct ", string.Empty).Replace("32_t", string.Empty).Replace("enum class ", string.Empty).SubstringBefore("//"));
				if (index == 2) list.Add(string.Empty);
			}
		}
	}
	
	// End of .h file
	list.Add("};");

	// Write all files
	var substringer = isClasses ? 1.. : 0..;
	int i = index == 2 ? 1 : 2;
	File.WriteAllLines($"Classes\\{name}\\Public\\" + array2[index].Split(' ')[i].Split(' ')[0][substringer] + ".h", list);
	File.WriteAllLines($"Classes\\{name}\\Private\\" + array2[index].Split(' ')[i].Split(' ')[0][substringer] + ".cpp", cppList);
}