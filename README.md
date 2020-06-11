# WheelSelect
Pass a delimited list into the form for a "wheel like" selection. Meant to be used as a quick selection utility. I'm really not sure why I built this. I had a thought and had to run with it. Do with it what you will.

## Main Functionality
* Simply pass two arguments into the exe and the interface will display in the center of the screen loaded with your data set.
    * ex: `WheelSelect.exe "," "option a,option b,option c"`
* Use the up/down arrow keys or the mouse wheel to scroll through the list.
* Push the Enter key when you have the desired option selected and the value will be saved to a file.

## Options
The App.config has settings that allow for some customization.
* **OutputLocation** The location the data is written to when the Enter key is pressed. Default is `c:\wheel_selection.txt`.
* **OutputMethod** Decides if the output should be appended or overwritten. Must be either `Append` or `Overwrite`.
* **ClearOutputFileOnStart** Decides if the output file should start empty. `true` or `false`.
* **ClearOutputFileOnEscape** Decides if the output file should be cleared on the Escape key press. `true` or `false`.

The following are color customizations and must be a valid value of the `System.Drawing.Color` struct.
* **WindowBackgroundColor** Sets the background color of the interface.
* **SelectedTextColor** Sets the text color of the primary selected value.
* **Offset1TextColor** Sets the text color of the value(s) 1 position above/below the primary selected value.
* **Offset2TextColor** Sets the text color of the value(s) 2 positions above/below the primary selected value.
* **Offset3TextColor** Sets the text color of the value(s) 3 position above/below the primary selected value.

Example:
```
<appSettings>
    <add key="OutputLocation" value="c:\wheel_selection.txt" />
    <add key="OutputMethod" value="Overwrite" />
    <add key="ClearOutputFileOnStart"  value="true" />
    <add key="ClearOutputFileOnEscape"  value="true" />
    <add key="WindowBackgroundColor" value="White" />
    <add key="SelectedTextColor" value="SteelBlue" />
    <add key="Offset1TextColor" value="DimGray" />
    <add key="Offset2TextColor" value="Gray" />
    <add key="Offset3TextColor" value="Silver" />
</appSettings>
```
