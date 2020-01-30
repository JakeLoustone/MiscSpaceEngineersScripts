/*
 * Fuel Gauge v3
 * -----------
 * 
 * 1. Put the name of the LCD Panel or Cockpit you want in the const string below.
 * 2. If you used a cockpit set the TextSurfaceIndex to the screen you want. (0 = middle, 1 = left, 2 = right)
 * 3. Enjoy!
 */

private const string LCDPanelName = "LCD Fuel Gauge";
private const int TextSurfaceIndex = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

void Main(string argument)
{
    float FuelCount = 0f;
    string ERR_TXT = "";
    List<IMyTerminalBlock> BlocksOnGridList = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(BlocksOnGridList, filterThis);
    Dictionary<string, string> EngineDictionary = new Dictionary<string, string>();
    List<IMyTextSurface> TextSerfaceList = new List<IMyTextSurface>();

    if (BlocksOnGridList.Count == 0)
    {
        ERR_TXT += "grid has no blocks\n";
    }
    else
    {
        for (int i = 0; i < BlocksOnGridList.Count; i++)
        {
            if (BlocksOnGridList[i].HasInventory)
            {
                float tempFuel = countItem(BlocksOnGridList[i].GetInventory(0), "Ingot", "Fuel");

                string tempBlockName = BlocksOnGridList[i].CustomName;

                if (tempBlockName.ToLower().Contains("engine"))
                {
                    string tempFuelString;

                    if (((IMyFunctionalBlock)BlocksOnGridList[i]).Enabled)
                    {
                        tempFuelString = string.Empty + (int)tempFuel;
                    }
                    else
                    {
                        tempFuelString = "off";
                    }

                    EngineDictionary.Add(tempBlockName, tempFuelString);
                }

                FuelCount = FuelCount + tempFuel;
            }

            if (BlocksOnGridList[i].CustomName == LCDPanelName)
            {
                IMyTextSurface TextSurface = null;

                TextSurface = ((IMyTextSurfaceProvider)BlocksOnGridList[i]).GetSurface(TextSurfaceIndex);
                if (TextSurface == null) TextSurface = (IMyTextSurface)BlocksOnGridList[i];

                if (TextSurface != null) TextSerfaceList.Add(TextSurface);
            }
        }
    }

    if (ERR_TXT != "")
    {
        Echo("Script Errors:\n" + ERR_TXT + "(make sure block ownership is set correctly)");
        return;
    }

    string outputString = "Fuel:\n" + (int)FuelCount + "";

    foreach (KeyValuePair<string, string> entry in EngineDictionary)
    {
        outputString += "\n" + entry.Key + ": " + entry.Value;
    }

    foreach (IMyTextSurface tempTextSurface in TextSerfaceList)
    {
        tempTextSurface.ContentType = ContentType.TEXT_AND_IMAGE;
        tempTextSurface.WriteText(outputString, false);
    }

    IMyTextSurface mesurface0 = Me.GetSurface(0);
    mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
    mesurface0.WriteText(outputString, false);

    Echo(outputString);
}

bool filterThis(IMyTerminalBlock block)
{
    return block.CubeGrid == Me.CubeGrid;
}

float countItem(IMyInventory inv, string itemType, string itemSubType)
{
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    inv.GetItems(items, null);
    float total = 0.0f;
    for (int i = 0; i < items.Count; i++)
    {
        if (items[i].Type.TypeId.ToString().EndsWith(itemType) && items[i].Type.SubtypeId.ToString() == itemSubType)
        {
            total += (float)items[i].Amount;
        }
    }

    return total;
}
