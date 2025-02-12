#pragma once

#include "Game/Save/Saveable.h"
#include "Chest.generated.h"

UCLASS()
class SNP_API AChest : public AActor, public ISaveable
{
	GENERATED_BODY()
	
public:

	UFUNCTION(BlueprintCallable)
	virtual TArray<uint8> GetSaveState() override;
	
	virtual void LoadSaveState(FMemoryReader SaveState) override;

	UPROPERTY(BlueprintReadWrite)
	TArray<FGameItem> ItemsList;
}; 
