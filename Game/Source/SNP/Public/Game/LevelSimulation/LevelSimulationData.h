#pragma once

#include "CoreMinimal.h"

class ULevelSimulationAsset;

class FLevelSimulationData
{
public:
	// Constructor
	explicit FLevelSimulationData(const ULevelSimulationAsset* DataAsset);
	
	// Data members
	TSubclassOf<AActor> BaseActor;

	FTransform SpawnTransform;

	FTransform ExitTransform;
};
