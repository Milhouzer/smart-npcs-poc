#include "Game/Furnitures/Chest.h"

#include "Net/UnrealNetwork.h"


void AChest::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
    
	DOREPLIFETIME(AChest, ItemsList);
}

TArray<uint8> AChest::GetSaveState()
{
	TArray<uint8> BinaryData;
	FMemoryWriter MemWriter(BinaryData);

	FChestSaveState SaveState;
	SaveState.ObjectId = 0;
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

void AChest::LoadSaveState(FMemoryReader MemReader)
{
	FChestSaveState SaveState;
	bool Loaded = SaveState.Serialize(MemReader);
	UE_LOG(LogTemp, Log, TEXT("Loaded chest data (%d): %s"), Loaded, *SaveState.ToString());

	ObjectName = "Chest";
	ItemsList = SaveState.Items;
	SetActorTransform(SaveState.Transform);
}
