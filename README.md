# WheelSelect

Pass a delimited list into the form for a "wheel like" selection. Meant to be used as a quick selection utility. I'm really not sure why I built this. I had an idea and had to run with it. Do with it what you will.

![wheel_select](https://github.com/fischgeek/WheelSelect/blob/readme-assets/WheelSelect/readme-assets/wheel_select2.png)

## Main Functionality

- Simply pass two (required) arguments (the delimiter and the list) into the exe and the interface will display in the center of the screen loaded with your data set.
  - ex: `WheelSelect.exe "," "option a,option b,option c"`
  - an optional third parameter can be an output file path
    - _this will override the OutputLocation setting in the App.config_.
  - ex: `WheelSelect.exe "," "option a,option b,option c" "c:\temp\wheelselect.txt"`
- Use the up/down arrow keys or the mouse wheel to scroll through the list, or type some letters as a keyword search.
- Push the Enter key when you have the desired option selected and the value will be saved to a file to do what you want with.

## Options

The App.config has settings that allow for some customization.

- **OutputLocation** The location the data is written to when the Enter key is pressed. Default is `c:\wheel_selection.txt`.
- **OutputMethod** Decides if the output should be appended or overwritten. Must be either `Append` or `Overwrite`.
- **ClearOutputFileOnStart** Decides if the output file should start empty. `true` or `false`.
- **ClearOutputFileOnEscape** Decides if the output file should be cleared on the Escape key press. `true` or `false`.
- **SyncWithWindowsTheme** Decides if the UI should follow your Windows Theme setting found in *Settings > Personalization > Colors > "Choose your default app mode"*
- **DefaultTheme** Decides which theme to use if you choose not to sync with the Windows Theme settings.

The following are color customizations and must be a valid value of the `System.Drawing.Color` struct.

- **LightModeWindowBackgroundColor** Sets the background color of the interface when in light mode.
- **LightModeSelectedTextColor** Sets the text color of the primary selected value when in light mode.
- **LightModeOffset1TextColor** Sets the text color of the value(s) 1 position above/below the primary selected value when in light mode.
- **LightModeOffset2TextColor** Sets the text color of the value(s) 2 positions above/below the primary selected value when in light mode.
- **LightModeOffset3TextColor** Sets the text color of the value(s) 3 position above/below the primary selected value when in light mode.

- **DarkModeWindowBackgroundColor** Sets the background color of the interface when in dark mode.
- **DarkModeSelectedTextColor** Sets the text color of the primary selected value when in dark mode.
- **ModeOffset1TextColor** Sets the text color of the value(s) 1 position above/below the primary selected value when in dark mode.
- **DarkModeOffset2TextColor** Sets the text color of the value(s) 2 positions above/below the primary selected value when in dark mode.
- **DarkModeOffset3TextColor** Sets the text color of the value(s) 3 position above/below the primary selected value when in dark mode.

Example:

```
<appSettings>
    <add key="OutputLocation" value="c:\dev\wheel_selection.txt" />
    <add key="OutputMethod" value="Overwrite" />
    <add key="ClearOutputFileOnStart" value="true" />
    <add key="ClearOutputFileOnEscape" value="true" />
    <add key="SyncWithWindowsTheme" value="true" />
    <add key="DefaultTheme" value="LightMode" />
    
    <add key="LightModeWindowBackgroundColor" value="White" />
    <add key="LightModeSelectedTextColor" value="SteelBlue" />
    <add key="LightModeOffset1TextColor" value="Silver" />
    <add key="LightModeOffset2TextColor" value="Gray" />
    <add key="LightModeOffset3TextColor" value="DimGray" />

    <add key="DarkModeWindowBackgroundColor" value="Black" />
    <add key="DarkModeSelectedTextColor" value="SteelBlue" />
    <add key="DarkModeOffset1TextColor" value="Silver" />
    <add key="DarkModeOffset2TextColor" value="Gray" />
    <add key="DarkModeOffset3TextColor" value="DimGray" />
</appSettings>
```
