#pragma once

#include "CoreMinimal.h"
#include "SaveState.generated.h"

// Base class for all saveable game elements
USTRUCT(BlueprintType)
struct SNP_API FBaseActorSaveState
{
	GENERATED_BODY()
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString ObjectName;
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FTransform Transform;

	virtual ~FBaseActorSaveState() {}
	
	virtual bool Serialize(FArchive& Ar)
	{
		Ar << ObjectName;
		Ar << Transform;
		return true;
	}
};

// Game Item specific data
USTRUCT(BlueprintType)
struct SNP_API FGameItem
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString ItemName;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int Quantity;
	
	bool Serialize(FArchive& Ar)
	{
		Ar << ItemName;
		Ar << Quantity;
		return true;
	}
};

// Chest specific data
USTRUCT(BlueprintType)
struct SNP_API FChestSaveState : public FBaseActorSaveState
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<FGameItem> Items;

	virtual bool Serialize(FArchive& Ar) override
	{
		if (!Super::Serialize(Ar))
			return false;

		int32 Count = Items.Num();
		Ar << Count;

		if (Ar.IsLoading())
			Items.SetNum(Count);
        
		for (auto& Item : Items)
		{
			if (!Item.Serialize(Ar))
				return false;
		}
		
		return true;
	}
	
	// Convert struct to string for logging
	FString ToString() const
	{
		return FString::Printf(TEXT("Name: %s, Location: %s, ItemCount: %d"),
			*ObjectName,
			*Transform.ToHumanReadableString(),
			Items.Num());
	}
};

// Harvester specific data
USTRUCT(BlueprintType)
struct SNP_API FHarvesterSaveState : public FBaseActorSaveState
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString CropName;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Growth;
	
	
	virtual bool Serialize(FArchive& Ar) override
	{
		if (!Super::Serialize(Ar))
			return false;
		
		Ar << CropName;
		Ar << Growth;
		return true;
	}
};

