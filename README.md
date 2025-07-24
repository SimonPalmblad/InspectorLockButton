# InspectorLockButton
A custom Unity Editor tool. Adds a lock button to GameObject variable(s) to enable/disable editing via the interface.

### Features
- UI Toolkit knowledge not required for basic usage.
- Integration with Unity's official UI Toolkit system.
- Context menu for quickly creating new lockable assets.
- Settings and styling options for lockable assets.
- Nested locked groups, giving you the ability to, for example, both be able to lock the entire inspector or just parts of it.
- A custom tool for setting up new lockable objects in your editor to avoid having to manually create each part.

### Installation
- Download the InspectorLockButton package in the latest release (don't download the codebase).
- Unzip the file.
- Add the package in Unity via the `Assets` -> `Import package` -> `Custom package` top menu.

### How to use add an Inspector Lock
>[!WARNING]
>Implementation instructions are not complete.

#### 1. Using the Create Assets menu
You can create all assets needed to use the Inspector lock via right-click context menu in Unity.
- `Create` -> `Inspector Lock` -> `Create Lockable Assets`
- Or the top menu: `Assets` -> `Create` -> `Inspector Lock` -> `Create Lockable Assets`
- Use the dialogue to create your assets.

<img width=50% height="50%" alt="image" src="https://github.com/user-attachments/assets/b8f16a6f-0e83-4e79-86e7-5bedc240a88d"/>

This window will create the following assets inside their specified folder path:
- A GameObject,
- A C# script for game logic,
- A C# editor script for custom Inspector functionality,
- and a UXML document to customize the Inspector.

Due to script compilation limits, the GameObject will have a placeholder component attached to it instead of the game logic script that has to be manually replaced via pressing the button on the placeholder component.




