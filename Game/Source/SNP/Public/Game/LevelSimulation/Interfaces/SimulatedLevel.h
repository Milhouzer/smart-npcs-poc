#pragma once

#include "Game/LevelSimulation/LevelSimulationData.h"
#include "SimulatedLevel.generated.h"

UINTERFACE(Blueprintable)
class SNP_API USimulatedLevel : public UInterface
{
	GENERATED_BODY()
};

class SNP_API ISimulatedLevel
{
	GENERATED_BODY()

public:
	virtual void Init(FLevelSimulationData* SimulationData) = 0;
	virtual void Simulate(float DeltaTime) = 0;
	virtual void Enter(APlayerController* PlayerController) = 0;
	virtual void Exit(APlayerController* PlayerController) = 0;
};