# Elegant Recorder

![image](https://user-images.githubusercontent.com/981184/209982997-48d8d5f2-24cb-49d8-bf92-72ed725861de.png)

A small automation tool and macro recorder for Windows. 

It can record and replay desktop applications with various degrees of accuracy.

## Usage

Before creating your first recording press the arrow button to expand the UI and type in the name of the new recording. 

Press `Record` and perform your actions on the desktop. Press `Stop` when you're done. Select a recording from your list and press `Replay` to run it.

![image](https://user-images.githubusercontent.com/981184/209984329-be6b00f6-e47e-4d84-9fec-bf5a9e117c7d.png)

## Configuration

Several options are available using the `Settings` button.

![image](https://user-images.githubusercontent.com/981184/209984741-b181e15f-d3a7-4c05-9fdc-c5eb7c5dd307.png)

All recordings are stored in the `Data folder` in **_json_** format. You can manually edit a recording file if you feel the need.

If you want to record sensitive data (like typing a password) you can use the `Encrypted` option. Your actions will no longer be visible in the json file. The encryption password is not stored anywhere, you will need to provide it when replaying the recording. 

## Triggers

![image](https://user-images.githubusercontent.com/981184/209986667-6a50f510-30f9-4ad6-9112-8a5376ae5f88.png)

You can have your recordings run based on several conditions, using the `Triggers` window. For example, you could chain multiple recordings together using the `Recording` trigger.

## Credits

This project uses some binaries from [UIAComWrapper](https://github.com/TestStack/UIAComWrapper)
