 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SettingsFunction : MenuFunction
{
    public virtual void Settings_Updated()
    {
        // Nothing required here.
    }

    public override void Function_Loaded()
    {
        base.Function_Loaded();

        // This will make sure our settings are loaded.

        GameSettingsManager.settings_update += Settings_Updated;
        Settings_Updated(); // In case our settings manager loads first, we should reload all values
    }
}
