#pragma once

#include "CoreMinimal.h"
#include "Engine/DataAsset.h"
#include "Engine/StaticMesh.h"
#include "Vehicle/CaravanInterior.h"
#include "LevelSimulationAsset.generated.h"

/**
 * LevelSimulationData: A data asset to define properties for level simulation.
 */
UCLASS(BlueprintType)
class SNP_API ULevelSimulationAsset : public UDataAsset
{
	GENERATED_BODY()

public:
	// Reference to the static mesh used in the level simulation
	// TODO: More generic type 
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Simulation")
	TSubclassOf<ACaravanInterior> BaseActor;
	
	// Reference to the static mesh used in the level simulation
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Simulation")
	FTransform SpawnTransform;
	
	// Reference to the static mesh used in the level simulation
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Simulation")
	FTransform ExitTransform;
};
