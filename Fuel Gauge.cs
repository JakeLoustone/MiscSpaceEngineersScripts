/*
 * Fuel Gauge v2
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
                FuelCount = FuelCount + countItem(BlocksOnGridList[i].GetInventory(0), "Ingot", "Fuel");
            }
        }
    }

    IMyTextSurface TextSurface = null;

    for (int i = 0; i < BlocksOnGridList.Count; i++)
    {
        if (BlocksOnGridList[i].CustomName == LCDPanelName)
        {
            TextSurface = ((IMyTextSurfaceProvider)BlocksOnGridList[i]).GetSurface(TextSurfaceIndex);
            if (TextSurface == null) TextSurface = (IMyTextSurface)BlocksOnGridList[i];
            break;
        }

    }

    if (ERR_TXT != "")
    {
        Echo("Script Errors:\n" + ERR_TXT + "(make sure block ownership is set correctly)");
        return;
    }
    else { Echo(""); }

    if (TextSurface != null)
    {
        TextSurface.ContentType = ContentType.TEXT_AND_IMAGE;
        TextSurface.FontSize = 4.0f;
        TextSurface.Alignment = TextAlignment.CENTER;
        TextSurface.Font = "monospace";
        TextSurface.WriteText("Fuel:\n" + (int)FuelCount + "", false);
    }

    IMyTextSurface mesurface0 = Me.GetSurface(0);
    mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
    mesurface0.FontSize = 4.0f;
    mesurface0.Alignment = TextAlignment.CENTER;
    mesurface0.Font = "monospace";
    mesurface0.WriteText("Fuel:\n" + (int)FuelCount + "", false);
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
