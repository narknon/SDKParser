# SDKParser
Parses dumped SDK's into CPP and header files.

# Current Issues
The SDK should be formatted like this
```cpp
// Class FortniteGame.FortBaseLayerAnimInstance
// Size: 0x360 (Inherited: 0x350)
struct UFortBaseLayerAnimInstance : UFortBaseAnimInstance {
	struct TWeakObjectPtr<struct UFortPlayerAnimInstance> MainAnimInstanceWeakPtr; // 0x350(0x08)
	char pad_358[0x8]; // 0x358(0x08)
};
```
