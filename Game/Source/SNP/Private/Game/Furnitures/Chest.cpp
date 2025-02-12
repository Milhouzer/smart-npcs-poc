#include "Game/Furnitures/Chest.h"

TArray<uint8> AChest::GetSaveState()
{
	TArray<uint8> BinaryData;
	FMemoryWriter MemWriter(BinaryData);

	FChestSaveState SaveState;
	SaveState.ObjectName = "Chest";
	SaveState.Transform = this->GetActorTransform();
	SaveState.Items = this->ItemsList;
	SaveState.Serialize(MemWriter);
	
	// Log the binary data size
	UE_LOG(LogTemp, Log, TEXT("Binary data size: %d bytes"), BinaryData.Num());
    
	// Optionally, log the hex representation of the data
	FString HexString;
	for (uint8 Byte : BinaryData)
	{
		HexString += FString::Printf(TEXT("%02X "), Byte);
	}
	UE_LOG(LogTemp, Log, TEXT("Binary data: %s"), *HexString);
    
	return BinaryData;
}

void AChest::LoadSaveState(FMemoryReader SaveState)
{
}
