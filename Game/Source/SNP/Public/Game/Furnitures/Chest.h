#pragma once

#include "Game/Save/Saveable.h"
#include "Chest.generated.h"

UCLASS()
class SNP_API AChest : public AActor, public ISaveable
{
	GENERATED_BODY()
	
public:
	
    virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

	UFUNCTION(BlueprintCallable)
	virtual TArray<uint8> GetSaveState() override;
	
	virtual void LoadSaveState(FMemoryReader SaveState) override;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString ObjectName;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Replicated)
	TArray<FGameItem> ItemsList;
}; 
