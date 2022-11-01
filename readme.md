# Elegant Recorder

![image](https://user-images.githubusercontent.com/981184/199304570-bbd2f66c-d14e-44df-8308-623cfbcbddb6.png)

A simple tool that allows you to record and replay your actions inside Windows applications. It can be used to record macros or automate tasks.

It works well with desktop applications such as .NET Windows Forms or Win32 apps.

It doesn't work so well with browsers or web frameworks.

## Usage

The tool is pretty straightforward to use. Open the settings to specify a recording file and change several other options. You can restrict the recording to a single executable by entering the full exe path or the short name (e.g. Notepad).

Press Record and perform some actions on the desktop. Press Stop when you're done. All the actions are saved in the file you specified, in a json format.

You can pause both the recording or the replay at any time.

## Credits

This project uses some binaries from [UIAComWrapper](https://github.com/TestStack/UIAComWrapper)
