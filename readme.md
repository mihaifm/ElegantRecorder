# Elegant Recorder

![image](https://user-images.githubusercontent.com/981184/191975186-e6527de6-3ff1-4c46-b0a1-ed4cde394ca0.png)

Experimental tool that allows you to record and replay your actions inside Windows applications.

It works well with applications generally supported by the Microsoft UI Automation framework, such as .NET Windows Forms or Win32 apps.
It does not work with browsers or web frameworks.

## Usage

Specify an empty file where to save the recording and an executable that you want to record.
For the exe you can enter the full path or only the short name (e.g. Notepad).

Press the Record button, do you work, press the Stop button. All the actions are saved in the file you specified, in a json format.

## Credits

This project uses some binaries from [UIAComWrapper](https://github.com/TestStack/UIAComWrapper)
