# Inspector Lock Button
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

# Get started with the Inspector Lock
>[!WARNING]
>Implementation instructions are not complete.

### 1. Adding Lockable Assets to your project
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

<details>
  <summary>Click to see an example GIF</summary>
  <img width=100% height=100% alt="asset-creation-gif" src="https://github.com/user-attachments/assets/9ebdf0a5-6e57-40b2-b7c0-dc067288e5b7"/>
</details>


### 2. Defining lockable variables
Any fields that are visible the Unity Inspector will per default be placed inside the Editor Lock element. Additions or removals of public fields will be handled automatically by the plugin.

```csharp
public class LockableAsset: MonoBehaviour, ILockableInspector
{
	// Implementation of InspectorLockable interface;
	[SerializeField]
	[HideInInspector]
	private bool[] m_InspectorLockStates;
	public string LockablePropertyPath => nameof(m_InspectorLockStates);

	// The fields below will be inside the Editor Lock element
	public string Name;
	public float Health;
}
```
A preview of how this code would look in Unity.

<img width="310" height="275" alt="image" src="https://github.com/user-attachments/assets/af3636f3-e1f0-46da-919f-be1bc870f608" />

<details>
  <summary>Click to see an example GIF</summary>
  <img width=100% height=100% alt="asset-creation-gif" src="https://github.com/user-attachments/assets/5b84f345-332c-4021-b6cf-017fbaff1bc8"/>
</details>

### 3. Further customization
This section is incomplete.

