/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

static readonly Dictionary<string, int> ComponentDict = new Dictionary<string, int>
{
    {"BulletproofGlass", 10},
    {"Computer", 100},
    {"Construction", 150},
    {"Detector", 10},
    {"Display", 100},
    {"Girder", 100},
    {"GravityGenerator", 10},
    {"InteriorPlate", 150},
    {"LargeTube", 50},
    {"Medical", 5},
    {"MetalGrid", 150},
    {"Motor", 100},
    {"PowerCell", 25},
    {"RadioCommunication", 10},
    {"Reactor", 100},
    {"SmallTube", 100},
    {"SolarCell", 5},
    {"SteelPlate", 350},
    {"Superconductor", 25},
    {"Thrust", 100}
};

public void Main(string argument, UpdateType updateSource)
{
    string ERR_TXT = "";
    List<IMyTerminalBlock> l0 = new List<IMyTerminalBlock>();
    IMyCargoContainer v0 = null;
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(l0);
    if (l0.Count == 0)
    {
        ERR_TXT += "no Large Cargo Container blocks found\n";
    }
    else
    {
        for (int i = 0; i < l0.Count; i++)
        {
            if (l0[i].CustomName == "zc Base-Only XL Cargo Container 3 Components")
            {
                v0 = (IMyCargoContainer)l0[i];
                break;
            }
        }
        if (v0 == null)
        {
            ERR_TXT += "no Large Cargo Container block named zc Base-Only XL Cargo Container 3 Components found\n";
        }
    }

    List<IMyTerminalBlock> v1 = new List<IMyTerminalBlock>();
    if (GridTerminalSystem.GetBlockGroupWithName("LG lasers") != null)
    {
        GridTerminalSystem.GetBlockGroupWithName("LG lasers").GetBlocksOfType<IMyShipWelder>(v1);
        if (v1.Count == 0)
        {
            ERR_TXT += "group LG lasers has no Welder blocks\n";
        }
    }
    else
    {
        ERR_TXT += "group LG lasers not found\n";
    }

    if (ERR_TXT != "")
    {
        Echo("Script Errors:\n" + ERR_TXT + "(make sure block ownership is set correctly)");
        return;
    }
    else { Echo(""); }

    int currentLaserWelder = 0;

    Int32.TryParse(Me.CustomData, out currentLaserWelder);

    if (currentLaserWelder > (v1.Count - 1))
    {
        currentLaserWelder = 0;
        Me.CustomData = "0";
    }

    Me.CustomData = "" + (currentLaserWelder + 1);

    for (int i = ComponentDict.Count - 1; i >= 0; i--)
    {
        KeyValuePair<string, int> Component = ComponentDict.ElementAt(i);
        string ComponentKey = Component.Key;
        int ComponentValue = Component.Value;

        float currentItemAmount = countItem(v1[currentLaserWelder].GetInventory(0), "Component", ComponentKey);
        float itemAmountToAdd = ComponentValue - currentItemAmount;

        if (itemAmountToAdd > 0)
        {
            transfer(v0.GetInventory(0), v1[currentLaserWelder].GetInventory(0), "Component", ComponentKey, itemAmountToAdd);
        }
    }

    Echo(v1[currentLaserWelder].CustomName);
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

void transfer(IMyInventory a, IMyInventory b, string type, string sType, float amount)
{
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    a.GetItems(items);

    float left = amount;
    for (int i = items.Count - 1; i >= 0; i--)
    {
        if (left > 0 && items[i].Type.TypeId.ToString().EndsWith(type) && items[i].Type.SubtypeId.ToString() == sType)
        {
            if ((float)items[i].Amount > left)
            {
                a.TransferItemTo(b, i, null, true, (VRage.MyFixedPoint)amount);
                left = 0;
                break;
            }
            else
            {
                left -= (float)items[i].Amount;
                a.TransferItemTo(b, i, null, true, null);
            }
        }
    }
}