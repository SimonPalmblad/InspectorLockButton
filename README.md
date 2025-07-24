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
- Add the package in Unity via the `Assets` -> `Import package` top menu.

### How to use add an Inspector Lock
>[!WARNING]
>Implementation instructions are not complete.

#### 1. Using the Create Assets menu
You can create all assets needed to use the Inspector lock via right-click context menu in Unity.
- `Create` -> `Inspector Lock` -> `Create Lockable Assets`
- Or the top menu: `Assets` -> `Create` -> `Inspector Lock` -> `Create Lockable Assets`
- Use the dialogue to create your assets.
	
